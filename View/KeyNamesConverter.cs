using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Forms;
using KeyPadawan.ViewModel;

namespace KeyPadawan.View
{
    class KeyNamesConverter : IValueConverter
    {
        private KeysConverter _keysConverter = new KeysConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var text = string.Empty;
            var buffer = (IEnumerable<Event>)value;

            foreach (var e in buffer)
            {
                if (e.IsChar)
                {
                    text += e.Char;
                }
                else
                {
                    switch (e.Keys)
                    {
                        case Keys.Back:
                            text = RemoveLatestChar(text);
                            break;
                        case Keys.Enter:
                            break;

                        default:
                            text += _keysConverter.ConvertTo(e.Keys, typeof(string));
                            break;
                    }
                }
            }
            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string RemoveLatestChar(string buffer)
        {
            if (!string.IsNullOrEmpty(buffer))
                return string.Empty;

            return buffer.Remove(buffer.Length - 1);
        }

    }
}
