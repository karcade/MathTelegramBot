using MathBot.Model.DatabaseModels;

namespace MathBot.Common.DTO
{
    public class RegisterUserDTO 
    {
        public long ChatId { get; set; }
        public string Username { get; set; }
        public DateTime DateRegistration { get; set; }
        public List<Test> Tests = new List<Test>();
    }
}
