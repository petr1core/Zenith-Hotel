using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Hotel_MVP;

public enum Role
{
    User,
    Admin
}

public partial class User // ex Guest
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("userId")]
    public Guid Id { get; set; }

    [Required]
    [Column("userFirstname", TypeName = "varchar(128)")]
    public required string UserFirstname { get; set; }

    [Required]
    [Column("userLastname", TypeName = "varchar(128)")]
    public required string UserLastname { get; set; }

    [Required]
    [Column("userPhoneNumber")]
    public required string UserPhoneNumber { get; set; }

    [Required]
    [Column("userEmail")]
    public required string UserEmail { get; set; }

    [Required]
    [Column("userPasswordHash")]
    public required string UserPasswordHash { get; set; }

    [Required]
    [Column("role")]
    public string RoleString { get; set; } = Role.User.ToString();

    [Required]
    [Column("registrationDate")]
    public DateTime RegistrationDate { get; set; } = DateTime.Now;

    [Required]
    [Column("lastLoginDate")]
    public DateTime LastLoginDate { get; set; } = DateTime.Now;

    // Храним строковое значение роли в базе, но работаем с enum в коде
    [NotMapped]
    public Role Role
    {
        get => Enum.Parse<Role>(RoleString, true);
        set => RoleString = value.ToString();
    }
}
