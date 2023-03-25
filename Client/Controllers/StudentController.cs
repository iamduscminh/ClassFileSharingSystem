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

        public async Task<IActionResult> Index(int courseId)
        {
            ViewData["courseId"] = courseId;
            HttpResponseMessage response = await _client.GetAsync(_studentApiUrl+ "GetStudentsByCourse/"+courseId);
            string strData = await response.Content.ReadAsStringAsync();

            var students = JsonConvert.DeserializeObject<List<ApplicationUser>>(strData);
            return View(students);
        }
        public async Task<IActionResult> EditStudent()
        {
            HttpResponseMessage response = await _client.GetAsync(_studentApiUrl+"GetStudents");
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

        public async Task<IActionResult> DeleteStudent(string id)
        {
            await _client.DeleteAsync($"{_studentApiUrl}DeleteStudent/{id}");

            return RedirectToAction("EditStudent");
        }

        public async Task<IActionResult> RemoveStudent(string studentId, string courseId)
        {
            var response = await _client.DeleteAsync($"{_studentApiUrl}RemoveStudent/{studentId}/{courseId}");
            var content = response.Content.ReadAsStringAsync();
            return RedirectToAction("Index", new { courseId = courseId});
        }

        [HttpPost]
        public async Task<IActionResult> SearchStudent()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            HttpResponseMessage rpCourse = await _client.GetAsync($"{_courseApiUrl}{userId}/{userRole}");
            string strDataCourse = await rpCourse.Content.ReadAsStringAsync();

            var courses = JsonConvert.DeserializeObject<List<Course>>(strDataCourse);
            ViewData["userId"] = userId;
            ViewData["teacherCourse"] = courses;

            var studentStr = Request.Form["adStudentSearch"].ToString().Trim();

            HttpResponseMessage response = await _client.GetAsync($"{_studentApiUrl}GetStudentsSearch/{studentStr}");
            string strData = await response.Content.ReadAsStringAsync();

            var students = JsonConvert.DeserializeObject<List<ApplicationUser>>(strData);
            return View(@"~/Views/Student/EditStudent.cshtml", students);
        }
    }
}
