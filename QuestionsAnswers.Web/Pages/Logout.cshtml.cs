using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QuestionsAnswers.Web.Pages
{
    public class LogoutModel : PageModel
    {
        // Method to log the user out and remove the authentication cookie
        public async Task<IActionResult> OnPostAsync()
        {
            // Remove the authentication cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirect to the login page after logout
            return RedirectToPage("/Index");
        }
    }
}
