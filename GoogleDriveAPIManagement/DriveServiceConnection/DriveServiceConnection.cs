using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace GoogleDriveAPIManagement.DriveServiceConnection
{
    public class DriveServiceConnection
    {
        public static DriveService GetConnectionInstance()
        {
            var credential = GoogleCredential.FromFile("service-account.json")
            .CreateScoped(new[] { DriveService.Scope.Drive });
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "FileSharingSystem"
            });
            return service;
        }
    }
}
