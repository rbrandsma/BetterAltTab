namespace BetterAltTab;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

public class TabSwitcher : Form
{
    bool altPressed = false;
    globalKeyboardHook gkh = new globalKeyboardHook();

    public TabSwitcher()
    {
        SetupHooks();
    }

    private void SetupTabSwitcherForm(TabSwitcherData formData)
    {
        this.FormBorderStyle = formData.BorderStyle;
        this.BackgroundImage = Image.FromFile(formData.BackgroundImagePath ?? "resources\\img\\background.png");
        this.StartPosition = formData.StartPosition;
        if (formData.WindowSize is null)
        {
            formData.WindowSize = new Rectangle(0, 0, 1920, 1080);
        }
        var WindowSize = formData.WindowSize;
        this.SetBounds(WindowSize.Value.X, WindowSize.Value.Y, WindowSize.Value.Width, WindowSize.Value.Height);
        this.Visible = formData.StartVisable;
        CreateProcessButtons();
    }


    private void SetupHooks()
    {
        gkh.HookedKeys.Add(Keys.Tab);
        gkh.HookedKeys.Add(Keys.LMenu); // Left Alt
        gkh.HookedKeys.Add(Keys.RMenu); // Right Alt
        gkh.HookedKeys.Add(Keys.Alt); // Alt key
        gkh.HookedKeys.Add(Keys.LShiftKey);
        gkh.KeyDown += new KeyEventHandler(gkh_KeyDown);
        gkh.KeyUp += new KeyEventHandler(gkh_KeyUp);

    }

    void gkh_KeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.LMenu:
            case Keys.RMenu:
            case Keys.Alt:
                // Alt key pressed, set flag
                altPressed = true;
                break;
            case Keys.Tab:
                if (altPressed)
                {
                    //Yes this is a hack, but it works if you have a better way, please let me know.
                    if (!this.Visible)
                    {
                        this.Controls.Clear();
                        CreateProcessButtons();
                        this.WindowState = FormWindowState.Minimized;
                        this.Show();
                        this.WindowState = FormWindowState.Normal;
                    }
                    if (!this.ContainsFocus)
                    {
                        this.WindowState = FormWindowState.Minimized;
                        this.WindowState = FormWindowState.Normal;
                    }
                    e.Handled = true; // Prevent further processing of the tab key to stop alt-tab behavior
                }
                break;
        }
    }

    void gkh_KeyUp(object? sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.LMenu:
            case Keys.RMenu:
            case Keys.Alt:
                // Alt key pressed, set flag
                altPressed = false;
                break;
        }
    }

    private void ShowMessageBox(string message)
    {
        MessageBox.Show(message, "Tab Switcher", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    private void CreateProcessButtons()
    {
        var pList = ProcessHelper.GetRunningProcesses();
        var buttonList = new List<Button>();
        foreach (var process in pList)
        {
            var button = CreateProcessButton(process);
            buttonList.Add(button);
        }
        buttonList[0].Location = new Point(10, 10);
        if (this == null)
        {
            throw new InvalidOperationException("Main form is not initialized.");
        }
        this.Controls.Add(buttonList[0]);
        var offset = 10;
        for (int i = 1; i < buttonList.Count; i++)
        {
            var startPoint = buttonList[i - 1].Location.X + buttonList[i - 1].Width + 10;
            if (startPoint + buttonList[i].Width > this.Width)
            {
                startPoint = 10;
                offset += buttonList[i - 1].Height + 10;
            }
            var button = buttonList[i];
            button.Location = new Point(startPoint, offset);
            this.Controls.Add(button);
        }
    }

    private Button CreateProcessButton(Process process)
    {
        string processName = process.MainWindowTitle;
        int procHndl = process.MainWindowHandle.ToInt32();
        Button button = new Button();
        button.Text = processName;
        button.Image = process.MainModule != null ? Icon.ExtractAssociatedIcon(process.MainModule.FileName)?.ToBitmap() : null;
        button.AutoSize = true;
        button.Click += (sender, e) =>
        {
            try
            {
                ProcessHelper.RestoreAndSwitchToExtern(procHndl);
                this.Hide(); // Hide the tab switcher after switching
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to switch to {processName}: {ex.Message}");
            }
        };
        return button;
    }
}
