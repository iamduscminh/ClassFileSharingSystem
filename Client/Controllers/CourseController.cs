using BusinessObjects.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Security.Claims;

namespace Client.Controllers
{
    public class CourseController : Controller
    {
        private readonly string _courseApiUrl;
        private readonly HttpClient _client;

        public CourseController()
        {
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _courseApiUrl = MyConfig.GetValue<string>("AppSettings:courseApiUrl");
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
            var userName = User.FindFirstValue(ClaimTypes.Name); // will give the user's userName
            var course = new Course()
            {
                TeacherId = userId,
            };
            HttpResponseMessage response = await _client.GetAsync(_courseApiUrl);
            string strData = await response.Content.ReadAsStringAsync();

            var courses = JsonConvert.DeserializeObject<List<Course>>(strData);
            return View(courses);            
        }
        [HttpPost]
        public async Task<IActionResult> CreateCourse()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId 
            var courseOldName = Request.Form["courseOldName"];
            var courseName = Request.Form["courseName"].FirstOrDefault();
            var courseCre = new Course()
            {
                CourseName = courseName,
                CreateDate = DateTime.Now,
                TeacherId = userId
            };

            var x = await _client.PostAsJsonAsync(_courseApiUrl, courseCre);
            return RedirectToAction("Index");
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