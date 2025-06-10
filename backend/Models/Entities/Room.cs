using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Hotel_MVP;

public enum RoomAvailability
{
    Free,
    Occupied,
    Maintenance,
    Reserved
}

public enum CleaningStatus
{
    Clean,
    NeedsCleaning,
    InProgress
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RoomType
{
    Standard,
    Superior,
    FamilyRoom,
    Deluxe,
    Suite
}

public partial class Room
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("roomId")]
    public Guid Id { get; set; }

    [Column("roomNumber")]
    public int RoomNumber { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Required]
    [Column("roomType")]
    public required RoomType RoomType { get; set; }

    [Required]
    [Column("availability")]
    public RoomAvailability Availability { get; set; }

    [Column("roomCharge", TypeName = "decimal(9,2)")]
    public decimal RoomCharge { get; set; }

    [Required]
    [Column("capacity")]
    public int Capacity { get; set; }

    [Required]
    [Column("area")]
    public decimal Area { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Required]
    [Column("cleaningStatus")]
    public CleaningStatus CleaningStatus { get; set; }

    [Column("lastCleaned")]
    public DateTime? LastCleaned { get; set; }

    [Column("floor")]
    public int Floor { get; set; }

    public virtual ICollection<RoomPhoto> Images { get; set; } = new List<RoomPhoto>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
