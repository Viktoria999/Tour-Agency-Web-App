using Microsoft.AspNetCore.Mvc;

namespace web_lab.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}