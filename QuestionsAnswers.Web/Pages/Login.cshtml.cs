using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using System.Net.Http;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
                return RedirectToPage("/Question/All");  // Redirect to the Dashboard if already logged in
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
                // Get the user data from the API response using System.Text.Json
                var userData = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON response into a JsonElement object
                var user = JsonSerializer.Deserialize<JsonElement>(userData);  // Use System.Text.Json.JsonSerializer

                // Extract userId and userName from the response
                var userId = user.GetProperty("userId").GetString();
                var userName = user.GetProperty("userName").GetString();

                // If login is successful, authenticate the user using cookie authentication
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, // Set this if you want the user to stay logged in
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Set cookie expiration
                };

                // Create the user claims with userId and userName
                var userClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Username),
                    new Claim("userId", userId), // Store userId in the claims
                    new Claim("userName", userName) // Store userName in the claims
                };

                var identity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Sign in the user using cookies
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

                return RedirectToPage("/Question/All"); // Redirect to the QuestionAll page after successful login
            }
            else
            {
                ErrorMessage = "No fue posible iniciar sesion, revisa tus credenciales.";
                return Page(); // If login fails, return the page with error message
            }
        }


    }
}
