using AutoMapper;
using MathBot.Common.DTO;
using MathBot.Model.DatabaseModels;

namespace MathBot.Common.Heplers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, RegisterUserDTO>().ReverseMap();
            CreateMap<Number, NumberDTO>().ReverseMap();
            CreateMap<Exercise, ExerciseDTO>().ReverseMap();
            CreateMap<Test, TestDTO>().ReverseMap();
        }
    }
}

