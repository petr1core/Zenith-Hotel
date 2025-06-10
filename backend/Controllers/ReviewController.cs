using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hotel_MVP.Data;
using Hotel_MVP.Models.Entities;
using Hotel_MVP.DTO;

namespace Hotel_MVP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(AppDbContext context, ILogger<ReviewController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Получает все отзывы для конкретного номера
        /// </summary>
        [HttpGet("room/{roomId}")]
        public async Task<IActionResult> GetRoomReviews(Guid roomId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool includeRejected = false)
        {
            try
            {
                _logger.LogInformation("GetRoomReviews called with roomId: {RoomId}, page: {Page}, pageSize: {PageSize}, includeRejected: {IncludeRejected}",
                    roomId, page, pageSize, includeRejected);

                var query = _context.Reviews
                    .Where(r => r.RoomId == roomId);

                _logger.LogInformation("Initial query created for roomId: {RoomId}", roomId);

                if (!includeRejected)
                {
                    _logger.LogInformation("Applying approved status filter");
                    query = query.Where(r => r.Status == ReviewStatus.Approved);
                }

                var totalReviews = await query.CountAsync();
                _logger.LogInformation("Total reviews found before pagination: {Count}", totalReviews);

                var totalPages = (int)Math.Ceiling(totalReviews / (double)pageSize);
                _logger.LogInformation("Calculated total pages: {TotalPages}", totalPages);

                var reviews = await query
                    .OrderByDescending(r => r.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Include(r => r.User)
                    .Select(r => new
                    {
                        id = r.Id,
                        rating = r.Rating,
                        comment = r.Comment,
                        createdAt = r.CreatedAt,
                        tag = r.Tag,
                        status = r.Status.ToString(),
                        user = r.User == null ? null : new
                        {
                            id = r.User.Id,
                            firstName = r.User.UserFirstname,
                            lastName = r.User.UserLastname
                        }
                    })
                    .AsNoTracking()
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} reviews after pagination", reviews.Count);

                Response.Headers["X-Total-Count"] = totalReviews.ToString();
                Response.Headers["X-Total-Pages"] = totalPages.ToString();

                return Ok(new { items = reviews, totalCount = totalReviews, totalPages = totalPages });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting room reviews. Exception details: {Message}", ex.Message);
                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner exception: {Message}", ex.InnerException.Message);
                }
                return StatusCode(500, new { error = "Ошибка при получении отзывов", details = ex.Message });
            }
        }

        /// <summary>
        /// Получает средний рейтинг номера
        /// </summary>
        [HttpGet("room/{roomId}/rating")]
        public async Task<IActionResult> GetRoomRating(Guid roomId)
        {
            try
            {
                _logger.LogInformation("GetRoomRating called for roomId: {RoomId}", roomId);

                var reviews = await _context.Reviews
                    .Where(r => r.RoomId == roomId && r.Status == ReviewStatus.Approved)
                    .AsNoTracking()
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} approved reviews for room {RoomId}", reviews.Count, roomId);

                if (!reviews.Any())
                {
                    _logger.LogInformation("No approved reviews found for room {RoomId}", roomId);
                    return Ok(new
                    {
                        averageRating = 0.0,
                        totalReviews = 0
                    });
                }

                var rating = reviews.Average(r => r.Rating);
                var reviewCount = reviews.Count;

                _logger.LogInformation("Calculated rating for room {RoomId}: Average = {Rating}, Count = {Count}",
                    roomId, Math.Round(rating, 1), reviewCount);

                return Ok(new
                {
                    averageRating = Math.Round(rating, 1),
                    totalReviews = reviewCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting room rating. Exception details: {Message}", ex.Message);
                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner exception: {Message}", ex.InnerException.Message);
                }
                return StatusCode(500, new { error = "Ошибка при получении рейтинга", details = ex.Message });
            }
        }

        /// <summary>
        /// Создает новый отзыв
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDTO dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    throw new InvalidOperationException("User ID not found in token");
                }
                var userId = Guid.Parse(userIdClaim.Value);

                // Проверяем, существует ли бронирование
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.Id == dto.BookingId && b.UserId == userId);

                if (booking == null)
                {
                    return BadRequest(new { error = "Бронирование не найдено" });
                }

                // Проверяем, не оставлял ли пользователь уже отзыв для этого бронирования
                var existingReview = await _context.Reviews
                    .AnyAsync(r => r.BookingId == dto.BookingId);

                if (existingReview)
                {
                    return BadRequest(new { error = "Отзыв для этого бронирования уже существует" });
                }

                var review = new Review
                {
                    UserId = userId,
                    RoomId = booking.RoomId,
                    BookingId = dto.BookingId,
                    Rating = dto.Rating,
                    Comment = dto.Comment,
                    CreatedAt = DateTime.UtcNow,
                    Status = ReviewStatus.Pending
                };

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Отзыв успешно создан и ожидает модерации" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review");
                return StatusCode(500, new { error = "Ошибка при создании отзыва" });
            }
        }

        /// <summary>
        /// Получает все отзывы, ожидающие модерации
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("pending")]
        public async Task<IActionResult> GetPendingReviews()
        {
            try
            {
                var pendingReviews = await _context.Reviews
                    .Where(r => r.Status == ReviewStatus.Pending)
                    .Include(r => r.User)
                    .Include(r => r.Room)
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new
                    {
                        r.Id,
                        r.Rating,
                        r.Comment,
                        r.CreatedAt,
                        User = r.User == null ? null : new { r.User.Id, FullName = r.User.UserFirstname + " " + r.User.UserLastname },
                        Room = r.Room == null ? null : new { r.Room.Id, r.Room.RoomNumber }
                    })
                    .ToListAsync();

                return Ok(pendingReviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending reviews");
                return StatusCode(500, new { error = "Ошибка при получении отзывов для модерации" });
            }
        }

        /// <summary>
        /// Модерация отзыва (только для администраторов)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{reviewId}/moderate")]
        public async Task<IActionResult> ModerateReview(Guid reviewId, [FromBody] ModerateReviewDTO dto)
        {
            try
            {
                var review = await _context.Reviews.FindAsync(reviewId);
                if (review == null)
                {
                    return NotFound(new { error = "Отзыв не найден" });
                }

                review.Status = dto.Status;
                review.ModeratorComment = dto.ModeratorComment;
                review.ModeratedAt = DateTime.UtcNow;

                if (review.Status == ReviewStatus.Approved)
                {
                    // Check if this is the first approved review for this room
                    var otherApprovedReviews = await _context.Reviews
                        .AnyAsync(r => r.RoomId == review.RoomId &&
                                       r.Id != review.Id &&
                                       r.Status == ReviewStatus.Approved);

                    if (!otherApprovedReviews)
                    {
                        review.Tag = "Первый гость";
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Статус отзыва успешно обновлен" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error moderating review");
                return StatusCode(500, new { error = "Ошибка при модерации отзыва" });
            }
        }

        /// <summary>
        /// Удаляет отзыв (только для администраторов)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview(Guid reviewId)
        {
            try
            {
                var review = await _context.Reviews.FindAsync(reviewId);
                if (review == null)
                {
                    return NotFound(new { error = "Отзыв не найден" });
                }

                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Отзыв успешно удален" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review");
                return StatusCode(500, new { error = "Ошибка при удалении отзыва" });
            }
        }
    }
}

