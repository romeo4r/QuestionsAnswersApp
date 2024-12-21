using System;

namespace QuestionsAnswers.API.Models 
{
    public class UserQA
    {
        public Guid Id { get; set; } 
        public string Username { get; set; } 
        public string Email { get; set; } 
        public string PasswordHash { get; set; } 
        public string Salt { get; set; } 
        public DateTime CreationDate { get; set; } 
        public bool IsActive { get; set; } 
    }
}
