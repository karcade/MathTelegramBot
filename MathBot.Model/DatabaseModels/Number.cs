using System.ComponentModel.DataAnnotations.Schema;

namespace MathBot.Model.DatabaseModels
{
    public class Number
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }
        public int Value { get; set; }

        [ForeignKey("ExerciseId")]
        public int ExerciseId { get; set; }
        public Exercise? Exercise { get; set; }
    }
}
