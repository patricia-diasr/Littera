using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Littera.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Littera.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;

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
        public IFormFile AvatarFile { get; set; }

        [BindProperty]
        public User AuthenticatedUser { get; set; }

        [BindProperty]
        public User EditUser { get; set; }

        [BindProperty]
        public PasswordUpdateModel PasswordUpdate { get; set; }

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
            var userId = AuthenticatedUser.Id;

            Collections = await _context.Collections
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.Priority)
                .ToListAsync() ?? new List<Collection>();

            Tags = await _context.Tags
                .Where(t => t.UserId == userId)
                .ToListAsync() ?? new List<Tag>();
        }

        public async Task<IActionResult> OnPostCollectionAsync() {
            var userId = AuthenticatedUser.Id;
            Collection.UserId = userId;

            _context.Collections.Add(Collection);
            await _context.SaveChangesAsync();

            return RedirectToPage(); 
        }
        public async Task<IActionResult> OnPostEditUserAsync() {
            var userId = AuthenticatedUser.Id;

            var userInDb = await _context.Users.FindAsync(userId);
            if (userInDb == null) {
                ModelState.AddModelError(string.Empty, "Usuário não encontrado.");
                return Page();
            }

            if (string.IsNullOrWhiteSpace(EditUser.Name)) {
                return new JsonResult(new { success = false, error = "O nome é obrigatório." });
            }

            userInDb.Name = EditUser.Name;

            bool wantsToChangePassword = !string.IsNullOrWhiteSpace(PasswordUpdate.NewPassword) ||
                                         !string.IsNullOrWhiteSpace(PasswordUpdate.ConfirmPassword);

            if (wantsToChangePassword) {
                if (string.IsNullOrWhiteSpace(PasswordUpdate.CurrentPassword)) {
                    return new JsonResult(new { success = false, error = "A senha atual é obrigatória para alterar a senha." });
                }

                if (PasswordUpdate.NewPassword.Length < 8) {
                    return new JsonResult(new { success = false, error = "A nova senha deve ter no mínimo 8 caracteres." });
                }

                if (PasswordUpdate.NewPassword != PasswordUpdate.ConfirmPassword) {
                    return new JsonResult(new { success = false, error = "As senhas não coincidem." });

                }

                if (userInDb.Password != PasswordUpdate.CurrentPassword) {
                    return new JsonResult(new { success = false, error = "Senha atual incorreta." });

                }

                userInDb.Password = PasswordUpdate.NewPassword;
            }

            if (AvatarFile != null && AvatarFile.Length > 0) {
                var uploadsFolder = Path.Combine("wwwroot", "uploads", "avatars");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(AvatarFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                if (!string.IsNullOrEmpty(userInDb.Photo)) {
                    var oldFilePath = Path.Combine("wwwroot", userInDb.Photo.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath)) {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                using (var fileStream = new FileStream(filePath, FileMode.Create)) {
                    await AvatarFile.CopyToAsync(fileStream);
                }

                userInDb.Photo = $"/uploads/avatars/{uniqueFileName}";
            }

            _context.Users.Update(userInDb);
            await _context.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userInDb.Id.ToString()),
                new Claim(ClaimTypes.Name, userInDb.Name),
                new Claim(ClaimTypes.Email, userInDb.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return new JsonResult(new { success = true });
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
            var userId = AuthenticatedUser.Id;
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

        public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next) {
            var userClaims = context.HttpContext.User;
            int userId = int.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _context.Users.FindAsync(userId);

            if (userClaims.Identity != null && userClaims.Identity.IsAuthenticated) {
                AuthenticatedUser = new User {
                    Id = userId,
                    Name = user.Name,
                    Email = user.Email,
                    Photo = user.Photo ?? ""
                };
            }

            await next(); 
        }

        public class PasswordUpdateModel {
            public string CurrentPassword { get; set; }
            public string NewPassword { get; set; }
            public string ConfirmPassword { get; set; }
        }
    }
}
