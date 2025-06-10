using Hotel_MVP.Data;
using Hotel_MVP.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_MVP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<List<Employee>> Get()
        {
            return await _context.Employees.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetById(Guid id)
        {
            var empl = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (empl == null)
            {
                return NotFound();
            }
            return empl;
        }


        [HttpPost]
        public async Task<ActionResult<Employee>> Create([FromBody] Employee empl)
        {
            _context.Employees.Add(empl);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = empl.Id }, empl);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Employee empl)
        {
            if (id != empl.Id)
            {
                return BadRequest();
            }

            _context.Entry(empl).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var empl = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (empl == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(empl);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

