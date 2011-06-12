namespace KeyPadawan.View.KeyProcessors
{
    using System.Windows.Forms;
    using KeyPadawan.ViewModel;

    class PasswordProcessor : IEventProcessor
    {
        private int _state = 0;
        private int _index = 0;
        private int _startIndex = 0;
        private int _endIndex = int.MaxValue;

        public bool TryProcessEvent(Event evnt, out string result)
        {
            result = null;
            var k = evnt.Keys;
            switch (_state)
            {
                case 0:
                    if (k.HasFlag(Keys.Control) && k.HasFlag(Keys.K)) _state = 1;
                    break;
                case 1:
                    if (k.HasFlag(Keys.Control) && k.HasFlag(Keys.L))
                    {
                        _state = 2;
                        _startIndex = _index;
                    }
                    else _state = 0;
                    break;
                case 2:
                    if (k.HasFlag(Keys.Control) && k.HasFlag(Keys.K)) _state = 3;
                    else if (_index > _startIndex && _index < _endIndex)
                    {
                        result = "*";
                        return true;
                    }
                    break;
                case 3:
                    if (k.HasFlag(Keys.Control) && k.HasFlag(Keys.L)) _state = 0;
                    else
                    {
                        _state = 0;
                        _endIndex = _index;
                    }
                    break;
            }

            _index++;

            return false;
        }
    }
}
