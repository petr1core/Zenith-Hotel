using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Hotel_MVP.Models.Entities
{
    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Cancelled
    }
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("bookingId")]
        public Guid Id { get; set; }

        [ForeignKey("User")]
        [Column("userId")]
        public Guid UserId { get; set; }

        [ForeignKey("Room")]
        [Column("roomId")]
        public Guid RoomId { get; set; }

        [Required]
        [Column("bookingStatus", TypeName = "varchar(128)")]
        public string BookingStatusString { get; set; } = BookingStatus.Pending.ToString();

        [Required]
        [Column("checkInDate")]
        public DateTime CheckInDate { get; set; }

        [Required]
        [Column("checkOutDate")]
        public DateTime CheckOutDate { get; set; }

        [NotMapped]
        public BookingStatus BookingStatus
        {
            get => Enum.Parse<BookingStatus>(BookingStatusString, true);
            set => BookingStatusString = value.ToString();
        }

        // Навигационные свойства
        public virtual User User { get; set; }
        public virtual Room Room { get; set; }
    }
}
