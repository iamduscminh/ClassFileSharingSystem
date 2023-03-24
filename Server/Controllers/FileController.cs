using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    public class FileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
