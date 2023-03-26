using BusinessObjects.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Client.Controllers
{
    [Authorize]
    public class FileController : Controller
    {
        private readonly HttpClient driveClient;
        private readonly HttpClient appClient;
        private readonly string _fileApiUrl;
        private readonly string _driveAPIUrl;
        public FileController()
        {
            driveClient = new HttpClient();
            appClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            appClient.DefaultRequestHeaders.Accept.Add(contentType);
            var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _fileApiUrl = MyConfig.GetValue<string>("AppSettings:fileApiUrl");
            _driveAPIUrl = MyConfig.GetValue<string>("AppSettings:fileDriveApiUrl");
        }

        [HttpPost]
        public async Task<IActionResult> UploadFormFile(IFormFile file)
        {
            var courseId = TempData["courseId"].ToString();
            var rsId = TempData["rsId"].ToString();
            if (file != null && file.Length > 0)
            {
                var content = new MultipartFormDataContent();
                content.Add(new StreamContent(file.OpenReadStream()), "formFile", file.FileName);

                var response = await driveClient.PostAsync(_driveAPIUrl + "/UploadFile", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                string cloudId = responseContent;
                if (!response.IsSuccessStatusCode)
                {
                    //Upload thất bại
                    TempData["message"] = "Có lỗi xảy ra, vui lòng thử lại sau";
                    return Redirect($"~/Course/Detail?id={courseId}&rsId={rsId}");
                }
                //Upload Thành công - Lưu vào DB

                BusinessObjects.Entities.File fileEntity = new BusinessObjects.Entities.File();
                fileEntity.CloudId = cloudId;
                fileEntity.FileName = file.FileName;
                fileEntity.CreateDate = DateTime.Now;
                fileEntity.ResourceId = Convert.ToInt32(rsId);
                try
                {
                    //UploadDB Thành công
                    HttpResponseMessage appResponse = await appClient.PostAsJsonAsync(_fileApiUrl, fileEntity);
                    TempData["message"] = "Upload Tài liệu thành công";
                    return Redirect($"~/Course/Detail?id={courseId}&rsId={rsId}");
                }
                catch (Exception ex)
                {
                    //Upload Thất bại
                    TempData["message"] = "Có lỗi xảy ra, vui lòng thử lại sau";
                    return Redirect($"~/Course/Detail?id={courseId}&rsId={rsId}");
                }
            }
            TempData["message"] = "Vui lòng chọn Tài liệu để Upload";
            return Redirect($"~/Course/Detail?id={courseId}&rsId={rsId}");
        }

        public async Task<IActionResult> DeleteFile(string cloudId, int courseId, int rsId)
        {
            var response = await driveClient.DeleteAsync(_driveAPIUrl + $"/DeleteFile/{cloudId}");
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                //Xóa thất bại trên Cloud
                TempData["message"] = "Có lỗi xảy ra, vui lòng thử lại sau";
                return Redirect($"~/Course/Detail?id={courseId}&rsId={rsId}");
            }
            else
            {
                //Xóa Thành công trên Cloud => Xóa trên DB
                var url = _fileApiUrl + $"{cloudId}";
                HttpResponseMessage appResponse = await appClient.DeleteAsync(url);
                if (!appResponse.IsSuccessStatusCode)
                {
                    //Xóa DB Thất bại
                    TempData["message"] = "Có lỗi xảy ra, vui lòng thử lại sau";
                    return Redirect($"~/Course/Detail?id={courseId}&rsId={rsId}");
                }
                else
                {
                    TempData["message"] = "Xóa Tài liệu thành công";
                    return Redirect($"~/Course/Detail?id={courseId}&rsId={rsId}");
                }
            }
        }

        public async Task<IActionResult> DownloadFile(string cloudId, int courseId, int rsId)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(_driveAPIUrl + $"/DownloadFile/{cloudId}");
            //var response = await driveClient.GetAsync(_driveAPIUrl + $"/DownloadFile/{cloudId}");
            if (response.IsSuccessStatusCode)
            {
                var fileStream = await response.Content.ReadAsStreamAsync();
                var fileResult = new FileStreamResult(fileStream, response.Content.Headers.ContentType.MediaType)
                {
                    FileDownloadName = response.Content.Headers.ContentDisposition.FileNameStar
                };
                return fileResult;
            }
            else
            {
                TempData["message"] = "Có lỗi xảy ra, vui lòng thử lại sau";
                return Redirect($"~/Course/Detail?id={courseId}&rsId={rsId}");
            }
        }
    }
}
