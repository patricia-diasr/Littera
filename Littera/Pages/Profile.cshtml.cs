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
        public Tag Tag { get; set; }

        public IList<Collection> Collections { get; set; }
        public IList<Tag> Tags { get; set; }

        public async Task OnGetAsync() {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            Collections = await _context.Collections
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.Priority)
                .ToListAsync();

            Tags = await _context.Tags
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostCollectionAsync() {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Collection.UserId = userId;

            _context.Collections.Add(Collection);
            await _context.SaveChangesAsync();

            return RedirectToPage(); 
        }

        public async Task<IActionResult> OnPostTagAsync() {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Tag.UserId = userId;

            _context.Tags.Add(Tag);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostLogoutAsync() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Signin"); 
        }
    }
}
