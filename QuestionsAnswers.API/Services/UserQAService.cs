using System.Data;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using QuestionsAnswers.API.Models;

namespace QuestionsAnswers.API.Services
{
    // Service class to handle user-related logic
    public class UserQAService
    {
        private readonly string _connectionString;

        // Constructor to inject the connection string from configuration
        public UserQAService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DBQuestionsAnswersConnection");
        }

        // Method to create a new user using the stored procedure sp_InsertUser
        public async Task<Guid> CreateUserQAAsync(string username, string email, string password)
        {
            // Generate salt and hash the password
            var salt = GenerateSalt();
            var hashedPassword = HashPassword(password, salt);

            using (var connection = new SqlConnection(_connectionString)) // Ensure using the correct SqlConnection
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_InsertUserQA", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Adding parameters for the stored procedure
                    command.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar) { Value = username });
                    command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar) { Value = email });
                    command.Parameters.Add(new SqlParameter("@PasswordHash", SqlDbType.NVarChar) { Value = hashedPassword });
                    command.Parameters.Add(new SqlParameter("@Salt", SqlDbType.NVarChar) { Value = salt });

                    // Execute the stored procedure and return the UserId
                    var userId = await command.ExecuteScalarAsync();
                    return (Guid)userId;
                }
            }
        }


        // Generate a random salt for password hashing
        private string GenerateSalt()
        {
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        // Hash the password combined with the salt
        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = password + salt;
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(bytes);
            }
        }



        // Method to get a UserQA by Id using the stored procedure sp_GetUserQAById
        public async Task<UserQA> GetUserQAByUsernameAsync(string username)
        {
            using (var connection = new SqlConnection(_connectionString)) // Ensure using the correct SqlConnection
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_GetUserQAByUsername", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Adding the parameter for the stored procedure
                    command.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar) { Value = username });

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            // Return the user details as a UserQA object
                            return new UserQA
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                                Salt = reader.GetString(reader.GetOrdinal("Salt")),
                                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                            };
                        }
                        return null;  // Return null if the user is not found
                    }
                }
            }
        }


    }
}
