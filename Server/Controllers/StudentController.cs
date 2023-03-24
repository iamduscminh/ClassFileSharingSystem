using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
