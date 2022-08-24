using System.ComponentModel.DataAnnotations.Schema;

namespace MathBot.Model.DatabaseModels
{
    public class Exercise
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }
        public List<Number> Numbers { get; set; } = new List<Number>();
        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        [ForeignKey("TestId")]
        public int TestId { get; set; }
        public Test? Test { get; set; }
        public int? AnswerNumberId { get; set; }
    }
}
