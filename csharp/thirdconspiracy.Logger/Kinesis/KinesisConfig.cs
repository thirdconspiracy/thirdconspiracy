namespace thirdconspiracy.Logger.Kinesis
{
    public class KinesisConfig
    {
        public bool IsEnabled { get; set; }
        public string Environment { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string StreamName { get; set; }
    }
}
