using BusinessObjects.Entities;
using Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Security.Claims;

namespace Client.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {
        private readonly string _courseApiUrl;
        private readonly string _resourceApiUrl;
        private readonly string _fileApiUrl;
        private readonly string _driveAPIUrl;
        private readonly HttpClient _client;
        private readonly HttpClient driveClient;
        private readonly string TempModelKey;
        public CourseController()
        {
            _client = new HttpClient();
            driveClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _courseApiUrl = MyConfig.GetValue<string>("AppSettings:courseApiUrl");
            _resourceApiUrl = MyConfig.GetValue<string>("AppSettings:resourceApiUrl");
            _fileApiUrl = MyConfig.GetValue<string>("AppSettings:fileApiUrl");
            _driveAPIUrl = MyConfig.GetValue<string>("AppSettings:fileDriveApiUrl");
            TempModelKey = "CourseTempModel";
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);

            ViewData["role"] = role;
            ViewData["teacherId"] = userId;
            ViewData["teacherEmail"] = userEmail;
            HttpResponseMessage response = await _client.GetAsync($"{_courseApiUrl}{userId}/{role}");
            string strData = await response.Content.ReadAsStringAsync();
            HttpContext.Session.SetString(TempModelKey, strData);
            var courses = JsonConvert.DeserializeObject<List<Course>>(strData);
            return View(courses);
        }
        [HttpPost]
        public async Task<IActionResult> CreateCourse()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var courseCode = Guid.NewGuid().ToString();
            var courseName = Request.Form["courseName"].ToString().Trim();
            var courseCre = new Course()
            {
                CourseName = courseName,
                CourseCode = courseCode,
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
            var courseCode = Request.Form["courseCode"].ToString().Trim();
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var courseUd = new Course()
            {
                CourseId = Convert.ToInt32(courseId),
                CourseName = courseName,
                CourseCode = courseCode,
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

        public async Task<IActionResult> Detail(int id, int rsId = 0)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            ViewData["role"] = role;
            if (id == 0) return View(new Course());
            HttpResponseMessage rpPrd = await _client.GetAsync($"{_courseApiUrl}{id}");
            string strDataPrd = await rpPrd.Content.ReadAsStringAsync();

            var course = JsonConvert.DeserializeObject<Course>(strDataPrd);
            if (course.Resources != null && course.Resources.Any())
            {
                if (rsId == 0) rsId = course.Resources.FirstOrDefault().ResourceId;
                HttpResponseMessage rs = await _client.GetAsync($"{_resourceApiUrl}{rsId}");
                string strDataRs = await rs.Content.ReadAsStringAsync();

                var resource = JsonConvert.DeserializeObject<Resource>(strDataRs);
                ViewData["files"] = resource.Files;
                ViewData["rsId"] = rsId;
                course.Resources.FirstOrDefault().Files = resource.Files;
            }
            


            return View(course);
        }
        public async Task<IActionResult> DeleteResource(string resourceId, int courseId)
        {
            HttpResponseMessage rs = await _client.GetAsync($"{_resourceApiUrl}{resourceId}");
            string strDataRs = await rs.Content.ReadAsStringAsync();
            var resource = JsonConvert.DeserializeObject<Resource>(strDataRs);

            foreach (var item in resource.Files)
            {
                var response = await driveClient.DeleteAsync(_driveAPIUrl + $"/DeleteFile/{item.CloudId}");
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    //Xóa thất bại trên Cloud
                    TempData["message"] = "Có lỗi xảy ra, vui lòng thử lại sau";
                    return RedirectToAction("Detail", new { id = courseId });
                }
                else
                {
                    //Xóa Thành công trên Cloud => Xóa trên DB
                    var url = _fileApiUrl + $"{item.CloudId}";
                    HttpResponseMessage appResponse = await _client.DeleteAsync(url);
                    if (!appResponse.IsSuccessStatusCode)
                    {
                        //Xóa DB Thất bại
                        TempData["message"] = "Có lỗi xảy ra, vui lòng thử lại sau";
                        return RedirectToAction("Detail", new { id = courseId });
                    }
                }
            }
            HttpResponseMessage contentDel = await _client.DeleteAsync($"{_resourceApiUrl}{resourceId}");
            var contentRpDel = contentDel.Content.ReadAsStringAsync();

            return RedirectToAction("Detail", new { id = courseId});
        }

        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            var strData = HttpContext.Session.GetString(TempModelKey);
            var courses = JsonConvert.DeserializeObject<List<Course>>(strData);
            var course = courses.FirstOrDefault(x => x.CourseId == courseId);

            if (course == null) RedirectToAction("Index");

            foreach (var r in course.Resources)
            {
                HttpResponseMessage rs = await _client.GetAsync($"{_resourceApiUrl}{r.ResourceId}");
                string strDataRs = await rs.Content.ReadAsStringAsync();
                var resource = JsonConvert.DeserializeObject<Resource>(strDataRs);

                foreach (var item in resource.Files)
                {
                    var response = await driveClient.DeleteAsync(_driveAPIUrl + $"/DeleteFile/{item.CloudId}");
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        //Xóa thất bại trên Cloud
                        TempData["message"] = "Có lỗi xảy ra, vui lòng thử lại sau";
                        RedirectToAction("Index");
                    }
                    else
                    {
                        //Xóa Thành công trên Cloud => Xóa trên DB
                        var url = _fileApiUrl + $"{item.CloudId}";
                        HttpResponseMessage appResponse = await _client.DeleteAsync(url);
                        if (!appResponse.IsSuccessStatusCode)
                        {
                            //Xóa DB Thất bại
                            TempData["message"] = "Có lỗi xảy ra, vui lòng thử lại sau";
                            RedirectToAction("Index");
                        }
                    }
                }
            }
            //Xóa Course trên Db
            HttpResponseMessage appResponseDelCourse = await _client.DeleteAsync($"{_courseApiUrl}{courseId}");
            var responseContentDel = await appResponseDelCourse.Content.ReadAsStringAsync();
            return RedirectToAction("Index");
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
            return RedirectToAction("Detail", new { id = courseId });
        }

        [HttpPost]
        public async Task<IActionResult> JoinCourse()
        {
            var courseCode = Request.Form["courseCode"].ToString().Trim();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var x = await _client.PostAsJsonAsync(_courseApiUrl+ userId + "/" + courseCode, courseCode);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> OutCourse(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var x = await _client.PostAsJsonAsync(_courseApiUrl+ "OutCourse" + "/" + userId + "/" + courseId, courseId);
            return RedirectToAction("Index");
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
                    return Redirect("~/Course");
                }
                return Redirect("~/Course");
            }
            return Redirect("~/Course");
        }

        
    }
}