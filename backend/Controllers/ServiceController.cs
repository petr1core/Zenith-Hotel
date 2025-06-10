using Hotel_MVP.Data;
using Hotel_MVP.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_MVP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServiceController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> GetById(Guid id)
        {
            var service = await _context.Services.FirstOrDefaultAsync(x => x.Id == id);
            if (service == null)
            {
                return NotFound();
            }
            return service;
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetActiveServices()
        {
            return Ok(await _context.Services
                .Where(s => s.IsActive)
                .ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult<Service>> Create([FromBody] Service service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = service.Id }, service);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Service service)
        {
            if (id != service.Id)
            {
                return BadRequest();
            }

            _context.Entry(service).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var service = await _context.Services.FirstOrDefaultAsync(x => x.Id == id);
            if (service == null)
            {
                return NotFound();
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

