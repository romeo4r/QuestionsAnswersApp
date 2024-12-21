using System;

namespace QuestionsAnswers.API.Models
{
    public class Answer
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public Guid UserQAId { get; set; }
        public string Response { get; set; }
        public DateTime CreationDate { get; set; }

        public Question Question { get; set; }
        public UserQA UserQA { get; set; }
    }
}
