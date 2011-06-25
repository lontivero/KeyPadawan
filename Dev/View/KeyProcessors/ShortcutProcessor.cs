namespace KeyPadawan.View.KeyProcessors
{
    using System.Windows.Forms;
    using KeyPadawan.ViewModel;

    class ShortcutProcessor : IEventProcessor
    {
        private KeysConverter _keysConverter = new KeysConverter();

        public bool TryProcessEvent(KeyboardEvent evnt, out string result)
        {
            result = string.Empty;
            if (!evnt.IsChar)
            {
                Keys keyCode = evnt.Keys & Keys.KeyCode;

                switch (keyCode)
                {
                    case Keys.LControlKey:
                    case Keys.RControlKey:
                    case Keys.LShiftKey:
                    case Keys.RShiftKey:
                    case Keys.RMenu:
                    case Keys.LMenu:
                        return true;
                }

                if (evnt.Keys.HasFlag((Keys)524288)) return false;
                if (!evnt.Keys.HasFlag(Keys.Control) && !evnt.Keys.HasFlag(Keys.Alt)) return false;
                if (evnt.Keys == Keys.Alt || evnt.Keys == Keys.Control) return false;

                //Keys n = keyCode & ~(Keys.LControlKey | Keys.RControlKey | Keys.LShiftKey | Keys.RShiftKey | Keys.LMenu | Keys.RMenu);
                //if (n == Keys.None) return true;

                result = (string)_keysConverter.ConvertTo(evnt.Keys, typeof(string));
                return true;
            }

            return false;
        }
    }
}
