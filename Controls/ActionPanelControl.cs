using System.ServiceProcess;
using WindowsServiceManager.Helpers;
using WindowsServiceManager.Models;
using WindowsServiceManager.UI;

namespace WindowsServiceManager.Controls
{
    public class ActionPanelControl : UserControl
    {
        private readonly Button _startButton;
        private readonly Button _stopButton;
        private readonly Button _restartButton;
        private readonly Button _changeStartupButton;
        private readonly Button _openServicesButton;
        private readonly ComboBox _startupTypeComboBox;
        private readonly Label _hintLabel;

        private ServiceInfo? _currentService;

        public event EventHandler? StartRequested;
        public event EventHandler? StopRequested;
        public event EventHandler? RestartRequested;
        public event EventHandler? OpenServicesRequested;
        public event EventHandler<string>? StartupTypeChangeRequested;

        public ActionPanelControl()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeManager.PanelColor;
            Padding = new Padding(16);

            Label title = new()
            {
                Text = "Операції",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = ThemeManager.TextColor,
                AutoSize = true,
                Location = new Point(16, 12)
            };

            _startButton = new Button
            {
                Text = "Запустити",
                Width = 130,
                Height = 36,
                Location = new Point(16, 44)
            };

            _stopButton = new Button
            {
                Text = "Зупинити",
                Width = 130,
                Height = 36,
                Location = new Point(154, 44)
            };

            _restartButton = new Button
            {
                Text = "Перезапустити",
                Width = 270,
                Height = 36,
                Location = new Point(16, 84)
            };

            Label startupLabel = new()
            {
                Text = "Тип запуску",
                Font = ThemeManager.SmallFont,
                ForeColor = ThemeManager.TextMutedColor,
                AutoSize = true,
                Location = new Point(16, 128)
            };

            _startupTypeComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.White,
                ForeColor = ThemeManager.TextColor,
                Font = ThemeManager.SmallFont,
                Width = 130,
                Height = 30,
                Location = new Point(16, 150),
                FlatStyle = FlatStyle.Standard,
                DrawMode = DrawMode.OwnerDrawFixed
            };

            _startupTypeComboBox.Items.Add("Автоматично");
            _startupTypeComboBox.Items.Add("Вручну");
            _startupTypeComboBox.Items.Add("Вимкнено");
            _startupTypeComboBox.SelectedIndex = 1;
            _startupTypeComboBox.DrawItem += StartupTypeComboBox_DrawItem;

            _changeStartupButton = new Button
            {
                Text = "Змінити",
                Width = 130,
                Height = 34,
                Location = new Point(154, 148)
            };

            _openServicesButton = new Button
            {
                Text = "Відкрити Services",
                Width = 270,
                Height = 34,
                Location = new Point(16, 190)
            };

            _hintLabel = new Label
            {
                Text = "Дії залежать від стану служби та прав адміністратора.",
                Font = ThemeManager.SmallFont,
                ForeColor = ThemeManager.TextMutedColor,
                AutoSize = false,
                Width = 300,
                Height = 38,
                Location = new Point(16, 228),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
            };

            ThemeManager.ApplyButtonTheme(_startButton, ThemeManager.SuccessColor);
            ThemeManager.ApplyDangerButtonTheme(_stopButton);
            ThemeManager.ApplyWarningButtonTheme(_restartButton);
            ThemeManager.ApplyButtonTheme(_changeStartupButton, ThemeManager.AccentColor);
            ThemeManager.ApplyButtonTheme(_openServicesButton, Color.FromArgb(226, 232, 238));

            _startButton.Height = 36;
            _stopButton.Height = 36;
            _restartButton.Height = 36;
            _changeStartupButton.Height = 34;
            _openServicesButton.Height = 34;

            _startButton.Click += (_, _) => StartRequested?.Invoke(this, EventArgs.Empty);
            _stopButton.Click += (_, _) => StopRequested?.Invoke(this, EventArgs.Empty);
            _restartButton.Click += (_, _) => RestartRequested?.Invoke(this, EventArgs.Empty);
            _openServicesButton.Click += (_, _) => OpenServicesRequested?.Invoke(this, EventArgs.Empty);

            _changeStartupButton.Click += (_, _) =>
            {
                string selectedStartupType = _startupTypeComboBox.SelectedItem?.ToString() ?? "Вручну";
                StartupTypeChangeRequested?.Invoke(this, selectedStartupType);
            };

