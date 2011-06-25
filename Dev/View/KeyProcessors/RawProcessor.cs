namespace KeyPadawan.View.KeyProcessors
{
    using KeyPadawan.ViewModel;
using System.Windows.Forms;

    class RawProcessor : IEventProcessor
    {
        private KeysConverter _converter = new KeysConverter();

        public bool TryProcessEvent(KeyboardEvent evnt, out string result)
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
            else
            {
                result = _converter.ConvertToString(evnt.Keys);
                result = result.Replace("+None", "");
                return true;
            }
        }
    }

}
