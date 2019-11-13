using Amazon.Runtime;

namespace thirdconspiracy.Logger.Kinesis
{
    internal interface IKinesisCredentialsProvider
    {
        AWSCredentials GetFulfillmentSystemUserCredentials();
    }
}
