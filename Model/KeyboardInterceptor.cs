namespace KeyPadawan
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Windows.Forms;

    internal class KeyboardInterceptor : IDisposable
    {
        private LowLevelKeyboardProc keyboardProc;
        private IntPtr previousKeyboardHandler = IntPtr.Zero;
        private bool IsCapsPressed;
        private bool IsShiftPressed;
        private bool IsAltControlPressed;
        private bool IsControlPressed;
        private bool IsAltPressed;

        public event KeyboardEventHandler KeyIntercepted;

        public KeyboardInterceptor()
        {
            this.keyboardProc = new LowLevelKeyboardProc(this.keyboardHandler);
            using (Process process = Process.GetCurrentProcess())
            {
                using (ProcessModule module = process.MainModule)
                {
                    this.previousKeyboardHandler = WinApi.SetWindowsHookEx(13, this.keyboardProc, WinApi.GetModuleHandle(module.ModuleName), 0);
                }
            }
        }

        private IntPtr keyboardHandler(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT kbdStruct)
        {
            if ((nCode >= 0) && ((wParam == ((IntPtr) 0x100)) || (wParam == ((IntPtr) 260))))
            {
                string str = string.Empty;
                int vkCode = kbdStruct.vkCode;
                Keys modifierKeys = Control.ModifierKeys;
                if (((Keys.Alt | Keys.Control) & Control.ModifierKeys) == (Keys.Alt | Keys.Control))
                {
                    this.IsAltControlPressed = true;
                }
                if ((Keys.Shift & Control.ModifierKeys) == Keys.Shift)
                {
                    this.IsShiftPressed = true;
                }
                if ((Keys.Alt & Control.ModifierKeys) == Keys.Alt)
                {
                    this.IsAltPressed = true;
                }
                if ((Keys.Control & Control.ModifierKeys) == Keys.Control)
                {
                    this.IsControlPressed = true;
                }
                switch (((Keys) vkCode))
                {
                    case Keys.Back:
                        str = "<BS>";
                        break;

                    case Keys.Tab:
                        str = "<TAB>";
                        break;

                    case Keys.Clear:
                        str = "<CLEAR>";
                        break;

                    case Keys.Return:
                        str = "<ENTER>";
                        break;

                    case Keys.Pause:
                        str = "<PAUSE>";
                        break;

                    case Keys.Capital:
                        if (this.IsCapsPressed)
                        {
                            this.IsCapsPressed = false;
                        }
                        else
                        {
                            this.IsCapsPressed = true;
                        }
                        break;

                    case Keys.Escape:
                        str = "<ESC>";
                        break;

                    case Keys.Space:
                        str = "<SPACE>";
                        break;

                    case Keys.Prior:
                        str = "<PGUP>";
                        break;

                    case Keys.Next:
                        str = "<PGDOWN>";
                        break;

                    case Keys.End:
                        str = "<END>";
                        break;

                    case Keys.Home:
                        str = "<HOME>";
                        break;

                    case Keys.Left:
                        str = "<LEFT>";
                        break;

                    case Keys.Up:
                        str = "<UP>";
                        break;

                    case Keys.Right:
                        str = "<RIGHT>";
                        break;

                    case Keys.Down:
                        str = "<DOWN>";
                        break;

                    case Keys.Select:
                        str = "<SELECT>";
                        break;

                    case Keys.Print:
                        str = "<PRINT>";
                        break;

                    case Keys.Execute:
                        str = "<EXECUTE>";
                        break;

                    case Keys.Snapshot:
                        str = "<PRINT SCRN>";
                        break;

                    case Keys.Insert:
                        str = "<INS>";
                        break;

                    case Keys.Delete:
                        str = "<DEL>";
                        break;

                    case Keys.Help:
                        str = "<HELP>";
                        break;

                    case Keys.D0:
                        if (this.IsShiftPressed)
                        {
                            str = "=";
                        }
                        else
                        {
                            str = "0";
                        }
                        break;

                    case Keys.D1:
                        if (this.IsShiftPressed || this.IsAltControlPressed)
                        {
                            if (this.IsAltControlPressed)
                            {
                                str = "|";
                            }
                            else
                            {
                                str = "!";
                            }
                        }
                        else
                        {
                            str = "1";
                        }
                        break;

                    case Keys.D2:
                        if (this.IsShiftPressed || this.IsAltControlPressed)
                        {
                            if (this.IsAltControlPressed)
                            {
                                str = "@";
                            }
                            else
                            {
                                str = "\"";
                            }
                        }
                        else
                        {
                            str = "2";
                        }
                        break;

                    case Keys.D3:
                        if (this.IsShiftPressed || this.IsAltControlPressed)
                        {
                            if (this.IsAltControlPressed)
                            {
                                str = "#";
                            }
                            else
                            {
                                str = "\x00b7";
                            }
                        }
                        else
                        {
                            str = "3";
                        }
                        break;

                    case Keys.D4:
                        if (this.IsShiftPressed)
                        {
                            str = "$";
                        }
                        else
                        {
                            str = "4";
                        }
                        break;

                    case Keys.D5:
                        if (this.IsShiftPressed)
                        {
                            str = "%";
                        }
                        else
                        {
                            str = "5";
                        }
                        break;

                    case Keys.D6:
                        if (this.IsShiftPressed)
                        {
                            str = "&";
                        }
                        else
                        {
                            str = "6";
                        }
                        break;

                    case Keys.D7:
                        if (this.IsShiftPressed)
                        {
                            str = "/";
                        }
                        else
                        {
                            str = "7";
                        }
                        break;

                    case Keys.D8:
                        if (this.IsShiftPressed)
                        {
                            str = "(";
                        }
                        else
                        {
                            str = "8";
                        }
                        break;

                    case Keys.D9:
                        if (this.IsShiftPressed)
                        {
                            str = ")";
                        }
                        else
                        {
                            str = "9";
                        }
                        break;

                    case Keys.LWin:
                    case Keys.RWin:
                    case Keys.LShiftKey:
                    case Keys.RShiftKey:
                    case Keys.LControlKey:
                    case Keys.RControlKey:
                    case Keys.LMenu:
                    case Keys.RMenu:
                        break;

                    case Keys.Apps:
                        str = "<APP>";
                        break;

                    case Keys.NumPad0:
                        str = "0";
                        break;

                    case Keys.NumPad1:
                        str = "1";
                        break;

                    case Keys.NumPad2:
                        str = "2";
                        break;

                    case Keys.NumPad3:
                        str = "3";
                        break;

                    case Keys.NumPad4:
                        str = "4";
                        break;

                    case Keys.NumPad5:
                        str = "5";
                        break;

                    case Keys.NumPad6:
                        str = "6";
                        break;

                    case Keys.NumPad7:
                        str = "7";
                        break;

                    case Keys.NumPad8:
                        str = "8";
                        break;

                    case Keys.NumPad9:
                        str = "9";
                        break;

                    case Keys.Multiply:
                        str = "*";
                        break;

                    case Keys.Add:
                        str = "+";
                        break;

                    case Keys.Subtract:
                        str = "-";
                        break;

                    case Keys.Decimal:
                        str = ",";
                        break;

                    case Keys.Divide:
                        str = "/";
                        break;

                    case Keys.F1:
                        str = "<F1>";
                        break;

                    case Keys.F2:
                        str = "<F2>";
                        break;

                    case Keys.F3:
                        str = "<F3>";
                        break;

                    case Keys.F4:
                        str = "<F4>";
                        break;

                    case Keys.F5:
                        str = "<F5>";
                        break;

                    case Keys.F6:
                        str = "<F6>";
                        break;

                    case Keys.F7:
                        str = "<F7>";
                        break;

                    case Keys.F8:
                        str = "<F8>";
                        break;

                    case Keys.F9:
                        str = "<F9>";
                        break;

                    case Keys.F10:
                        str = "<F10>";
                        break;

                    case Keys.F11:
                        str = "<F11>";
                        break;

                    case Keys.F12:
                        str = "<F12>";
                        break;

                    case Keys.F13:
                        str = "<F13>";
                        break;

                    case Keys.F14:
                        str = "<F14>";
                        break;

                    case Keys.F15:
                        str = "<F15>";
                        break;

                    case Keys.F16:
                        str = "<F16>";
                        break;

                    case Keys.F17:
                        str = "<F17>";
                        break;

                    case Keys.F18:
                        str = "<F18>";
                        break;

                    case Keys.F19:
                        str = "<F19>";
                        break;

                    case Keys.F20:
                        str = "<F20>";
                        break;

                    case Keys.F21:
                        str = "<F21>";
                        break;

                    case Keys.F22:
                        str = "<F22>";
                        break;

                    case Keys.F23:
                        str = "<F23>";
                        break;

                    case Keys.F24:
                        str = "<F24>";
                        break;

                    case Keys.NumLock:
                        str = "<NUMLOCK>";
                        break;

                    case Keys.Scroll:
                        str = "<SCROLL LOCK>";
                        break;

                    case Keys.OemSemicolon:
                        if (this.IsShiftPressed || this.IsAltControlPressed)
                        {
                            if (this.IsAltControlPressed)
                            {
                                str = "[";
                            }
                            else
                            {
                                str = "^";
                            }
                        }
                        else
                        {
                            str = "`";
                        }
                        break;

                    case Keys.Oemplus:
                        if (this.IsShiftPressed || this.IsAltControlPressed)
                        {
                            if (this.IsAltControlPressed)
                            {
                                str = "]";
                            }
                            else
                            {
                                str = "*";
                            }
                        }
                        else
                        {
                            str = "+";
                        }
                        break;

                    case Keys.Oemcomma:
                        if (this.IsShiftPressed)
                        {
                            str = ";";
                        }
                        else
                        {
                            str = ",";
                        }
                        break;

                    case Keys.OemMinus:
                        if (this.IsShiftPressed)
                        {
                            str = "_";
                        }
                        else
                        {
                            str = "-";
                        }
                        break;

                    case Keys.OemPeriod:
                        if (this.IsShiftPressed)
                        {
                            str = ":";
                        }
                        else
                        {
                            str = ".";
                        }
                        break;

                    case Keys.OemQuestion:
                        if (this.IsShiftPressed || this.IsAltControlPressed)
                        {
                            if (this.IsAltControlPressed)
                            {
                                str = "}";
                            }
                            else
                            {
                                str = "\x00c7";
                            }
                        }
                        else
                        {
                            str = "\x00e7";
                        }
                        break;

                    case Keys.Oemtilde:
                        if (this.IsShiftPressed)
                        {
                            str = "~";
                        }
                        else
                        {
                            str = "`";
                        }
                        break;

                    case Keys.OemOpenBrackets:
                        if (this.IsShiftPressed)
                        {
                            str = "?";
                        }
                        else
                        {
                            str = "'";
                        }
                        break;

                    case Keys.OemPipe:
                        if (this.IsShiftPressed || this.IsAltControlPressed)
                        {
                            if (this.IsAltControlPressed)
                            {
                                str = @"\";
                            }
                            else
                            {
                                str = "\x00aa";
                            }
                        }
                        else
                        {
                            str = "\x00ba";
                        }
                        break;

                    case Keys.OemCloseBrackets:
                        if (this.IsShiftPressed)
                        {
                            str = "\x00bf";
                        }
                        else
                        {
                            str = "\x00a1";
                        }
                        break;

                    case Keys.OemQuotes:
                        if (this.IsShiftPressed || this.IsAltControlPressed)
                        {
                            if (this.IsAltControlPressed)
                            {
                                str = "{";
                            }
                            else
                            {
                                str = "\x00a8";
                            }
                        }
                        else
                        {
                            str = "\x00b4";
                        }
                        break;

                    case Keys.OemBackslash:
                        if (this.IsShiftPressed)
                        {
                            str = ">";
                        }
                        else
                        {
                            str = "<";
                        }
                        break;

                    default:
                        if (!this.IsShiftPressed && !this.IsCapsPressed)
                        {
                            str = ((Keys) vkCode).ToString().ToLower();
                        }
                        if (this.IsShiftPressed && !this.IsCapsPressed)
                        {
                            str = ((Keys) vkCode).ToString().ToUpper();
                        }
                        if (!this.IsShiftPressed && this.IsCapsPressed)
                        {
                            str = ((Keys) vkCode).ToString().ToUpper();
                        }
                        if (this.IsShiftPressed && this.IsCapsPressed)
                        {
                            str = ((Keys) vkCode).ToString().ToLower();
                        }
                        break;
                }
                if (!this.IsAltControlPressed || ((((0xbf != vkCode) && (0x31 != vkCode)) && ((50 != vkCode) && (0x33 != vkCode))) && (((0xba != vkCode) && (220 != vkCode)) && ((0xde != vkCode) && (0xbb != vkCode)))))
                {
                    if (this.IsControlPressed && !string.IsNullOrEmpty(str))
                    {
                        str = "CTRL+" + str;
                    }
                    if (this.IsAltControlPressed && !string.IsNullOrEmpty(str))
                    {
                        str = "ALT+" + str;
                    }
                }
                this.IsShiftPressed = false;
                this.IsAltControlPressed = false;
                this.IsControlPressed = false;
                this.IsAltControlPressed = false;
                this.OnKeyIntercepted(new KeyboardEventArgs(str));
            }
            return WinApi.CallNextHookEx(this.previousKeyboardHandler, nCode, wParam, ref kbdStruct);
        }

        public void Dispose()
        {
            WinApi.UnhookWindowsHookEx(this.previousKeyboardHandler);
        }

        public void OnKeyIntercepted(KeyboardEventArgs e)
        {
            if (this.KeyIntercepted != null)
            {
                this.KeyIntercepted(e);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        public class KeyboardEventArgs : EventArgs
        {
            private string kbdStruct;

            public KeyboardEventArgs(string ascii)
            {
                this.kbdStruct = ascii;
            }

            public string Ascii
            {
                get
                {
                    return this.kbdStruct;
                }
            }
        }

        public delegate void KeyboardEventHandler(KeyboardInterceptor.KeyboardEventArgs e);

        internal delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, ref KeyboardInterceptor.KBDLLHOOKSTRUCT lParam);

        [ComVisible(false), SuppressUnmanagedCodeSecurity]
        internal class WinApi
        {
            internal const int VK_CAPITAL = 20;
            internal const int VK_CONTROL = 0x11;
            internal const int VK_MENU = 0x12;
            internal const int VK_SHIFT = 0x10;
            internal const int WH_KEYBOARD_LL = 13;
            internal const int WM_KEYDOWN = 0x100;
            internal const int WM_SYSKEYDOWN = 260;

            [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
            internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref KeyboardInterceptor.KBDLLHOOKSTRUCT lParam);
            [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
            internal static extern IntPtr GetModuleHandle(string lpModuleName);
            [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
            internal static extern IntPtr SetWindowsHookEx(int idHook, KeyboardInterceptor.LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
            [return: MarshalAs(UnmanagedType.Bool)]
            [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
            internal static extern bool UnhookWindowsHookEx(IntPtr hhk);
        }
    }
}

