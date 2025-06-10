using backend.DTO;
using Hotel_MVP.Data;
using Hotel_MVP.Models.Entities;
using Hotel_MVP.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Hotel_MVP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RoomController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RoomController> _logger;

        public RoomController(AppDbContext context, ILogger<RoomController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Получает список всех номеров с пагинацией и сортировкой
        /// </summary>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(IEnumerable<Room>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Room>>> Get(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "RoomNumber",
            [FromQuery] bool ascending = true)
        {
            try
            {
                _logger.LogInformation("Getting rooms with pagination: page={Page}, pageSize={PageSize}, sortBy={SortBy}, ascending={Ascending}",
                    page, pageSize, sortBy, ascending);

                var query = _context.Rooms
                    .Include(r => r.Images)
                    .AsNoTracking();

                // Применяем сортировку
                query = sortBy.ToLower() switch
                {
                    "price" => ascending ? query.OrderBy(r => r.RoomCharge) : query.OrderByDescending(r => r.RoomCharge),
                    "number" => ascending ? query.OrderBy(r => r.RoomNumber) : query.OrderByDescending(r => r.RoomNumber),
                    "floor" => ascending ? query.OrderBy(r => r.Floor) : query.OrderByDescending(r => r.Floor),
                    _ => ascending ? query.OrderBy(r => r.RoomNumber) : query.OrderByDescending(r => r.RoomNumber)
                };

                var totalCount = await query.CountAsync();
                var rooms = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                Response.Headers.Add("X-Total-Pages", Math.Ceiling((double)totalCount / pageSize).ToString());

                _logger.LogInformation("Successfully retrieved {Count} rooms", rooms.Count);
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting rooms");
                return StatusCode(500, new { error = "Internal server error occurred while getting rooms" });
            }
        }

        /// <summary>
        /// Получает номер по ID
        /// </summary>
        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(typeof(Room), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RoomWithImagesDto>> GetById(Guid id)
        {
            try
            {
                _logger.LogInformation("Getting room with ID: {RoomId}", id);

                var roomDto = await _context.Rooms
                    .Where(r => r.Id == id)
                    .Include(r => r.Images)
                    .Select(r => new RoomWithImagesDto
                    {
                        Id = r.Id,
                        RoomNumber = r.RoomNumber,
                        RoomType = r.RoomType,
                        Availability = r.Availability,
                        RoomCharge = r.RoomCharge,
                        Capacity = r.Capacity,
                        Area = r.Area,
                        Description = r.Description,
                        CleaningStatus = r.CleaningStatus,
                        LastCleaned = r.LastCleaned,
                        Floor = r.Floor,
                        Photos = r.Images.Select(p => new RoomPhotoDto
                        {
                            Id = p.Id,
                            PhotoUrl = p.PhotoUrl,
                            IsPrimary = p.IsPrimary,
                            UploadDate = p.UploadDate,
                            Description = p.Description,
                            OrderIndex = p.OrderIndex
                        }).ToList(),
                        AverageRating = _context.Reviews
                                        .Where(rev => rev.RoomId == r.Id && rev.Status == ReviewStatus.Approved)
                                        .Average(rev => (double?)rev.Rating) ?? 0,
                        ReviewCount = _context.Reviews
                                        .Count(rev => rev.RoomId == r.Id && rev.Status == ReviewStatus.Approved)
                    })
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (roomDto == null)
                {
                    _logger.LogWarning("Room with ID {RoomId} not found", id);
                    return NotFound(new { error = $"Room with ID {id} not found" });
                }

                _logger.LogInformation("Successfully retrieved room with ID: {RoomId}", id);
                return Ok(roomDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting room {RoomId}", id);
                return StatusCode(500, new { error = "Internal server error occurred while getting room" });
            }
        }

        /// <summary>
        /// Получает список доступных номеров с фильтрацией
        /// </summary>
        [HttpGet("available")]
        [ProducesResponseType(typeof(IEnumerable<Room>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAvailableRooms([FromQuery] RoomFilterDTO filter)
        {
            try
            {
                _logger.LogInformation("Getting available rooms with filter: {Filter}",
                    JsonSerializer.Serialize(filter));

                var query = _context.Rooms
                    .Include(r => r.Images)
                    .AsNoTracking()
                    .Where(r => r.Availability == RoomAvailability.Free);

                if (filter.MinPrice.HasValue)
                    query = query.Where(r => r.RoomCharge >= filter.MinPrice.Value);
                if (filter.MaxPrice.HasValue)
                    query = query.Where(r => r.RoomCharge <= filter.MaxPrice.Value);
                if (filter.RoomType.HasValue)
                    query = query.Where(r => r.RoomType == filter.RoomType.Value);
                if (filter.MinCapacity.HasValue)
                    query = query.Where(r => r.Capacity >= filter.MinCapacity.Value);
                if (filter.Floor.HasValue)
                    query = query.Where(r => r.Floor == filter.Floor.Value);

                var rooms = await query
                    .OrderBy(r => r.RoomNumber)
                    .ToListAsync();

                _logger.LogInformation("Successfully retrieved {Count} available rooms", rooms.Count);
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting available rooms");
                return StatusCode(500, new { error = "Internal server error occurred while getting available rooms" });
            }
        }

        /// <summary>
        /// Получает список номеров с изображениями
        /// </summary>
        [HttpGet("with-images")]
        [ProducesResponseType(typeof(IEnumerable<RoomWithImagesDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<RoomWithImagesDto>>> GetRoomsWithImages(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12,
            [FromQuery] string? roomType = null,
            [FromQuery] int minRating = 0,
            [FromQuery] string? priceRange = null,
            [FromQuery] string? capacity = null)
        {
            try
            {
                _logger.LogInformation("Getting rooms with images: page={Page}, pageSize={PageSize}, roomType={RoomType}, priceRange={PriceRange}, capacity={Capacity}",
                    page, pageSize, roomType, priceRange, capacity);

                var query = _context.Rooms
                    .Include(r => r.Images)
                    .Include(r => r.Reviews)
                    .AsQueryable();

                // Применяем фильтры
                if (!string.IsNullOrEmpty(roomType))
                {
                    query = query.Where(r => r.RoomType == (RoomType)Enum.Parse(typeof(RoomType), roomType));
                }

                if (!string.IsNullOrEmpty(priceRange))
                {
                    switch (priceRange.ToLower())
                    {
                        case "low":
                            query = query.Where(r => r.RoomCharge <= 2000);
                            break;
                        case "medium":
                            query = query.Where(r => r.RoomCharge > 2000 && r.RoomCharge <= 4000);
                            break;
                        case "high":
                            query = query.Where(r => r.RoomCharge > 4000);
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(capacity) && capacity != "all")
                {
                    var capacityValue = int.Parse(capacity);
                    query = query.Where(r => r.Capacity >= capacityValue);
                }

                var totalCount = await query.CountAsync();
                _logger.LogInformation("Total rooms count after filtering: {TotalCount}", totalCount);

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                Response.Headers.Add("Access-Control-Expose-Headers", "X-Total-Count");

                var rooms = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(r => new RoomWithImagesDto
                    {
                        Id = r.Id,
                        RoomNumber = r.RoomNumber,
                        RoomType = r.RoomType,
                        Availability = r.Availability,
                        RoomCharge = r.RoomCharge,
                        Capacity = r.Capacity,
                        Area = r.Area,
                        Description = r.Description,
                        CleaningStatus = r.CleaningStatus,
                        LastCleaned = r.LastCleaned,
                        Floor = r.Floor,
                        Photos = r.Images.Select(p => new RoomPhotoDto
                        {
                            Id = p.Id,
                            PhotoUrl = p.PhotoUrl,
                            IsPrimary = p.IsPrimary,
                            UploadDate = p.UploadDate,
                            Description = p.Description,
                            OrderIndex = p.OrderIndex
                        }).ToList(),
                        AverageRating = _context.Reviews
                                        .Where(rev => rev.RoomId == r.Id && rev.Status == ReviewStatus.Approved)
                                        .Average(rev => (double?)rev.Rating) ?? 0,
                        ReviewCount = _context.Reviews
                                        .Count(rev => rev.RoomId == r.Id && rev.Status == ReviewStatus.Approved)
                    })
                    .ToListAsync();

                _logger.LogInformation("Successfully retrieved {Count} rooms with images", rooms.Count);
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting rooms with images");
                return StatusCode(500, new { error = "Internal server error occurred while getting rooms with images" });
            }
        }

        /// <summary>
        /// Создает новый номер
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Room), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Room>> Create([FromBody] Hotel_MVP.DTO.CreateRoomDTO dto)
        {
            try
            {
                _logger.LogInformation("Creating new room: {Room}", JsonSerializer.Serialize(dto));

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state while creating room");
                    return BadRequest(new { error = "Invalid model state", details = ModelState });
                }

                if (await _context.Rooms.AnyAsync(r => r.RoomNumber == dto.RoomNumber))
                {
                    _logger.LogWarning("Room with number {RoomNumber} already exists", dto.RoomNumber);
                    return BadRequest(new { error = $"Room with number {dto.RoomNumber} already exists" });
                }

                var room = new Room
                {
                    Id = Guid.NewGuid(),
                    RoomNumber = dto.RoomNumber,
                    RoomType = dto.RoomType,
                    Availability = dto.Availability,
                    RoomCharge = dto.RoomCharge,
                    Capacity = dto.Capacity,
                    Area = dto.Area,
                    Description = dto.Description,
                    CleaningStatus = dto.CleaningStatus,
                    LastCleaned = dto.LastCleaned,
                    Floor = dto.Floor,
                    Images = new List<RoomPhoto>()
                };

                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();

                if (dto.Images != null && dto.Images.Count > 0)
                {
                    foreach (var img in dto.Images)
                    {
                        var photo = new RoomPhoto
                        {
                            Id = Guid.NewGuid(),
                            PhotoUrl = img.PhotoUrl,
                            IsPrimary = img.IsPrimary,
                            UploadDate = img.UploadDate,
                            Description = img.Description,
                            OrderIndex = img.OrderIndex,
                            RoomId = room.Id
                        };
                        _context.RoomPhotos.Add(photo);
                        room.Images.Add(photo);
                    }
                    await _context.SaveChangesAsync();
                }

                var createdRoom = await _context.Rooms
                    .Include(r => r.Images)
                    .FirstOrDefaultAsync(r => r.Id == room.Id);

                _logger.LogInformation("Successfully created room with ID: {RoomId}", room.Id);
                return CreatedAtAction(nameof(GetById), new { id = room.Id }, createdRoom);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating room");
                return StatusCode(500, new { error = "Internal server error occurred while creating room" });
            }
        }

        /// <summary>
        /// Обновляет существующий номер
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromBody] Room room)
        {
            try
            {
                _logger.LogInformation("Updating room with ID: {RoomId}", id);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state while updating room");
                    return BadRequest(new { error = "Invalid model state", details = ModelState });
                }

                if (id != room.Id)
                {
                    _logger.LogWarning("Room ID mismatch: {Id} != {RoomId}", id, room.Id);
                    return BadRequest(new { error = "Room ID mismatch" });
                }

                var existingRoom = await _context.Rooms.FindAsync(id);
                if (existingRoom == null)
                {
                    _logger.LogWarning("Room with ID {RoomId} not found", id);
                    return NotFound(new { error = $"Room with ID {id} not found" });
                }

                if (room.RoomNumber != existingRoom.RoomNumber &&
                    await _context.Rooms.AnyAsync(r => r.RoomNumber == room.RoomNumber))
                {
                    _logger.LogWarning("Room with number {RoomNumber} already exists", room.RoomNumber);
                    return BadRequest(new { error = $"Room with number {room.RoomNumber} already exists" });
                }

                _context.Entry(existingRoom).CurrentValues.SetValues(room);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully updated room with ID: {RoomId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating room {RoomId}", id);
                return StatusCode(500, new { error = "Internal server error occurred while updating room" });
            }
        }

        /// <summary>
        /// Удаляет номер
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting room with ID: {RoomId}", id);

                var room = await _context.Rooms
                    .Include(r => r.Images)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (room == null)
                {
                    _logger.LogWarning("Room with ID {RoomId} not found", id);
                    return NotFound(new { error = $"Room with ID {id} not found" });
                }

                _context.RoomPhotos.RemoveRange(room.Images);
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted room with ID: {RoomId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting room {RoomId}", id);
                return StatusCode(500, new { error = "Internal server error occurred while deleting room" });
            }
        }

        /// <summary>
        /// Получает список доступных номеров с учетом дат заезда и выезда
        /// </summary>
        [HttpGet("available-by-dates")]
        [ProducesResponseType(typeof(IEnumerable<RoomWithImagesDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAvailableRoomsByDates([FromQuery] RoomAvailabilityFilterDTO filter)
        {
            try
            {
                _logger.LogInformation("Server timezone info - Current: {CurrentTZ}, Local: {LocalTZ}, UTC Offset: {UTCOffset}",
                    TimeZoneInfo.Local.DisplayName,
                    TimeZoneInfo.Local.StandardName,
                    TimeZoneInfo.Local.BaseUtcOffset);

                _logger.LogInformation("Received dates from client - Raw CheckIn: {CheckIn}, Raw CheckOut: {CheckOut}",
                    filter.CheckInDate, filter.CheckOutDate);

                // Нормализуем даты к началу дня
                var checkInDate = filter.CheckInDate.Date;
                var checkOutDate = filter.CheckOutDate.Date;
                var today = DateTime.Today;

                _logger.LogInformation("After Date normalization - CheckIn: {CheckIn}, CheckOut: {CheckOut}, Today: {Today}",
                    checkInDate, checkOutDate, today);

                if (checkInDate >= checkOutDate)
                {
                    _logger.LogWarning("Invalid date range - CheckIn >= CheckOut");
                    return BadRequest(new { error = "Дата заезда должна быть раньше даты выезда" });
                }

                if (checkInDate < today)
                {
                    _logger.LogWarning("Invalid check-in date - CheckIn: {CheckIn} < Today: {Today}", checkInDate, today);
                    return BadRequest(new { error = "Дата заезда не может быть в прошлом" });
                }

                // Получаем все номера с их бронированиями
                var query = _context.Rooms
                    .Include(r => r.Images)
                    .AsNoTracking();

                // Применяем базовые фильтры
                if (filter.MinPrice.HasValue)
                    query = query.Where(r => r.RoomCharge >= filter.MinPrice.Value);
                if (filter.MaxPrice.HasValue)
                    query = query.Where(r => r.RoomCharge <= filter.MaxPrice.Value);
                if (filter.RoomType.HasValue)
                    query = query.Where(r => r.RoomType == filter.RoomType.Value);
                if (filter.MinCapacity.HasValue)
                    query = query.Where(r => r.Capacity >= filter.MinCapacity.Value);
                if (filter.Floor.HasValue)
                    query = query.Where(r => r.Floor == filter.Floor.Value);

                // Получаем все бронирования для этих номеров в указанный период
                var bookings = await _context.Bookings
                    .Where(b =>
                        b.CheckOutDate.Date > checkInDate &&
                        b.CheckInDate.Date < checkOutDate)
                    .ToListAsync();

                // Фильтруем бронирования по статусу в памяти
                var activeBookings = bookings.Where(b => b.BookingStatusString != BookingStatus.Cancelled.ToString()).ToList();

                _logger.LogInformation("Found {Count} overlapping bookings for period {CheckIn} - {CheckOut}",
                    activeBookings.Count, checkInDate, checkOutDate);

                // Фильтруем номера, исключая те, которые имеют пересекающиеся бронирования
                var bookedRoomIds = activeBookings.Select(b => b.RoomId).Distinct();
                var availableRoomsQuery = query.Where(r => !bookedRoomIds.Contains(r.Id));

                // Получаем общее количество доступных номеров
                var totalCount = await availableRoomsQuery.CountAsync();

                // Применяем пагинацию
                var page = Math.Max(1, filter.Page);
                var pageSize = Math.Max(1, Math.Min(50, filter.PageSize));
                var rooms = await availableRoomsQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(r => new RoomWithImagesDto
                    {
                        Id = r.Id,
                        RoomNumber = r.RoomNumber,
                        RoomType = r.RoomType,
                        Availability = r.Availability,
                        RoomCharge = r.RoomCharge,
                        Capacity = r.Capacity,
                        Area = r.Area,
                        Description = r.Description,
                        CleaningStatus = r.CleaningStatus,
                        LastCleaned = r.LastCleaned,
                        Floor = r.Floor,
                        Photos = r.Images.Select(i => new RoomPhotoDto
                        {
                            Id = i.Id,
                            PhotoUrl = i.PhotoUrl,
                            IsPrimary = i.IsPrimary,
                            UploadDate = i.UploadDate,
                            Description = i.Description,
                            OrderIndex = i.OrderIndex
                        }).ToList(),
                        AverageRating = _context.Reviews
                                        .Where(rev => rev.RoomId == r.Id && rev.Status == ReviewStatus.Approved)
                                        .Average(rev => (double?)rev.Rating) ?? 0,
                        ReviewCount = _context.Reviews
                                        .Count(rev => rev.RoomId == r.Id && rev.Status == ReviewStatus.Approved)
                    })
                    .ToListAsync();

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                Response.Headers.Add("Access-Control-Expose-Headers", "X-Total-Count");

                _logger.LogInformation("Found {Count} available rooms for dates {CheckIn} - {CheckOut}",
                    rooms.Count, checkInDate, checkOutDate);
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting available rooms by dates: {Error}", ex.Message);
                return StatusCode(500, new { error = "Произошла ошибка при получении доступных номеров" });
            }
        }

        /// <summary>
        /// Проверяет доступность номеров на указанные даты
        /// </summary>
        [HttpGet]
        [Route("availability")]
        [ProducesResponseType(typeof(List<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckRoomsAvailability(
            [FromQuery(Name = "checkIn")] DateTime checkIn,
            [FromQuery(Name = "checkOut")] DateTime checkOut)
        {
            try
            {
                _logger.LogInformation("Checking rooms availability for dates: {CheckIn} - {CheckOut}",
                    checkIn, checkOut);

                // Преобразуем даты в UTC
                var checkInUtc = DateTime.SpecifyKind(checkIn.Date, DateTimeKind.Utc);
                var checkOutUtc = DateTime.SpecifyKind(checkOut.Date, DateTimeKind.Utc);

                if (checkInUtc >= checkOutUtc)
                {
                    return BadRequest(new { error = "Дата заезда должна быть раньше даты выезда" });
                }

                if (checkInUtc < DateTime.UtcNow.Date)
                {
                    return BadRequest(new { error = "Дата заезда не может быть в прошлом" });
                }

                // Получаем занятые номера на указанные даты
                var bookedRooms = await _context.Bookings
                    .Where(b =>
                        b.BookingStatusString != BookingStatus.Cancelled.ToString() &&
                        // Проверяем пересечение периодов
                        // Бронирование пересекается, если:
                        // 1. Начало бронирования находится в запрашиваемом периоде
                        // 2. ИЛИ конец бронирования находится в запрашиваемом периоде
                        // 3. ИЛИ бронирование полностью охватывает запрашиваемый период
                        ((b.CheckInDate.Date <= checkOutUtc && b.CheckInDate.Date >= checkInUtc) || // Начало в периоде
                         (b.CheckOutDate.Date >= checkInUtc && b.CheckOutDate.Date <= checkOutUtc) || // Конец в периоде
                         (b.CheckInDate.Date <= checkInUtc && b.CheckOutDate.Date >= checkOutUtc))) // Охватывает период
                    .Select(b => b.RoomId)
                    .Distinct()
                    .ToListAsync();

                _logger.LogInformation("Found {Count} booked rooms for period {CheckIn} - {CheckOut}",
                    bookedRooms.Count, checkInUtc, checkOutUtc);

                return Ok(bookedRooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking rooms availability");
                return StatusCode(500, new { error = "Ошибка при проверке доступности номеров" });
            }
        }

        /// <summary>
        /// Получает список номеров с учетом фильтров
        /// </summary>
        [HttpGet]
        [Route("filter")]
        [ProducesResponseType(typeof(IEnumerable<RoomWithImagesDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFilteredRooms(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] RoomType? roomType = null,
            [FromQuery] int? minCapacity = null,
            [FromQuery] int? maxCapacity = null,
            [FromQuery] int? floor = null,
            [FromQuery] string? excludeRoomIds = null)
        {
            try
            {
                var query = _context.Rooms
                    .Include(r => r.Images)
                    .AsNoTracking();

                // Применяем фильтры
                if (minPrice.HasValue)
                    query = query.Where(r => r.RoomCharge >= minPrice.Value);
                if (maxPrice.HasValue)
                    query = query.Where(r => r.RoomCharge <= maxPrice.Value);
                if (roomType.HasValue)
                    query = query.Where(r => r.RoomType == roomType.Value);
                if (minCapacity.HasValue)
                    query = query.Where(r => r.Capacity >= minCapacity.Value);
                if (maxCapacity.HasValue)
                    query = query.Where(r => r.Capacity <= maxCapacity.Value);
                if (floor.HasValue)
                    query = query.Where(r => r.Floor == floor.Value);
                if (!string.IsNullOrEmpty(excludeRoomIds))
                {
                    var excludeIds = excludeRoomIds.Split(',')
                        .Where(id => Guid.TryParse(id, out _))
                        .Select(id => Guid.Parse(id))
                        .ToList();

                    if (excludeIds.Any())
                        query = query.Where(r => !excludeIds.Contains(r.Id));
                }

                var totalCount = await query.CountAsync();

                var rooms = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(r => new RoomWithImagesDto
                    {
                        Id = r.Id,
                        RoomNumber = r.RoomNumber,
                        RoomType = r.RoomType,
                        Availability = r.Availability,
                        RoomCharge = r.RoomCharge,
                        Capacity = r.Capacity,
                        Area = r.Area,
                        Description = r.Description,
                        CleaningStatus = r.CleaningStatus,
                        LastCleaned = r.LastCleaned,
                        Floor = r.Floor,
                        Photos = r.Images.Select(i => new RoomPhotoDto
                        {
                            Id = i.Id,
                            PhotoUrl = i.PhotoUrl,
                            IsPrimary = i.IsPrimary,
                            UploadDate = i.UploadDate,
                            Description = i.Description,
                            OrderIndex = i.OrderIndex
                        }).ToList(),
                        AverageRating = _context.Reviews
                                        .Where(rev => rev.RoomId == r.Id && rev.Status == ReviewStatus.Approved)
                                        .Average(rev => (double?)rev.Rating) ?? 0,
                        ReviewCount = _context.Reviews
                                        .Count(rev => rev.RoomId == r.Id && rev.Status == ReviewStatus.Approved)
                    })
                    .ToListAsync();

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                Response.Headers.Add("Access-Control-Expose-Headers", "X-Total-Count");

                return Ok(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting filtered rooms");
                return StatusCode(500, new { error = "Ошибка при получении отфильтрованных номеров" });
            }
        }
    }
}

