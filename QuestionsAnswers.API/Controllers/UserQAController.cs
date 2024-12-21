using Microsoft.AspNetCore.Mvc;
using QuestionsAnswers.API.DTOs;
using QuestionsAnswers.API.Services;
using System;
using System.Threading.Tasks;

namespace QuestionsAnswers.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserQAController : ControllerBase
    {
        private readonly UserQAService _userService;

        // Constructor to inject UserService
        public UserQAController(UserQAService userService)
        {
            _userService = userService;
        }

        // POST: api/userQA/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateUserQA([FromBody] CreateUserDto createUserDto)
        {
            if (createUserDto == null)
                return BadRequest("Invalid user data.");

            try
            {
                // Call UserService to create the user
                var userId = await _userService.CreateUserQAAsync(
                    createUserDto.Username,
                    createUserDto.Email,
                    createUserDto.Password);  // The password is assumed to be plain text here

                // Return the response with the user ID
                return Ok(new { message = "User created successfully.", userId = userId });
            }
            catch (Exception ex)
            {
                // If an error occurs, return BadRequest with the error message
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/userQA/username/{username}
        [HttpGet("{username}")]
        public async Task<IActionResult> GetUserQAByUsername(string username)
        {
            try
            {
                // Call UserQAService to get the user by username
                var user = await _userService.GetUserQAByUsernameAsync(username);

                // Check if user is found
                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                // Return the user data
                return Ok(user);
            }
            catch (Exception ex)
            {
                // If an error occurs, return BadRequest with the error message
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
