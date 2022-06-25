using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace S3BucketApi.Services
{
    public interface IS3Service
    {
        Task<byte[]> DownloadFileAsync(string file);

        Task<bool> UploadFileAsync(IFormFile file);

        Task DeleteFileAsync(string fileName, string versionId = "");

        bool IsFileExists(string fileName, string versionId);
    }
}
