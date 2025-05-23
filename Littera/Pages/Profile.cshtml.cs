using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Littera.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Littera.Data;
using System.Security.Claims;

namespace Littera.Pages
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        
        private readonly LitteraContext _context;
        
        public ProfileModel(LitteraContext context) {
            _context = context;
        }

        [BindProperty]
        public Collection Collection { get; set; }

        [BindProperty]
        public Collection CollectionEdit { get; set; }

        [BindProperty]
        public int CollectionDeleteId { get; set; }

        public IList<Collection> Collections { get; set; }

        [BindProperty]
        public Tag Tag { get; set; }

        [BindProperty]
        public Tag TagEdit { get; set; }

        [BindProperty]
        public int TagDeleteId { get; set; }

        public IList<Tag> Tags { get; set; }

        public async Task OnGetAsync() {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            Collections = await _context.Collections
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.Priority)
                .ToListAsync() ?? new List<Collection>();

            Tags = await _context.Tags
                .Where(t => t.UserId == userId)
                .ToListAsync() ?? new List<Tag>();
        }

        public async Task<IActionResult> OnPostCollectionAsync() {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Collection.UserId = userId;

            _context.Collections.Add(Collection);
            await _context.SaveChangesAsync();

            return RedirectToPage(); 
        }

        public async Task<IActionResult> OnPostEditCollectionAsync() {
            var collection = await _context.Collections.FindAsync(CollectionEdit.Id);

            if (collection != null) {
                collection.Name = CollectionEdit.Name;
                collection.Color = CollectionEdit.Color;
                collection.Priority = CollectionEdit.Priority;

                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteCollectionAsync() {
            var collection = await _context.Collections.FindAsync(CollectionDeleteId);

            if (collection != null) {
                _context.Collections.Remove(collection);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostTagAsync() {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Tag.UserId = userId;

            _context.Tags.Add(Tag);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditTagAsync() {
            var tag = await _context.Tags.FindAsync(TagEdit.Id);

            if (tag != null) {
                tag.Name = TagEdit.Name;
                tag.Color = TagEdit.Color;

                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteTagAsync() {
            var tag = await _context.Tags.FindAsync(TagDeleteId);

            if (tag != null) {
                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostLogoutAsync() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Signin"); 
        }
    }
}
