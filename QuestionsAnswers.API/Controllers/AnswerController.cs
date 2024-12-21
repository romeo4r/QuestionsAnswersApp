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
    public class AnswerController : ControllerBase
    {
        private readonly AnswerService _answerService;

        // Constructor to inject AnswerService
        public AnswerController(AnswerService answerService)
        {
            _answerService = answerService;
        }

        // POST: api/answer/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateAnswer([FromBody] CreateAnswerDto createAnswerDto)
        {
            if (createAnswerDto == null)
                return BadRequest("Invalid answer data.");

            try
            {
                // Call AnswerService to create the answer
                var answerId = await _answerService.CreateAnswerAsync(
                    createAnswerDto.QuestionId,
                    createAnswerDto.UserQAId,
                    createAnswerDto.Response);

                return Ok(new { message = "Answer created successfully.", answerId = answerId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/answer/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnswerById(Guid id)
        {
            try
            {
                var answer = await _answerService.GetAnswerByIdAsync(id);

                if (answer == null)
                {
                    return NotFound(new { message = "Answer not found." });
                }

                return Ok(answer);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/answer/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAnswer(Guid id, [FromBody] UpdateAnswerDto updateAnswerDto)
        {
            if (updateAnswerDto == null)
                return BadRequest("Invalid answer data.");

            try
            {
                var updatedAnswer = await _answerService.UpdateAnswerAsync(id, updateAnswerDto.Response);

                if (updatedAnswer == null)
                {
                    return NotFound(new { message = "Answer not found." });
                }

                return Ok(new { message = "Answer updated successfully.", answer = updatedAnswer });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/answer/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnswer(Guid id)
        {
            try
            {
                var isDeleted = await _answerService.DeleteAnswerAsync(id);

                if (!isDeleted)
                {
                    return NotFound(new { message = "Answer not found." });
                }

                return Ok(new { message = "Answer deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/answer/all/{questionId}
        [HttpGet("all/{questionId}")]
        public async Task<IActionResult> GetAllAnswersByQuestion(Guid questionId)
        {
            try
            {
                var answers = await _answerService.GetAnswersByQuestionDescAsync(questionId);

                if (answers == null || answers.Count == 0)
                {
                    return NotFound(new { message = "No answers found for this question." });
                }

                return Ok(answers);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
