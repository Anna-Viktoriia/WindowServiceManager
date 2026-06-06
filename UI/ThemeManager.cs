using System.Drawing;
using System.Windows.Forms;

namespace WindowsServiceManager.UI
{
    public static class ThemeManager
    {
        public static readonly Color BackgroundColor = Color.FromArgb(247, 244, 238);
        public static readonly Color PanelColor = Color.FromArgb(255, 252, 246);
        public static readonly Color PanelLightColor = Color.FromArgb(239, 232, 221);
        public static readonly Color SidebarColor = Color.FromArgb(38, 45, 61);

        public static readonly Color AccentColor = Color.FromArgb(217, 82, 64);
        public static readonly Color AccentSoftColor = Color.FromArgb(255, 232, 225);
        public static readonly Color SuccessColor = Color.FromArgb(40, 125, 89);
        public static readonly Color SuccessSoftColor = Color.FromArgb(224, 242, 232);
        public static readonly Color WarningColor = Color.FromArgb(170, 105, 25);
        public static readonly Color WarningSoftColor = Color.FromArgb(255, 239, 202);
        public static readonly Color DangerColor = Color.FromArgb(181, 48, 61);
        public static readonly Color DangerSoftColor = Color.FromArgb(251, 224, 227);

        public static readonly Color TextColor = Color.FromArgb(36, 39, 47);
        public static readonly Color TextMutedColor = Color.FromArgb(101, 94, 84);
        public static readonly Color BorderColor = Color.FromArgb(220, 211, 198);
        public static readonly Color SidebarMutedColor = Color.FromArgb(198, 203, 214);

        public static Font TitleFont => new("Segoe UI", 20, FontStyle.Bold);
        public static Font SubtitleFont => new("Segoe UI", 10, FontStyle.Regular);
        public static Font NormalFont => new("Segoe UI", 10, FontStyle.Regular);
        public static Font SmallFont => new("Segoe UI", 9, FontStyle.Regular);
        public static Font ButtonFont => new("Segoe UI", 9, FontStyle.Bold);

        public static void ApplyFormTheme(Form form)
        {
            form.BackColor = BackgroundColor;
            form.ForeColor = TextColor;
            form.Font = NormalFont;
            form.StartPosition = FormStartPosition.CenterScreen;
        }

        public static void ApplyPanelTheme(Control control)
        {
            control.BackColor = PanelColor;
            control.ForeColor = TextColor;
            control.Font = NormalFont;
        }

        public static void ApplyButtonTheme(Button button, Color? backColor = null)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.BackColor = backColor ?? AccentColor;
            button.ForeColor = GetReadableButtonText(button.BackColor);
            button.Font = ButtonFont;
            button.Height = 38;
            button.Cursor = Cursors.Hand;
            button.FlatAppearance.BorderColor = ControlPaint.Dark(button.BackColor, 0.05f);
            button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(button.BackColor, 0.12f);
            button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(button.BackColor, 0.08f);
        }

        public static void ApplyDangerButtonTheme(Button button)
        {
            ApplyButtonTheme(button, DangerColor);
        }

        public static void ApplyWarningButtonTheme(Button button)
        {
            ApplyButtonTheme(button, WarningColor);
            button.ForeColor = Color.White;
        }

        public static void ApplyGridTheme(DataGridView grid)
        {
            grid.BackgroundColor = PanelColor;
            grid.BorderStyle = BorderStyle.None;
            grid.EnableHeadersVisualStyles = false;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            grid.ColumnHeadersDefaultCellStyle.BackColor = PanelLightColor;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = TextColor;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = PanelLightColor;
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
            grid.ColumnHeadersHeight = 36;

            grid.DefaultCellStyle.BackColor = PanelColor;
            grid.DefaultCellStyle.ForeColor = TextColor;
            grid.DefaultCellStyle.SelectionBackColor = AccentSoftColor;
            grid.DefaultCellStyle.SelectionForeColor = TextColor;
            grid.DefaultCellStyle.Font = NormalFont;
            grid.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);

            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(251, 247, 239);
            grid.GridColor = BorderColor;
            grid.RowHeadersVisible = false;
            grid.RowTemplate.Height = 34;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.ReadOnly = true;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        public static void ApplyInputTheme(Control control)
        {
            control.BackColor = Color.White;
            control.ForeColor = TextColor;
            control.Font = NormalFont;
        }

        private static Color GetReadableButtonText(Color background)
        {
            int brightness = (background.R * 299 + background.G * 587 + background.B * 114) / 1000;
            return brightness > 176 ? TextColor : Color.White;
        }
    }
}
