namespace MathBot.Model.DatabaseModels
{
    public class User
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }
        public long ChatId { get; set; }
        public string Username { get; set; }
        public DateTime DateRegistration { get; set; }
        
        public List<Test> Tests = new List<Test>();
    }
}
