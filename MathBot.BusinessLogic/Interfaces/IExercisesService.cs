using MathBot.Common.DTO;
using MathBot.Model.DatabaseModels;

namespace MathBot.BusinessLogic.Interfaces
{
    public interface IExercisesService
    {
        Exercise Get(int id);
        ExerciseDTO GetDTO(int id);
        List<Number> GetNumbers(int exerciseId);
        int Result(List<Number> numbers);
        int NumberById(List<Number> numbers, int index);
        void PutUserAnswer(Exercise exercise, int index);
        int GetUserAnswer(Exercise exercise);
        Exercise Create(int testId);
        Exercise GetLastExercise(int testId);
    }
}
