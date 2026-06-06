using WindowsServiceManager.UI;

namespace WindowsServiceManager.Controls
{
    public class SidebarPanel : UserControl
    {
        private readonly Button _servicesButton;
        private readonly Button _logButton;
        private readonly Button _aboutButton;
        private readonly Button _refreshButton;
        private readonly Label _adminStatusLabel;
        private readonly Label _statisticsLabel;

        public event EventHandler? ServicesClicked;
        public event EventHandler? LogClicked;
        public event EventHandler? AboutClicked;
        public event EventHandler? RefreshRequested;

        public SidebarPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeManager.SidebarColor;
            Padding = new Padding(18, 12, 18, 12);

            _servicesButton = CreateNavButton("Служби", true);
            _logButton = CreateNavButton("Журнал дій", false);
            _aboutButton = CreateNavButton("Про програму", false);

            _refreshButton = new Button
            {
                Text = "Оновити",
                Width = 116,
                Height = 38,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            ThemeManager.ApplyButtonTheme(_refreshButton, ThemeManager.AccentColor);

            _adminStatusLabel = new Label
            {
                Text = "Admin Mode: unknown",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ThemeManager.WarningColor,
                BackColor = ThemeManager.WarningSoftColor,
                Width = 176,
                Height = 30,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            _statisticsLabel = new Label
            {
                Text = "Служб: 0",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(55, 64, 83),
                Width = 330,
                Height = 30,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            _servicesButton.Click += (_, _) =>
            {
                SelectButton(_servicesButton);
                ServicesClicked?.Invoke(this, EventArgs.Empty);
            };

            _logButton.Click += (_, _) =>
            {
                SelectButton(_logButton);
                LogClicked?.Invoke(this, EventArgs.Empty);
            };

            _aboutButton.Click += (_, _) =>
            {
                SelectButton(_aboutButton);
                AboutClicked?.Invoke(this, EventArgs.Empty);
            };

            _refreshButton.Click += (_, _) => RefreshRequested?.Invoke(this, EventArgs.Empty);

            Controls.Add(_servicesButton);
            Controls.Add(_logButton);
            Controls.Add(_aboutButton);
            Controls.Add(_statisticsLabel);
            Controls.Add(_adminStatusLabel);
            Controls.Add(_refreshButton);

            Resize += (_, _) => LayoutControls();
            LayoutControls();
        }

        public void SelectServices()
        {
            SelectButton(_servicesButton);
        }

        public void SetAdminState(bool isAdmin)
        {
            _adminStatusLabel.Text = isAdmin ? "Admin Mode: Active" : "Admin Mode: Not active";
            _adminStatusLabel.ForeColor = isAdmin ? ThemeManager.SuccessColor : ThemeManager.WarningColor;
            _adminStatusLabel.BackColor = isAdmin ? ThemeManager.SuccessSoftColor : ThemeManager.WarningSoftColor;
        }

        public void SetStatistics(int total, int running, int stopped)
        {
            _statisticsLabel.Text = $"Усього: {total}   Працюють: {running}   Зупинені: {stopped}";
        }

        private void LayoutControls()
        {
            int left = 18;
            int top = 16;

            _servicesButton.Location = new Point(left, top);
            _logButton.Location = new Point(_servicesButton.Right + 10, top);
            _aboutButton.Location = new Point(_logButton.Right + 10, top);

            _refreshButton.Location = new Point(Width - _refreshButton.Width - 18, 16);
            _adminStatusLabel.Location = new Point(_refreshButton.Left - _adminStatusLabel.Width - 12, 20);
            _statisticsLabel.Location = new Point(_adminStatusLabel.Left - _statisticsLabel.Width - 12, 20);
        }

        private void SelectButton(Button selectedButton)
        {
            ApplyButtonState(_servicesButton, selectedButton == _servicesButton);
            ApplyButtonState(_logButton, selectedButton == _logButton);
            ApplyButtonState(_aboutButton, selectedButton == _aboutButton);
        }

        private static void ApplyButtonState(Button button, bool selected)
        {
            button.BackColor = selected ? Color.FromArgb(255, 252, 246) : Color.FromArgb(55, 64, 83);
            button.ForeColor = selected ? ThemeManager.SidebarColor : Color.White;
            button.FlatAppearance.BorderColor = selected ? ThemeManager.PanelColor : Color.FromArgb(80, 91, 115);
            button.FlatAppearance.MouseOverBackColor = selected ? ThemeManager.PanelColor : Color.FromArgb(67, 78, 101);
        }

        private static Button CreateNavButton(string text, bool selected)
        {
            Button button = new()
            {
                Text = text,
                Width = 132,
                Height = 38,
                TextAlign = ContentAlignment.MiddleCenter
            };

            ThemeManager.ApplyButtonTheme(button, selected ? ThemeManager.PanelColor : Color.FromArgb(55, 64, 83));
            ApplyButtonState(button, selected);

            return button;
        }
    }
}
