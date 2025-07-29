namespace BetterAltTab;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

internal static class TabSwitcherData
{
    internal static Image BackgroundImage { get; set; } = null!;
}
public class TabSwitcher : Form
{
    int hWnd;
    bool altPressed = true;
    globalKeyboardHook gkh = new globalKeyboardHook();

    public TabSwitcher()
    {
        SetupForm();
        SetupHooks();
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
        base.OnVisibleChanged(e);
        if (this.Visible)
        {
            hWnd = Process.GetCurrentProcess().MainWindowHandle.ToInt32();
            ProcessHelper.SetFocusToExternalApp(hWnd);
            //
            //
            //Need to find out why I can't grab the application focus
            //
            //
        }
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
                    if (!this.Visible)
                    {
                        // Alt+Tab pressed, show the tab switcher
                        this.Controls.Clear();
                        CreateProcessButtons();
                        this.Show();
                    }
                    else
                    {
                        this.Focus();
                    }
                    e.Handled = true; // Prevent further processing of the key event
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
    private void SetupForm()
    {
        this.FormBorderStyle = FormBorderStyle.None;
        this.BackgroundImage = TabSwitcherData.BackgroundImage;
        this.StartPosition = FormStartPosition.CenterScreen;
        var rect = Screen.PrimaryScreen?.Bounds;
        if (rect is null)
        {
            rect = new Rectangle(0, 0, 1920, 1080);
        }
        this.SetBounds(0, 0, (rect.Value.Width / 3) * 2, (rect.Value.Height / 3) * 2);
        this.Visible = false;
        CreateProcessButtons();
    }

    private void CreateProcessButtons()
    {
        var pList = ProcessHelper.GetRunningProcesses();
        var buttonList = new List<Button>();
        foreach (var process in pList)
        {
            var button = CreateProcessButton(process.Item1, process.Item2);
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

    private Button CreateProcessButton(string processName, int hWnd)
    {
        Button button = new Button();
        button.Text = processName;
        button.AutoSize = true;
        button.Click += (sender, e) =>
        {
            try
            {
                ProcessHelper.SetFocusToExternalApp(hWnd);
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
