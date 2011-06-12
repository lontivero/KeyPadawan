namespace KeyPadawan.View
{
    using System.Windows.Controls;
    using KeyPadawan.ViewModel;
    using KeyPadawan.Windows.Controls;
    using System.Windows.Forms;
    using System;
    using System.Windows;
    using System.Windows.Interop;
    using System.Runtime.InteropServices;

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
            this.MouseDoubleClick += PadawanWindow_MouseDoubleClick;
        }

        void PadawanWindow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Width = SystemParameters.PrimaryScreenWidth;
            Top = SystemParameters.PrimaryScreenHeight * 2 / 3;
            Left = 0;
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

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            KeyPadawan.Properties.Settings.Default.Save(); 
            base.OnClosing(e); 
        }
    }
}
