using Elasticsearch.Net;
using FindProgrammingProject.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace FindProgrammingProject.Controllers
{
    public class SearchingController : Controller
    {
        public IActionResult Index()
        { 
            return View();
        }
    }
}
