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

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var text = string.Empty;
            var buffer = (IEnumerable<Event>)value;

            foreach (var e in buffer)
            {
                if(IsAltPressed(e.Keys) || IsControlPressed(e.Keys)) continue;

                if (e.IsChar)
                {
                    if((Keys)e.Char == Keys.Back)
                        text = RemoveLatestChar(text);
                    else
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
                            text += '\u21b5';
                            break;
                        case Keys.Escape:
                            text += '\u238b';
                            break;
                        case Keys.Tab:
                            text += "   ";
                            break;

                        default:
                            text += _keysConverter.ConvertTo(e.Keys, typeof(string));
                            break;
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

        private string RemoveLatestChar(string buffer)
        {
            if (string.IsNullOrEmpty(buffer))
                return string.Empty;

            return buffer.Remove(buffer.Length - 1);
        }
    }
}
