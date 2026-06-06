using Microsoft.Win32;
using System.ComponentModel;
using System.ServiceProcess;
using WindowsServiceManager.Helpers;
using WindowsServiceManager.Models;

namespace WindowsServiceManager.Services
{
    public class WindowsServiceManager
    {
        private const int OperationTimeoutSeconds = 20;

        public List<ServiceInfo> GetServices()
        {
            List<ServiceInfo> services = new();

            foreach (ServiceController controller in ServiceController.GetServices())
            {
                try
                {
                    int startupValue = GetStartupRegistryValue(controller.ServiceName);
                    string description = GetDescriptionFromRegistry(controller.ServiceName);
                    bool isCritical = CriticalServiceProtector.IsCritical(controller.ServiceName);

                    services.Add(new ServiceInfo
                    {
                        ServiceName = controller.ServiceName,
                        DisplayName = controller.DisplayName,
                        Description = description,
                        Status = controller.Status,
                        StatusText = ServiceStatusHelper.ToUkrainianText(controller.Status),
                        StartupTypeText = StartupTypeHelper.FromRegistryValue(startupValue),
                        CanStop = controller.CanStop,
                        IsCritical = isCritical
                    });
                }
                catch
                {
                    services.Add(new ServiceInfo
                    {
                        ServiceName = controller.ServiceName,
                        DisplayName = controller.DisplayName,
                        Description = "Не вдалося отримати повний опис служби.",
                        Status = controller.Status,
                        StatusText = ServiceStatusHelper.ToUkrainianText(controller.Status),
                        StartupTypeText = "Невідомо",
                        CanStop = controller.CanStop,
                        IsCritical = CriticalServiceProtector.IsCritical(controller.ServiceName)
                    });
                }
                finally
                {
                    controller.Dispose();
                }
            }

            return services
                .OrderBy(service => service.DisplayName)
                .ToList();
        }

        public ServiceActionResult StartService(string serviceName)
        {
            const string action = "Запуск служби";

            try
            {
                using ServiceController controller = new(serviceName);
                controller.Refresh();

                if (controller.Status == ServiceControllerStatus.Running)
                {
                    return ServiceActionResult.Success(serviceName, action, "Служба вже запущена.");
                }

                if (controller.Status == ServiceControllerStatus.StartPending)
                {
                    return ServiceActionResult.Fail(serviceName, action, "Служба вже перебуває у процесі запуску.");
                }

                controller.Start();
                controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(OperationTimeoutSeconds));

                return ServiceActionResult.Success(serviceName, action, "Службу успішно запущено.");
            }
            catch (InvalidOperationException ex)
            {
                return ServiceActionResult.Fail(
                    serviceName,
                    action,
                    "Не вдалося запустити службу. Можливо, бракує прав адміністратора або служба вимкнена.",
                    ex.Message);
            }
            catch (Win32Exception ex)
            {
                return ServiceActionResult.Fail(serviceName, action, "Windows відхилила запуск служби.", ex.Message);
            }
            catch (Exception ex)
            {
                return ServiceActionResult.Fail(serviceName, action, "Виникла непередбачена помилка під час запуску служби.", ex.Message);
            }
        }

