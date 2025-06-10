using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Hotel_MVP.DTO;
using Hotel_MVP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;

namespace Hotel_MVP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;

        public AuthController(AuthService authService, ILogger<AuthController> logger, IConfiguration configuration)
        {
            _authService = authService;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            _logger.LogInformation($"Попытка входа: {loginDto.Login}");

            var (success, token, message) = await _authService.LoginAsync(loginDto);

            if (!success)
            {
                _logger.LogWarning($"Неудачная попытка входа: {loginDto.Login} - {message}");
                return BadRequest(new { message });
            }

            _logger.LogInformation($"Успешный вход: {loginDto.Login}");
            return Ok(new { token, message });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDTO dto)
        {
            // Проверка существования пользователя
            if (_authService.UserExists(dto.UserEmail))
            {
                return BadRequest(new { message = "Email already exists" });
            }

            // Создание пользователя
            var user = _authService.CreateUser(
                dto.UserEmail,
                dto.UserPassword,
                dto.Role,
                dto.UserFirstname,
                dto.UserLastname,
                dto.UserPhoneNumber
            );

            // Генерация токена
            var token = _authService.GenerateToken(user);

            return Ok(new AuthResponseDTO
            {
                Token = token,
                Role = user.Role.ToString()
            });
        }

        [HttpGet("validate")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            try
            {
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    _logger.LogWarning("Invalid authorization header format");
                    return Unauthorized(new { isValid = false, reason = "Invalid authorization header" });
                }

                var token = authHeader.Replace("Bearer ", "");
                var tokenHandler = new JwtSecurityTokenHandler();

                if (!tokenHandler.CanReadToken(token))
                {
                    _logger.LogWarning("Cannot read token");
                    return Unauthorized(new { isValid = false, reason = "Invalid token format" });
                }

                var jwtToken = tokenHandler.ReadJwtToken(token);

                // Проверяем срок действия токена
                if (jwtToken.ValidTo < DateTime.UtcNow)
                {
                    _logger.LogWarning($"Token expired at {jwtToken.ValidTo}");
                    return Unauthorized(new { isValid = false, reason = "Token expired" });
                }

                // Проверяем issuer и audience
                if (jwtToken.Issuer != _configuration["Jwt:Issuer"] ||
                    !jwtToken.Audiences.Contains(_configuration["Jwt:Audience"]))
                {
                    _logger.LogWarning("Invalid issuer or audience");
                    return Unauthorized(new { isValid = false, reason = "Invalid issuer or audience" });
                }

                _logger.LogInformation("Token validation successful");
                return Ok(new { isValid = true });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Token validation failed: {ex.Message}");
                return StatusCode(500, new { isValid = false, reason = "Server error during token validation" });
            }
        }
    }
}