namespace KeyPadawan
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Windows.Forms;
using System.Windows.Interop;

    class KeyboardInterceptor : IDisposable
    {
        private delegate IntPtr LowLevelKeyboardProc(int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam);

        private LowLevelKeyboardProc keyboardProc;
        private IntPtr previousKeyboardHandler = IntPtr.Zero;
        public EventHandler<KeyEventArgs> KeyDown;
        public EventHandler<KeyEventArgs> KeyUp;
        public EventHandler<KeyPressEventArgs> KeyPress;
        private bool isHooked = false;

        public KeyboardInterceptor()
        {
            HookKeyboard();
        }

        ~KeyboardInterceptor()
        {
            UnHookKeyboard();
        }

        private void HookKeyboard()
        {
            VerifyPreviousHooking();

            this.keyboardProc = new LowLevelKeyboardProc(this.keyboardHandler);
            using (Process process = Process.GetCurrentProcess())
            {
                using (ProcessModule module = process.MainModule)
                {
                    this.previousKeyboardHandler = WinApi.SetWindowsHookEx(WinApi.WH_KEYBOARD_LL, this.keyboardProc, WinApi.GetModuleHandle(module.ModuleName), 0);
                    isHooked = true;
                }
            }
        }

        private void VerifyPreviousHooking()
        {
            if (isHooked)
                throw new InvalidOperationException("KeyPadawan is already hooked!");
        }

        private void UnHookKeyboard()
        {
            if (isHooked)
            {
                WinApi.UnhookWindowsHookEx(this.previousKeyboardHandler);
                isHooked = false;
            }
        }

        private IntPtr keyboardHandler(int nCode, int wParam, ref KBDLLHOOKSTRUCT kbdStruct)
        {
            if (nCode >= 0) 
            {
                Keys virtualKeyCode = (Keys)kbdStruct.virtualKeyCode;
                Keys keyData = BuildKeyData(virtualKeyCode);
                var keyEventArgs = new KeyEventArgs(keyData);

                int kbevent = wParam;
                if (kbevent == WinApi.WM_DEADCHAR || kbevent == WinApi.WM_SYSDEADCHAR)
                {
                    System.Diagnostics.Debugger.Break();
                }

                if (kbevent == WinApi.WM_KEYDOWN || kbevent == WinApi.WM_SYSKEYDOWN)
                {
                    RaiseKeyDownEvent(keyEventArgs);
                }
                else if (kbevent == WinApi.WM_KEYUP || kbevent == WinApi.WM_SYSKEYUP)
                {
                    RaiseKeyUpEvent(keyEventArgs);
                }

                if (kbevent == WinApi.WM_KEYDOWN)
                {
                    byte[] inBuffer;

                    if (TryGetAscii(kbdStruct, out inBuffer))
                    {
                        char key = (char)inBuffer[0];
                        bool isDownShift = IsKeyPressed(WinApi.VK_SHIFT);
                        bool isDownCapslock = WinApi.GetKeyState(WinApi.VK_CAPITAL) != 0;

                        if ((isDownCapslock ^ isDownShift) && Char.IsLetter(key))
                            key = Char.ToUpper(key);

                        RaiseKeyPressEvent(key);
                    }
                }
            }

            return WinApi.CallNextHookEx(this.previousKeyboardHandler, nCode, wParam, ref kbdStruct);
        }

        private static Keys BuildKeyData(Keys virtualKeyCode )
        {
            bool isDownControl = IsKeyPressed(WinApi.VK_LCONTROL) || IsKeyPressed(WinApi.VK_RCONTROL);
            bool isDownShift = IsKeyPressed(WinApi.VK_LSHIFT) || IsKeyPressed(WinApi.VK_RSHIFT);
            bool isDownAlt = IsKeyPressed(WinApi.VK_LALT) || IsKeyPressed(WinApi.VK_RALT);

            return virtualKeyCode |
                (isDownControl ? Keys.Control : Keys.None) |
                (isDownShift ? Keys.Shift : Keys.None) |
                (isDownAlt ? Keys.Alt : Keys.None);
        }

        private static bool IsKeyPressed(byte virtualKeyCode)
        {
            return (WinApi.GetKeyState(virtualKeyCode) & 0x80) != 0;
        }

        private void RaiseKeyPressEvent(char key)
        {
            var _keyPress = KeyPress;
            if (_keyPress != null)
            {
                _keyPress(this, new KeyPressEventArgs(key));
            }
        }

        private void RaiseKeyDownEvent(KeyEventArgs args)
        {
            var _keyDown = KeyDown;
            if (_keyDown != null)
            {
                _keyDown(this, args);
            }
        }

        private void RaiseKeyUpEvent(KeyEventArgs args)
        {
            var _keyUp = KeyUp;
            if (_keyUp != null)
            {
                _keyUp(this, args);
            }
        }

        private static bool TryGetAscii(KBDLLHOOKSTRUCT kbdStruct, out byte[] inBuffer)
        {
            var keyState = new byte[256];
            WinApi.GetKeyboardState(keyState);
            inBuffer = new byte[2];

            int chars = WinApi.ToAscii(kbdStruct.virtualKeyCode, kbdStruct.scanCode, keyState, inBuffer, kbdStruct.flags);
            chars = WinApi.ToAscii(kbdStruct.virtualKeyCode, kbdStruct.scanCode, keyState, inBuffer, kbdStruct.flags);

            return chars == 1;
        }


        public void Dispose()
        {
            UnHookKeyboard();
        }

        
        [StructLayout(LayoutKind.Sequential)]
        internal struct KBDLLHOOKSTRUCT
        {
            public int virtualKeyCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        [ComVisible(false), SuppressUnmanagedCodeSecurity]
        private static class WinApi
        {
            internal const int WH_KEYBOARD_LL = 0x0D;

            internal const byte VK_CAPITAL = 0x14;
            internal const byte VK_CONTROL = 0x11;
            internal const byte VK_MENU = 0x12;
            internal const byte VK_SHIFT = 0x10;
            internal const byte VK_LSHIFT = 0xA0;
            internal const byte VK_RSHIFT = 0xA1;
            internal const byte VK_LALT = 0xA4;
            internal const byte VK_LCONTROL = 0xA2;
            internal const byte VK_NUMLOCK = 0x90;
            internal const byte VK_RALT = 0xA5;
            internal const byte VK_RCONTROL = 0x3;

            internal const int WM_KEYDOWN = 0x100;
            internal const int WM_SYSKEYDOWN = 0x104;
            internal const int WM_KEYUP = 0x101;
            internal const int WM_SYSKEYUP = 0x105;
            internal const int WM_DEADCHAR = 0x0103;
            internal const int WM_SYSDEADCHAR = 0x0107;

            [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
            internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam);
            
            [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
            internal static extern IntPtr GetModuleHandle(string lpModuleName);
            
            [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
            internal static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

            [return: MarshalAs(UnmanagedType.Bool)]
            [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
            internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            internal static extern short GetKeyState(int vKey);

            [DllImport("user32.dll")]
            internal static extern int ToAscii(int vKey, int nCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);

            [DllImport("user32.dll")]
            internal static extern int GetKeyboardState(byte[] pbKeyState);

            [DllImport("user32.dll")]
            internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

            [return: MarshalAs(UnmanagedType.Bool)]
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        }
    }
}

