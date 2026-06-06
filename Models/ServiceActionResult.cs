namespace WindowsServiceManager.Models
{
    public class ServiceActionResult
    {
        public bool IsSuccess { get; set; }

        public string ServiceName { get; set; } = string.Empty;

        public string ActionName { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string ErrorDetails { get; set; } = string.Empty;

        public static ServiceActionResult Success(string serviceName, string actionName, string message)
        {
            return new ServiceActionResult
            {
                IsSuccess = true,
                ServiceName = serviceName,
                ActionName = actionName,
                Message = message
            };
        }

        public static ServiceActionResult Fail(string serviceName, string actionName, string message, string errorDetails = "")
        {
            return new ServiceActionResult
            {
                IsSuccess = false,
                ServiceName = serviceName,
                ActionName = actionName,
                Message = message,
                ErrorDetails = errorDetails
            };
        }
    }
}