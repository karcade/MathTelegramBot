using MathBot.Model.DatabaseModels;

namespace MathBot.Common.DTO
{
    public class ExerciseDTO
    {
        public List<Number> Numbers { get; set; } = new();
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public TestDTO Test { get; set; }
        public int? AnswerNumberId { get; set; }



        /*public List<Number> Numbers { get; set; } = new();
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public int TestId { get; set; }
        public Test Test { get; set; }
        public int? AnswerNumberId { get; set; }*/
    }
}
