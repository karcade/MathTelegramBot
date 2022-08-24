using MathBot.Model.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace MathBot.Model.DatabaseModels
{
    public class Test
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public User? User { get; set; }
        public DateTime StartTime { get; set; } // = DateTime.UtcNow;
        public List<Exercise> Exercises { get; set; } = new List<Exercise>();
        public TestType Type { get; set; }
        public int? Count { get; set; } = 0;
        public int? RightAnswers { get; set; } = 0;
        public bool IsStoped  { get; set; } = false;
    }
}
