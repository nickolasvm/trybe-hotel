using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class UserRepository : IUserRepository
    {
        protected readonly ITrybeHotelContext _context;
        public UserRepository(ITrybeHotelContext context)
        {
            _context = context;
        }
        public UserDto GetUserById(int userId)
        {
            throw new NotImplementedException();
        }

        public UserDto Login(LoginDto login)
        {
            var user = _context.Users.Where(user => user.Email == login.Email).FirstOrDefault();

            if (user == null || user.Password != login.Password) throw new InvalidOperationException("Incorrect e-mail or password");

            return new UserDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                UserType = user.UserType
            };
        }

        public UserDto Add(UserDtoInsert user)
        {
            bool userExist = _context.Users.Any(u => u.Email == user.Email);
            if (userExist) throw new Exception("User email already exists");

            var newUser = _context.Users.Add(new User
            {
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                UserType = "client"
            });

            _context.SaveChanges();

            return new UserDto
            {
                UserId = newUser.Entity.UserId,
                Name = newUser.Entity.Name,
                Email = newUser.Entity.Email,
                UserType = newUser.Entity.UserType,
            };
        }

        public UserDto GetUserByEmail(string userEmail)
        {
            var user = _context.Users.Where(user => user.Email == userEmail).FirstOrDefault();

            if (user == null) throw new InvalidOperationException("E-mail not found");

            return new UserDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                UserType = user.UserType,
            };
        }

        public IEnumerable<UserDto> GetUsers()
        {
            var users = _context.Users.Select(user => new UserDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                UserType = user.UserType,
            });

            return users.ToList();
        }

    }
}