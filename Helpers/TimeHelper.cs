namespace WindowsServiceManager.Helpers
{
    public static class TimeHelper
    {
        public static string NowText()
        {
            return DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }
    }
}