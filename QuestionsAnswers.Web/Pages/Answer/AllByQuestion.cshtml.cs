using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace QuestionsAnswers.Web.Pages.Answer
{
    public class AllByQuestionModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AllByQuestionModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Properties to hold the Question and Answers data
        public QuestionViewModel Question { get; set; }
        public List<AnswerViewModel> Answers { get; set; }
        public string ErrorMessage { get; set; } // Property to hold error message

        // OnGet method to fetch the data
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

            // Fetch the answers related to the question from the API
            var answersResponse = await httpClient.GetAsync($"https://localhost:5001/api/answer/all/{questionId}");

            if (answersResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // If there are no answers, display a custom message
                ErrorMessage = "No answers found for this question.";
                Answers = new List<AnswerViewModel>(); // Initialize empty answers list
            }
            else if (answersResponse.IsSuccessStatusCode)
            {
                var answersContent = await answersResponse.Content.ReadAsStringAsync();
                Answers = JsonConvert.DeserializeObject<List<AnswerViewModel>>(answersContent);
            }
            else
            {
                // Handle other potential errors
                ErrorMessage = "An error occurred while fetching answers.";
            }

            return Page();
        }

    }

    // ViewModel for Question
    public class QuestionViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string UserName { get; set; }
        public DateTime CreationDate { get; set; }
    }

    // ViewModel for Answer
    public class AnswerViewModel
    {
        public string Response { get; set; }
        public string UserName { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
