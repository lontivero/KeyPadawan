namespace KeyPadawan.View
{
    using System.Windows.Controls;
    using KeyPadawan.ViewModel;
    using KeyPadawan.Windows.Controls;
    using System.Windows.Forms;
    using System;

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
            App.ki.KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs args)
        {
            this.StopFadingOut();
        }

        private void OnAfterFadeOut(object sender, EventArgs args)
        {
            model.CleanBuffer();
        }
    }
}