            Paint += (_, e) =>
            {
                using Pen borderPen = new(ThemeManager.BorderColor);
                e.Graphics.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
            };

            Controls.Add(title);
            Controls.Add(_startButton);
            Controls.Add(_stopButton);
            Controls.Add(_restartButton);
            Controls.Add(startupLabel);
            Controls.Add(_startupTypeComboBox);
            Controls.Add(_changeStartupButton);
            Controls.Add(_openServicesButton);
            Controls.Add(_hintLabel);

            Resize += (_, _) => ResizeControls();

            SetService(null);
        }

        public void SetService(ServiceInfo? service)
        {
            _currentService = service;

            if (_currentService == null)
            {
                _startButton.Enabled = false;
                _stopButton.Enabled = false;
                _restartButton.Enabled = false;
                _startupTypeComboBox.Enabled = false;
                _changeStartupButton.Enabled = false;
                _openServicesButton.Enabled = true;
                _hintLabel.Text = "Виберіть службу зі списку.";
                _hintLabel.ForeColor = ThemeManager.TextMutedColor;
                return;
            }

            bool isRunning = _currentService.Status == ServiceControllerStatus.Running;
            bool isStopped = _currentService.Status == ServiceControllerStatus.Stopped;
            bool isPaused = _currentService.Status == ServiceControllerStatus.Paused;
            bool isCritical = _currentService.IsCritical;

            _startButton.Enabled = isStopped || isPaused;
            _stopButton.Enabled = isRunning && _currentService.CanStop && !isCritical;
            _restartButton.Enabled = isRunning && _currentService.CanStop && !isCritical;
            _openServicesButton.Enabled = true;

            bool startupTypeCanBeChanged =
                !isCritical && StartupTypeHelper.IsEditableStartupType(_currentService.StartupTypeText);

            _startupTypeComboBox.Enabled = startupTypeCanBeChanged;
            _changeStartupButton.Enabled = startupTypeCanBeChanged;

            int index = _startupTypeComboBox.Items.IndexOf(_currentService.StartupTypeText);
            if (index >= 0)
            {
                _startupTypeComboBox.SelectedIndex = index;
            }

            if (isCritical)
            {
                _hintLabel.Text = "Критична служба: небезпечні дії заблоковані.";
                _hintLabel.ForeColor = ThemeManager.WarningColor;
            }
            else
            {
                _hintLabel.Text = "Дії залежать від стану служби та прав адміністратора.";
                _hintLabel.ForeColor = ThemeManager.TextMutedColor;
            }

            ResizeControls();
        }

        private void ResizeControls()
        {
            int availableWidth = Width - 32;
            int smallButtonWidth = Math.Max(118, (availableWidth - 10) / 2);

            _startButton.Width = smallButtonWidth;
            _stopButton.Width = smallButtonWidth;
            _stopButton.Left = _startButton.Right + 10;

            _restartButton.Width = availableWidth;

            _startupTypeComboBox.Width = smallButtonWidth;
            _changeStartupButton.Width = smallButtonWidth;
            _changeStartupButton.Left = _startupTypeComboBox.Right + 10;

            _openServicesButton.Width = availableWidth;

            _hintLabel.Width = availableWidth;
            _hintLabel.Height = Math.Max(30, Height - _hintLabel.Top - 8);
        }

        private void StartupTypeComboBox_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }

            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            Color backgroundColor = isSelected
                ? ThemeManager.AccentSoftColor
                : Color.White;

            Color textColor = ThemeManager.TextColor;

            using Brush backgroundBrush = new SolidBrush(backgroundColor);
            using Brush textBrush = new SolidBrush(textColor);

            e.Graphics.FillRectangle(backgroundBrush, e.Bounds);

            string itemText = _startupTypeComboBox.Items[e.Index]?.ToString() ?? string.Empty;

            Rectangle textBounds = new(
                e.Bounds.Left + 6,
                e.Bounds.Top + 2,
                e.Bounds.Width - 12,
                e.Bounds.Height - 4);

            e.Graphics.DrawString(
                itemText,
                _startupTypeComboBox.Font,
                textBrush,
                textBounds);

            e.DrawFocusRectangle();
        }
    }
}
