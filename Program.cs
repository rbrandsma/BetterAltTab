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

        //Setup the initial MainForm

        BATApplicationContext context = new BATApplicationContext();
        Application.Run(context);
    }
}
