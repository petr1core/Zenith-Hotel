using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Hotel_MVP.Models.Entities;

namespace Hotel_MVP.DTO
{
    public class ModerateReviewDTO
    {
        [Required]
        public ReviewStatus Status { get; set; }

        [StringLength(512, ErrorMessage = "Комментарий модератора не может превышать 512 символов")]
        public string? ModeratorComment { get; set; }
    }
}