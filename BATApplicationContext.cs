namespace BetterAltTab;

using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Configuration.Internal;
using System.Data;

// The class that handles the creation of the application windows


internal class BATApplicationContext : ApplicationContext
{

    private int _formCount;
    private TabSwitcher tabSwitcher;
    private ConfigDataV1? configData;
    private readonly string configPath = Application.UserAppDataPath + "\\GhostTweaks\\BetterAltTab\\config.json";

    internal BATApplicationContext()
    {
        _formCount = 0;

        // Handle the ApplicationExit event to know when the application is exiting.
        Application.ApplicationExit += new EventHandler(this.OnApplicationExit);

        var configDataLoaded = ReadFormDataFromFile();

        if (!configDataLoaded || configData == null)
        {
            ExitThread();
            throw new DataException("Failed to Load Config");
        }

        tabSwitcher = new TabSwitcher(configData.TabSwitcherData);
        tabSwitcher.Closed += new EventHandler(OnFormClosed);
        tabSwitcher.Closing += new CancelEventHandler(OnFormClosing);
        _formCount++;

        // Show forms.
        tabSwitcher.Show();
    }

/// <summary>
/// Shows a confirmation prompt to the user with the specified message and 3 options.
/// </summary>
/// <param name="message">The Message to show the user</param>
/// <returns>1 for yes, 0 for no, and -1 for cancel</returns>
    private int ShowConfirmationMessage(string message)
    {
        var msg = MessageBox.Show(message, "Confirm?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        return msg switch
        {
            DialogResult.Yes => 1,
            DialogResult.No => 0,
            _ => -1,
        };

    }

    private static void ShowErrorMessage(string message)
    {
        // Show an error message to the user.
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private static void ShowWarningMessage(string message)
    {
        MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private static void ShowInformationMessage(string message)
    {
        MessageBox.Show(message, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void OnApplicationExit(object? sender, EventArgs e)
    {
        // When the application is exiting, write the application data to the user file and close it.
        WriteFormDataToFile();
    }

    private void OnFormClosing(object? sender, CancelEventArgs e)
    {
      if(configData == null){ 
        ShowErrorMessage("configData not Loaded when exiting. How did we get here?");
        return;
      }
        // When a form is closing, remember the form position so it
        // can be saved in the user data file.
        if (sender is TabSwitcher)
        {
            if(configData.TabSwitcherData == null){
              ShowErrorMessage("tabSwitcherData is not loaded!");
              return;
            }
            configData.TabSwitcherData.StartPosition = FormStartPosition.Manual; 
            configData.TabSwitcherData.Location = tabSwitcher.Location;
        }
        else{
          return;
        }
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
        if(configData is null) return false;
        var jsonSerializedData = JsonSerializer.Serialize(configData);
        try
        {
            File.WriteAllText(configPath, jsonSerializedData);
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
        if(!File.Exists(configPath)){
          var cont = ShowConfirmationMessage($"Config file not found at: {configPath}\nLoad Default Config?");
          switch (cont)
            {
                case 1:
                    configData = new ConfigDataV1();
                    return true;
                default:
                    return false;
            }
        }
        try
        {
            string jsonSerializedData = File.ReadAllText(configPath);
            ConfigDataBase? basicInfo = JsonSerializer.Deserialize<ConfigDataBase>(jsonSerializedData);
            if (basicInfo != null)
            {
                switch (basicInfo.ConfigVersion)
                {
                    case 0.5F:
                        var conf = ShowConfirmationMessage($"Config file incompatable at: {configPath}\nLoad Default Config?");
                        configData = new ConfigDataV1();
                        return conf == 1;
                    case 1:
                        configData = JsonSerializer.Deserialize<ConfigDataV1>(jsonSerializedData);
                        break;
                    default:
                        throw new DataException($"Unknown Config Version. Found v{basicInfo.ConfigVersion}. Expected between {ConfigDataBase.MinVersion} and {ConfigDataBase.MaxVersion}");
                }
            }
            else
            {
                throw new DataException("JSON data malformed!");
            }
        }
        catch (Exception e)
        {
            ShowErrorMessage("Failed to access Config file. Error: " + e.Message);
            return false;
        }
        return true;
    }
}
