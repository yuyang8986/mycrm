using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.File;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Communications.Requests.FileUpload;
using MyCRM.Shared.Communications.Responses.UploadFile;

namespace MyCRM.API.Controllers
{
    [Route("api/uploadfile")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountUserService _accountUserService;
        private CloudStorageAccount _cloudStorageAccount;
        private CloudBlobClient _cloudBlobClient;
        private CloudBlobContainer _cloudBlobContainer;

        public UploadFileController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IAccountUserService accountUserService)
        {
            _accountUserService = accountUserService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _cloudStorageAccount = CloudStorageAccount.Parse(_configuration["StorageConnectionString"]);
            _cloudBlobClient = _cloudStorageAccount.CreateCloudBlobClient();
        }

        [HttpPost("{DealId}")]
        public async Task<IActionResult> Upload(string DealId, [FromForm] FileUploadRequest request)
        {
            if (request.File == null)
            {
                return BadRequest("Null File");
            }

            using (var stream = request.File.OpenReadStream())
            {
                await UploadFileToBlob(request.File.FileName, request.FileType, DealId, stream);
            }

            return Ok();
        }

        [HttpPost]
        [Route("web/{DealId}")]
        public async Task<IActionResult> UploadByteArray(string DealId, [FromBody] FileArrayUploadRequest request)
        {
            if (request.File == null)
            {
                return BadRequest("Null File");
            }

            using (var stream = new MemoryStream(request.File))
            {
                await UploadFileToBlob(request.FileName, request.FileType, DealId, stream);
            }

            return Ok();
        }

        [HttpGet("{DealId}")]
        public async Task<List<FileItem>> GetFileList(string DealId)
        {
            try
            {
                var user = await _accountUserService.GetUserWithEmployeeOrganizationData();
                var containerName = user.Organization.Name.ToLower() + user.OrganizationId.ToString().ToLower();
                containerName = containerName.Replace(" ", "");
                _cloudBlobContainer = _cloudBlobClient.GetContainerReference(containerName);
                CloudBlob blob;
                List<FileItem> fileItems = new List<FileItem>();
                BlobContinuationToken continuationToken = null;
                do
                {
                    BlobResultSegment resultSegment = await _cloudBlobContainer.ListBlobsSegmentedAsync(string.Empty,
                   true, BlobListingDetails.Metadata, null, null, null, null);
                    foreach (var blobItem in resultSegment.Results)
                    {
                        blob = (CloudBlob)blobItem;
                        string[] nameArray = blob.Name.Split('/');
                        if (nameArray[2] == DealId)
                        {
                            FileItem file = new FileItem();
                            file.Name = nameArray.Last();
                            file.URL = blob.Uri.ToString();
                            fileItems.Add(file);
                        }
                        continuationToken = resultSegment.ContinuationToken;
                    }
                    return fileItems;
                } while (continuationToken != null);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<bool> UploadFileToBlob(string fileName, string fileType, string DealId, Stream stream = null)
        {
            try
            {
                var user = await _accountUserService.GetUserWithEmployeeOrganizationData();
                var containerName = user.Organization.Name.ToLower() + user.OrganizationId.ToString().ToLower();
                containerName = containerName.Replace(" ", "");
                _cloudBlobContainer = _cloudBlobClient.GetContainerReference(containerName);
                await _cloudBlobContainer.CreateIfNotExistsAsync();
                fileName = $"{user.Id}/{fileType}/{DealId}/{fileName}";
                BlobContainerPermissions permissions = new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Container
                };
                await _cloudBlobContainer.SetPermissionsAsync(permissions);
                CloudBlockBlob cloudBlockBlob = _cloudBlobContainer.GetBlockBlobReference(fileName);
                if (stream != null)
                {
                    await cloudBlockBlob.UploadFromStreamAsync(stream);
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}