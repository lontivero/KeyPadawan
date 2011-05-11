using System.Windows.Controls;
using System;
using System.Windows.Threading;
using System.Windows;
using System.Diagnostics;

namespace KeyPadawan.UI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PadawanWindow : TransparentWindow
    {
        public PadawanWindow()
        {
            this.DataContext = new ViewModel();
            InitializeComponent();
        }
    }
}
