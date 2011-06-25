namespace KeyPadawan.View.KeyProcessors
{
    using System.Collections.Generic;
    using System.Windows.Forms;
    using KeyPadawan.ViewModel;

    class SymbolicKeysTranslator : IEventProcessor
    {
        private Dictionary<Keys, string> _translation = new Dictionary<Keys, string>
        {
            { Keys.Enter, "\u21b5" },
            { Keys.Escape, "\u238b" },
            { Keys.Left, "\u21e6" },
            { Keys.Right, "\u21e8" },
            { Keys.Up, "\u21e7" },
            { Keys.Down, "\u21e9" }
        };

        public bool TryProcessEvent(KeyboardEvent evnt, out string result)
        {
            result = null;

            if (!evnt.IsChar && _translation.ContainsKey(evnt.Keys))
            {
                result = _translation[evnt.Keys];
                return true;
            }

            return false;
        }
    }
}
