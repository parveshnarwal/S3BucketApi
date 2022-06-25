using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace S3BucketApi.Services
{
    public class S3Service : IS3Service
    {
        private readonly string _bucketName;
        private readonly IAmazonS3 _awsS3Client;
        public S3Service(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken, string region, string bucketName)
        {
            _bucketName = bucketName;
            _awsS3Client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, RegionEndpoint.GetBySystemName(region));
        }
        public async Task DeleteFileAsync(string fileName, string versionId = "")
        {
			DeleteObjectRequest request = new DeleteObjectRequest
			{
				BucketName = _bucketName,
				Key = fileName
			};

			if (!string.IsNullOrEmpty(versionId))
				request.VersionId = versionId;

			await _awsS3Client.DeleteObjectAsync(request);
		}

        public async Task<byte[]> DownloadFileAsync(string file)
        {
			MemoryStream ms = null;

			try
			{
				GetObjectRequest getObjectRequest = new GetObjectRequest
				{
					BucketName = _bucketName,
					Key = file
				};

				using (var response = await _awsS3Client.GetObjectAsync(getObjectRequest))
				{
					if (response.HttpStatusCode == HttpStatusCode.OK)
					{
						using (ms = new MemoryStream())
						{
							await response.ResponseStream.CopyToAsync(ms);
						}
					}
				}

				if (ms is null || ms.ToArray().Length < 1)
					throw new FileNotFoundException(string.Format("The document '{0}' is not found", file));

				return ms.ToArray();
			}
			catch (Exception)
			{
				throw;
			}
		}

        public async Task<bool> UploadFileAsync(IFormFile file)
        {
			try
			{
				using (var newMemoryStream = new MemoryStream())
				{
					file.CopyTo(newMemoryStream);

					var uploadRequest = new TransferUtilityUploadRequest
					{
						InputStream = newMemoryStream,
						Key = file.FileName,
						BucketName = _bucketName,
						ContentType = file.ContentType
					};

					var fileTransferUtility = new TransferUtility(_awsS3Client);

					await fileTransferUtility.UploadAsync(uploadRequest);

					return true;
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

        public bool IsFileExists(string fileName, string versionId)
        {
            try
            {
                GetObjectMetadataRequest request = new GetObjectMetadataRequest()
                {
                    BucketName = _bucketName,
                    Key = fileName,
                    VersionId = !string.IsNullOrEmpty(versionId) ? versionId : null
                };

                var response = _awsS3Client.GetObjectMetadataAsync(request).Result;

                return true;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException is AmazonS3Exception awsEx)
                {
                    if (string.Equals(awsEx.ErrorCode, "NoSuchBucket"))
                        return false;

                    else if (string.Equals(awsEx.ErrorCode, "NotFound"))
                        return false;
                }

                throw;
            }
        }
    }
}
