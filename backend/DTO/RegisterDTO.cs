using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

public class RegisterDTO
{
    public string UserEmail { get; set; }
    public string UserPassword { get; set; }
    public string Role { get; set; }
    public string UserFirstname { get; set; }
    public string UserLastname { get; set; }
    public string UserPhoneNumber { get; set; }
}