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
        private List<KeyboardEvent> _buffer = new List<KeyboardEvent>();
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

        public KeyboardEvent[] Buffer
        {
            get { return _buffer.ToArray(); }
        }

        private void AddToBuffer(KeyboardEvent evnt)
        {
            _buffer.Add(evnt);
            OnPropertyChanged("Buffer");
        }

        private void OnKeyPressed(object sender, KeyPressEventArgs args)
        {
            if (CharIsPrintable(args.KeyChar) || args.KeyChar == '\b')
            {
                var c = args.KeyChar;
                AddToBuffer(KeyboardEvent.FromChar(c));
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
            var k = args.KeyCode;
            var isPrintableKey =
                (k >= Keys.D0 && k <= Keys.D9) ||
                (k >= Keys.A && k <= Keys.Z) ||
                (k >= Keys.NumPad0 && k <= Keys.NumPad9)
                || k == Keys.Space
                || k == Keys.Multiply
                || k == Keys.Add
                || k == Keys.Separator
                || k == Keys.Subtract
                || k == Keys.Decimal
                || k == Keys.Divide
                || k == Keys.OemSemicolon
                || k == Keys.Oemplus
                || k == Keys.Oemcomma
                || k == Keys.OemMinus
                || k == Keys.OemPeriod
                || k == Keys.OemQuestion
                || k == Keys.Oemtilde
                || k == Keys.OemOpenBrackets
                || k == Keys.OemPipe
                || k == Keys.OemCloseBrackets
                || k == Keys.OemQuotes
                || k == Keys.OemBackslash
                || k == Keys.OemClear;

            if (!isPrintableKey || args.Alt || args.Control)
            {
                AddToBuffer(KeyboardEvent.FromKeys(args.KeyData));
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

    public class KeyboardEvent
    {
        private KeyboardEvent(){}
        public static KeyboardEvent FromKeys(Keys keys)
        {
            return new KeyboardEvent{ Keys = keys, Char = '\0' };
        }

        public static KeyboardEvent FromChar(char character)
        {
            return new KeyboardEvent{ Keys = Keys.None, Char = character };
        }

        public bool IsChar { get { return Char != '\0'; } }
        public Keys Keys { get; private set; }
        public char Char { get; private set; }
    }
}
