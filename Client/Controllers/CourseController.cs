using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class CourseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Detail()
        {
            return View();
        }
        public IActionResult ViewStudent()
        {
            return View(@"~/Views/Student/Index.cshtml");
        }
        public IActionResult AddStudent()
        {
            return View(@"~/Views/Student/AddStudent.cshtml");
        }
    }
}