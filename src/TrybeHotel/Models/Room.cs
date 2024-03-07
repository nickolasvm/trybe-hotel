namespace TrybeHotel.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Room
{
    public int RoomId { get; set; }
    public string? Name { get; set; }
    public int Capacity { get; set; }
    public string? Image { get; set; }

    // Foreign key
    public int HotelId { get; set; }

    // Navigation
    public virtual Hotel? Hotel { get; set; }
    public virtual ICollection<Booking>? Bookings { get; set; }
}