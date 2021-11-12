using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.Communications.Requests.FileUpload
{
    public class ScanCardRequest
    {
        public IFormFile File { get; set; }
    }
}
