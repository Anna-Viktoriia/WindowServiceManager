using System.ComponentModel;
using WindowsServiceManager.Models;
using WindowsServiceManager.UI;

namespace WindowsServiceManager.Controls
{
    public class LogPanelControl : UserControl
    {
        private readonly DataGridView _grid;
        private readonly Button _exportButton;
        private readonly Button _clearButton;
        private readonly Button _openButton;

        public event EventHandler? ExportRequested;
        public event EventHandler? ClearRequested;
        public event EventHandler? OpenRequested;

        public LogPanelControl()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeManager.PanelColor;
            Padding = new Padding(16);
            Margin = new Padding(0, 14, 0, 0);

            Label title = new()
            {
                Text = "Останні події",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = ThemeManager.TextColor,
                AutoSize = true,
                Location = new Point(16, 10)
            };

            _openButton = new Button
            {
                Text = "Відкрити",
                Width = 105,
                Height = 30,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            _exportButton = new Button
            {
                Text = "Експорт",
                Width = 105,
                Height = 30,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            _clearButton = new Button
            {
                Text = "Очистити",
                Width = 105,
                Height = 30,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            ThemeManager.ApplyButtonTheme(_openButton, ThemeManager.AccentColor);
            ThemeManager.ApplyButtonTheme(_exportButton, ThemeManager.SuccessColor);
            ThemeManager.ApplyDangerButtonTheme(_clearButton);

            _openButton.Height = 30;
            _exportButton.Height = 30;
            _clearButton.Height = 30;

            _openButton.Click += (_, _) => OpenRequested?.Invoke(this, EventArgs.Empty);
            _exportButton.Click += (_, _) => ExportRequested?.Invoke(this, EventArgs.Empty);
            _clearButton.Click += (_, _) => ClearRequested?.Invoke(this, EventArgs.Empty);

            _grid = new DataGridView
            {
                Location = new Point(16, 48),
                Width = Width - 32,
                Height = Height - 64,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoGenerateColumns = false
            };

            ConfigureGrid();
            ThemeManager.ApplyGridTheme(_grid);

            Paint += (_, e) =>
            {
                using Pen borderPen = new(ThemeManager.BorderColor);
                e.Graphics.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
            };

            Controls.Add(title);
            Controls.Add(_openButton);
            Controls.Add(_exportButton);
            Controls.Add(_clearButton);
            Controls.Add(_grid);

            Resize += (_, _) => AlignButtons();
            AlignButtons();
        }

        public void SetLogs(BindingList<LogEntry> logs)
        {
            _grid.DataSource = logs;
        }

        private void AlignButtons()
        {
            _clearButton.Location = new Point(Width - _clearButton.Width - 16, 10);
            _exportButton.Location = new Point(_clearButton.Left - _exportButton.Width - 8, 10);
            _openButton.Location = new Point(_exportButton.Left - _openButton.Width - 8, 10);
        }

        private void ConfigureGrid()
        {
            _grid.Columns.Clear();

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Час",
                DataPropertyName = nameof(LogEntry.TimeText),
                FillWeight = 18
            });

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Дія",
                DataPropertyName = nameof(LogEntry.ActionName),
                FillWeight = 20
            });

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Служба",
                DataPropertyName = nameof(LogEntry.ServiceName),
                FillWeight = 18
            });

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Результат",
                DataPropertyName = nameof(LogEntry.Result),
                FillWeight = 14
            });

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Повідомлення",
                DataPropertyName = nameof(LogEntry.Message),
                FillWeight = 40
            });
        }
    }
}
