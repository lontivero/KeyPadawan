using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

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
            App.ki.KeyIntercepted += new KeyboardInterceptor.KeyboardEventHandler(this.OnKeyPressed);
        }

        private void OnKeyPressed(KeyboardInterceptor.KeyboardEventArgs args)
        {
            Buffer += args.Ascii;
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
