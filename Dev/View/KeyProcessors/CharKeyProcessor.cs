namespace KeyPadawan.View.KeyProcessors
{
    using System.Windows.Forms;
    using KeyPadawan.ViewModel;

    class CharKeyProcessor : IEventProcessor
    {
        private KeysConverter _keysConverter = new KeysConverter();

        public bool TryProcessEvent(Event evnt, out string result)
        {
            result = null;
            if (evnt.IsChar)
            {
                result = evnt.Char.ToString();
                return true;
            }

            return false;
        }
    }

}
