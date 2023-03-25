using BusinessObjects.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Client.Controllers
{
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
                    return Redirect("~/Course/Detail");
                }
                //Upload Thành công - Lưu vào DB

                BusinessObjects.Entities.File fileEntity = new BusinessObjects.Entities.File();
                fileEntity.CloudId = cloudId;
                fileEntity.FileName = file.FileName;
                fileEntity.CreateDate = DateTime.Now;
                fileEntity.ResourceId = 1;
                try
                {
                    //UploadDB Thành công
                    HttpResponseMessage appResponse = await appClient.PostAsJsonAsync(_fileApiUrl, fileEntity);
                    return Redirect("~/Course/Detail");
                }
                catch (Exception ex)
                {
                    //Upload Thất bại
                    return Redirect("~/Course/Detail");
                }
            }
            return Redirect("~/Course/Detail");
        }

        public async Task<IActionResult> DeleteFile(string cloudId)
        {
            var response = await driveClient.DeleteAsync(_driveAPIUrl + $"/DeleteFile/{cloudId}");
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                //Xóa thất bại trên Cloud
                return Redirect("~/Course/Detail");
            }
            else
            {
                //Xóa Thành công trên Cloud => Xóa trên DB
                var url = _fileApiUrl + $"{cloudId}";
                HttpResponseMessage appResponse = await appClient.DeleteAsync(url);
                if (!appResponse.IsSuccessStatusCode)
                {
                    //Xóa DB Thất bại
                    return Redirect("~/Course/Detail");
                }
                else
                {
                    return Redirect("~/Course/Detail");
                }
            }
        }

        public async Task<IActionResult> DownloadFile(string cloudId)
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
                return Redirect("~/Course/Detail");
            }
        }
    }
}
