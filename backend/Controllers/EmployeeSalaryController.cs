using Hotel_MVP.Data;
using Hotel_MVP.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_MVP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeSalaryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeSalaryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeSalary>> GetById(Guid id)
        {
            var emplSal = await _context.EmployeeSalaries.FirstOrDefaultAsync(x => x.Id == id);
            if (emplSal == null)
            {
                return NotFound();
            }
            return emplSal;
        }


        [HttpPost]
        public async Task<ActionResult<EmployeeSalary>> Create([FromBody] EmployeeSalary emplSal)
        {
            _context.EmployeeSalaries.Add(emplSal);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = emplSal.Id }, emplSal);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] EmployeeSalary emplSal)
        {
            if (id != emplSal.Id)
            {
                return BadRequest();
            }

            _context.Entry(emplSal).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var emplSal = await _context.EmployeeSalaries.FirstOrDefaultAsync(x => x.Id == id);
            if (emplSal == null)
            {
                return NotFound();
            }

            _context.EmployeeSalaries.Remove(emplSal);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

