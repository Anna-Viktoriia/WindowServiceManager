namespace WindowsServiceManager.Models
{
    public class LogEntry
    {
        public DateTime Time { get; set; }

        public string TimeText => Time.ToString("dd.MM.yyyy HH:mm:ss");

        public string ServiceName { get; set; } = string.Empty;

        public string ActionName { get; set; } = string.Empty;

        public string Result { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}