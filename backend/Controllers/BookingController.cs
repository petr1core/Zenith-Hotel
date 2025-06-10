using Hotel_MVP.Data;
using Hotel_MVP.DTO;
using Hotel_MVP.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Hotel_MVP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<BookingController> _logger;

        public BookingController(AppDbContext context, ILogger<BookingController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<List<Booking>> Get()
        {
            return await _context.Bookings.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetById(Guid id)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.Id == id);
            if (booking == null)
            {
                return NotFound();
            }
            return booking;
        }


        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<Booking>> Create([FromBody] BookingDTO dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized("User not authenticated or UserId not found in token.");
            }

            var booking = new Booking
            {
                UserId = userId,
                RoomId = dto.RoomId,
                CheckInDate = dto.CheckInDate.ToUniversalTime(),
                CheckOutDate = dto.CheckOutDate.ToUniversalTime(),
                BookingStatus = dto.BookingStatus ?? BookingStatus.Pending
            };

            /*
            if (dto.SelectedServices != null)
            {
            foreach (var serviceId in dto.SelectedServices)
            {
                var serviceOrder = new ServiceOrder
                {
                    ServiceId = serviceId,
                    UserId = booking.UserId,
                    RoomId = booking.RoomId,
                    OrderDate = DateOnly.FromDateTime(DateTime.UtcNow.Date),
                    OrderStatus = "Pending"
                };
                _context.ServiceOrders.Add(serviceOrder);
            }
            }
            */

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] BookingUpdateDTO dto)
        {
            var existingBooking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id);

            if (existingBooking == null)
            {
                return NotFound();
            }

            // Обновляем только необходимые поля
            existingBooking.BookingStatus = dto.BookingStatus;
            existingBooking.CheckInDate = dto.CheckInDate;
            existingBooking.CheckOutDate = dto.CheckOutDate;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Получает все бронирования пользователя для конкретного номера
        /// </summary>
        [Authorize]
        [HttpGet("user/room/{roomId}")]
        public async Task<IActionResult> GetUserRoomBookings(Guid roomId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                    throw new InvalidOperationException("User ID not found"));

                var bookings = await _context.Bookings
                    .Where(b => b.UserId == userId && b.RoomId == roomId)
                    .Select(b => new
                    {
                        b.Id,
                        b.CheckInDate,
                        b.CheckOutDate,
                        Status = b.BookingStatusString,
                        CanReview = b.CheckOutDate < DateTime.UtcNow &&
                                  !_context.Reviews.Any(r => r.BookingId == b.Id)
                    })
                    .ToListAsync();

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user room bookings");
                return StatusCode(500, new { error = "Ошибка при получении бронирований" });
            }
        }
    }
}

