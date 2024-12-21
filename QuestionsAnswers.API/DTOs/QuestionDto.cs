using System;

namespace QuestionsAnswers.API.DTOs
{
    public class CreateQuestionDto
    {
        public Guid UserQAId { get; set; }
        public string Title { get; set; }
    }
    public class UpdateQuestionDto
    {
        public string Title { get; set; }
        public bool IsClosed { get; set; }
    }
}
