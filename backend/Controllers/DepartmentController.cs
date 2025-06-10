using Hotel_MVP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hotel_MVP.Models.Entities;

namespace Hotel_MVP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DepartmentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetById(Guid id)
        {
            var dept = await _context.Departments.FirstOrDefaultAsync(x => x.Id == id);
            if (dept == null)
            {
                return NotFound();
            }
            return dept;
        }


        [HttpPost]
        public async Task<ActionResult<Department>> Create([FromBody] Department dept)
        {
            _context.Departments.Add(dept);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = dept.Id }, dept);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Department dept)
        {
            if (id != dept.Id)
            {
                return BadRequest();
            }

            _context.Entry(dept).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var dept = await _context.Departments.FirstOrDefaultAsync(x => x.Id == id);
            if (dept == null)
            {
                return NotFound();
            }

            _context.Departments.Remove(dept);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

