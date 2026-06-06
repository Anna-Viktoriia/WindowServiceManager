using System.ServiceProcess;

namespace WindowsServiceManager.Models
{
    public class ServiceInfo
    {
        public string ServiceName { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public ServiceControllerStatus Status { get; set; }

        public string StatusText { get; set; } = string.Empty;

        public string StartupTypeText { get; set; } = string.Empty;

        public bool CanStop { get; set; }

        public bool IsCritical { get; set; }

        public string CanStopText => CanStop ? "Так" : "Ні";

        public string IsCriticalText => IsCritical ? "Так" : "Ні";
    }
}