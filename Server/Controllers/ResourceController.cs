using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    public class ResourceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
