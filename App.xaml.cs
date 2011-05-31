namespace KeyPadawan
{
    using System.Windows;
    using KeyPadawan.Windows.Controls;
    using System.Reflection;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static internal KeyboardInterceptor ki;
        private System.Windows.Forms.NotifyIcon _notifyIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            SetupTrayIcon();
            ki = new KeyboardInterceptor();
        }

        private void SetupTrayIcon()
        {
            var menu = new System.Windows.Forms.ContextMenu(
                new[]{
                    new System.Windows.Forms.MenuItem
                    {
                        Text = Version,
                        Enabled = false,
                    },
                    new System.Windows.Forms.MenuItem{
                        Text = "-",
                    },
                    new System.Windows.Forms.MenuItem("Exit", (s,a)=>MainWindow.Close())
                    // TODO: always visible, filters, dock/undoc
                });

            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            _notifyIcon.Icon = KeyPadawan.Properties.Resources.keyboard_pencil;
            _notifyIcon.BalloonTipTitle = "KeyPadawan";
            _notifyIcon.BalloonTipText = "xxxxxxxxx";
            _notifyIcon.Text = "KeyPadawan";
            _notifyIcon.Visible = true;
            _notifyIcon.ContextMenu = menu;
            _notifyIcon.DoubleClick += (sender, args) => 
                ((AutoHideWindow)MainWindow).StopFadingOut();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ki.Dispose();
            _notifyIcon.Visible = false;
        }

        public string Version
        {
            get
            {
                return "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
    }
}
