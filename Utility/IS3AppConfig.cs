namespace S3BucketApi.Utility
{
    public interface IS3AppConfig
    {
        string AwsAccessKey { get; set; }
        string AwsSecretAccessKey { get; set; }
        string AwsSessionToken { get; set; }
        string BucketName { get; set; }
        string Region { get; set; }



    }
}
