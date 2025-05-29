using Littera.Data;
using Littera.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Net;
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

        [BindProperty]
        public List<Tag> Tags { get; set; }

        [BindProperty]
        public List<Collection> Collections { get; set; }

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

            Tags = await _context.BookTags
                .Where(bt => bt.BookId == id)
                .Include(bt => bt.Tag)
                .Select(bt => bt.Tag)
                .ToListAsync();

            Collections = await _context.BookCollections
                .Where(bc => bc.BookId == id)
                .Include(bc => bc.Collection)
                .Select(bc => bc.Collection)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteBookAsync(int bookId) {
            var bookToDelete = await _context.Books.FindAsync(bookId);

            if (!string.IsNullOrEmpty(bookToDelete.Cover)) {
                var oldFilePath = Path.Combine("wwwroot", bookToDelete.Cover.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath)) {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            _context.Books.Remove(bookToDelete);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }

    }
}
