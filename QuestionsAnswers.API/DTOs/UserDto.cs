namespace QuestionsAnswers.API.DTOs
{
    public class CreateUserDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UpdateQuestionDto
    {
        public string Title { get; set; }
        public bool IsClosed { get; set; }
    }
}
