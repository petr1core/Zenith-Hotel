using Hotel_MVP.Data;
using Hotel_MVP.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_MVP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceOrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServiceOrderController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceOrder>> GetById(Guid id)
        {
            var serviceOrder = await _context.ServiceOrders.FirstOrDefaultAsync(x => x.Id == id);
            if (serviceOrder == null)
            {
                return NotFound();
            }
            return serviceOrder;
        }


        [HttpPost]
        public async Task<ActionResult<ServiceOrder>> Create([FromBody] ServiceOrder serviceOrder)
        {
            _context.ServiceOrders.Add(serviceOrder);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = serviceOrder.Id }, serviceOrder);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ServiceOrder serviceOrder)
        {
            if (id != serviceOrder.Id)
            {
                return BadRequest();
            }

            _context.Entry(serviceOrder).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var serviceOrder = await _context.ServiceOrders.FirstOrDefaultAsync(x => x.Id == id);
            if (serviceOrder == null)
            {
                return NotFound();
            }

            _context.ServiceOrders.Remove(serviceOrder);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

