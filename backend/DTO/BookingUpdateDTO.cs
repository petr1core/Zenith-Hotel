using Hotel_MVP.Models.Entities;

namespace Hotel_MVP.DTO
{
    public class BookingUpdateDTO
    {
        public BookingStatus BookingStatus { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }
}