        public ServiceActionResult StopService(string serviceName)
        {
            const string action = "Зупинка служби";

            try
            {
                if (CriticalServiceProtector.IsCritical(serviceName))
                {
                    return ServiceActionResult.Fail(serviceName, action, CriticalServiceProtector.GetWarningText(serviceName));
                }

                using ServiceController controller = new(serviceName);
                controller.Refresh();

                if (controller.Status == ServiceControllerStatus.Stopped)
                {
                    return ServiceActionResult.Success(serviceName, action, "Служба вже зупинена.");
                }

                if (!controller.CanStop)
                {
                    return ServiceActionResult.Fail(serviceName, action, "Цю службу неможливо зупинити через системні обмеження.");
                }

                controller.Stop();
                controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(OperationTimeoutSeconds));

                return ServiceActionResult.Success(serviceName, action, "Службу успішно зупинено.");
            }
            catch (InvalidOperationException ex)
            {
                return ServiceActionResult.Fail(serviceName, action, "Не вдалося зупинити службу. Можливо, бракує прав адміністратора.", ex.Message);
            }
            catch (Win32Exception ex)
            {
                return ServiceActionResult.Fail(serviceName, action, "Windows відхилила зупинку служби.", ex.Message);
            }
            catch (Exception ex)
            {
                return ServiceActionResult.Fail(serviceName, action, "Виникла непередбачена помилка під час зупинки служби.", ex.Message);
            }
        }

        public ServiceActionResult RestartService(string serviceName)
        {
            const string action = "Перезапуск служби";

            try
            {
                if (CriticalServiceProtector.IsCritical(serviceName))
                {
                    return ServiceActionResult.Fail(
                        serviceName,
                        action,
                        CriticalServiceProtector.GetWarningText(serviceName));
                }

                using ServiceController controller = new(serviceName);
                controller.Refresh();

                if (controller.Status == ServiceControllerStatus.Stopped)
                {
                    controller.Start();
                    controller.WaitForStatus(
                        ServiceControllerStatus.Running,
                        TimeSpan.FromSeconds(OperationTimeoutSeconds));

                    return ServiceActionResult.Success(
                        serviceName,
                        action,
                        "Служба була зупинена, тому її було запущено.");
                }

                if (controller.Status != ServiceControllerStatus.Running)
                {
                    return ServiceActionResult.Fail(
                        serviceName,
                        action,
                        $"Служба перебуває у стані \"{controller.Status}\". Перезапуск доступний тільки для працюючої або зупиненої служби.");
                }

                if (!controller.CanStop)
                {
                    return ServiceActionResult.Fail(
                        serviceName,
                        action,
                        "Служба працює, але її неможливо зупинити через системні обмеження.");
                }

                controller.Stop();
                controller.WaitForStatus(
                    ServiceControllerStatus.Stopped,
                    TimeSpan.FromSeconds(OperationTimeoutSeconds));

                controller.Refresh();

                controller.Start();
                controller.WaitForStatus(
                    ServiceControllerStatus.Running,
                    TimeSpan.FromSeconds(OperationTimeoutSeconds));

                controller.Refresh();

                return ServiceActionResult.Success(
                    serviceName,
                    action,
                    "Службу успішно перезапущено: виконано зупинку та повторний запуск.");
            }
            catch (InvalidOperationException ex)
            {
                return ServiceActionResult.Fail(
                    serviceName,
                    action,
                    "Не вдалося перезапустити службу. Можливо, бракує прав адміністратора або служба не підтримує цю операцію.",
                    ex.Message);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                return ServiceActionResult.Fail(
                    serviceName,
                    action,
                    "Windows відхилила перезапуск служби.",
                    ex.Message);
            }
            catch (System.ServiceProcess.TimeoutException ex)
            {
                return ServiceActionResult.Fail(
                    serviceName,
                    action,
                    "Служба не встигла змінити стан за відведений час.",
                    ex.Message);
            }
            catch (Exception ex)
            {
                return ServiceActionResult.Fail(
                    serviceName,
                    action,
                    "Виникла непередбачена помилка під час перезапуску служби.",
                    ex.Message);
            }
        }

        public ServiceActionResult ChangeStartupType(string serviceName, string startupTypeText)
        {
            const string action = "Зміна типу запуску";

            try
            {
                if (CriticalServiceProtector.IsCritical(serviceName))
                {
                    return ServiceActionResult.Fail(
                        serviceName,
                        action,
                        "Зміну типу запуску для критичних служб заблоковано для безпеки.");
                }

                if (!StartupTypeHelper.IsEditableStartupType(startupTypeText))
                {
                    return ServiceActionResult.Fail(
                        serviceName,
                        action,
                        "Обраний тип запуску не підтримується для зміни.");
                }

                int registryValue = StartupTypeHelper.ToRegistryValue(startupTypeText);

                using RegistryKey? key = Registry.LocalMachine.OpenSubKey(
                    $@"SYSTEM\CurrentControlSet\Services\{serviceName}",
                    writable: true);

                if (key == null)
                {
                    return ServiceActionResult.Fail(
                        serviceName,
                        action,
                        "Не вдалося відкрити запис служби у реєстрі Windows.");
                }

                key.SetValue("Start", registryValue, RegistryValueKind.DWord);

                return ServiceActionResult.Success(
                    serviceName,
                    action,
                    $"Тип запуску служби змінено на: {startupTypeText}.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return ServiceActionResult.Fail(
                    serviceName,
                    action,
                    "Недостатньо прав для зміни типу запуску служби. Запустіть програму від імені адміністратора.",
                    ex.Message);
            }
            catch (Exception ex)
            {
                return ServiceActionResult.Fail(
                    serviceName,
                    action,
                    "Не вдалося змінити тип запуску служби.",
                    ex.Message);
            }
        }

        public void OpenServicesConsole()
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "services.msc",
                UseShellExecute = true
            });
        }

        private static int GetStartupRegistryValue(string serviceName)
        {
            using RegistryKey? key = Registry.LocalMachine.OpenSubKey(
                $@"SYSTEM\CurrentControlSet\Services\{serviceName}");

            object? value = key?.GetValue("Start");

            if (value is int intValue)
            {
                return intValue;
            }

            return -1;
        }

        private static string GetDescriptionFromRegistry(string serviceName)
        {
            using RegistryKey? key = Registry.LocalMachine.OpenSubKey(
                $@"SYSTEM\CurrentControlSet\Services\{serviceName}");

            object? value = key?.GetValue("Description");

            return value?.ToString() ?? "Опис відсутній.";
        }
    }
}