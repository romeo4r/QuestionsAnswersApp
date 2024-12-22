using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QuestionsAnswers.Web.Pages.Question
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty]
        public string Title { get; set; }

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // This method handles the GET request to show the create question form
        public void OnGet()
        {
        }

        // This method handles the POST request to create the question
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();  // Return the same page if validation fails
            }

            // Get the UserId from the cookie (or session, depending on how you store it)
            var userId = User.FindFirst("userId")?.Value;

            // Create the request payload for the API (Title and UserQAId)
            var createQuestionDto = new
            {
                Title = Title,
                UserQAId = userId
            };

            var client = _httpClientFactory.CreateClient("ApiClient");

            // Convert the data into JSON
            var content = new StringContent(JsonConvert.SerializeObject(createQuestionDto), Encoding.UTF8, "application/json");

            // Send the request to the API to create the question
            var response = await client.PostAsync("https://localhost:5001/api/question/create", content);

            if (response.IsSuccessStatusCode)
            {
                // If creation is successful, redirect to the question list or another page
                return RedirectToPage("/Question/All");  // Redirect to the list of questions
            }
            else
            {
                // If the creation fails, show the error message
                TempData["ErrorMessage"] = "Fallo al crear la pregunta, intente de nuevo.";
                return Page();  // Stay on the current page to show the error
            }
        }
    }
}
