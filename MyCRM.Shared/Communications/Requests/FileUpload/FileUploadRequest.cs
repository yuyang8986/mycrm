using Microsoft.AspNetCore.Http;
using MyCRM.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.Communications.Requests.FileUpload
{
    public class FileUploadRequest
    {
        //public string DealId { get; set; }
        public IFormFile File { get; set; }

        //public byte[] File { get; set; }

        public string FileName { get; set; }
        public string FileType = UploadFileTypes.DealFiles;
    }

    public class FileArrayUploadRequest
    {
        //public string DealId { get; set; }
        //public IFormFile File { get; set; }
        public byte[] File { get; set; }

        public string FileName { get; set; }
        public string FileType = UploadFileTypes.DealFiles;
    }
}