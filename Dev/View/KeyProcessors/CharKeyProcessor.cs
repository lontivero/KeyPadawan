namespace KeyPadawan.View.KeyProcessors
{
    using System.Windows.Forms;
    using KeyPadawan.ViewModel;

    class CharKeyProcessor : IEventProcessor
    {
        public bool TryProcessEvent(KeyboardEvent evnt, out string result)
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
