using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Hotel_MVP.Data;
using System.Text;
using Hotel_MVP.DTO;
using Hotel_MVP.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Hotel_MVP.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<(bool success, string token, string message)> LoginAsync(LoginDTO loginDto)
        {
            try
            {
                // Ищем пользователя по email или телефону
                var user = await _context.Users
                    .FirstOrDefaultAsync(u =>
                        (u.UserEmail != null && u.UserEmail == loginDto.Login) ||
                        (u.UserPhoneNumber != null && u.UserPhoneNumber == loginDto.Login));

                if (user == null)
                {
                    return (false, null, "Пользователь не найден");
                }

                // Проверяем пароль
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.UserPasswordHash))
                {
                    return (false, null, "Неверный пароль");
                }

                // Обновляем время последнего входа
                user.LastLoginDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Генерируем JWT токен
                var token = GenerateJwtToken(user);

                return (true, token, "Успешный вход");
            }
            catch (Exception ex)
            {
                return (false, null, $"Ошибка при входе: {ex.Message}");
            }
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.UserFirstname} {user.UserLastname}"),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Email, user.UserEmail ?? ""),
                new Claim(ClaimTypes.MobilePhone, user.UserPhoneNumber ?? ""),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public User Authenticate(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserEmail == email);
            if (user == null || !VerifyPassword(password, user.UserPasswordHash))
                return null;

            return user;
        }

        public string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.UserEmail),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            // Реализация проверки пароля с использованием BCrypt
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }

        public bool UserExists(string email)
        {
            return _context.Users.Any(u => u.UserEmail == email);
        }

        public User CreateUser(string email, string password, string role, string firstname, string lastname, string phone)
        {
            var user = new User
            {
                UserEmail = email,
                UserPasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = Enum.Parse<Role>(role, true),
                UserFirstname = firstname,
                UserLastname = lastname,
                UserPhoneNumber = phone,
                RegistrationDate = DateTime.UtcNow,
                LastLoginDate = DateTime.UtcNow
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            // После сохранения user.Id уже есть

            if (role == "User")
            {
                var guestProfile = new GuestProfile
                {
                    UserId = user.Id,
                    IsVerified = false,
                    IsBanned = false,
                    ReviewCount = 0,
                    LastReviewDate = null,
                    Notes = null,
                    BanReason = null,
                    AvatarURL = null
                };
                _context.GuestProfiles.Add(guestProfile);
            }
            else if (role == "Admin")
            {
                var adminProfile = new AdminProfile
                {
                    UserId = user.Id,
                    AvatarURL = null,
                    Notes = null
                };
                _context.AdminProfiles.Add(adminProfile);
            }

            _context.SaveChanges();

            return user;
        }
    }
}