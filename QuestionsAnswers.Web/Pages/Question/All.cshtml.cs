using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace QuestionsAnswers.Web.Pages.Question
{
    //[Authorize]  It is necessary to check why the middleware is throwing an error
    public class QuestionAllModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
        public string ErrorMessage { get; set; }
        public string UserId { get; set; }  // This will hold the UserId

        // Bind properties for creating a new question
        [BindProperty]
        public string NewQuestionTitle { get; set; }

        public QuestionAllModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET request to fetch all questions for the logged-in user
        public async Task<IActionResult> OnGetAsync()
        {
            // Get the UserId from the HTTP context (this will be available after authentication)
            UserId = User.FindFirstValue(ClaimTypes.Name);

            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                var response = await client.GetAsync("https://localhost:5001/api/question/desc");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Questions = JsonConvert.DeserializeObject<List<QuestionDto>>(content);
                }
                else
                {
                    ErrorMessage = "An error occurred while fetching questions.";
                }
            }
            catch
            {
                ErrorMessage = "An error occurred while fetching questions.";
            }

            return Page();
        }

        // POST request to create a new question
        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(NewQuestionTitle))
            {
                ErrorMessage = "Question title cannot be empty.";
                return Page();
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            var newQuestion = new { Title = NewQuestionTitle };

            var content = new StringContent(JsonConvert.SerializeObject(newQuestion), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:5001/api/question/create", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/QuestionAll");
            }
            else
            {
                ErrorMessage = "Failed to create question.";
                return Page();
            }
        }

        // POST request to close a question
        public async Task<IActionResult> OnPostCloseQuestionAsync(string questionId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                var response = await client.PostAsync($"https://localhost:5001/api/question/close/{questionId}", null);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/QuestionAll");
                }
                else
                {
                    ErrorMessage = "Failed to close the question.";
                    return Page();
                }
            }
            catch
            {
                ErrorMessage = "An error occurred while closing the question.";
                return Page();
            }
        }
    }

    // DTO for Questions
    public class QuestionDto
    {
        public string Id { get; set; }
        public string UserQAId { get; set; }
        public string Title { get; set; }
        public bool IsClosed { get; set; }
        public string CreationDate { get; set; }

        public string UserName { get; set; }
    }
}
