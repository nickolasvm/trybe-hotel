namespace TrybeHotel.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Hotel
{
    public int HotelId { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }

    // Foreign key
    public int CityId { get; set; }

    // Navigation
    public virtual City? City { get; set; }
    public virtual ICollection<Room>? Rooms { get; set; }
}