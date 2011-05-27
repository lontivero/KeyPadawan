namespace KeyPadawan.View
{
    using System.Windows.Controls;
    using KeyPadawan.ViewModel;
    using KeyPadawan.Windows.Controls;
    using System.Windows.Forms;
    using System;
    using System.Windows;

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PadawanWindow : AutoHideWindow
    {
        private readonly KeyLogModel model;

        public PadawanWindow()
        {
            InitializeComponent();
            model = new KeyLogModel();
            this.DataContext = model;
            this.AfterFadeOut += OnAfterFadeOut;
            this.IsVisibleChanged += (s, e) => StopFadingOut();
        }

        private void OnAfterFadeOut(object sender, EventArgs args)
        {
            model.CleanBuffer();
        }

        private void textBlock_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            if(!string.IsNullOrEmpty(this.textBlock.Text))
                StopFadingOut();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Width = SystemParameters.PrimaryScreenWidth;
            Top = SystemParameters.PrimaryScreenHeight * 2 / 3;
        }
    }
}
