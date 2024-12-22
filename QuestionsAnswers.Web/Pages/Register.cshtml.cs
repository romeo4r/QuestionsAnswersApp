using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QuestionsAnswers.Web.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public string SuccessMessage { get; set; } // For displaying success message

        public string ErrorMessage { get; set; } // For displaying error messages

        // Constructor to initialize HttpClientFactory
        public RegisterModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET request for Register page
        public void OnGet()
        {
            // Here we can load data if needed for the Register page
        }

        // POST request to handle user registration
        public async Task<IActionResult> OnPostAsync()
        {
            // Validate the passwords match
            if (Password != ConfirmPassword)
            {
                TempData["ErrorMessage"] = "Las contraseñas no coinciden. Intente de nuevo";
                return Page();
            }

            // Create the request payload for user registration
            var registerDto = new
            {
                Username = Username,
                Email = Email,
                Password = Password
            };

            var client = _httpClientFactory.CreateClient("ApiClient");

            // Convert the data into JSON
            var content = new StringContent(JsonConvert.SerializeObject(registerDto), Encoding.UTF8, "application/json");

            // First, check if the username already exists by calling the API
            var userExistsResponse = await client.GetAsync($"https://localhost:5001/api/userQA/{Username}");

            if (userExistsResponse.IsSuccessStatusCode)
            {
                // If the user exists, show an error message
                TempData["ErrorMessage"] = "El usuario ya existe. Elija otro nombre de usuario";
                return RedirectToPage("/Register"); // Stay on the register page to show error message
            }

            // Send the request to the API for user registration
            var response = await client.PostAsync("https://localhost:5001/api/userQA/create", content);

            if (response.IsSuccessStatusCode)
            {
                // Set the success message in TempData
                TempData["SuccessMessage"] = "Usuario registrado correctamente!";
                return RedirectToPage("/Register"); // Stay on the register page to show success message
            }
            else
            {
                // If registration fails, show the error message
                TempData["ErrorMessage"] = "Error en el registro, intente de nuevo"; // Stay on the register page to show error message
                return RedirectToPage("/Register");
            }
        }
    }
}
