using Littera.Data;
using Littera.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Littera.Pages.Books
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly LitteraContext _context;

        [BindProperty]
        public Book Book { get; set; }

        [BindProperty]
        public Author Author { get; set; }

        public DetailsModel(LitteraContext context) {
            _context = context;
        }

        public async Task OnGetAsync(int id) {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = int.Parse(claim.Value);

            Book = await _context.Books
                .Where(b => b.UserId == userId && b.Id == id)
                .FirstOrDefaultAsync();

            Author = await _context.Authors
                .Where(a => a.Id == Book.AuthorId)
                .FirstOrDefaultAsync();
        }
    }
}
