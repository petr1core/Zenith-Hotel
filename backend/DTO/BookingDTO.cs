using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel_MVP.Models.Entities;

namespace Hotel_MVP.DTO
{
    public class BookingDTO
    {
        public Guid UserId { get; set; }
        public Guid RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public BookingStatus? BookingStatus { get; set; }
        public List<Guid>? SelectedServices { get; set; } = [];
    }
}