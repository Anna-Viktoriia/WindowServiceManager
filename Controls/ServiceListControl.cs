using System.ServiceProcess;
using WindowsServiceManager.Helpers;
using WindowsServiceManager.Models;
using WindowsServiceManager.UI;

namespace WindowsServiceManager.Controls
{
    public class ServiceListControl : UserControl
    {
        private readonly TextBox _searchBox;
        private readonly ComboBox _filterComboBox;
        private readonly Label _countLabel;
        private readonly DataGridView _grid;

        private List<ServiceInfo> _allServices = new();

        private string _sortProperty = nameof(ServiceInfo.DisplayName);
        private bool _sortAscending = true;

        public event EventHandler<ServiceInfo?>? SelectedServiceChanged;

        public ServiceInfo? SelectedService
        {
            get
            {
                if (_grid.CurrentRow?.DataBoundItem is ServiceInfo service)
                {
                    return service;
                }

                return null;
            }
        }

        public ServiceListControl()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeManager.PanelColor;
            Padding = new Padding(16);
            Margin = new Padding(0);

            Label title = new()
            {
                Text = "Каталог служб",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = ThemeManager.TextColor,
                AutoSize = true,
                Location = new Point(16, 12)
            };

            _countLabel = new Label
            {
                Text = "0 записів",
                Font = ThemeManager.SmallFont,
                ForeColor = ThemeManager.TextMutedColor,
                AutoSize = true,
                Location = new Point(18, 38)
            };

            _searchBox = new TextBox
            {
                PlaceholderText = "Пошук за назвою служби...",
                BorderStyle = BorderStyle.FixedSingle,
                Font = ThemeManager.NormalFont,
                Location = new Point(16, 66),
                Width = 360,
                Height = 30,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            ThemeManager.ApplyInputTheme(_searchBox);

            _filterComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.White,
                ForeColor = ThemeManager.TextColor,
                Font = ThemeManager.NormalFont,
                Width = 170,
                Height = 30,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            _filterComboBox.Items.Add("Усі служби");
            _filterComboBox.Items.Add("Працюють");
            _filterComboBox.Items.Add("Зупинені");
            _filterComboBox.Items.Add("Призупинені");
            _filterComboBox.SelectedIndex = 0;

            _grid = new DataGridView
            {
                Location = new Point(16, 110),
                Width = Width - 32,
                Height = Height - 126,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoGenerateColumns = false
            };

            ConfigureGrid();
            ThemeManager.ApplyGridTheme(_grid);

            _searchBox.TextChanged += (_, _) => ApplyFilterAndSort();
            _filterComboBox.SelectedIndexChanged += (_, _) => ApplyFilterAndSort();

            _grid.SelectionChanged += (_, _) =>
            {
                SelectedServiceChanged?.Invoke(this, SelectedService);
            };

            _grid.CellFormatting += Grid_CellFormatting;

            _grid.ColumnHeaderMouseClick += (_, e) =>
            {
                DataGridViewColumn column = _grid.Columns[e.ColumnIndex];

                if (string.IsNullOrWhiteSpace(column.DataPropertyName))
                {
                    return;
                }

                string clickedProperty = column.DataPropertyName;

                if (_sortProperty == clickedProperty)
                {
                    _sortAscending = !_sortAscending;
                }
                else
                {
                    _sortProperty = clickedProperty;
                    _sortAscending = true;
                }

                ApplyFilterAndSort();
            };

            Paint += (_, e) =>
            {
                using Pen borderPen = new(ThemeManager.BorderColor);
                e.Graphics.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
            };

            Controls.Add(title);
            Controls.Add(_countLabel);
            Controls.Add(_searchBox);
            Controls.Add(_filterComboBox);
            Controls.Add(_grid);

            Resize += (_, _) =>
            {
                _filterComboBox.Location = new Point(Width - _filterComboBox.Width - 16, 66);
                _searchBox.Width = Math.Max(220, _filterComboBox.Left - 32);
            };

            _filterComboBox.Location = new Point(Width - _filterComboBox.Width - 16, 66);
        }

        public void SetServices(List<ServiceInfo> services)
        {
            _allServices = services;
            ApplyFilterAndSort();
        }

