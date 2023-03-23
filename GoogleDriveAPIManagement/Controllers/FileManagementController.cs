using Microsoft.AspNetCore.Mvc;
using Google.Apis.Drive.v3.Data;
using GoogleDriveAPIManagement.DriveServiceConnection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting.Server;
using Google.Apis.Upload;
using static Google.Apis.Requests.BatchRequest;
using System.Net;

namespace GoogleDriveAPIManagement
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FileManagementController : Controller
    {
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult>  UploadFile(IFormFile formFile)
        {
            IConfiguration configuration = GetConfiguration("appsettings.json");
            string folderId = configuration["UploadFolder"].ToString();

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = formFile.FileName,
                Parents = new[] { folderId }
            };

            var stream = formFile.OpenReadStream();
            var mimeType = GetMimeType(formFile.FileName);


            //byte[] byteArray = System.IO.File.ReadAllBytes("Test.txt");
            //var stream = new MemoryStream(byteArray);

            var service = DriveServiceConnection.DriveServiceConnection.GetConnectionInstance();
            var uploadRequest = service.Files.Create(fileMetadata, stream, mimeType);
            uploadRequest.Fields = "id";

            var progress = await uploadRequest.UploadAsync();

            if(progress.Status == UploadStatus.Completed)
            {
                var file = uploadRequest.ResponseBody;
                return Ok(file.Id);
            }
            else
            {
                return new ObjectResult("Upload Failed") { StatusCode = 500 };
            }
        }

        [HttpGet("{fileId}")]
        public async Task<ActionResult<FileResult>> DownloadFile(string fileId)
        {
            var service = DriveServiceConnection.DriveServiceConnection.GetConnectionInstance();
            Google.Apis.Drive.v3.Data.File file = service.Files.Get(fileId).Execute();
            var request = service.Files.Get(fileId);

            using (var stream = new MemoryStream())
            {
                await request.DownloadAsync(stream);

                // Create a new FileStreamResult with the file's content and metadata.
                var fileStreamResult = new FileStreamResult(new MemoryStream(stream.ToArray()), file.MimeType)
                {
                    FileDownloadName = file.Name
                };
                return fileStreamResult;
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Google.Apis.Drive.v3.Data.File>>> GetAllFile()
        {
            var service = DriveServiceConnection.DriveServiceConnection.GetConnectionInstance();
            var fileList = new List<Google.Apis.Drive.v3.Data.File>();

            IConfiguration configuration = GetConfiguration("appsettings.json");
            string folderId = configuration["UploadFolder"].ToString();

            string pageToken = null;
            do
            {
                var request2 = service.Files.List();
                request2.Q = $"'{folderId}' in parents and trashed=false";
                request2.IncludeItemsFromAllDrives = true;
                request2.SupportsAllDrives = true;

                request2.PageToken = pageToken;
                var result = request2.Execute();
                fileList.AddRange(result.Files);

                pageToken = result.NextPageToken;
            } while (pageToken != null);

            return fileList;
        }

        [HttpDelete("{fileId}")]
        public async Task<IActionResult> DeleteFile(string fileId)
        {
            IConfiguration configuration = GetConfiguration("appsettings.json");
            string folderId = configuration["UploadFolder"].ToString();

            var service = DriveServiceConnection.DriveServiceConnection.GetConnectionInstance();

            var deleteRequest = service.Files.Delete(fileId);
            try
            {
                var response = deleteRequest.Execute();
                return Ok(fileId);
            }
            catch (Exception)
            {
                return new ObjectResult("Delete Failed") { StatusCode = 500 };
            }
        }
        private static string GetMimeType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            switch (ext)
            {
                case ".doc":
                    return "application/msword";
                case ".pdf":
                    return "application/pdf";
                case ".png":
                    return "image/png";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".txt":
                    return "text/plain";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".pptx":
                    return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                default:
                    return "application/octet-stream";
            }
        }

        private IConfiguration GetConfiguration(string name)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(name, optional: true, reloadOnChange: true);

            IConfiguration configuration = builder.Build();
            return configuration;
        }
    }
}
