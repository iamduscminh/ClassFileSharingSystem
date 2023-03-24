using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
