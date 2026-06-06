using System.ServiceProcess;
using WindowsServiceManager.Controls;
using WindowsServiceManager.Models;
using WindowsServiceManager.Services;
using WindowsServiceManager.UI;

namespace WindowsServiceManager
{
    public partial class Form1 : Form
    {
        private readonly Services.WindowsServiceManager _serviceManager;
        private readonly LogService _logService;

        private SidebarPanel _sidebarPanel = null!;
        private ServiceListControl _serviceListControl = null!;
        private ServiceDetailsControl _serviceDetailsControl = null!;
        private ActionPanelControl _actionPanelControl = null!;
        private LogPanelControl _logPanelControl = null!;

        private ServiceInfo? _selectedService;
        private List<ServiceInfo> _loadedServices = new();

        public Form1()
        {
            InitializeComponent();

            _serviceManager = new Services.WindowsServiceManager();
            _logService = new LogService();

            ConfigureForm();
            BuildInterface();
            BindEvents();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            bool isAdmin = AdminChecker.IsRunningAsAdministrator();
            _sidebarPanel.SetAdminState(isAdmin);

            if (!isAdmin)
            {
                _logService.AddInfo(
                    "Перевірка прав",
                    "-",
                    "Програма запущена без прав адміністратора. Перегляд служб доступний, але запуск/зупинка деяких служб може бути заблокована Windows.");

                MessageBoxHelper.ShowWarning(
                    "Програма запущена без прав адміністратора.\n\n" +
                    "Список служб буде доступний, але запуск, зупинка або перезапуск багатьох служб можуть не працювати.\n\n" +
                    "Для повного функціоналу перезапустіть програму від імені адміністратора.");
            }

            LoadServices();
        }

        private void ConfigureForm()
        {
            Text = "ServiceMaster - Windows Service Manager";
            MinimumSize = new Size(1220, 720);
            Size = new Size(1380, 820);

            ThemeManager.ApplyFormTheme(this);
        }

        private void BuildInterface()
        {
            Controls.Clear();

            TableLayoutPanel root = MainLayoutBuilder.CreateRootLayout();
            TableLayoutPanel content = MainLayoutBuilder.CreateContentLayout();
            TableLayoutPanel workArea = MainLayoutBuilder.CreateMainWorkAreaLayout();
            TableLayoutPanel rightPanel = MainLayoutBuilder.CreateRightPanelLayout();

            _sidebarPanel = new SidebarPanel();
            _serviceListControl = new ServiceListControl();
            _serviceDetailsControl = new ServiceDetailsControl();
            _actionPanelControl = new ActionPanelControl();
            _logPanelControl = new LogPanelControl();

            _logPanelControl.SetLogs(_logService.Entries);

            rightPanel.Controls.Add(_serviceDetailsControl, 0, 0);
            rightPanel.Controls.Add(_actionPanelControl, 0, 1);

            workArea.Controls.Add(_serviceListControl, 0, 0);
            workArea.Controls.Add(rightPanel, 1, 0);

            content.Controls.Add(workArea, 0, 0);
            content.Controls.Add(_logPanelControl, 0, 1);

            root.Controls.Add(_sidebarPanel, 0, 0);
            root.Controls.Add(content, 0, 1);

            Controls.Add(root);
        }

        private void BindEvents()
        {
            _sidebarPanel.RefreshRequested += (_, _) => LoadServices();

            _sidebarPanel.ServicesClicked += (_, _) =>
            {
                _sidebarPanel.SelectServices();
                MessageBoxHelper.ShowInfo("Основний екран зі списком служб уже відкритий.");
            };

            _sidebarPanel.LogClicked += (_, _) =>
            {
                ShowFullLogWindow();
            };

            _sidebarPanel.AboutClicked += (_, _) =>
            {
                ShowAboutWindow();
            };

            _serviceListControl.SelectedServiceChanged += (_, service) =>
            {
                _selectedService = service;
                _serviceDetailsControl.SetService(service);
                _actionPanelControl.SetService(service);
            };

            _actionPanelControl.StartRequested += async (_, _) =>
            {
                await ExecuteServiceActionAsync(
                    "запустити",
                    serviceName => _serviceManager.StartService(serviceName));
            };

            _actionPanelControl.StopRequested += async (_, _) =>
            {
                await ExecuteServiceActionAsync(
                    "зупинити",
                    serviceName => _serviceManager.StopService(serviceName));
            };

            _actionPanelControl.RestartRequested += async (_, _) =>
            {
                await ExecuteServiceActionAsync(
                    "перезапустити",
                    serviceName => _serviceManager.RestartService(serviceName));
            };

            _actionPanelControl.OpenServicesRequested += (_, _) =>
            {
                try
                {
                    _serviceManager.OpenServicesConsole();

                    _logService.AddInfo(
                        "Відкриття services.msc",
                        "-",
                        "Відкрито стандартну консоль керування службами Windows.");
                }
                catch (Exception ex)
                {
                    MessageBoxHelper.ShowError(
                        "Не вдалося відкрити стандартну консоль служб Windows.\n\n" +
                        ex.Message);

                    _logService.AddInfo(
                        "Відкриття services.msc",
                        "-",
                        $"Помилка відкриття services.msc: {ex.Message}");
                }
            };

            _actionPanelControl.StartupTypeChangeRequested += async (_, startupTypeText) =>
            {
                await ChangeStartupTypeAsync(startupTypeText);
            };

            _logPanelControl.OpenRequested += (_, _) =>
            {
                ShowFullLogWindow();
            };

            _logPanelControl.ExportRequested += (_, _) =>
            {
                ExportLog();
            };

            _logPanelControl.ClearRequested += (_, _) =>
            {
                ClearLog();
            };
        }

