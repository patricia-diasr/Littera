using Littera.Data;
using Littera.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Littera.Pages
{
    public class SignupModel : PageModel
    {
        private readonly LitteraContext _context;

        public SignupModel(LitteraContext context) {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; }

        public void OnGet() {
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid)
                return Page();

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Email);
            if (existingUser != null) {
                ModelState.AddModelError("User.Email", "E-mail já cadastrado.");
                return Page();
            }

            _context.Users.Add(User);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Signin");
        }

    }
}
