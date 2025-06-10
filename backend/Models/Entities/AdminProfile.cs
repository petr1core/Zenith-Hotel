
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Hotel_MVP;

public partial class AdminProfile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("adminProfileId")]
    public Guid Id { get; set; }

    [ForeignKey("UserId")]
    [Column("adminId")]
    public Guid UserId { get; set; }

    [Required]
    [Column("AvatarURL")]
    public string? AvatarURL { get; set; } = null;

    [Column("Notes")]
    public string? Notes { get; set; } = null;

    public virtual User User { get; set; }
}

