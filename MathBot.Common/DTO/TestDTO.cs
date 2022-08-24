using MathBot.Model.DatabaseModels;
using MathBot.Model.Enum;

namespace MathBot.Common.DTO
{
    public class TestDTO
    {
        public RegisterUserDTO User { get; set; }
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        //public List<Exercise> Exercises { get; set; } = new();
        public TestType Type { get; set; }
    }
}
