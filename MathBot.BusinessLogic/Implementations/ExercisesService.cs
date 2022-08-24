using AutoMapper;
using MathBot.BusinessLogic.Interfaces;
using MathBot.Common.DTO;
using MathBot.Model;
using MathBot.Model.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace MathBot.BusinessLogic.Implementations
{
    public class ExercisesService: IExercisesService
    {
        //private ITestsService _testsService;
        private INumbersService _numbersService;
        private ApplicationContext _context;
        private readonly IMapper _mapper; 

        public ExercisesService(ApplicationContext context, IMapper mapper, INumbersService numbersService) //, NumbersService numbersService, ITestsService testsService
        {
            _context = context;
            _mapper = mapper;
            _numbersService = numbersService; 
        }

        public List<Number> GetNumbers(int exerciseId)
        {
            Console.WriteLine(" ---GetNumbers(int exerciseId) "+exerciseId);
            List<Number> Numbers = new List<Number>();
            for (int i = 0; i < 8; i++)
            {
                Numbers.Add(_numbersService.Generate(exerciseId));//
            }
            return Numbers;
        }

        public Exercise Create(int testId)//chatId need???
        {
            /*ExerciseDTO exerciseDTO = new ExerciseDTO();
            exerciseDTO.StartTime = DateTime.UtcNow;
            exerciseDTO.Test = _testsService.GetDTO(testId);
            var exercise = _mapper.Map<ExerciseDTO, Exercise>(exerciseDTO);*/

            Exercise exercise = new Exercise();
            exercise.StartTime = DateTime.UtcNow;
            exercise.TestId = testId;

            _context.Exercises.Add(exercise);
            _context.SaveChanges();//
            Console.WriteLine("-----------exercise.Id is "+exercise.Id);
            exercise.Numbers = GetNumbers(exercise.Id);

            _context.SaveChanges();

            return exercise;
        }

        public int Result(List<Number> numbers)
        {
            List<int> numbersValue=new List<int>(); 
            foreach(Number number in numbers)
            {
                numbersValue.Add(number.Value);
            }
            numbersValue.Sort();
            return numbersValue.ElementAt(numbers.Count() - 2);
        }

        public int NumberById(List<Number> numbers, int index)
        {
            List<int> numbersValue = new List<int>();
            foreach (Number number in numbers)
            {
                numbersValue.Add(number.Value);
            }
            return numbersValue[index];
        }

        public int GetUserAnswer(Exercise exercise)
        {
            List<int> numbersValue = new List<int>();
            foreach (Number number in exercise.Numbers)
            {
                numbersValue.Add(number.Value);
            }
            return numbersValue[(int)exercise.AnswerNumberId];
        }

        public Exercise GetLastExercise(int testId)
        {
            return _context.Exercises.OrderByDescending(t => t.StartTime).Where(x => x.TestId == testId).FirstOrDefault();
        }

        public void PutUserAnswer(Exercise exercise, int index)
        {
            exercise.AnswerNumberId = index;
            _context.SaveChanges();
        }

        public Exercise Get(int id)
        {
            if (!_context.Exercises.Any(x => x.Id == id)) return null;
            return _context.Exercises.Where(x => x.Id == id).FirstOrDefault();
        }

        public ExerciseDTO GetDTO(int id)
        {
            Exercise exercise = _context.Exercises.AsNoTracking().FirstOrDefault(m => m.Id == id);

            ExerciseDTO exerciseDTO = _mapper.Map<ExerciseDTO>(exercise);
            return exerciseDTO;
        }
    }
}
