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
                OnPropertyChanged("Buffer");
            }
        }

        public KeyLogModel()
        {
            App.ki.KeyPress += OnKeyPressed;
            App.ki.KeyDown  += OnKeyDown;
            App.ki.KeyUp    += OnKeyUp;
        }

        private void OnKeyPressed(object sender, KeyPressEventArgs args)
        {
            if (args.KeyChar == (char)Keys.Enter || args.KeyChar == (char)Keys.Back) return;
            var c = args.KeyChar;

            Buffer += c;
        }

        private void OnKeyDown(object sender, KeyEventArgs args)
        {
            if (args.KeyValue == (int)Keys.Back)
            {
                if (!string.IsNullOrEmpty(Buffer))
                {
                    Buffer = Buffer.Substring(0, Buffer.Length - 1);
                }
            }
            Debug.Print(args.KeyCode.ToString());
            //else
            //{
            //    if (args.Alt || args.Control)
            //    {
            //        Buffer += new KeysConverter().ConvertTo(args.KeyData, typeof(string));
            //    }
            //}
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
    }
}
