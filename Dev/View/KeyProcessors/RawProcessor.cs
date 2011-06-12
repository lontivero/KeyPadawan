namespace KeyPadawan.View.KeyProcessors
{
    using KeyPadawan.ViewModel;
using System.Windows.Forms;

    class RawProcessor : IEventProcessor
    {
        private KeysConverter _converter = new KeysConverter();

        public bool TryProcessEvent(Event evnt, out string result)
        {
            result = null;
            if (evnt.Keys.HasFlag((Keys)524288))
            {
                return false;
            }

            Keys n = evnt.Keys & Keys.KeyCode;

            if (evnt.IsChar)
            {
                result = evnt.Char.ToString();
                return true;
            }
            else if (n != Keys.LControlKey && n != Keys.RControlKey && n != Keys.LShiftKey && n != Keys.RShiftKey)
            {
                result = _converter.ConvertToString(evnt.Keys);
                return true;
            }

            return false;
        }
    }

}
