namespace BetterAltTab;

internal static class MainFormData
{
    internal static Image BackgroundImage { get; set; } = null!;
}
public static class MainForm
{
    public static Form mainForm = new Form();

    internal static void SetupMainForm()
    {
        mainForm = new Form();
        mainForm.FormBorderStyle = FormBorderStyle.None;
        mainForm.BackgroundImage = MainFormData.BackgroundImage;
        mainForm.StartPosition = FormStartPosition.CenterScreen;
        var rect = Screen.PrimaryScreen?.Bounds;
        if (rect is null)
        {
            rect = new Rectangle(0, 0, 1920, 1080);
        }
        mainForm.SetBounds(0, 0, (rect.Value.Width / 3) * 2, (rect.Value.Height / 3) * 2);
        CreateProcessButtons();
    }
    private static void AddTestButtons()
    {
        Button confirmButton = new Button();
        confirmButton.Text = "Confirm";
        confirmButton.Location = new Point(10, 10);
        confirmButton.Click += (sender, e) =>
        {
            MessageBox.Show("Confirmed!");
        };
        Button cancelButton = new Button();
        cancelButton.Text = "Cancel";
        cancelButton.Location = new Point(confirmButton.Left, confirmButton.Height + confirmButton.Top + 10);
        cancelButton.Click += (sender, e) =>
        {
            mainForm.Close();
        };
        mainForm.Controls.Add(confirmButton);
        mainForm.Controls.Add(cancelButton);
        mainForm.AcceptButton = confirmButton;
        mainForm.CancelButton = cancelButton;
    }


    private static void CreateProcessButtons()
    {
        var pList = ProcessHelper.GetRunningProcesses();
        var buttonList = new List<Button>();
        foreach (var process in pList)
        {
            var button = CreateProcessButton(process.Item1, process.Item2);
            buttonList.Add(button);
        }
        buttonList[0].Location = new Point(10, 10);
        if (mainForm == null)
        {
            throw new InvalidOperationException("Main form is not initialized.");
        }
        mainForm.Controls.Add(buttonList[0]);
        var offset = 10;
        for (int i = 1; i < buttonList.Count; i++)
        {
            var startPoint = buttonList[i - 1].Location.X + buttonList[i - 1].Width + 10;
            if (startPoint + buttonList[i].Width > mainForm.Width)
            {
                startPoint = 10;
                offset += buttonList[i - 1].Height + 10;
            }
            var button = buttonList[i];
            button.Location = new Point(startPoint, offset);
            mainForm.Controls.Add(button);
        }
    }

    private static Button CreateProcessButton(string processName, int hWnd)
    {
        Button button = new Button();
        button.Text = processName;
        button.AutoSize = true;
        button.Click += (sender, e) =>
        {
            try
            {
                ProcessHelper.SetFocusToExternalApp(hWnd);
                mainForm.Close();
                mainForm.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to switch to {processName}: {ex.Message}");
            }
        };
        return button;
    }
}
