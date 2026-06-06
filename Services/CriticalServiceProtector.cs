namespace WindowsServiceManager.Services
{
    public static class CriticalServiceProtector
    {
        private static readonly HashSet<string> CriticalServices = new(StringComparer.OrdinalIgnoreCase)
        {
            "RpcSs",
            "EventLog",
            "PlugPlay",
            "Winmgmt",
            "WinDefend",
            "SecurityHealthService",
            "LanmanWorkstation",
            "Dhcp",
            "Dnscache",
            "NlaSvc",
            "Schedule",
            "Power",
            "LSM",
            "SamSs",
            "AudioEndpointBuilder"
        };

        public static bool IsCritical(string serviceName)
        {
            return CriticalServices.Contains(serviceName);
        }

        public static string GetWarningText(string serviceName)
        {
            return $"Служба \"{serviceName}\" позначена як критична або важлива для стабільної роботи Windows. Дію заблоковано для безпеки.";
        }
    }
}