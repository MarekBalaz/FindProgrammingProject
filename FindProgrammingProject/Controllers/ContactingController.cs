using Microsoft.AspNetCore.Mvc;

namespace FindProgrammingProject.Controllers
{
    public class ContactingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
