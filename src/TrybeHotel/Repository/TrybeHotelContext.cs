using Microsoft.EntityFrameworkCore;
using TrybeHotel.Models;

namespace TrybeHotel.Repository;
public class TrybeHotelContext : DbContext, ITrybeHotelContext
{
    public DbSet<City> Cities { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public TrybeHotelContext(DbContextOptions<TrybeHotelContext> options) : base(options)
    {
    }
    public TrybeHotelContext() { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // SQL Server 

            // var connectionString = @"
            //     Server=localhost;
            //     Database=TrybeHotel;
            //     User=SA;
            //     Password=TrybeHotel12!;
            //     TrustServerCertificate=True";
            // optionsBuilder.UseSqlServer(connectionString);

            // MySQL

            var connectionString = @"
                Server=localhost;
                Port=3308;
                Database=TrybeHotel;
                User Id=root;
                Password=123456;";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), null);
        }
    }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        //  City
        var mbCity = mb.Entity<City>();

        mbCity
            .HasKey(c => c.CityId);
        mbCity
            .Property(c => c.Name)
            .IsRequired();
        mbCity
            .Property(c => c.State)
            .IsRequired();

        // Hotel
        var mbHotel = mb.Entity<Hotel>();

        mbHotel
            .HasKey(h => h.HotelId);
        mbHotel
            .Property(h => h.Name)
            .IsRequired();
        mbHotel
            .Property(h => h.Address)
            .IsRequired();
        mbHotel
            .HasOne(h => h.City)
            .WithMany(h => h.Hotels)
            .HasForeignKey(h => h.CityId);

        // Room
        var mbRoom = mb.Entity<Room>();

        mbRoom
            .HasKey(r => r.RoomId);
        mbRoom
            .Property(r => r.Name)
            .IsRequired();
        mbRoom
            .Property(r => r.Capacity)
            .IsRequired();
        mbRoom
            .Property(r => r.Image)
            .IsRequired();
        mbRoom
            .HasOne(r => r.Hotel)
            .WithMany(r => r.Rooms)
            .HasForeignKey(r => r.HotelId);

        // User
        var mbUser = mb.Entity<User>();

        mbUser
            .HasKey(u => u.UserId);
        mbUser
            .Property(u => u.Name)
            .IsRequired();
        mbUser
            .Property(u => u.Email)
            .IsRequired();
        mbUser
            .Property(u => u.Password)
            .IsRequired();
        mbUser
            .Property(u => u.UserType)
            .IsRequired();

        // Booking
        var mbBooking = mb.Entity<Booking>();

        mbBooking
            .HasKey(b => b.BookingId);
        mbBooking
            .Property(b => b.CheckIn)
            .IsRequired();
        mbBooking
            .Property(b => b.CheckOut)
            .IsRequired();
        mbBooking
            .Property(b => b.GuestQuant)
            .IsRequired();
    }
}