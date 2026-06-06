using System.Drawing;
using System.ServiceProcess;
using WindowsServiceManager.UI;

namespace WindowsServiceManager.Helpers
{
    public static class ServiceStatusHelper
    {
        public static string ToUkrainianText(ServiceControllerStatus status)
        {
            return status switch
            {
                ServiceControllerStatus.Running => "Працює",
                ServiceControllerStatus.Stopped => "Зупинена",
                ServiceControllerStatus.Paused => "Призупинена",
                ServiceControllerStatus.StartPending => "Запускається",
                ServiceControllerStatus.StopPending => "Зупиняється",
                ServiceControllerStatus.PausePending => "Призупиняється",
                ServiceControllerStatus.ContinuePending => "Відновлюється",
                _ => "Невідомо"
            };
        }

        public static Color GetStatusColor(ServiceControllerStatus status)
        {
            return status switch
            {
                ServiceControllerStatus.Running => ThemeManager.SuccessColor,
                ServiceControllerStatus.Stopped => ThemeManager.DangerColor,
                ServiceControllerStatus.Paused => ThemeManager.WarningColor,
                ServiceControllerStatus.StartPending => ThemeManager.AccentColor,
                ServiceControllerStatus.StopPending => ThemeManager.WarningColor,
                _ => ThemeManager.TextMutedColor
            };
        }
    }
}