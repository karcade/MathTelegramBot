using MathBot.Common.DTO;
using MathBot.Model.DatabaseModels;

namespace MathBot.BusinessLogic.Interfaces
{
    public interface INumbersService
    {
        Number Get(int id);
        NumberDTO GetDTO(int id);

        //Number Create();
        Number Generate(int exerciseId);
        int GenerateNumber();
    }
}
