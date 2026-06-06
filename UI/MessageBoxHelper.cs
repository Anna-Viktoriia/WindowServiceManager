namespace WindowsServiceManager.UI
{
    public static class MessageBoxHelper
    {
        public static void ShowInfo(string message)
        {
            MessageBox.Show(
                message,
                UiConstants.AppTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        public static void ShowWarning(string message)
        {
            MessageBox.Show(
                message,
                UiConstants.AppTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        public static void ShowError(string message)
        {
            MessageBox.Show(
                message,
                UiConstants.AppTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        public static bool Confirm(string message)
        {
            DialogResult result = MessageBox.Show(
                message,
                UiConstants.AppTitle,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            return result == DialogResult.Yes;
        }
    }
}