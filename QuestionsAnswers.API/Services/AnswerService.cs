using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using QuestionsAnswers.API.Models;

namespace QuestionsAnswers.API.Services
{
    public class AnswerService
    {
        private readonly string _connectionString;

        // Constructor to inject the connection string from configuration
        public AnswerService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DBQuestionsAnswersConnection");
        }

        // Method to create a new answer using the stored procedure sp_InsertAnswer
        public async Task<Guid> CreateAnswerAsync(Guid questionId, Guid userQAId, string response)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_InsertAnswer", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Adding parameters for the stored procedure
                    command.Parameters.Add(new SqlParameter("@QuestionId", SqlDbType.UniqueIdentifier) { Value = questionId });
                    command.Parameters.Add(new SqlParameter("@UserQAId", SqlDbType.UniqueIdentifier) { Value = userQAId });
                    command.Parameters.Add(new SqlParameter("@Response", SqlDbType.NVarChar) { Value = response });

                    // Execute the stored procedure and return the AnswerId
                    var answerId = await command.ExecuteScalarAsync();
                    return (Guid)answerId;
                }
            }
        }

        // Method to get an answer by Id using the stored procedure sp_GetAnswerById
        public async Task<Answer> GetAnswerByIdAsync(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_GetAnswerById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Adding the parameter for the stored procedure
                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = id });

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Answer
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                QuestionId = reader.GetGuid(reader.GetOrdinal("QuestionId")),
                                UserQAId = reader.GetGuid(reader.GetOrdinal("UserQAId")),
                                Response = reader.GetString(reader.GetOrdinal("Response")),
                                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate"))
                            };
                        }
                        return null;  // Return null if the answer is not found
                    }
                }
            }
        }

        // Method to update an existing answer using the stored procedure sp_UpdateAnswer
        public async Task<Answer> UpdateAnswerAsync(Guid id, string response)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_UpdateAnswer", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Adding parameters for the stored procedure
                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = id });
                    command.Parameters.Add(new SqlParameter("@Response", SqlDbType.NVarChar) { Value = response });

                    // Execute the stored procedure
                    await command.ExecuteNonQueryAsync();

                    return await GetAnswerByIdAsync(id);  // Return the updated answer
                }
            }
        }

        // Method to delete an answer using the stored procedure sp_DeleteAnswer
        public async Task<bool> DeleteAnswerAsync(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_DeleteAnswer", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Adding the parameter for the stored procedure
                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = id });

                    var result = await command.ExecuteNonQueryAsync();
                    return result > 0;  // Return true if the delete was successful
                }
            }
        }

        // Method to get all answers for a specific question, ordered by CreationDate descending
        public async Task<List<Answer>> GetAnswersByQuestionDescAsync(Guid questionId)
        {
            var answers = new List<Answer>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_GetAnswersByQuestionDesc", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Adding the parameter for the stored procedure
                    command.Parameters.Add(new SqlParameter("@QuestionId", SqlDbType.UniqueIdentifier) { Value = questionId });

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            answers.Add(new Answer
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                QuestionId = reader.GetGuid(reader.GetOrdinal("QuestionId")),
                                UserQAId = reader.GetGuid(reader.GetOrdinal("UserQAId")),
                                Response = reader.GetString(reader.GetOrdinal("Response")),
                                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate"))
                            });
                        }
                    }
                }
            }

            return answers;  // Return the list of answers
        }
    }
}
