using Microsoft.AspNetCore.Mvc;

namespace FindProgrammingProject.Controllers
{
    public class ProjectController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
