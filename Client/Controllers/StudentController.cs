using BusinessObjects.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Client.Controllers
{
    public class StudentController : Controller
    {
        private readonly string _studentApiUrl;
        private readonly string _courseApiUrl;
        private readonly HttpClient _client;

        public StudentController()
        {
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _studentApiUrl = MyConfig.GetValue<string>("AppSettings:studentApiUrl");
            _courseApiUrl = MyConfig.GetValue<string>("AppSettings:courseApiUrl");
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);
            
            HttpResponseMessage response = await _client.GetAsync(_studentApiUrl);
            string strData = await response.Content.ReadAsStringAsync();

            var students = JsonConvert.DeserializeObject<List<ApplicationUser>>(strData);
            return View(students);
        }
        public async Task<IActionResult> EditStudent()
        {
            HttpResponseMessage response = await _client.GetAsync(_studentApiUrl);
            string strData = await response.Content.ReadAsStringAsync();
            var students = JsonConvert.DeserializeObject<List<ApplicationUser>>(strData);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            
            HttpResponseMessage rpCourse = await _client.GetAsync($"{_courseApiUrl}{userId}/{userRole}");
            string strDataCourse = await rpCourse.Content.ReadAsStringAsync();

            var courses = JsonConvert.DeserializeObject<List<Course>>(strDataCourse);
            ViewData["userId"] = userId;
            ViewData["teacherCourse"] = courses;

            return View(students);
        }
    }
}
