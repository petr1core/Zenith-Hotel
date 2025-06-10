using System;
using System.Collections.Generic;
using Hotel_MVP;

namespace backend.DTO
{
    public class RoomWithImagesDto
    {
        public Guid Id { get; set; }
        public int RoomNumber { get; set; }
        public RoomType RoomType { get; set; }
        public RoomAvailability Availability { get; set; }
        public decimal RoomCharge { get; set; }
        public int Capacity { get; set; }
        public decimal Area { get; set; }
        public string? Description { get; set; }
        public CleaningStatus CleaningStatus { get; set; }
        public DateTime? LastCleaned { get; set; }
        public int Floor { get; set; }
        public List<RoomPhotoDto> Photos { get; set; } = new List<RoomPhotoDto>();
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }

    public class RoomPhotoDto
    {
        public Guid Id { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
        public string? Description { get; set; }
        public int OrderIndex { get; set; }
    }
}