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
            //Check if the user is already authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Login");  
            }

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

                    // Format the CreationDate for each question
                    foreach (var question in Questions)
                    {
                        if (DateTime.TryParse(question.CreationDate, out DateTime creationDate))
                        {
                            // Format the date as "yyyy/MM/dd HH:mm:ss"
                            question.CreationDate = creationDate.ToString("yyyy/MM/dd HH:mm:ss");
                        }
                        else
                        {
                            // If the date is invalid, set a default value
                            question.CreationDate = "Fecha invalida";
                        }
                    }
                }
                else
                {
                    ErrorMessage = "Ocurrio un error al recuperar las preguntas.";
                }
            }
            catch
            {
                ErrorMessage = "Ocurrio un error al recuperar las preguntas.";
            }

            return Page();
        }

        // POST request to create a new question
        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(NewQuestionTitle))
            {
                ErrorMessage = "La pregunta no puede estar vacía.";
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
                ErrorMessage = "Error al crear la pregunta.";
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
                    ErrorMessage = "Error al cerrar la pregunta.";
                    return Page();
                }
            }
            catch
            {
                ErrorMessage = "Un error ocurrió cuando se estaba la pregunta.";
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
