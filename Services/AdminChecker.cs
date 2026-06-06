using System.Security.Principal;

namespace WindowsServiceManager.Services
{
    public static class AdminChecker
    {
        public static bool IsRunningAsAdministrator()
        {
            using WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}