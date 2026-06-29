using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LMS.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
namespace LMS.BLL.Services.Implementation
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }
        public async Task<string> UploadFileAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var result = await _cloudinary.UploadAsync(new RawUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "lms/files",
                AccessMode = "public"
            });
            return result.SecureUrl.ToString();
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var result = await _cloudinary.UploadAsync(new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "lms/images"
            });
            return result.SecureUrl.ToString();
        }

        public async Task<string> UploadVideoAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var result = await _cloudinary.UploadAsync(new VideoUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "lms/videos"
            });
            return result.SecureUrl.ToString();
        }
    }
}
