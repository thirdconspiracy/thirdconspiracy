using System;
using Amazon;
using Amazon.Kinesis;

namespace thirdconspiracy.Logger.Kinesis
{
    public class KinesisClientProvider : IKinesisClientProvider
    {
        private readonly KinesisConfig _cfg;
        private readonly RegionEndpoint _serviceStatLoggingEndpoint = RegionEndpoint.USEast1;
        private readonly Lazy<IKinesisCredentialsProvider> _credProvider;

        private IKinesisCredentialsProvider CredProvider => _credProvider.Value;

        public KinesisClientProvider(KinesisConfig cfg)
        {
            _cfg = cfg;
            _credProvider = new Lazy<IKinesisCredentialsProvider>(() => new KinesisCredentialsProvider(cfg));
        }

        public AmazonKinesisClient GetKinesisClient()
        {
            return new AmazonKinesisClient(
                CredProvider.GetFulfillmentSystemUserCredentials(),
                _serviceStatLoggingEndpoint);
        }

        public string GetStatLoggingKinesisStreamName()
        {
            return _cfg.StreamName;
        }
    }
}