namespace BetterAltTab;

using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Text.Json;
// The class that handles the creation of the application windows
internal class BATApplicationContext : ApplicationContext
{

    private int _formCount;
    private TabSwitcher tabSwitcher;
    private ConfigDataV1? tabSwitcherData;
    private readonly string configPath = Application.UserAppDataPath + "\\GhostTweaks\\BetterAltTab\\config.json";

    internal BATApplicationContext()
    {
        _formCount = 0;

        // Handle the ApplicationExit event to know when the application is exiting.
        Application.ApplicationExit += new EventHandler(this.OnApplicationExit);

        tabSwitcher = new TabSwitcher();
        tabSwitcher.Closed += new EventHandler(OnFormClosed);
        tabSwitcher.Closing += new CancelEventHandler(OnFormClosing);
        _formCount++;

        var configDataFound = ReadFormDataFromFile();
        if (ReadFormDataFromFile())
        {
            // If the data was read from the file, set the form
            // positions manually.
            tabSwitcher.StartPosition = FormStartPosition.Manual;
        }
        else
        {
            ExitThread();
        }

        // Show forms.
        tabSwitcher.Show();
    }

    private static void ShowErrorMessage(string message)
    {
        // Show an error message to the user.
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private void OnApplicationExit(object? sender, EventArgs e)
    {
        // When the application is exiting, write the application data to the
        // user file and close it.
        WriteFormDataToFile();

        try
        {
            // Ignore any errors that might occur while closing the file handle.
            configFHDL.Close();
        }
        catch { }
    }

    private void OnFormClosing(object? sender, CancelEventArgs e)
    {
        // When a form is closing, remember the form position so it
        // can be saved in the user data file.

        // if (sender is TabSwitcher)
        //     tabSwitcherPosition = ((Form)sender).Bounds;
    }

    private void OnFormClosed(object? sender, EventArgs e)
    {
        // When a form is closed, decrement the count of open forms.

        // When the count gets to 0, exit the app by calling
        // ExitThread().
        _formCount--;
        if (_formCount == 0)
        {
            ExitThread();
        }
    }

    private bool WriteFormDataToFile()
    {
        var jsonSerializedData =
        try
        {
            // Set the write position to the start of the file and write
            configFHDL.Flush();

            configFHDL.SetLength(dataToWrite.Length);
            return true;
        }
        catch
        {
            // An error occurred while attempting to write, return false.
            return false;
        }
    }

    private bool ReadFormDataFromFile()
    {

        try
        {
            string jsonSerializedData = File.ReadAllText(configPath);
            ConfigDataBase? basicInfo = JsonSerializer.Deserialize<ConfigDataBase>(jsonSerializedData);
            if (basicInfo != null)
            {
                switch (basicInfo.ConfigVersion)
                {
                    case 1:


                    default:
                        break;
                }
            }
        }
        catch (Exception e)
        {
            ShowErrorMessage("Failed to access Config file. Error: " + e.Message);

        }
        if (configFHDL.Length != 0)
        {
            byte[] dataToRead = new byte[configFHDL.Length];

            try
            {
                // Set the read position to the start of the file and read.
                configFHDL.Seek(0, SeekOrigin.Begin);
                configFHDL.Read(dataToRead, 0, dataToRead.Length);
            }
            catch (IOException e)
            {
                string errorInfo = e.ToString();
                // An error occurred while attempt to read, return false.
                return false;
            }

            // Parse out the data to get the window rectangles
            data = encoding.GetString(dataToRead);

            try
            {
                // Convert the string data to rectangles
                RectangleConverter rectConv = new RectangleConverter();
                string form1pos = data.Substring(1, data.IndexOf("~", 1) - 1);

                tabSwitcherPosition = (Rectangle)rectConv.ConvertFromString(form1pos);

                // string form2pos = data.Substring(data.IndexOf("~", 1) + 1);
                // _form2Position = (Rectangle)rectConv.ConvertFromString(form2pos);

                return true;
            }
            catch
            {
                // Error occurred while attempting to convert the rectangle data.
                // Return false to use default values.
                return false;
            }
        }
        else
        {
            // No data in the file, return false to use default values.
            return false;
        }
    }
}
