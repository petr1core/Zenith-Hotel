using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Hotel_MVP;

public class RoomPhoto
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string PhotoUrl { get; set; }

    [Required]
    public bool IsPrimary { get; set; } = false;

    [Required]
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;

    public string Description { get; set; }

    [Required]
    public int OrderIndex { get; set; } = 0;

    [ForeignKey("Room")]
    [Column("roomId")]
    public Guid RoomId { get; set; }

    public virtual Room Room { get; set; }
}
