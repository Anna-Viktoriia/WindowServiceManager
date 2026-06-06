namespace WindowsServiceManager.Helpers
{
    public static class StartupTypeHelper
    {
        public static string FromRegistryValue(int value)
        {
            return value switch
            {
                0 => "Boot",
                1 => "System",
                2 => "Автоматично",
                3 => "Вручну",
                4 => "Вимкнено",
                _ => "Невідомо"
            };
        }

        public static int ToRegistryValue(string startupTypeText)
        {
            return startupTypeText switch
            {
                "Автоматично" => 2,
                "Вручну" => 3,
                "Вимкнено" => 4,
                _ => 3
            };
        }

        public static bool IsEditableStartupType(string startupTypeText)
        {
            return startupTypeText == "Автоматично"
                   || startupTypeText == "Вручну"
                   || startupTypeText == "Вимкнено";
        }
    }
}