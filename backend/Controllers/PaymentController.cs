// using Hotel_MVP.Data;
// using Hotel_MVP.Models.Entities;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;

// namespace Hotel_MVP.Controllers
// {
//     [Route("api/[controller]")]
//     [ApiController]
//     public class PaymentController : ControllerBase
//     {
//         private readonly AppDbContext _context;

//         public PaymentController(AppDbContext context)
//         {
//             _context = context;
//         }

//         [HttpGet("{id}")]
//         public async Task<ActionResult<Payment>> GetById(Guid id)
//         {
//             var payment = await _context.Payments.FirstOrDefaultAsync(x => x.Id == id);
//             if (payment == null)
//             {
//                 return NotFound();
//             }
//             return payment;
//         }


//         [HttpPost]
//         public async Task<ActionResult<Payment>> Create([FromBody] Payment payment)
//         {
//             _context.Payments.Add(payment);
//             await _context.SaveChangesAsync();
//             return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
//         }

//         [HttpPut("{id}")]
//         public async Task<IActionResult> Update(Guid id, [FromBody] Payment payment)
//         {
//             if (id != payment.Id)
//             {
//                 return BadRequest();
//             }

//             _context.Entry(payment).State = EntityState.Modified;
//             await _context.SaveChangesAsync();
//             return NoContent();
//         }

//         [HttpDelete("{id}")]
//         public async Task<IActionResult> Delete(Guid id)
//         {
//             var payment = await _context.Payments.FirstOrDefaultAsync(x => x.Id == id);
//             if (payment == null)
//             {
//                 return NotFound();
//             }

//             _context.Payments.Remove(payment);
//             await _context.SaveChangesAsync();
//             return NoContent();
//         }
//     }
// }

