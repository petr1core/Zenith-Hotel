using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel_MVP.DTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Необходимо указать email или номер телефона")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Необходимо указать пароль")]
        public string Password { get; set; }
    }

    // public class RegisterDTO : LoginDTO
    // {
    //     [Required]
    //     public string Role { get; set; }
    // }

    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public string Role { get; set; }
    }
}