namespace BetterAltTab;

using System.Diagnostics;
using System.Runtime.InteropServices;

static class InterceptKeys
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_SYSKEYDOWN = 0x0104; // For Alt key combos
    private const int WM_KEYUP = 0x0101;
    private const int WM_SYSKEYUP = 0x0105;
    private static LowLevelKeyboardProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;

    private static bool[] modifiersDown = new bool[0];
    private static Keys[] _keys = new Keys[0];

    public static void InitializeKeyboardHook(Keys[] keys)
    {
        if (_hookID != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_hookID);
            _hookID = IntPtr.Zero;
        }
        modifiersDown = new bool[keys.Length - 1];
        _keys = keys;
        _hookID = SetHook(_proc);
    }

    public static void ReleaseKeyboardHook()
    {
        if (_hookID != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_hookID);
            _hookID = IntPtr.Zero;
        }
        modifiersDown = new bool[0];
    }

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule? curModule = curProcess.MainModule)
        {
            if (curModule == null)
            {
                throw new InvalidOperationException("Current process module is null. How did we get here?");
            }
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    private delegate IntPtr LowLevelKeyboardProc(
        int nCode, IntPtr wParam, IntPtr lParam);

    private static IntPtr HookCallback(
        int nCode, IntPtr wParam, IntPtr lParam)
    {
        Keys vkCode = (Keys)Marshal.ReadInt32(lParam);
        if (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
        {
            for (var i = 0; i < modifiersDown.Length; i++)
            {
                if (vkCode == _keys[i])
                {
                    modifiersDown[i] = true;
                }
            }
        }
        else
        {
            for (var i = 0; i < modifiersDown.Length; i++)
            {
                if (vkCode == _keys[i])
                {
                    modifiersDown[i] = false;
                }
            }
        }
        for (var i = 0; i < modifiersDown.Length; i++)
        {
            if (!modifiersDown[i])
            {
                return CallNextHookEx(_hookID, nCode, wParam, lParam);
            }
        }

        Application.Run(MainForm.mainForm);
        MainForm.SetupMainForm();
        return 1;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook,
        LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
        IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
}
