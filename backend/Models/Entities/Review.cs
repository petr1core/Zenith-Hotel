using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Hotel_MVP.Models.Entities;
using System.Text.Json.Serialization;

namespace Hotel_MVP;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReviewStatus
{
    Pending,    // Ожидает модерации
    Approved,   // Одобрен
    Rejected    // Отклонен
}

public partial class Review
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("reviewId")]
    public Guid Id { get; set; }

    [ForeignKey("User")]
    [Column("userId")]
    public Guid UserId { get; set; }

    [ForeignKey("Room")]
    [Column("roomId")]
    public Guid RoomId { get; set; }

    [ForeignKey("Booking")]
    [Column("bookingId")]
    public Guid BookingId { get; set; }

    [Required]
    [Range(1, 5)]
    [Column("rating")]
    public int Rating { get; set; }

    [Required]
    [StringLength(1000)]
    [Column("comment", TypeName = "varchar(1000)")]
    public string Comment { get; set; }

    [Required]
    [Column("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ModeratedAt { get; set; }

    [Required]
    [Column("status")]
    public ReviewStatus Status { get; set; } = ReviewStatus.Pending;

    [Column("moderatorComment", TypeName = "varchar(512)")]
    public string? ModeratorComment { get; set; }

    [Column("tag", TypeName = "varchar(100)")]
    public string? Tag { get; set; }

    [JsonIgnore]
    public virtual User User { get; set; }

    [JsonIgnore]
    public virtual Room Room { get; set; }

    [JsonIgnore]
    public virtual Booking Booking { get; set; }
}
