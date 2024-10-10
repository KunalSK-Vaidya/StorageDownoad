using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using StorageDownoad.Models;
using System.Diagnostics;
using System.Net.Mime;
using System.Runtime;

namespace StorageDownoad.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Download(string fileName)
        {
            if (fileName == null)
                return NotFound();

            var connectionstring = "DefaultEndpointsProtocol=https;AccountName=skyhighstoragetest;AccountKey=**************************************************;EndpointSuffix=core.windows.net";

            string contentType = "application/octet-stream";
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionstring);
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient("downloads");
            BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);
            if ((bool)blobClient.Exists() && blobClient != null)
            {
                ContentDisposition contentDisposition = new ContentDisposition
                {
                    FileName = fileName,
                    Inline = false
                };
                using BlobDownloadStreamingResult blobDownloadStreamingResult = (BlobDownloadStreamingResult)(await blobClient.DownloadStreamingAsync());
                return new FileStreamResult(blobDownloadStreamingResult.Content, contentType);
            }
            else
            {
                throw new FileNotFoundException("Blob Client/File Not Found");
            }
        }
    }
}
