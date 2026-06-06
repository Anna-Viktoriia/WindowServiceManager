using System.ComponentModel;
using WindowsServiceManager.Models;
using WindowsServiceManager.UI;

namespace WindowsServiceManager.Controls
{
    public class LogViewerForm : Form
    {
        private readonly DataGridView _grid;

        public LogViewerForm(BindingList<LogEntry> logs)
        {
            Text = "Журнал дій - ServiceMaster";
            Size = new Size(1000, 600);
            MinimumSize = new Size(850, 480);
            StartPosition = FormStartPosition.CenterParent;

            ThemeManager.ApplyFormTheme(this);

            Label title = new()
            {
                Text = "Повний журнал дій",
                Font = ThemeManager.TitleFont,
                ForeColor = ThemeManager.TextColor,
                AutoSize = true,
                Location = new Point(18, 16)
            };

            Label subtitle = new()
            {
                Text = "У цьому вікні відображаються всі дії, виконані користувачем у програмі.",
                Font = ThemeManager.SmallFont,
                ForeColor = ThemeManager.TextMutedColor,
                AutoSize = true,
                Location = new Point(22, 54)
            };

            _grid = new DataGridView
            {
                Location = new Point(22, 92),
                Width = ClientSize.Width - 44,
                Height = ClientSize.Height - 114,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoGenerateColumns = false
            };

            ConfigureGrid();
            ThemeManager.ApplyGridTheme(_grid);
            _grid.DataSource = logs;

            Controls.Add(title);
            Controls.Add(subtitle);
            Controls.Add(_grid);
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
                FillWeight = 18
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
                FillWeight = 45
            });
        }
    }
}
