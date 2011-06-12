using System;
using System.Windows.Forms;

namespace KeyPadawan.Model
{
    interface IKeyboardInterceptor
    {
        event EventHandler<KeyEventArgs> KeyDown;
        event EventHandler<KeyEventArgs> KeyUp;
        event EventHandler<KeyPressEventArgs> KeyPress;
    }
}
