using Microsoft.AspNetCore.Mvc;

namespace web_lab.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
