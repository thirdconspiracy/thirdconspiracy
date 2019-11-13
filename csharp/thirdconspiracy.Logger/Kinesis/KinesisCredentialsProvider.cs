using Amazon.Runtime;

namespace thirdconspiracy.Logger.Kinesis
{
    internal class KinesisCredentialsProvider : IKinesisCredentialsProvider
    {
        private KinesisConfig _cfg;

        public KinesisCredentialsProvider(KinesisConfig cfg)
        {
            _cfg = cfg;
        }


        public AWSCredentials GetFulfillmentSystemUserCredentials()
        {
            return new BasicAWSCredentials(_cfg.PublicKey, _cfg.PrivateKey);
        }
    }
}
