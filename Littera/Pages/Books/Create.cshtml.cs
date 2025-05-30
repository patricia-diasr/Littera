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
    public class CreateModel : PageModel
    {
        private readonly LitteraContext _context;

        [BindProperty]
        public Book Book { get; set; }

        [BindProperty]
        public IFormFile BookCoverFile { get; set; }

        [BindProperty]
        public Author Author { get; set; }

        [BindProperty]
        public List<Tag> Tags { get; set; }

        [BindProperty]
        public string SelectedTagIds { get; set; }

        [BindProperty]
        public List<Collection> Collections { get; set; }

        [BindProperty]
        public string SelectedCollectionsIds { get; set; }

        public CreateModel(LitteraContext context) {
            _context = context;
        }

        public async Task OnGetAsync() {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = int.Parse(claim.Value);

            Tags = await _context.Tags
                .Where(t => t.UserId == userId)
                .ToListAsync() ?? new List<Tag>();

            Collections = await _context.Collections
                .Where(c => c.UserId == userId)
                .ToListAsync() ?? new List<Collection>();
        }

        public async Task<IActionResult> OnPostAsync() {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);

            int userId = int.Parse(claim.Value);
            Book.UserId = userId;

            var existingAuthor = await _context.Authors.FirstOrDefaultAsync(a => a.Name == Author.Name);

            if (existingAuthor != null) {
                Book.AuthorId = existingAuthor.Id;
            }
            else {
                Author.UserId = userId;
                _context.Authors.Add(Author);

                await _context.SaveChangesAsync(); 
                Book.AuthorId = Author.Id;
            }

            if (BookCoverFile != null && BookCoverFile.Length > 0) {
                var uploadsFolder = Path.Combine("wwwroot", "uploads", "books");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(BookCoverFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create)) {
                    await BookCoverFile.CopyToAsync(fileStream);
                }

                Book.Cover = $"/uploads/books/{uniqueFileName}";
            }

            _context.Books.Add(Book);
            await _context.SaveChangesAsync();

            List<int> tagIds = new List<int>();

            if (!string.IsNullOrWhiteSpace(SelectedTagIds)) {
                tagIds = SelectedTagIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                   .Select(int.Parse)
                   .ToList();
            }

            foreach (var tagId in tagIds) {
                var bookTag = new BookTag {
                    BookId = Book.Id,
                    TagId = tagId
                };
                _context.BookTags.Add(bookTag);
            }

            List<int> collectionsIds = new List<int>();

            if (!string.IsNullOrWhiteSpace(SelectedCollectionsIds)) {
                collectionsIds = SelectedCollectionsIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                   .Select(int.Parse)
                   .ToList();
            }

            foreach (var collectionId in collectionsIds) {
                var bookCollection = new BookCollection {
                    BookId = Book.Id,
                    CollectionId = collectionId
                };
                _context.BookCollections.Add(bookCollection);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
