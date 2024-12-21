using Microsoft.AspNetCore.Mvc;
using QuestionsAnswers.API.DTOs;
using QuestionsAnswers.API.Services;
using QuestionsAnswers.API.Models;
using System;
using System.Threading.Tasks;

namespace QuestionsAnswers.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly QuestionService _questionService;

        // Constructor to inject QuestionService
        public QuestionController(QuestionService questionService)
        {
            _questionService = questionService;
        }

        // POST: api/question/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateQuestion([FromBody] CreateQuestionDto createQuestionDto)
        {
            if (createQuestionDto == null)
                return BadRequest("Invalid question data.");

            try
            {
                // Call QuestionService to create the question
                var questionId = await _questionService.CreateQuestionAsync(
                    createQuestionDto.UserQAId,
                    createQuestionDto.Title);

                // Return the response with the question ID
                return Ok(new { message = "Question created successfully.", questionId = questionId });
            }
            catch (Exception ex)
            {
                // If an error occurs, return BadRequest with the error message
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/question/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestionById(Guid id)
        {
            try
            {
                // Call QuestionService to get the question by id
                var question = await _questionService.GetQuestionByIdAsync(id);

                // Check if question is found
                if (question == null)
                {
                    return NotFound(new { message = "Question not found." });
                }

                // Return the question data
                return Ok(question);
            }
            catch (Exception ex)
            {
                // If an error occurs, return BadRequest with the error message
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/question/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateQuestion(Guid id, [FromBody] UpdateQuestionDto updateQuestionDto)
        {
            if (updateQuestionDto == null)
                return BadRequest("Invalid question data.");

            try
            {
                // Call QuestionService to update the question
                var updatedQuestion = await _questionService.UpdateQuestionAsync(id, updateQuestionDto.Title, updateQuestionDto.IsClosed);

                // Check if update was successful
                if (updatedQuestion == null)
                {
                    return NotFound(new { message = "Question not found." });
                }

                return Ok(new { message = "Question updated successfully.", question = updatedQuestion });
            }
            catch (Exception ex)
            {
                // If an error occurs, return BadRequest with the error message
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/question/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(Guid id)
        {
            try
            {
                // Call QuestionService to delete the question
                var isDeleted = await _questionService.DeleteQuestionAsync(id);

                if (!isDeleted)
                {
                    return NotFound(new { message = "Question not found." });
                }

                return Ok(new { message = "Question deleted successfully." });
            }
            catch (Exception ex)
            {
                // If an error occurs, return BadRequest with the error message
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/question/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAllQuestions()
        {
            try
            {
                // Call QuestionService to get all questions
                var questions = await _questionService.GetAllQuestionsAsync();

                return Ok(questions);
            }
            catch (Exception ex)
            {
                // If an error occurs, return BadRequest with the error message
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/question/desc
        [HttpGet("desc")]
        public async Task<IActionResult> GetQuestionsOrderedByDateDesc()
        {
            try
            {
                // Call QuestionService to get all questions ordered by CreationDate descending
                var questions = await _questionService.GetQuestionsOrderedByDateDescAsync();

                return Ok(questions);
            }
            catch (Exception ex)
            {
                // If an error occurs, return BadRequest with the error message
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
