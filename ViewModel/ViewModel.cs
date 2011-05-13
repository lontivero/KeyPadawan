using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

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
            Buffer += args.KeyChar;
        }

        private void OnKeyDown(object sender, KeyEventArgs args)
        {
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
