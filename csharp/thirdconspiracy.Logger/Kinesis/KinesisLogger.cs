using System;
using System.IO;
using System.Text;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Newtonsoft.Json;

namespace thirdconspiracy.Logger.Kinesis
{
    public class KinesisLogger
    {
        #region Member Variables

        private readonly KinesisConfig _cfg;
        private readonly Lazy<AmazonKinesisClient> _client;
        private readonly Lazy<string> _streamName;

        private AmazonKinesisClient Client => _client.Value;
        private string StreamName => _streamName.Value;

        #endregion Member Variables

        #region CTOR

        public KinesisLogger(KinesisConfig cfg)
        {
            _cfg = cfg;
            _client = new Lazy<AmazonKinesisClient>(() => new KinesisClientProvider(_cfg).GetKinesisClient());
            _streamName = new Lazy<string>(() => new KinesisClientProvider(_cfg).GetStatLoggingKinesisStreamName());
        }

        #endregion CTOR

        public void LogDocument<T>(string rootIndex, string elasticType, T documentData)
            where T : class
        {
            var actualIndex = GetFormattedIndex(rootIndex);
            var kinesisWrapper = new KinesisLogWrapper<T>(actualIndex, elasticType, false, documentData);
            var json = JsonConvert.SerializeObject(kinesisWrapper);
            var dataBytes = Encoding.UTF8.GetBytes(json);

            var putRecordRequest = new PutRecordRequest()
            {
                StreamName = StreamName,
                Data = new MemoryStream(dataBytes),
                PartitionKey = Guid.NewGuid().ToString()
            };

            var response = Client.PutRecordAsync(putRecordRequest).Result;
        }

        private string GetFormattedIndex(string rootIndex)
        {
            var now = DateTimeOffset.UtcNow;
            var index = $"{_cfg}_{rootIndex}_{now:yyyy-MM-dd}";
            return index.ToLower();
        }

        private class KinesisLogWrapper<T> where T : class
        {
            [JsonProperty(propertyName: "index")]
            public string Index { get; }

            [JsonProperty(propertyName: "type")]
            public string Type { get; }

            [JsonProperty(propertyName: "isMultiRecord")]
            public bool IsMultiRecord { get; }

            [JsonProperty(propertyName: "data")]
            public T Data { get; }

            internal KinesisLogWrapper(string index, string type, bool isMultiRecord, T data)
            {
                Index = index;
                Type = type;
                Data = data;
                IsMultiRecord = isMultiRecord;
            }
        }

    }
}
