using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

namespace KeyPadawan.ViewModel
{
    public class KeyLogModel : INotifyPropertyChanged
    {
        private string buffer;
        public string Buffer
        {
            get { return buffer; }
            set
            {
                buffer = value;
                Debug.Print("{0} {1}", value, buffer);
                OnPropertyChanged("Buffer");
            }
        }

        public KeyLogModel()
        {
            Buffer = string.Empty;

            App.ki.KeyPress += OnKeyPressed;
            App.ki.KeyDown  += OnKeyDown;
            App.ki.KeyUp    += OnKeyUp;
        }

        private void OnKeyPressed(object sender, KeyPressEventArgs args)
        {
            //if (args.KeyChar == (char)Keys.Enter) return;
            var c = args.KeyChar;

            switch ((Keys)c)
            {
                case Keys.Back:
                    RemoveLatestChar();
                    break;
                case Keys.Enter:
                    break;
            
                default:
                    Buffer += c;
                    break;
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs args)
        {
            //var kc = new KeysConverter();
            ////if(args.Alt || args.Control)
            ////{
            //    Buffer += kc.ConvertToString(args.KeyData);
            ////}
        }

        private void OnKeyUp(object sender, KeyEventArgs args)
        {
        }

        private void RemoveLatestChar()
        {
            if (!string.IsNullOrEmpty(Buffer))
            {
                Buffer = Buffer.Remove(Buffer.Length - 1);
            }
        }

        #region INotifyPropertyChanged Members
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion 
    }
}
