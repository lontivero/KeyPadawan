namespace KeyPadawan.View
{
    using System.Windows.Controls;
    using KeyPadawan.ViewModel;
    using KeyPadawan.Windows.Controls;

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PadawanWindow : AutoHideWindow
    {
        public PadawanWindow()
        {
            this.DataContext = new KeyLogModel();
            InitializeComponent();
        }
    }
}
