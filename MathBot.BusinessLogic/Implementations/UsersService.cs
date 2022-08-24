using AutoMapper;
using MathBot.BusinessLogic.Interfaces;
using MathBot.Common.DTO;
using MathBot.Model;
using MathBot.Model.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace MathBot.BusinessLogic.Implementations
{
    public class UsersService : IUsersService
    {
        private ApplicationContext _context;
        private readonly IMapper _mapper;

        public UsersService(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddTest(long chatId, Test test)
        {
            User user = GetByTelegramId(chatId);
            user.Tests.Add(test);
            test.UserId = user.Id;
            _context.SaveChanges();
        }

        public void Create(RegisterUserDTO userDTO)
        {
            var user = _mapper.Map<RegisterUserDTO, User>(userDTO);

            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            _context.Users.Remove(Get(id));
            _context.SaveChanges();
        }

        public User Get(int id)
        {
            if (!_context.Users.Any(x => x.Id == id)) throw new Exception("User not found");
            return _context.Users.Where(x => x.Id == id).FirstOrDefault();
        }

        public User GetByTelegramId(long chatId)
        {
            if (!_context.Users.Any(x => x.ChatId == chatId)) return null;
            return _context.Users.Where(x => x.ChatId == chatId).FirstOrDefault();
        }

        public List<User> GetAll()
        {
            if(_context.Users == null) return null;
            return _context.Users.AsNoTracking().ToList();
        }

        public RegisterUserDTO GetDTO(int id)
        {
            var user = new User();

            user = _context.Users.AsNoTracking().FirstOrDefault(m => m.Id == id);

            RegisterUserDTO userDTO = _mapper.Map<RegisterUserDTO>(user);
            return userDTO;
        }

    }
}
