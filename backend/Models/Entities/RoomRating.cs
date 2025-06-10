using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Hotel_MVP;

public partial class RoomRating
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ratingId")]
    public Guid Id { get; set; }

    [ForeignKey("Room")]
    [Column("roomId")]
    public Guid RoomId { get; set; }

    [ForeignKey("User")]
    [Column("userId")]
    public Guid UserId { get; set; }

    [Required]
    [Range(0, 10)]
    [Column("roomRate")]
    public int RoomRate { get; set; }

    [Required]
    [Column("comment", TypeName = "varchar(512)")]
    public required string Comment { get; set; }

    public virtual Room Room { get; set; }
    public virtual User User { get; set; }
}