        private void LoadServices()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                _loadedServices = _serviceManager.GetServices();
                _serviceListControl.SetServices(_loadedServices);

                int total = _loadedServices.Count;
                int running = _loadedServices.Count(service => service.Status == ServiceControllerStatus.Running);
                int stopped = _loadedServices.Count(service => service.Status == ServiceControllerStatus.Stopped);

                _sidebarPanel.SetStatistics(total, running, stopped);

                _logService.AddInfo(
                    "Оновлення списку",
                    "-",
                    $"Завантажено служб: {total}.");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError(
                    "Не вдалося завантажити список служб Windows.\n\n" +
                    ex.Message);

                _logService.AddInfo(
                    "Оновлення списку",
                    "-",
                    $"Помилка завантаження служб: {ex.Message}");
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async Task ExecuteServiceActionAsync(
            string actionText,
            Func<string, ServiceActionResult> action)
        {
            if (_selectedService == null)
            {
                MessageBoxHelper.ShowWarning("Спочатку виберіть службу зі списку.");
                return;
            }

            string serviceName = _selectedService.ServiceName;

            bool confirmed = MessageBoxHelper.Confirm(
                $"Ви дійсно хочете {actionText} службу \"{_selectedService.DisplayName}\"?\n\n" +
                $"Системна назва: {serviceName}");

            if (!confirmed)
            {
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                Enabled = false;

                ServiceActionResult result = await Task.Run(() => action(serviceName));

                _logService.AddFromResult(result);

                if (result.IsSuccess)
                {
                    MessageBoxHelper.ShowInfo(result.Message);
                }
                else
                {
                    MessageBoxHelper.ShowWarning(result.Message);
                }

                LoadServices();
                RestoreSelectedService(serviceName);
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError(
                    "Виникла помилка під час виконання дії.\n\n" +
                    ex.Message);

                _logService.AddInfo(
                    "Виконання дії",
                    serviceName,
                    $"Помилка: {ex.Message}");
            }
            finally
            {
                Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private async Task ChangeStartupTypeAsync(string startupTypeText)
        {
            if (_selectedService == null)
            {
                MessageBoxHelper.ShowWarning("Спочатку виберіть службу зі списку.");
                return;
            }

            string serviceName = _selectedService.ServiceName;

            bool confirmed = MessageBoxHelper.Confirm(
                $"Ви дійсно хочете змінити тип запуску служби \"{_selectedService.DisplayName}\"?\n\n" +
                $"Системна назва: {serviceName}\n" +
                $"Новий тип запуску: {startupTypeText}");

            if (!confirmed)
            {
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                Enabled = false;

                ServiceActionResult result = await Task.Run(() =>
                    _serviceManager.ChangeStartupType(serviceName, startupTypeText));

                _logService.AddFromResult(result);

                if (result.IsSuccess)
                {
                    MessageBoxHelper.ShowInfo(result.Message);
                }
                else
                {
                    MessageBoxHelper.ShowWarning(result.Message);
                }

                LoadServices();
                RestoreSelectedService(serviceName);
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError(
                    "Виникла помилка під час зміни типу запуску служби.\n\n" +
                    ex.Message);

                _logService.AddInfo(
                    "Зміна типу запуску",
                    serviceName,
                    $"Помилка: {ex.Message}");
            }
            finally
            {
                Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void RestoreSelectedService(string serviceName)
        {
            ServiceInfo? service = _loadedServices
                .FirstOrDefault(item => item.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));

            _selectedService = service;
            _serviceDetailsControl.SetService(service);
            _actionPanelControl.SetService(service);
        }

        private void ShowFullLogWindow()
        {
            using LogViewerForm form = new(_logService.Entries);
            form.ShowDialog(this);
            _sidebarPanel.SelectServices();
        }

        private void ShowAboutWindow()
        {
            using AboutForm form = new();
            form.ShowDialog(this);
            _sidebarPanel.SelectServices();
        }

        private void ExportLog()
        {
            if (_logService.Entries.Count == 0)
            {
                MessageBoxHelper.ShowWarning("Журнал дій порожній. Експортувати нічого.");
                return;
            }

            using SaveFileDialog dialog = new()
            {
                Title = "Зберегти журнал дій",
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                FileName = $"service_log_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                _logService.ExportToTextFile(dialog.FileName);

                MessageBoxHelper.ShowInfo("Журнал дій успішно експортовано.");

                _logService.AddInfo(
                    "Експорт журналу",
                    "-",
                    $"Журнал збережено у файл: {dialog.FileName}");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError(
                    "Не вдалося експортувати журнал дій.\n\n" +
                    ex.Message);
            }
        }

        private void ClearLog()
        {
            bool confirmed = MessageBoxHelper.Confirm(
                "Ви дійсно хочете очистити журнал дій?");

            if (!confirmed)
            {
                return;
            }

            _logService.Clear();

            _logService.AddInfo(
                "Очищення журналу",
                "-",
                "Журнал дій очищено користувачем.");
        }
    }
}
