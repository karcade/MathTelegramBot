using AutoMapper;
using MathBot.BusinessLogic.Interfaces;
using MathBot.Common.DTO;
using MathBot.Model.DatabaseModels;
using MathBot.Model;
using Microsoft.EntityFrameworkCore;

namespace MathBot.BusinessLogic.Implementations
{
    public class NumbersService: INumbersService
    {
        private static readonly Random getrandom = new Random();

        private ApplicationContext _context;
        private readonly IMapper _mapper;
        //private IExercisesService _exercisesService;

        public NumbersService(ApplicationContext context, IMapper mapper)//, IExercisesService exercisesService
        {
            _context = context;
            _mapper = mapper;
           // _exercisesService = exercisesService;
        }

        /*public Number Create()
        {
            NumberDTO numberDTO = new NumberDTO();
            numberDTO.Value = getrandom.Next(100, 1000);
            var number = _mapper.Map<NumberDTO, Number>(numberDTO);
            _context.Numbers.Add(number);
            _context.SaveChanges();
            return number;
        }*/

        public int GenerateNumber()
        {
            return getrandom.Next(100, 1000);
        }

        /*public Number Generate(int exerciseId)
        {
            NumberDTO numberDTO = new NumberDTO();
            numberDTO.Value = GenerateNumber();
            numberDTO.Exercise = _exercisesService.GetDTO(exerciseId);

            var number = _mapper.Map<NumberDTO, Number>(numberDTO);

            _context.Numbers.Add(number);
            _context.SaveChanges();

            return number;
        }*/

        public Number Generate(int exerciseId)
        {
            Number number = new Number();
            number.Value = GenerateNumber();
            //number.Exercise = _exercisesService.Get(exerciseId);
            number.ExerciseId = exerciseId;

            _context.Numbers.Add(number);
            _context.SaveChanges();

            return number;
        }

        public Number Get(int id)
        {
            if (!_context.Numbers.Any(x => x.Id == id)) return null;
            return _context.Numbers.Where(x => x.Id == id).FirstOrDefault();
        }

        public NumberDTO GetDTO(int id)
        {
            Number number = _context.Numbers.AsNoTracking().FirstOrDefault(m => m.Id == id);

            NumberDTO numberDTO = _mapper.Map<NumberDTO>(number);
            return numberDTO;
        }
    }
}
