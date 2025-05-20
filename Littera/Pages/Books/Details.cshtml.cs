using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Littera.Pages.Books
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
