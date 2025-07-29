using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BetterAltTab
{
    public class ProcessHelper
    {
        public const int SW_RESTORE = 9;

        internal static void SetFocusToExternalApp(int hWnd)
        {
            IntPtr ipHwnd = hWnd;
            Thread.Sleep(100);
            SetForegroundWindow(ipHwnd);
        }

        internal static void RestoreAndSwitchToExtern(int hWnd)
        {
            ShowWindow(hWnd, SW_RESTORE);
        }

        internal static List<Process> GetRunningProcesses()
        {
            var processList = new List<Process>();

            var ignoredProcesses = new HashSet<string>
                {
                    "BetterAltTab",
                    "BetterAltTab.exe",
                    "Windows Input Experience",
                    "Settings"
                };
            var processes = Process.GetProcesses();
            foreach (var process in processes)
            {

                try
                {
                    var text = process.MainWindowTitle;
                    if (ignoredProcesses.Contains(text)) continue;
                    // var hWnd = process.MainWindowHandle;
                    // var processInfo = new Tuple<string, int>(text, hWnd.ToInt32());
                    if (!string.IsNullOrEmpty(text) && process.MainWindowHandle != 0) processList.Add(process);
                }
                catch (Exception)
                {
                    // Ignore processes that throw exceptions when accessing MainWindowTitle
                }
            }
            return processList;
        }

        //API-declaration

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

    }
}
