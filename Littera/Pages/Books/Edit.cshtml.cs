using Littera.Data;
using Littera.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Littera.Pages.Books
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly LitteraContext _context;

        [BindProperty]
        public Book Book { get; set; }

        [BindProperty]
        public Author Author { get; set; }

        [BindProperty]
        public IFormFile BookCoverFile { get; set; }

        public EditModel(LitteraContext context) {
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

                if (!string.IsNullOrEmpty(Book.Cover)) {
                    var oldFilePath = Path.Combine("wwwroot", Book.Cover.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath)) {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                using (var fileStream = new FileStream(filePath, FileMode.Create)) {
                    await BookCoverFile.CopyToAsync(fileStream);
                }

                Book.Cover = $"/uploads/books/{uniqueFileName}";
            }

            _context.Books.Update(Book);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Books/Details", new { id = Book.Id });
        }
    }
}
