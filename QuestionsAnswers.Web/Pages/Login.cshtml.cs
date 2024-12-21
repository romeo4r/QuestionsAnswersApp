using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;

namespace QuestionsAnswers.Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Check if the user is already authenticated
        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)  // Check if the user is already authenticated
            {
                return RedirectToPage("/Dashboard");  // Redirect to the Dashboard if already logged in
            }

            return Page();  // If not authenticated, show the login page
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _httpClientFactory.CreateClient("ApiClient");

            var loginDto = new { Username, Password };
            var content = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:5001/api/userQA/login", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Dashboard");  // Redirect to Dashboard after successful login
            }
            else
            {
                ErrorMessage = "Invalid login attempt.";
                return Page();
            }
        }
    }
}
