using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http; // Para acceder a las cookies
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace QuestionsAnswers.Web.Pages.Answer
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public QuestionViewModel Question { get; set; }  // Question data
        public string CreatorUserName { get; set; }  // User who created the question
        [BindProperty]
        public string Respond { get; set; }  // The answer entered by the user

        public async Task<IActionResult> OnGetAsync(Guid questionId)
        {
            var httpClient = _httpClientFactory.CreateClient();

            // Fetch the question details from the API
            var questionResponse = await httpClient.GetAsync($"https://localhost:5001/api/question/{questionId}");
            if (!questionResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var questionContent = await questionResponse.Content.ReadAsStringAsync();
            Question = JsonConvert.DeserializeObject<QuestionViewModel>(questionContent);

            // Get the creator's username (assuming it's part of the Question object)
            CreatorUserName = Question.UserName;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid questionId)
        {
            var httpClient = _httpClientFactory.CreateClient();

            // Retrieve UserQAId from the cookie (assuming it's stored in the cookie)
            var userQAId = User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userQAId))
            {
                return BadRequest("User ID is required.");
            }

            // Prepare the data to send to the API
            var newAnswer = new
            {
                QuestionId = questionId,
                UserQAId = Guid.Parse(userQAId),  // Convert cookie value to Guid
                Response = Respond
            };

            var content = new StringContent(JsonConvert.SerializeObject(newAnswer), Encoding.UTF8, "application/json");

            // Send the POST request to create a new answer for the question
            var response = await httpClient.PostAsync($"https://localhost:5001/api/answer/create", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Answer/AllByQuestion", new { questionId });
            }

            return BadRequest("An error occurred while creating the answer.");
        }
    }

}
