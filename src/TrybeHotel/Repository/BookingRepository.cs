using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class BookingRepository : IBookingRepository
    {
        protected readonly ITrybeHotelContext _context;
        public BookingRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        public BookingResponse Add(BookingDtoInsert booking, string email)
        {
            var room = GetRoomById(booking.RoomId);
            if (booking.GuestQuant > room.Capacity)
            {
                throw new BadHttpRequestException("Guest quantity over room capacity");
            }

            var userId = _context.Users.First(x => x.Email == email)!.UserId;

            var newBooking = _context.Bookings.Add(new Booking
            {
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                GuestQuant = booking.GuestQuant,
                RoomId = booking.RoomId,
                UserId = userId,
            });
            _context.SaveChanges();

            var hotel = _context.Hotels.First(x => x.HotelId == room.HotelId);
            var city = _context.Cities.First(x => x.CityId == hotel.CityId);

            return new BookingResponse
            {
                BookingId = newBooking.Entity.BookingId,
                CheckIn = newBooking.Entity.CheckIn,
                CheckOut = newBooking.Entity.CheckOut,
                GuestQuant = newBooking.Entity.GuestQuant,
                Room = new RoomDto
                {
                    RoomId = room.RoomId,
                    Name = room.Name,
                    Capacity = room.Capacity,
                    Image = room.Image,
                    Hotel = new HotelDto
                    {
                        HotelId = hotel.HotelId,
                        Name = hotel.Name,
                        Address = hotel.Address,
                        CityId = hotel.CityId,
                        CityName = city.Name,
                        State = city.State,
                    }
                }
            };
        }

        public BookingResponse GetBooking(int bookingId, string email)
        {
            var booking = _context.Bookings.First(x => x.BookingId == bookingId);
            var userId = _context.Users.First(x => x.Email == email)!.UserId;

            if (booking.UserId != userId) throw new UnauthorizedAccessException();

            var room = GetRoomById(booking.RoomId);
            var hotel = _context.Hotels.First(x => x.HotelId == room.HotelId);
            var city = _context.Cities.First(x => x.CityId == hotel.CityId);

            return new BookingResponse
            {
                BookingId = booking.BookingId,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                GuestQuant = booking.GuestQuant,
                Room = new RoomDto
                {
                    RoomId = room.RoomId,
                    Name = room.Name,
                    Capacity = room.Capacity,
                    Image = room.Image,
                    Hotel = new HotelDto
                    {
                        HotelId = hotel.HotelId,
                        Name = hotel.Name,
                        Address = hotel.Address,
                        CityId = hotel.CityId,
                        CityName = city.Name,
                        State = city.State,
                    }
                }
            };
        }

        public Room GetRoomById(int RoomId)
        {
            return _context.Rooms.First(x => x.RoomId == RoomId);
        }

    }

}