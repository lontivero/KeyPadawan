using System.Runtime.InteropServices;

namespace System.Windows.Controls
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);
    }
}
