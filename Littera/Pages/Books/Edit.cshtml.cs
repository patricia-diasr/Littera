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

        [BindProperty]
        public List<Tag> Tags { get; set; }

        public List<Tag> InitialTags { get; set; }

        [BindProperty]
        public string SelectedTagIds { get; set; }

        [BindProperty]
        public List<Collection> Collections { get; set; }

        public List<Collection> InitialCollections { get; set; }

        [BindProperty]
        public string SelectedCollectionIds { get; set; }

        public EditModel(LitteraContext context) {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int id) {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = int.Parse(claim.Value);

            Book = await _context.Books
                .Where(b => b.UserId == userId && b.Id == id)
                .FirstOrDefaultAsync();

            if (Book == null) {
                return RedirectToPage("Index");
            }

            Author = await _context.Authors
                .Where(a => a.Id == Book.AuthorId)
                .FirstOrDefaultAsync();

            Tags = await _context.Tags
                .Where(t => t.UserId == userId)
                .ToListAsync() ?? new List<Tag>();

            Collections = await _context.Collections
                .Where(c => c.UserId == userId)
                .ToListAsync() ?? new List<Collection>();

            InitialTags = await _context.BookTags
                .Where(bt => bt.BookId == id)
                .Select(bt => bt.Tag)
                .ToListAsync();

            InitialCollections = await _context.BookCollections
                .Where(bc => bc.BookId == id)
                .Select(bc => bc.Collection)
                .ToListAsync();

            return Page();
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

            InitialTags = await _context.BookTags
                .Where(bt => bt.BookId == Book.Id)
                .Select(bt => bt.Tag)
                .ToListAsync();

            var initialTagIds = InitialTags?.Select(t => t.Id).ToList() ?? new List<int>();

            List<int> selectedTagIds = new List<int>();
            if (!string.IsNullOrWhiteSpace(SelectedTagIds)) {
                selectedTagIds = SelectedTagIds
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList();
            }

            var tagsToAdd = selectedTagIds.Except(initialTagIds);
            foreach (var tagId in tagsToAdd) {
                var bookTag = new BookTag {
                    BookId = Book.Id,
                    TagId = tagId
                };
                _context.BookTags.Add(bookTag);
            }

            var tagsToRemove = initialTagIds.Except(selectedTagIds);
            foreach (var tagId in tagsToRemove) {
                var bookTagToRemove = _context.BookTags
                    .FirstOrDefault(bt => bt.BookId == Book.Id && bt.TagId == tagId);
                if (bookTagToRemove != null) {
                    _context.BookTags.Remove(bookTagToRemove);
                }
            }

            InitialCollections = await _context.BookCollections
                .Where(bc => bc.BookId == Book.Id)
                .Select(bc => bc.Collection)
                .ToListAsync();

            var initialCollectionIds = InitialCollections?.Select(c => c.Id).ToList() ?? new List<int>();

            List<int> selectedCollectionIds = new List<int>();
            if (!string.IsNullOrWhiteSpace(SelectedCollectionIds)) {
                selectedCollectionIds = SelectedCollectionIds
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList();
            }

            var collectionsToAdd = selectedCollectionIds.Except(initialCollectionIds);
            foreach (var collectionId in collectionsToAdd) {
                var bookCollection = new BookCollection {
                    BookId = Book.Id,
                    CollectionId = collectionId
                };
                _context.BookCollections.Add(bookCollection);
            }

            var collectionsToRemove = initialCollectionIds.Except(selectedCollectionIds);
            foreach (var collectionId in collectionsToRemove) {
                var bookCollectonToRemove = _context.BookCollections
                    .FirstOrDefault(bc => bc.BookId == Book.Id && bc.CollectionId == collectionId);
                if (bookCollectonToRemove != null) {
                    _context.BookCollections.Remove(bookCollectonToRemove);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Books/Details", new { id = Book.Id });
        }
    }
}
