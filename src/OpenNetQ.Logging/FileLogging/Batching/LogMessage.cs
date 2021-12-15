namespace OpenNetQ.Logging.FileLogging.Batching
{
    public struct LogMessage
    {
        public DateTimeOffset Timestamp { get; set; }

        public string Message { get; set; }
    }
}
