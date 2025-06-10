using Hotel_MVP.Data;
using Hotel_MVP.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_MVP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomRatingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoomRatingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomRating>> GetById(Guid id)
        {
            var roomRating = await _context.RoomRatings.FirstOrDefaultAsync(x => x.Id == id);
            if (roomRating == null)
            {
                return NotFound();
            }
            return roomRating;
        }


        [HttpPost]
        public async Task<ActionResult<RoomRating>> Create([FromBody] RoomRating roomRating)
        {
            _context.RoomRatings.Add(roomRating);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = roomRating.Id }, roomRating);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] RoomRating roomRating)
        {
            if (id != roomRating.Id)
            {
                return BadRequest();
            }

            _context.Entry(roomRating).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var roomRating = await _context.RoomRatings.FirstOrDefaultAsync(x => x.Id == id);
            if (roomRating == null)
            {
                return NotFound();
            }

            _context.RoomRatings.Remove(roomRating);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

