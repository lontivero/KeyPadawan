using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Forms;
using KeyPadawan.ViewModel;

namespace KeyPadawan.View
{
    interface IEventProcessor
    {
        bool TryProcessEvent(Event evnt, out string result);
    }

    class SymbolicKeysTranslator : IEventProcessor
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
                    else if(_index > _startIndex && _index < _endIndex)
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

    class PasswordProcessor : IEventProcessor
    {
        private Dictionary<Keys, string> _translation = new Dictionary<Keys, string>
        {
            { Keys.Enter, "\u21b5" },
            { Keys.Escape, "\u238b" }
        };

        public bool TryProcessEvent(Event evnt, out string result)
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

    class DefaultProcessor : IEventProcessor
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
            else
            {
                result = (string)_keysConverter.ConvertTo(evnt.Keys, typeof(string));
                return true;
            }
        }
    }

    class NaturalTextConverter : IValueConverter
    {
        private LinkedList<IEventProcessor> _chainOfProcessors;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            _chainOfProcessors = new LinkedList<IEventProcessor>();
            _chainOfProcessors.AddLast(new PasswordProcessor());
            _chainOfProcessors.AddLast(new SymbolicKeysTranslator());
            _chainOfProcessors.AddLast(new DefaultProcessor());

            var textToDisplay = string.Empty;
            var buffer = (IEnumerable<Event>)value;

            foreach (var e in buffer)
            {
                var xxx = _chainOfProcessors.GetEnumerator();
                while (xxx.MoveNext())
                {
                    string r = string.Empty;
                    var yyy = xxx.Current;
                    var done = yyy.TryProcessEvent(e, out r);
                    textToDisplay += r ?? "";
                    if (done) break;
                }
            }

            var l = textToDisplay.Length;
            return (l < 30) ? textToDisplay : "..." + textToDisplay.Substring(l - 27);
        }

        private static string RemoveLatestChar(string buffer)
        {
            if (string.IsNullOrEmpty(buffer))
                return string.Empty;

            return buffer.Remove(buffer.Length - 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
