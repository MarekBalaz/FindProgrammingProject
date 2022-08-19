using Microsoft.AspNetCore.Mvc;

namespace FindProgrammingProject.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
