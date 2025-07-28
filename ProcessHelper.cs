using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BetterAltTab
{
    public class ProcessHelper
    {
        internal static void SetFocusToExternalApp(int hWnd)
        {
            IntPtr ipHwnd = hWnd;
            Thread.Sleep(100);
            SetForegroundWindow(ipHwnd);
        }

        //API-declaration
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        internal static List<Tuple<string, int>> GetRunningProcesses()
        {
            var ignoredProcesses = new HashSet<string>
                {
                    "BetterAltTab",
                    "BetterAltTab.exe",
                    "Windows Input Experience",
                    "Settings"
                };
            var currentUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            var processes = Process.GetProcesses();
            var processList = new List<Tuple<string, int>>();
            foreach (var process in processes)
            {

                try
                {
                    var text = process.MainWindowTitle;
                    if (ignoredProcesses.Contains(text)) continue;
                    var hWnd = process.MainWindowHandle;
                    var processInfo = new Tuple<string, int>(text, hWnd.ToInt32());
                    if (!string.IsNullOrEmpty(text) && hWnd != 0) processList.Add(processInfo);
                }
                catch (Exception)
                {
                    // Ignore processes that throw exceptions when accessing MainWindowTitle
                }
            }
            return processList;
        }
    }
}
