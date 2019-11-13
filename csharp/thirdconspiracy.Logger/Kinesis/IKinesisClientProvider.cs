using Amazon.Kinesis;

namespace thirdconspiracy.Logger.Kinesis
{
    public interface IKinesisClientProvider
    {
        string GetStatLoggingKinesisStreamName();
        AmazonKinesisClient GetKinesisClient();
    }

}
