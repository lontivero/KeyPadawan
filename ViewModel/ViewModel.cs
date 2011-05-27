using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Globalization;

namespace KeyPadawan.ViewModel
{
    public class KeyLogModel : INotifyPropertyChanged
    {
        private List<Event> buffer;
        public IEnumerable<Event> Buffer
        {
            get { return buffer; }
        }

        private void AddToBuffer(Event evnt)
        {
            buffer.Add(evnt);
            OnPropertyChanged("Buffer");
        }

        public KeyLogModel()
        {
            buffer = new List<Event>();

            App.ki.KeyPress += OnKeyPressed;
            App.ki.KeyDown  += OnKeyDown;
            App.ki.KeyUp    += OnKeyUp;
        }

        private void OnKeyPressed(object sender, KeyPressEventArgs args)
        {
            if (CharIsPrintable(args.KeyChar))
            {
                var c = args.KeyChar;
                AddToBuffer(Event.FromChar(c));
            }
        }

        private static bool CharIsPrintable(char c)
        {
            var unicodeCategory = char.GetUnicodeCategory(c);
            if (((unicodeCategory == UnicodeCategory.Control) && (unicodeCategory != UnicodeCategory.Format)) && ((unicodeCategory != UnicodeCategory.LineSeparator) && (unicodeCategory != UnicodeCategory.ParagraphSeparator)))
            {
                return (unicodeCategory == UnicodeCategory.OtherNotAssigned);
            }
            return true;
        }

        private void OnKeyDown(object sender, KeyEventArgs args)
        {
            if(!CharIsPrintable((char)args.KeyValue))
            AddToBuffer(Event.FromKeys(args.KeyData));
        }

        private void OnKeyUp(object sender, KeyEventArgs args)
        {
        }

        #region INotifyPropertyChanged Members
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion 
    
        internal void CleanBuffer()
        {
            buffer.Clear();
            OnPropertyChanged("Buffer");
        }
    }

    public class Event
    {
        private Event(){}
        public static Event FromKeys(Keys k)
        {
            return new Event{ Keys = k, Char = '\0' };
        }

        public static Event FromChar(char c)
        {
            return new Event{ Keys = Keys.None, Char = c };
        }

        public bool IsChar { get { return Char != '\0'; } }
        public Keys Keys { get; private set; }
        public char Char { get; private set; }
    }
}
