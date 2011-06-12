using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using KeyPadawan.Model;

namespace KeyPadawan.ViewModel
{
    public class KeyLogModel : INotifyPropertyChanged
    {
        private List<Event> _buffer = new List<Event>();
        private IKeyboardInterceptor _interceptor;

        internal KeyLogModel()
            : this (new KeyboardInterceptor())
        {
        }

        internal KeyLogModel(IKeyboardInterceptor interceptor)
        {
            _interceptor = interceptor;

            _interceptor.KeyPress += OnKeyPressed;
            _interceptor.KeyDown += OnKeyDown;
            _interceptor.KeyUp += OnKeyUp;
        }

        public Event[] Buffer
        {
            get { return _buffer.ToArray(); }
        }

        private void AddToBuffer(Event evnt)
        {
            _buffer.Add(evnt);
            OnPropertyChanged("Buffer");
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
            var special = new [] {
                Keys.Escape, Keys.Enter, 
                Keys.Insert, Keys.Delete,
                Keys.PageUp , Keys.PageDown,
                Keys.Pause , Keys.Play , Keys.PrintScreen,
                Keys.Up, Keys.Down , Keys.Left , Keys.Right,
                Keys.Tab,
                Keys.Home, Keys.End
            };

            var isSpecial = special.Contains(args.KeyCode);

            if (args.Control || args.Alt || isSpecial || args.KeyData >= Keys.F1 && args.KeyData <= Keys.F24)
            {
                AddToBuffer(Event.FromKeys(args.KeyData));
            }
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
            _buffer.Clear();
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
