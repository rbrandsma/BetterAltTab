namespace BetterAltTab;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        var imagePath = "resources\\img\\background.png";

        var mainForm = new MainForm(imagePath);
        Application.Run(mainForm.mainForm);
        //handle low level hook here
    }
}
