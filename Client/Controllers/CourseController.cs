using BusinessObjects.Entities;
using Client.Models;
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
        private readonly string _resourceApiUrl;
        private readonly HttpClient _client;

        public CourseController()
        {
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _courseApiUrl = MyConfig.GetValue<string>("AppSettings:courseApiUrl");
            _resourceApiUrl = MyConfig.GetValue<string>("AppSettings:resourceApiUrl");
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            var userEmail = User.FindFirstValue(ClaimTypes.Email); 
            var role = User.FindFirstValue(ClaimTypes.Role); 
            ViewData["teacherId"] = userId;
            ViewData["teacherEmail"] = userEmail;
            HttpResponseMessage response = await _client.GetAsync(_courseApiUrl);
            string strData = await response.Content.ReadAsStringAsync();

            var courses = JsonConvert.DeserializeObject<List<Course>>(strData);
            return View(courses);            
        }
        [HttpPost]
        public async Task<IActionResult> CreateCourse()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            var courseOldName = Request.Form["courseOldName"].ToString().Trim(); 
            var courseName = Request.Form["courseName"].ToString().Trim(); 
            var courseCre = new Course()
            {
                CourseName = courseName,
                CreateDate = DateTime.Now,
                TeacherId = userId
            };

            var x = await _client.PostAsJsonAsync(_courseApiUrl, courseCre);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCourse()
        {
            var courseId = Request.Form["courseId"].ToString().Trim();
            var courseName = Request.Form["courseName"].ToString().Trim();
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var courseUd = new Course()
            {
                CourseId = Convert.ToInt32(courseId),
                CourseName = courseName,
                TeacherId = teacherId
                
            };

            var content = JsonConvert.SerializeObject(courseUd);
            var buffer = System.Text.Encoding.UTF8.GetBytes(content);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _client.PutAsync($"{_courseApiUrl}{courseId}", byteContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateResource()
        {
            var resourceId = Request.Form["resourceId"].ToString().Trim();
            var resourceName = Request.Form["resourceName"].ToString().Trim();
            var courseId = Convert.ToInt32(Request.Form["courseId"].ToString().Trim());
            var resourceUd = new Resource()
            {
                ResourceId = Convert.ToInt32(resourceId),
                ResourceName = resourceName,

            };

            var content = JsonConvert.SerializeObject(resourceUd);
            var buffer = System.Text.Encoding.UTF8.GetBytes(content);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _client.PutAsync($"{_resourceApiUrl}{resourceId}", byteContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            return RedirectToAction("Detail", new { id = courseId });
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (id == 0) return View(new Course());
            HttpResponseMessage rpPrd = await _client.GetAsync($"{_courseApiUrl}{id}");
            string strDataPrd = await rpPrd.Content.ReadAsStringAsync();

            var course = JsonConvert.DeserializeObject<Course>(strDataPrd);
            return View(course);
        }

        [HttpPost]
        public async Task<IActionResult> CreateResource()
        {
            var folderName = Request.Form["folderName"].ToString().Trim();
            var courseId = Convert.ToInt32(Request.Form["courseId"].ToString().Trim());
            var rsCre = new Resource()
            {
                ResourceName = folderName,
                CreateDate = DateTime.Now,
                CourseId = courseId
            };

            var x = await _client.PostAsJsonAsync(_resourceApiUrl, rsCre);
            return RedirectToAction("Detail", new {id = courseId});
        }

        [HttpPost]
        public async Task<IActionResult> UploadFormFile(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var content = new MultipartFormDataContent();
                content.Add(new StreamContent(file.OpenReadStream()), "formFile", file.FileName);

                var clientFile = new HttpClient();
                var response = await clientFile.PostAsync("http://localhost:2507/api/FileManagement/UploadFile", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return Redirect("~/Course/Detail");
                }
                return Redirect("~/Course/Detail");
            }
            return Redirect("~/Course/Detail");
        }
    }
}