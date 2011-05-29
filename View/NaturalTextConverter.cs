using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Forms;
using KeyPadawan.ViewModel;

namespace KeyPadawan.View
{
 

    class NaturalTextConverter : IValueConverter
    {
        private KeysConverter _keysConverter = new KeysConverter();
        private Dictionary<Keys, Func<string, string>> _translation = new Dictionary<Keys, Func<string, string>>
        {
            { Keys.Enter, s=> s + "\u21b5" },
            { Keys.Escape, s=> s + "\u238b" },
            { Keys.Tab, s=> s + "   "  },
            { Keys.Back, s=> RemoveLatestChar(s) } 
        };

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var text = string.Empty;
            var buffer = (IEnumerable<Event>)value;

            foreach (var e in buffer)
            {
                if(IsAltPressed(e.Keys) || IsControlPressed(e.Keys)) continue;

                if (e.IsChar)
                {
                    text += e.Char;
                }
                else
                {
                    if (_translation.ContainsKey(e.Keys))
                    {
                        text = _translation[e.Keys](text);
                    }
                    else
                    {
                        text += _keysConverter.ConvertTo(e.Keys, typeof(string));
                    }
                }
            }
            
            var l = text.Length;
            return text.Substring(l < 30 ? 0 : l - 30);
        }

        private bool IsControlPressed(Keys keys)
        {
            return (keys & Keys.Control) == Keys.Control;
        }

        private bool IsAltPressed(Keys keys)
        {
            return (keys & Keys.Alt) == Keys.Alt;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static string RemoveLatestChar(string buffer)
        {
            if (string.IsNullOrEmpty(buffer))
                return string.Empty;

            return buffer.Remove(buffer.Length - 1);
        }
    }
}
