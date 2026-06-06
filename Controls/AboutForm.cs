using WindowsServiceManager.UI;

namespace WindowsServiceManager.Controls
{
    public class AboutForm : Form
    {
        public AboutForm()
        {
            Text = "Про програму - ServiceMaster";
            Size = new Size(620, 440);
            MinimumSize = new Size(620, 440);
            MaximumSize = new Size(620, 440);
            StartPosition = FormStartPosition.CenterParent;

            ThemeManager.ApplyFormTheme(this);

            Panel panel = new()
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeManager.PanelColor,
                Padding = new Padding(26)
            };

            Label title = new()
            {
                Text = "ServiceMaster",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = ThemeManager.SidebarColor,
                AutoSize = true,
                Location = new Point(24, 22)
            };

            Label subtitle = new()
            {
                Text = "Програмне забезпечення для управління службами Windows",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = ThemeManager.TextColor,
                AutoSize = true,
                Location = new Point(28, 76)
            };

            TextBox infoBox = new()
            {
                Multiline = true,
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = ThemeManager.TextColor,
                Font = ThemeManager.NormalFont,
                Location = new Point(28, 118),
                Size = new Size(540, 210),
                Text =
                    "ServiceMaster — це desktop-застосунок для операційної системи Windows, " +
                    "який дозволяє переглядати, шукати, фільтрувати та адмініструвати служби Windows.\r\n\r\n" +
                    "Основні можливості:\r\n" +
                    "• перегляд списку служб Windows;\r\n" +
                    "• запуск, зупинка та перезапуск служб;\r\n" +
                    "• зміна типу запуску служби;\r\n" +
                    "• пошук і фільтрація служб;\r\n" +
                    "• перегляд детальної інформації про службу;\r\n" +
                    "• журналювання дій користувача;\r\n" +
                    "• захист від випадкової зміни критичних служб.\r\n\r\n" +
                    "Курсовий проєкт з дисципліни «Системне програмне забезпечення»."
            };

            Button closeButton = new()
            {
                Text = "Закрити",
                Width = 120,
                Height = 36,
                Location = new Point(448, 348)
            };

            ThemeManager.ApplyButtonTheme(closeButton);
            closeButton.Click += (_, _) => Close();

            panel.Controls.Add(title);
            panel.Controls.Add(subtitle);
            panel.Controls.Add(infoBox);
            panel.Controls.Add(closeButton);

            Controls.Add(panel);
        }
    }
}
