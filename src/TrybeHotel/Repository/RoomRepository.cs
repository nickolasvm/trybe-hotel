using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class RoomRepository : IRoomRepository
    {
        protected readonly ITrybeHotelContext _context;
        public RoomRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        public IEnumerable<RoomDto> GetRooms(int HotelId)
        {
            var rooms =
                from room in _context.Rooms
                where room.HotelId == HotelId
                select new RoomDto
                {
                    RoomId = room.RoomId,
                    Name = room.Name,
                    Capacity = room.Capacity,
                    Image = room.Image,

                    Hotel = new HotelDto
                    {
                        HotelId = room.HotelId,
                        Name = room.Hotel!.Name,
                        Address = room.Hotel!.Address,
                        CityId = room.Hotel!.CityId,
                        CityName = room.Hotel!.City!.Name,
                        State = room.Hotel!.City!.State,
                    }
                };

            return rooms.ToList();
        }

        public RoomDto AddRoom(Room room)
        {
            _context.Rooms.Add(room);
            _context.SaveChanges();

            Hotel hotel = _context.Hotels.First(hotel => hotel.HotelId == room.HotelId);

            return new RoomDto
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
                    CityName = _context.Cities.First(city => city.CityId == hotel.CityId).Name,
                    State = _context.Cities.First(city => city.CityId == hotel.CityId).State,
                }
            };
        }

        public void DeleteRoom(int RoomId)
        {
            Room roomToDelete = _context.Rooms.First(room => room.RoomId == RoomId);
            _context.Rooms.Remove(roomToDelete);
            _context.SaveChanges();
        }
    }
}