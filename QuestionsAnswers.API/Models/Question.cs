using System;

namespace QuestionsAnswers.API.Models
{
    public class Question
    {
        public Guid Id { get; set; }
        public Guid UserQAId { get; set; }
        public string Title { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsClosed { get; set; }
    }
}
