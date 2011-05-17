namespace KeyPadawan
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Windows.Forms;
using System.Windows.Interop;
    using System.Text;

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
                Keys virtualKeyCode = (Keys)kbdStruct.KeyCode;
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
                    string inBuffer;

                    if (TryGetAscii(kbdStruct, out inBuffer))
                    {
                        if (inBuffer.Length == 1)
                        {
                            char key = inBuffer[0];
                            bool isDownShift = IsKeyPressed(WinApi.VK_SHIFT);
                            bool isDownCapslock = WinApi.GetKeyState(WinApi.VK_CAPITAL) != 0;

                            if ((isDownCapslock ^ isDownShift) && Char.IsLetter(key))
                                key = Char.ToUpper(key);

                            RaiseKeyPressEvent(key);
                        }
                        else
                        {
                            RaiseKeyPressEvent(inBuffer[0]);
                            RaiseKeyPressEvent(inBuffer[1]);
                        }
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

        private static bool TryGetAscii(KBDLLHOOKSTRUCT kbdStruct, out string inBuffer)
        {
            var keyState = new byte[256];
            WinApi.GetKeyboardState(keyState);
            inBuffer = ToUnicode(kbdStruct);
            return !string.IsNullOrEmpty(inBuffer); 
        }

        private static DeadKeyInfo _lastDeadKey;

        private sealed class DeadKeyInfo
        {
            public DeadKeyInfo(KBDLLHOOKSTRUCT info, byte[] keyState)
            {
                KeyCode = (Keys)info.KeyCode;
                ScanCode = info.ScanCode;

                KeyboardState = keyState;
            }

            public readonly Keys KeyCode;
            public readonly UInt32 ScanCode;
            public readonly Byte[] KeyboardState;
        }

        private static string ToUnicode(KBDLLHOOKSTRUCT info)
        {
            string result = null;

            var keyState = new byte[256];
            var buffer = new StringBuilder(128);

            WinApi.GetKeyboardState(keyState);

            var layout = GetForegroundKeyboardLayout();
            var count = ToUnicode(info.KeyCode, info.ScanCode, keyState, buffer, layout);

            if (count > 0)
            {
                result = buffer.ToString(0, count);

                if (_lastDeadKey != null)
                {
                    ToUnicode(_lastDeadKey.KeyCode,
                              _lastDeadKey.ScanCode,
                              _lastDeadKey.KeyboardState,
                              buffer,
                              layout);

                    _lastDeadKey = null;
                }
            }
            else if (count < 0)
            {
                _lastDeadKey = new DeadKeyInfo(info, keyState);

                while (count < 0)
                {
                    count = ToUnicode(Keys.Decimal, buffer, layout);
                }
            }

            return result;
        }

        private static IntPtr GetForegroundKeyboardLayout()
        {
            var foregroundWnd = WinApi.GetForegroundWindow();

            if (foregroundWnd != IntPtr.Zero)
            {
                uint processId;
                uint threadId = WinApi.GetWindowThreadProcessId(foregroundWnd, out processId);

                return WinApi.GetKeyboardLayout(threadId);
            }

            return IntPtr.Zero;
        
        }
        private static int ToUnicode(Keys vk, StringBuilder buffer, IntPtr hkl)
        {
            return ToUnicode(vk, ToScanCode(vk), new byte[256], buffer, hkl);
        }

        private static int ToUnicode(Keys vk, uint sc, byte[] keyState, StringBuilder buffer, IntPtr hkl)
        {
            return WinApi.ToUnicodeEx((uint)vk, sc, keyState, buffer, buffer.Capacity, 0, hkl);
        }

        private static uint ToScanCode(Keys vk)
        {
            return WinApi.MapVirtualKey((uint)vk, 0);
        }

        public void Dispose()
        {
            UnHookKeyboard();
        }

        
        [StructLayout(LayoutKind.Sequential)]
        internal struct KBDLLHOOKSTRUCT
        {
            public Keys KeyCode;
            public uint ScanCode;
            public uint Flags;
            public uint Time;
            public uint ExtraInfo;
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
            internal const byte VK_ALTGR = 0x78;

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

            [DllImport("user32.dll")]
            internal static extern uint MapVirtualKey(uint uCode, uint uMapType);

            [DllImport("user32.dll")]
            internal static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[]
               lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff,
               int cchBuff, uint wFlags, IntPtr dwhkl);
            
            [DllImport("user32.dll")]
            internal static extern IntPtr GetForegroundWindow();

            [DllImport("user32.dll", SetLastError = true)]
            internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

            [DllImport("user32.dll")]
            internal static extern IntPtr GetKeyboardLayout(uint idThread);
        }


        internal enum VirtualKeyMapType
        {
            VirtualKeyToScanCode = 0,
            ScanCodeToVirtualKey = 1,
            VirtualKeyToChar = 2,
            ScanCodeToVirtualKeyEx = 3,
            VirtualKeyToScanCodeEx = 4,
        }
    }
}

