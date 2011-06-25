namespace KeyPadawan
{
    using System.Windows;
    using KeyPadawan.Windows.Controls;
    using System.Reflection;
    using System.Windows.Forms;
    using KeyPadawan.Properties;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private System.Windows.Forms.NotifyIcon _notifyIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            SetupTrayIcon();
        }

        private MenuItem mnuShortcutsMode;
        private MenuItem mnuRawMode;
        private MenuItem mnuNormalMode;

        private void SetupTrayIcon()
        {
            mnuShortcutsMode = new System.Windows.Forms.MenuItem("Shortcuts mode");
            mnuShortcutsMode.Name = "mnuShortcutsMode";
            mnuShortcutsMode.RadioCheck = true;
            mnuShortcutsMode.Checked = KeyPadawan.Properties.Settings.Default.ShortcutsMode;
            mnuShortcutsMode.Click += mnuProcessingChanged;
            
            mnuRawMode = new System.Windows.Forms.MenuItem("Raw mode");
            mnuRawMode.Name = "mnuRawMode";
            mnuRawMode.RadioCheck = true; 
            mnuRawMode.Checked = KeyPadawan.Properties.Settings.Default.RawMode;
            mnuRawMode.Click += mnuProcessingChanged;

            mnuNormalMode = new System.Windows.Forms.MenuItem("Normal mode");
            mnuNormalMode.Name = "mnuNormalMode";
            mnuNormalMode.RadioCheck = true;
            mnuNormalMode.Checked = KeyPadawan.Properties.Settings.Default.NormalMode;
            mnuNormalMode.Click += mnuProcessingChanged;

            var menu = new System.Windows.Forms.ContextMenu(
                new [] {
                    mnuNormalMode,
                    mnuRawMode,
                    mnuShortcutsMode,
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

            Settings.Default.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Default_PropertyChanged);
        }

        void Default_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            mnuRawMode.Checked = Settings.Default.RawMode;
            mnuShortcutsMode.Checked = Settings.Default.ShortcutsMode;
            mnuNormalMode.Checked = Settings.Default.NormalMode;
        }

        void mnuProcessingChanged(object sender, System.EventArgs e)
        {
            var mnu = sender as System.Windows.Forms.MenuItem;
            if (mnu.Name == "mnuShortcutsMode")
            {
                Settings.Default.ShortcutsMode = true;
                Settings.Default.RawMode = false;
                Settings.Default.NormalMode = false;
            }
            else if (mnu.Name == "mnuRawMode")
            {
                Settings.Default.ShortcutsMode = false;
                Settings.Default.RawMode = true;
                Settings.Default.NormalMode = false;
            }
            else if (mnu.Name == "mnuNormalMode")
            {
                Settings.Default.ShortcutsMode = false;
                Settings.Default.RawMode = false;
                Settings.Default.NormalMode = true;
            }

            Settings.Default.Save();
        }

        protected override void OnExit(ExitEventArgs e)
        {
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
