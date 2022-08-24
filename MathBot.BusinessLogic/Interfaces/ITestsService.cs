using MathBot.Common.DTO;
using MathBot.Model.DatabaseModels;
using MathBot.Model.Enum;

namespace MathBot.BusinessLogic.Interfaces
{
    public interface ITestsService
    {   
        Test Get(int id);
        TestDTO GetDTO(int id);
        bool IsRunningTest(long ChatId);
        bool IsRunningProduction(long ChatId);

        Test Create(long testId, DateTime startTime, TestType type);

        Test GetLastTest(long ChatId);

        void AddExercise(Test test, Exercise exercise);

        void AddCounts(Test test);

        void AddRightAnswer(Test test);
    }
}
