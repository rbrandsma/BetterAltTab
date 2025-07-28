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
        MainFormData.BackgroundImage = Image.FromFile(imagePath);

        //Setup the initial MainForm
        MainForm.SetupMainForm();

        //Call InterceptKeys to initialize the keyboard hook
        //From here on, the application will listen for Alt+Tab key combinations
        //InterceptKeys will handle redrawing the MainForm and managing the process selection
        InterceptKeys.InitializeKeyboardHook(new Keys[] { Keys.Alt, Keys.Tab });
        //Application.Run(MainForm.mainForm);

        var debugcloser = new Form();
        Application.Run(debugcloser);
        InterceptKeys.ReleaseKeyboardHook();
        MainForm.mainForm.Close();
        MainForm.mainForm.Dispose();
        debugcloser.Close();
        debugcloser.Dispose();
    }
}
