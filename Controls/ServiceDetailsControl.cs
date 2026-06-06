using WindowsServiceManager.Models;
using WindowsServiceManager.UI;

namespace WindowsServiceManager.Controls
{
    public class ServiceDetailsControl : UserControl
    {
        private readonly Label _serviceNameValue;
        private readonly Label _displayNameValue;
        private readonly Label _statusValue;
        private readonly Label _startupTypeValue;
        private readonly Label _canStopValue;
        private readonly Label _criticalValue;
        private readonly TextBox _descriptionBox;

        public ServiceDetailsControl()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeManager.PanelColor;
            Padding = new Padding(16);
            Margin = new Padding(0, 0, 0, 12);

            Label title = new()
            {
                Text = "Паспорт служби",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = ThemeManager.TextColor,
                AutoSize = true,
                Location = new Point(16, 12)
            };

            _serviceNameValue = CreateValueLabel(46);
            _displayNameValue = CreateValueLabel(82);
            _statusValue = CreateValueLabel(118);
            _startupTypeValue = CreateValueLabel(154);
            _canStopValue = CreateValueLabel(190);
            _criticalValue = CreateValueLabel(226);

            Controls.Add(title);

            AddField("Назва служби", _serviceNameValue, 46);
            AddField("Відображувана назва", _displayNameValue, 82);
            AddField("Стан", _statusValue, 118);
            AddField("Тип запуску", _startupTypeValue, 154);
            AddField("Можна зупинити", _canStopValue, 190);
            AddField("Критична", _criticalValue, 226);

            Label descriptionLabel = new()
            {
                Text = "Опис",
                Font = ThemeManager.SmallFont,
                ForeColor = ThemeManager.TextMutedColor,
                AutoSize = true,
                Location = new Point(16, 264)
            };

            _descriptionBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = ThemeManager.TextColor,
                Font = ThemeManager.SmallFont,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(16, 286),
                Width = Width - 32,
                Height = 80,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
            };

            Paint += (_, e) =>
            {
                using Pen borderPen = new(ThemeManager.BorderColor);
                e.Graphics.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
            };

            Controls.Add(descriptionLabel);
            Controls.Add(_descriptionBox);

            Resize += (_, _) =>
            {
                int valueWidth = Math.Max(160, Width - 182);
                _serviceNameValue.Width = valueWidth;
                _displayNameValue.Width = valueWidth;
                _statusValue.Width = valueWidth;
                _startupTypeValue.Width = valueWidth;
                _canStopValue.Width = valueWidth;
                _criticalValue.Width = valueWidth;
                _descriptionBox.Width = Width - 32;
                _descriptionBox.Height = Math.Max(55, Height - _descriptionBox.Top - 16);
            };

            Clear();
        }

        public void SetService(ServiceInfo? service)
        {
            if (service == null)
            {
                Clear();
                return;
            }

            _serviceNameValue.Text = service.ServiceName;
            _displayNameValue.Text = service.DisplayName;
            _statusValue.Text = service.StatusText;
            _startupTypeValue.Text = service.StartupTypeText;
            _canStopValue.Text = service.CanStopText;
            _criticalValue.Text = service.IsCriticalText;
            _descriptionBox.Text = service.Description;

            _criticalValue.ForeColor = service.IsCritical
                ? ThemeManager.WarningColor
                : ThemeManager.SuccessColor;
        }

        private void Clear()
        {
            _serviceNameValue.Text = "Не вибрано";
            _displayNameValue.Text = "-";
            _statusValue.Text = "-";
            _startupTypeValue.Text = "-";
            _canStopValue.Text = "-";
            _criticalValue.Text = "-";
            _descriptionBox.Text = "Виберіть службу зі списку, щоб переглянути детальну інформацію.";
        }

        private void AddField(string labelText, Label valueLabel, int top)
        {
            Label label = new()
            {
                Text = labelText,
                Font = ThemeManager.SmallFont,
                ForeColor = ThemeManager.TextMutedColor,
                AutoSize = false,
                Location = new Point(16, top),
                Width = 138,
                Height = 22
            };

            Controls.Add(label);
            Controls.Add(valueLabel);
        }

        private static Label CreateValueLabel(int top)
        {
            return new Label
            {
                Text = "-",
                Font = ThemeManager.NormalFont,
                ForeColor = ThemeManager.TextColor,
                AutoEllipsis = true,
                Location = new Point(156, top),
                Width = 260,
                Height = 22,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
        }
    }
}
