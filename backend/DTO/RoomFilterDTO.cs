using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Hotel_MVP;

namespace backend.DTO
{
    public class RoomFilterDTO
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public RoomType? RoomType { get; set; }
        public int? MinCapacity { get; set; }
        public int? Floor { get; set; }
        public bool? IncludeImages { get; set; }
    }
}