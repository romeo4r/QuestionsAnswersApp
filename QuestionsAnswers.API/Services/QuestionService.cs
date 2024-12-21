using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using QuestionsAnswers.API.Models;
using System.Collections.Generic;

namespace QuestionsAnswers.API.Services
{
    public class QuestionService
    {
        private readonly string _connectionString;

        // Constructor to inject the connection string from configuration
        public QuestionService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DBQuestionsAnswersConnection");
        }

        // Method to create a new question using the stored procedure sp_InsertQuestion
        public async Task<Guid> CreateQuestionAsync(Guid userQAId, string title)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_InsertQuestion", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Adding parameters for the stored procedure
                    command.Parameters.Add(new SqlParameter("@UserQAId", SqlDbType.UniqueIdentifier) { Value = userQAId });
                    command.Parameters.Add(new SqlParameter("@Title", SqlDbType.NVarChar) { Value = title });

                    // Execute the stored procedure and return the QuestionId
                    var questionId = await command.ExecuteScalarAsync();
                    return (Guid)questionId;
                }
            }
        }

        // Method to get a question by Id using the stored procedure sp_GetQuestionById
        public async Task<Question> GetQuestionByIdAsync(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_GetQuestionById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Adding the parameter for the stored procedure
                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = id });

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            // Return the question details as a Question object
                            return new Question
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                UserQAId = reader.GetGuid(reader.GetOrdinal("UserQAId")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                                IsClosed = reader.GetBoolean(reader.GetOrdinal("IsClosed"))
                            };
                        }
                        return null;  // Return null if the question is not found
                    }
                }
            }
        }

        // Method to update an existing question using the stored procedure sp_UpdateQuestion
        public async Task<Question> UpdateQuestionAsync(Guid id, string title, bool isClosed)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_UpdateQuestion", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Adding parameters for the stored procedure
                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = id });
                    command.Parameters.Add(new SqlParameter("@Title", SqlDbType.NVarChar) { Value = title });
                    command.Parameters.Add(new SqlParameter("@IsClosed", SqlDbType.Bit) { Value = isClosed });

                    // Execute the stored procedure
                    await command.ExecuteNonQueryAsync();

                    return await GetQuestionByIdAsync(id);  // Return the updated question
                }
            }
        }


        // Method to delete a question using the stored procedure sp_DeleteQuestion
        public async Task<bool> DeleteQuestionAsync(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_DeleteQuestion", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Adding the parameter for the stored procedure
                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = id });

                    var result = await command.ExecuteNonQueryAsync();

                    return result > 0;  // Return true if the delete was successful
                }
            }
        }

        // Method to get all questions using the stored procedure sp_GetAllQuestions
        public async Task<List<Question>> GetAllQuestionsAsync()
        {
            var questions = new List<Question>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_GetAllQuestionsDesc", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            questions.Add(new Question
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                UserQAId = reader.GetGuid(reader.GetOrdinal("UserQAId")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                                IsClosed = reader.GetBoolean(reader.GetOrdinal("IsClosed"))
                            });
                        }
                    }
                }
            }

            return questions;
        }

        // Method to get all questions ordered by CreationDate in descending order using sp_GetQuestionsOrderedByDateDesc
        public async Task<List<Question>> GetQuestionsOrderedByDateDescAsync()
        {
            var questions = new List<Question>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_GetQuestionsOrderedByDateDesc", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            questions.Add(new Question
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                UserQAId = reader.GetGuid(reader.GetOrdinal("UserQAId")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                                IsClosed = reader.GetBoolean(reader.GetOrdinal("IsClosed"))
                            });
                        }
                    }
                }
            }

            return questions;
        }
    }
}
