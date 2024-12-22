using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace QuestionsAnswers.Web.Pages.Question
{
    public class CloseModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Constructor to initialize the HttpClientFactory
        public CloseModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // OnPost method to close the question and redirect to /Question/All
        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            // Make a PUT request to the API to close the question
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.PutAsJsonAsync($"https://localhost:5001/api/question/close/{id}", new
            {
                IsClosed = true // Mark the question as closed
            });

            // If the response is successful, redirect to the "All Questions" page
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Question/All");  // Redirect to the list of all questions
            }

            // If there is an error, return a BadRequest response with the error message
            return BadRequest("Error al cerrar la pregunta.");
        }
    }
}
