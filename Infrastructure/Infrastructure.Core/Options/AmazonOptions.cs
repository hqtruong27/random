namespace Infrastructure.Options;

public class AwsOptions
{
    public string AccessKey { get; set; } = default!;
    public string SecretKey { get; set; } = default!;
    public string TopicArn { get; set; } = default!;
    public string SqsQueueUrl { get; set; } = default!;
    public string Region { get; set; } = default!;
}
