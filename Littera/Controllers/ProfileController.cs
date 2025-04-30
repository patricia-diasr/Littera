using Microsoft.AspNetCore.Mvc;

namespace Littera.Controllers {
    public class ProfileController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
