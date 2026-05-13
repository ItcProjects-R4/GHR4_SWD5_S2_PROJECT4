using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.BLL.Services.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadVideoAsync(IFormFile file);
        Task<string> UploadImageAsync(IFormFile file);
        Task<string> UploadFileAsync(IFormFile file);
    }
}
