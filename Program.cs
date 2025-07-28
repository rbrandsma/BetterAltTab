namespace BetterAltTab;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        //Setup the user defined values for the different Aspects
        //Load them into the MainFormData class
        var imagePath = "resources\\img\\background.png";
        TabSwitcherData.BackgroundImage = Image.FromFile(imagePath);

        //Setup the initial MainForm
        Application.Run(new TabSwitcher());
        //Call InterceptKeys to initialize the keyboard hook
        //From here on, the application will listen for Alt+Tab key combinations

    }
}
