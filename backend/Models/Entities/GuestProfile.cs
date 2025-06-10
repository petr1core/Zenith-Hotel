
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Hotel_MVP;

public partial class GuestProfile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("guestProfileId")]
    public Guid Id { get; set; }

    [ForeignKey("UserId")]
    [Column("userId")]
    public Guid UserId { get; set; }

    [Required]
    [Column("AvatarURL")]
    public string? AvatarURL { get; set; } = null;

    [Column("ReviewCount")]
    public int? ReviewCount { get; set; } = 0;

    [Column("LastReviewDate")]
    public DateTime? LastReviewDate { get; set; } = null;

    [Column("Notes")]
    public string? Notes { get; set; } = null;

    [Required]
    [Column("isVerified")]
    public bool IsVerified { get; set; } = false;
    [Required]
    [Column("isBanned")]
    public bool IsBanned { get; set; } = false;

    [Required]
    [Column("banReason")]
    public string? BanReason { get; set; } = null;

    public virtual User User { get; set; }
}

