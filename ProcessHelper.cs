using System.Runtime.InteropServices;

namespace BetterAltTab
{
    public class ProcessHelper
    {
        public static void SetFocusToExternalApp(int hWnd)
        {
            IntPtr ipHwnd = hWnd;
            Thread.Sleep(100);
            SetForegroundWindow(ipHwnd);
        }

        //API-declaration
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

    }
}
