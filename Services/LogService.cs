using System.ComponentModel;
using System.Text;
using WindowsServiceManager.Models;

namespace WindowsServiceManager.Services
{
    public class LogService
    {
        public BindingList<LogEntry> Entries { get; } = new();

        public void AddInfo(string actionName, string serviceName, string message)
        {
            Entries.Insert(0, new LogEntry
            {
                Time = DateTime.Now,
                ActionName = actionName,
                ServiceName = serviceName,
                Result = "Інформація",
                Message = message
            });
        }

        public void AddFromResult(ServiceActionResult result)
        {
            Entries.Insert(0, new LogEntry
            {
                Time = DateTime.Now,
                ActionName = result.ActionName,
                ServiceName = result.ServiceName,
                Result = result.IsSuccess ? "Успішно" : "Помилка",
                Message = string.IsNullOrWhiteSpace(result.ErrorDetails)
                    ? result.Message
                    : $"{result.Message} Деталі: {result.ErrorDetails}"
            });
        }

        public void Clear()
        {
            Entries.Clear();
        }

        public void ExportToTextFile(string filePath)
        {
            StringBuilder builder = new();

            builder.AppendLine("ЖУРНАЛ ДІЙ ПРОГРАМИ SERVICEMASTER");
            builder.AppendLine("Програмне забезпечення для управління службами Windows");
            builder.AppendLine(new string('-', 90));
            builder.AppendLine();

            foreach (LogEntry entry in Entries.OrderBy(item => item.Time))
            {
                builder.AppendLine($"Час: {entry.TimeText}");
                builder.AppendLine($"Дія: {entry.ActionName}");
                builder.AppendLine($"Служба: {entry.ServiceName}");
                builder.AppendLine($"Результат: {entry.Result}");
                builder.AppendLine($"Повідомлення: {entry.Message}");
                builder.AppendLine(new string('-', 90));
            }

            File.WriteAllText(filePath, builder.ToString(), Encoding.UTF8);
        }
    }
}