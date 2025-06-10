using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hotel_MVP;
using Hotel_MVP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/room/{roomId}/images")]
    [ApiController]
    public class RoomPhotosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoomPhotosController(AppDbContext context)
        {
            _context = context;
        }

        // Получение всех изображений номера
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomPhoto>>> GetRoomPhotos(Guid roomId)
        {
            return await _context.RoomPhotos
                .Where(img => img.RoomId == roomId)
                .OrderBy(img => img.OrderIndex)
                .ToListAsync();
        }

        // Загрузка нового изображения
        [HttpPost]
        public async Task<ActionResult<RoomPhoto>> UploadRoomPhoto(
            Guid roomId,
            [FromForm] IFormFile file,
            [FromForm] int order)
        {
            Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");
            Console.WriteLine($"Uploads Path: {Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads")}");
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("File not selected");

                // Сохраняем файл на сервере
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Сохраняем в БД
                var image = new RoomPhoto
                {
                    PhotoUrl = $"/uploads/{uniqueFileName}",
                    OrderIndex = order,
                    RoomId = roomId
                };

                _context.RoomPhotos.Add(image);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetRoomPhotos), new { roomId = roomId }, image);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{imageId}")]
        public async Task<IActionResult> DeleteImage(Guid roomId, Guid imageId)
        {
            var image = await _context.RoomPhotos.FindAsync(imageId);
            if (image == null) return NotFound();

            // Удаляем файл
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.PhotoUrl.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.RoomPhotos.Remove(image);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}