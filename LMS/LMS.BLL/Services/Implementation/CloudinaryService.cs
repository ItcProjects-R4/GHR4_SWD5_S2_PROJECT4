using LMS.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.BLL.Services.Implementation
{
    public class CloudinaryService : ICloudinaryService
    {
        public Task<string> UploadFileAsync(IFormFile file)
        {
            throw new NotImplementedException();
        }

        public Task<string> UploadImageAsync(IFormFile file)
        {
            throw new NotImplementedException();
        }

        public Task<string> UploadVideoAsync(IFormFile file)
        {
            throw new NotImplementedException();
        }
    }
}
