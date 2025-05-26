using Littera.Data;
using Littera.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;

namespace Littera.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly LitteraContext _context;

        [BindProperty]
        public List<Book> AlreadyRead { get; set; }

        [BindProperty]
        public List<Book> Reading { get; set; }

        [BindProperty]
        public List<Book> ToBeRead { get; set; }

        public IndexModel(LitteraContext context) {
            _context = context;
        }

        public async Task OnGetAsync() {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = int.Parse(claim.Value);

            AlreadyRead = await _context.Books
                .Where(b => b.UserId == userId)
                .Where(b => b.Status == "Lido")
            .ToListAsync() ?? new List<Book>();

            Reading = await _context.Books
                .Where(b => b.UserId == userId)
                .Where(b => b.Status == "Lendo")
            .ToListAsync() ?? new List<Book>();

            ToBeRead = await _context.Books
                .Where(b => b.UserId == userId)
                .Where(b => b.Status == "Quero Ler")
            .ToListAsync() ?? new List<Book>();
        }
    }
}
