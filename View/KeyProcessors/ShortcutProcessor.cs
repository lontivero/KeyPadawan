namespace KeyPadawan.View.KeyProcessors
{
    using System.Windows.Forms;
    using KeyPadawan.ViewModel;

    class ShortcutProcessor : IEventProcessor
    {
        private KeysConverter _keysConverter = new KeysConverter();
        private bool _leftControl = false;

        public bool TryProcessEvent(Event evnt, out string result)
        {
            result = null;
            if (!evnt.IsChar)
            {
                if (evnt.Keys.HasFlag((Keys)524288))
                {
                    return false;
                }

                if (evnt.Keys.HasFlag(Keys.Control | Keys.LMenu))
                {
                    result = "";
                    return true;
                }

                Keys keyCode = evnt.Keys & Keys.KeyCode;
                Keys n = keyCode & ~(Keys.LControlKey | Keys.RControlKey | Keys.LShiftKey | Keys.RShiftKey | Keys.Alt);
                if (n == Keys.None) return false;
                result = (string)_keysConverter.ConvertTo(evnt.Keys, typeof(string));
                return true;
            }

            return false;
        }
    }
}
