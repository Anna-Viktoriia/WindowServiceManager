namespace WindowsServiceManager.UI
{
    public static class MainLayoutBuilder
    {
        public static TableLayoutPanel CreateRootLayout()
        {
            TableLayoutPanel root = new()
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeManager.BackgroundColor,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, UiConstants.HeaderHeight));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            return root;
        }

        public static TableLayoutPanel CreateContentLayout()
        {
            TableLayoutPanel content = new()
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeManager.BackgroundColor,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(18, 16, 18, 18),
                Margin = new Padding(0)
            };

            content.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, UiConstants.LogPanelHeight));

            return content;
        }

        public static TableLayoutPanel CreateMainWorkAreaLayout()
        {
            TableLayoutPanel workArea = new()
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeManager.BackgroundColor,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            workArea.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 61));
            workArea.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 39));

            return workArea;
        }

        public static TableLayoutPanel CreateRightPanelLayout()
        {
            TableLayoutPanel rightPanel = new()
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeManager.BackgroundColor,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(14, 0, 0, 0),
                Margin = new Padding(0)
            };

            rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 52));
            rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 48));

            return rightPanel;
        }
    }
}
