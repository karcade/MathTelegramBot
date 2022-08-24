using MathBot.Common.DTO;
using MathBot.Model.DatabaseModels;

namespace MathBot.BusinessLogic.Interfaces
{
    public interface IUsersService
    {
        RegisterUserDTO GetDTO(int id);
        User Get(int id);
        User GetByTelegramId(long chatId);
        void AddTest(long chatId, Test test);
        List<User> GetAll();
        void Create(RegisterUserDTO userDTO);
        void Delete(int id);
    }
}
