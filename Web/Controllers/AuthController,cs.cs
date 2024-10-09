using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class AuthController_cs : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }
    }
}