        private void ApplyFilterAndSort()
        {
            string query = _searchBox.Text.Trim();

            IEnumerable<ServiceInfo> filtered = _allServices;

            if (!string.IsNullOrWhiteSpace(query))
            {
                filtered = filtered.Where(service =>
                    service.ServiceName.Contains(query, StringComparison.OrdinalIgnoreCase)
                    || service.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase)
                    || service.Description.Contains(query, StringComparison.OrdinalIgnoreCase));
            }

            filtered = _filterComboBox.SelectedIndex switch
            {
                1 => filtered.Where(service => service.Status == ServiceControllerStatus.Running),
                2 => filtered.Where(service => service.Status == ServiceControllerStatus.Stopped),
                3 => filtered.Where(service => service.Status == ServiceControllerStatus.Paused),
                _ => filtered
            };

            List<ServiceInfo> sorted = SortServices(filtered).ToList();
            _countLabel.Text = $"{sorted.Count} з {_allServices.Count} записів";

            _grid.DataSource = sorted;

            UpdateSortGlyphs();

            if (_grid.Rows.Count > 0)
            {
                _grid.Rows[0].Selected = true;
            }

            SelectedServiceChanged?.Invoke(this, SelectedService);
        }

        private IEnumerable<ServiceInfo> SortServices(IEnumerable<ServiceInfo> services)
        {
            Func<ServiceInfo, object> keySelector = _sortProperty switch
            {
                nameof(ServiceInfo.ServiceName) => service => service.ServiceName,
                nameof(ServiceInfo.DisplayName) => service => service.DisplayName,
                nameof(ServiceInfo.StatusText) => service => service.StatusText,
                nameof(ServiceInfo.StartupTypeText) => service => service.StartupTypeText,
                nameof(ServiceInfo.IsCriticalText) => service => service.IsCriticalText,
                _ => service => service.DisplayName
            };

            return _sortAscending
                ? services.OrderBy(keySelector).ThenBy(service => service.DisplayName)
                : services.OrderByDescending(keySelector).ThenBy(service => service.DisplayName);
        }

        private void ConfigureGrid()
        {
            _grid.Columns.Clear();

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Системна назва",
                DataPropertyName = nameof(ServiceInfo.ServiceName),
                FillWeight = 22,
                SortMode = DataGridViewColumnSortMode.Programmatic
            });

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Назва",
                DataPropertyName = nameof(ServiceInfo.DisplayName),
                FillWeight = 36,
                SortMode = DataGridViewColumnSortMode.Programmatic
            });

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Стан",
                DataPropertyName = nameof(ServiceInfo.StatusText),
                FillWeight = 16,
                SortMode = DataGridViewColumnSortMode.Programmatic
            });

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Запуск",
                DataPropertyName = nameof(ServiceInfo.StartupTypeText),
                FillWeight = 15,
                SortMode = DataGridViewColumnSortMode.Programmatic
            });

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Критична",
                DataPropertyName = nameof(ServiceInfo.IsCriticalText),
                FillWeight = 11,
                SortMode = DataGridViewColumnSortMode.Programmatic
            });
        }

        private void UpdateSortGlyphs()
        {
            foreach (DataGridViewColumn column in _grid.Columns)
            {
                column.HeaderCell.SortGlyphDirection = SortOrder.None;

                if (column.DataPropertyName == _sortProperty)
                {
                    column.HeaderCell.SortGlyphDirection = _sortAscending
                        ? SortOrder.Ascending
                        : SortOrder.Descending;
                }
            }
        }

        private void Grid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            if (_grid.Rows[e.RowIndex].DataBoundItem is not ServiceInfo service)
            {
                return;
            }

            if (_grid.Columns[e.ColumnIndex].DataPropertyName == nameof(ServiceInfo.StatusText))
            {
                e.CellStyle.ForeColor = ServiceStatusHelper.GetStatusColor(service.Status);
                e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }

            if (_grid.Columns[e.ColumnIndex].DataPropertyName == nameof(ServiceInfo.IsCriticalText)
                && service.IsCritical)
            {
                e.CellStyle.ForeColor = ThemeManager.WarningColor;
                e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
        }
    }
}
