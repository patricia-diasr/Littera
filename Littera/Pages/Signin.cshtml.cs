using Littera.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Littera.Pages
{
    public class SigninModel : PageModel
    {
        private readonly LitteraContext _context;

        public SigninModel(LitteraContext context) {
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public async Task<IActionResult> OnPostAsync() {

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password)) {
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "Preencha todos os campos.");
                return Page();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email && u.Password == Password);

            if (user == null) {
                ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToPage("/Index"); 
        }
    }
}
