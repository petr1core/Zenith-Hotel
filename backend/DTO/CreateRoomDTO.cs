namespace Hotel_MVP.DTO;


public class CreateRoomDTO
{
    public Guid Id { get; set; }
    public int RoomNumber { get; set; }
    public required RoomType RoomType { get; set; }
    public RoomAvailability Availability { get; set; }
    public decimal RoomCharge { get; set; }
    public int Capacity { get; set; }
    public decimal Area { get; set; }
    public string? Description { get; set; }
    public CleaningStatus CleaningStatus { get; set; }
    public DateTime? LastCleaned { get; set; }
    public int Floor { get; set; }
    public List<CreateRoomPhotoDTO> Images { get; set; }
}

public class CreateRoomPhotoDTO
{
    public Guid Id { get; set; }
    public string PhotoUrl { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime UploadDate { get; set; }
    public string Description { get; set; }
    public int OrderIndex { get; set; }
    public Guid RoomId { get; set; }
}