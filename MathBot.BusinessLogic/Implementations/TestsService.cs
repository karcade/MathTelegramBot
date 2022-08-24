using AutoMapper;
using MathBot.BusinessLogic.Interfaces;
using MathBot.Common.DTO;
using MathBot.Model;
using MathBot.Model.DatabaseModels;
using MathBot.Model.Enum;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace MathBot.BusinessLogic.Implementations
{
    public class TestsService:ITestsService
    {
        //private IExercisesService _exercisesService;
        private IUsersService _usersService;
        private ApplicationContext _context;
        private readonly IMapper _mapper;

        public TestsService(ApplicationContext context, IMapper mapper, IUsersService usersService) //,, ExercisesService exercisesServic IUsersService usersService
        {
            _context = context;
            _mapper = mapper;
            _usersService = usersService; 
        }

        public bool IsRunningTest(long ChatId)
        {
            bool isActiveTest = false;
            try
            {
                User user = _usersService.GetByTelegramId(ChatId); //(y => y.StartTime <= DateTime.UtcNow.AddSeconds(20)) != null) 
            
                if (_context.Tests.Any(x => x.UserId == user.Id))
                {
                    DateTime startime = _context.Tests.Where(x => x.UserId == user.Id).Where(s => s.Type == TestType.Test).OrderByDescending(t => t.StartTime).FirstOrDefault().StartTime;
                    if (DateTime.UtcNow <= startime.AddSeconds(20)) isActiveTest = true;
                    Console.WriteLine("----Start time = "+startime.ToString());
                }
            }
            catch (System.NullReferenceException e) { Console.WriteLine("In (_context.Tests.Where(x => x.UserId == user.Id) there are no tests"); }
            Console.WriteLine("isActiveTest = " + isActiveTest);
            return isActiveTest;
        }

        public bool IsRunningProduction(long ChatId)
        {
            bool isActiveProduction = false;
            User user = _usersService.GetByTelegramId(ChatId);
            try
            {
                if (_context.Tests.Any(x => x.UserId == user.Id))
                {
                    DateTime startime = _context.Tests.Where(x => x.UserId == user.Id).Where(s => s.Type == TestType.Production).OrderByDescending(t => t.StartTime).FirstOrDefault().StartTime;
                    if (DateTime.UtcNow <= startime.AddMinutes(1)) isActiveProduction = true;
                    Console.WriteLine("----Start time Production = " + startime.ToString());
                }
            }
            catch { Console.WriteLine("In(_context.Tests.Where(x => x.UserId == user.Id) there are no tests"); }
            Console.WriteLine("isActiveProduction = " + isActiveProduction);
            return isActiveProduction;
        }
         
        public Test GetLastTest(long ChatId)
        {
            User user = _usersService.GetByTelegramId(ChatId);
            return _context.Tests.Where(x => x.UserId == user.Id).OrderByDescending(t => t.StartTime).FirstOrDefault();
        }

        public Test Create(long testId, DateTime startTime, TestType type)//chatId need???
        {
            Test test = new Test();
            test.StartTime = startTime; // DateTime.UtcNow;
            test.UserId = (_usersService.GetByTelegramId(testId)).Id;
            test.Type = type;
            test.Count = 0;
            test.RightAnswers = 0;

            _context.Tests.Add(test);

            _context.SaveChanges();

            return test;
        }

        public void AddExercise(Test test, Exercise exercise)
        {
            test.Exercises.Add(exercise);
            _context.SaveChanges();
        }

        public void AddCounts(Test test)
        {
            test.Count++;
            _context.SaveChanges();
        }

        public void AddRightAnswer(Test test)
        {
            test.RightAnswers++;
            _context.SaveChanges();
        }

        /*public Test StartTest(long ChatId)
        {
            if (!IsRunningTest(ChatId) && !IsRunningProduction(ChatId))
            {
                while(true)
                TestDTO testDTO = new TestDTO();
                //create new test
                //generate exercise _exercisesService.Generate
                
                    ExerciseDTO exerciseDTO = new ExerciseDTO();
                    exerciseDTO.StartTime = DateTime.UtcNow;
                    exerciseDTO.Test = _testsService.GetDTO(testId);
                    var exercise = _mapper.Map<ExerciseDTO, Exercise>(exerciseDTO);

                    _context.Exercises.Add(exercise);

                    exercise.Numbers = GetNumbers(exercise.Id);

                    _context.SaveChanges();

                    return exercise;
                
                var test = _mapper.Map<TestDTO, Test>(testDTO);

                _context.Tests.Add(test);
                _context.SaveChanges();

                return test;
            }
            else return null;
        }*/

        public Test Get(int id)
        {
            if (!_context.Tests.Any(x => x.Id == id)) return null;
            return _context.Tests.Where(x => x.Id == id).FirstOrDefault();
        }

        public TestDTO GetDTO(int id)
        {
            Test test = _context.Tests.AsNoTracking().FirstOrDefault(m => m.Id == id);

            TestDTO testDTO = _mapper.Map<TestDTO>(test);
            return testDTO;
        }
    }
}
