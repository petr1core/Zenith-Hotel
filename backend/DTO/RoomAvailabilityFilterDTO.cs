using System;
using System.ComponentModel.DataAnnotations;

namespace Hotel_MVP.DTO
{
    public class RoomAvailabilityFilterDTO
    {
        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        // Параметры пагинации
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;

        // Дополнительные фильтры
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public RoomType? RoomType { get; set; }
        public int? MinCapacity { get; set; }
        public int? Floor { get; set; }
        public bool? IncludeImages { get; set; }
    }
}