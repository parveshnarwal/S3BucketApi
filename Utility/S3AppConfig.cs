namespace S3BucketApi.Utility
{
    public class S3AppConfig : IS3AppConfig
    {
        public string AwsAccessKey { get; set; }
        public string AwsSecretAccessKey { get; set; }
        public string AwsSessionToken { get; set; }
        public string BucketName { get; set; }
        public string Region { get; set; }
    }
}
