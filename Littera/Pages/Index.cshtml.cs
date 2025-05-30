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

        public List<Book> AlreadyRead { get; set; }

        public List<Book> Reading { get; set; }

        public List<Book> ToBeRead { get; set; }

        public Dictionary<Collection, List<Book>> CollectionsWithBooks { get; set; }

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

            var collections = await _context.Collections
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.Priority)
                .ToListAsync();

            var collectionIds = collections.Select(c => c.Id).ToList();

            var booksInCollections = await _context.BookCollections
                .Where(bc => collectionIds.Contains(bc.CollectionId))
                .Include(bc => bc.Book)
                .ToListAsync();

            CollectionsWithBooks = collections
                .Where(c => booksInCollections.Any(bc => bc.CollectionId == c.Id))
                .ToDictionary(
                    collection => collection,
                    collection => booksInCollections
                                    .Where(bc => bc.CollectionId == collection.Id)
                                    .Select(bc => bc.Book)
                                    .ToList()
                );
        }
    }
}
