using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using S3BucketApi.Services;
using S3BucketApi.Utility;
using System;
using System.Net;

namespace S3BucketApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class S3BucketController : ControllerBase
    {
        
        private readonly IS3Service s3Service;

        public S3BucketController(IS3Service s3Service)
        {
            this.s3Service = s3Service;
        }


        [HttpGet("{documentName}")]
        public IActionResult Get(string documentName)
        {
            try
            {
                if (string.IsNullOrEmpty(documentName))
                    return BadRequest("The 'documentName' parameter is required");

                var document = s3Service.DownloadFileAsync(documentName).Result;

                return File(document, "application/octet-stream", documentName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post(IFormFile file)
        {
            try
            {
                if (file is null || file.Length <= 0)
                    return BadRequest("File is required to upload");

                var result = s3Service.UploadFileAsync(file);

                return Created(string.Empty, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{fileName}/{version?}")]
        public IActionResult Delete(string fileName, string version)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                    return BadRequest("The 'documentName' parameter is required");

                if (!s3Service.IsFileExists(fileName, version))
                    return NotFound();

                s3Service.DeleteFileAsync(fileName);

                return Ok(string.Format("The document '{0}' is deleted successfully", fileName));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        
    }
}
