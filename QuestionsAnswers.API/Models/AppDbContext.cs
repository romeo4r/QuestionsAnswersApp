using Microsoft.EntityFrameworkCore;
using QuestionsAnswers.API.Models;  

namespace QuestionsAnswers.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Define the DBSet for the User model, which represents the Users table
        public DbSet<UserQA> Users { get; set; }
    }
}
