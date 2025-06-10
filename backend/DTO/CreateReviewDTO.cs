using System.ComponentModel.DataAnnotations;

namespace Hotel_MVP.DTO
{
    public class CreateReviewDTO
    {
        [Required]
        public Guid BookingId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Оценка должна быть от 1 до 5")]
        public int Rating { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Комментарий должен содержать от 10 до 1000 символов")]
        public string Comment { get; set; }
    }
}