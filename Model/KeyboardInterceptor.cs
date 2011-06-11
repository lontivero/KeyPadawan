namespace KeyPadawan.Model
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Windows.Forms;

    class KeyboardInterceptor : IKeyboardInterceptor, IDisposable
    {
        private LowLevelKeyboardProc keyboardProc;
        private IntPtr _previousKeyboardHandler = IntPtr.Zero;
        private bool _isHooked = false;
        private readonly KeysConverter _keyConverter = new KeysConverter();
        private static DeadKeyInfo _lastDeadKey;

        private EventHandler<KeyEventArgs> KeyDown;
        private EventHandler<KeyEventArgs> KeyUp;
        private EventHandler<KeyPressEventArgs> KeyPress;
        private bool _disposed = false;

        public KeyboardInterceptor()
        {
            HookKeyboard();
        }

        ~KeyboardInterceptor()
        {
            Dispose(false);
        }

        private void HookKeyboard()
        {
            VerifyPreviousHooking();

            this.keyboardProc = new LowLevelKeyboardProc(this.keyboardHandler);
            using (Process process = Process.GetCurrentProcess())
            {
                using (ProcessModule module = process.MainModule)
                {
                    this._previousKeyboardHandler = NativeMethods.SetWindowsHookEx(NativeMethods.WH_KEYBOARD_LL, this.keyboardProc, NativeMethods.GetModuleHandle(module.ModuleName), 0);
                    _isHooked = true;
                }
            }
        }

        private void VerifyPreviousHooking()
        {
            if (_isHooked)
            {
                throw new InvalidOperationException("KeyPadawan is already hooked!");
            }
        }

        private void UnHookKeyboard()
        {
            if (_isHooked)
            {
                NativeMethods.UnhookWindowsHookEx(this._previousKeyboardHandler);
                _isHooked = false;
            }
        }

        private IntPtr keyboardHandler(int nCode, int wParam, ref KBDLLHOOKSTRUCT kbdStruct)
        {
            if (nCode >= 0) 
            {
                Keys virtualKeyCode = (Keys)kbdStruct.KeyCode;
                Keys keyData = BuildKeyData(virtualKeyCode);
                var keyEventArgs = new KeyEventArgs(keyData);


                if (wParam == NativeMethods.WM_KEYDOWN || wParam == NativeMethods.WM_SYSKEYDOWN)
                {
                    Debug.Print("Press({0})", (int)virtualKeyCode);
                    RaiseKeyDownEvent(keyEventArgs);

                    string buffer = ToUnicode(kbdStruct);
                    if (!string.IsNullOrEmpty(buffer))
                    {
                        foreach (var rawKey in buffer)
                        {
                            var key = _keyConverter.ConvertToString(rawKey)[0];
                            RaiseKeyPressEvent(key);
                        }
                    }
                }
                else if (wParam == NativeMethods.WM_KEYUP || wParam == NativeMethods.WM_SYSKEYUP)
                {
                    Debug.Print("Release({0})", (int)virtualKeyCode);
                    RaiseKeyUpEvent(keyEventArgs);
                }
            }

            return NativeMethods.CallNextHookEx(this._previousKeyboardHandler, nCode, wParam, ref kbdStruct);
        }

        private static Keys BuildKeyData(Keys virtualKeyCode )
        {
            bool isDownControl = IsKeyPressed(NativeMethods.VK_LCONTROL) || IsKeyPressed(NativeMethods.VK_RCONTROL);
            bool isDownShift = IsKeyPressed(NativeMethods.VK_LSHIFT) || IsKeyPressed(NativeMethods.VK_RSHIFT);
            bool isDownAlt = IsKeyPressed(NativeMethods.VK_LALT) || IsKeyPressed(NativeMethods.VK_RALT) || IsKeyPressed(NativeMethods.VK_RMENU);
            bool isAltGr = IsKeyPressed(NativeMethods.VK_RMENU) && IsKeyPressed(NativeMethods.VK_LCONTROL);

            return virtualKeyCode |
                (isDownControl ? Keys.Control : Keys.None) |
                (isDownShift ? Keys.Shift : Keys.None) |
                (isDownAlt ? Keys.Alt : Keys.None) |
                (isAltGr ? (Keys)524288 : Keys.None);
        }

        private static bool IsKeyPressed(byte virtualKeyCode)
        {
            return (NativeMethods.GetKeyState(virtualKeyCode) & 0x80) != 0;
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

        private static string ToUnicode(KBDLLHOOKSTRUCT info)
        {
            string result = null;

            var keyState = new byte[256];
            var buffer = new StringBuilder(128);

            NativeMethods.GetKeyboardState(keyState);
            bool isAltGr = IsKeyPressed(NativeMethods.VK_RMENU) && IsKeyPressed(NativeMethods.VK_LCONTROL);
            if (isAltGr) keyState[NativeMethods.VK_LCONTROL] = keyState[NativeMethods.VK_LALT] = 0x80;

            var layout = GetForegroundKeyboardLayout();
            var count = ToUnicode((Keys)info.KeyCode, info.ScanCode, keyState, buffer, layout);

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
            var foregroundWnd = NativeMethods.GetForegroundWindow();

            if (foregroundWnd != IntPtr.Zero)
            {
                uint processId;
                uint threadId = NativeMethods.GetWindowThreadProcessId(foregroundWnd, out processId);

                return NativeMethods.GetKeyboardLayout(threadId);
            }

            return IntPtr.Zero;
        }

        private static int ToUnicode(Keys vk, StringBuilder buffer, IntPtr hkl)
        {
            return ToUnicode(vk, ToScanCode(vk), new byte[256], buffer, hkl);
        }

        private static int ToUnicode(Keys vk, uint sc, byte[] keyState, StringBuilder buffer, IntPtr hkl)
        {
            return NativeMethods.ToUnicodeEx((uint)vk, sc, keyState, buffer, buffer.Capacity, 0, hkl);
        }

        private static uint ToScanCode(Keys vk)
        {
            return NativeMethods.MapVirtualKey((uint)vk, 0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    UnHookKeyboard();
                }

                _disposed = true;
            }
        }

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

        event EventHandler<KeyEventArgs> IKeyboardInterceptor.KeyDown
        {
            add { this.KeyDown += value; }
            remove { this.KeyDown -= value; }
        }

        event EventHandler<KeyEventArgs> IKeyboardInterceptor.KeyUp
        {
            add { this.KeyUp += value; }
            remove { this.KeyUp -= value; }
        }

        event EventHandler<KeyPressEventArgs> IKeyboardInterceptor.KeyPress
        {
            add { this.KeyPress += value; }
            remove { this.KeyPress -= value; }
        }
    }
}

