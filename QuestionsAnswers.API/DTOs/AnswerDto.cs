using System;

namespace QuestionsAnswers.API.DTOs
{
    public class CreateAnswerDto
    {
        public Guid QuestionId { get; set; }
        public Guid UserQAId { get; set; }
        public string Response { get; set; }
    }

    public class UpdateAnswerDto
    {
        public string Response { get; set; }
    }
}
