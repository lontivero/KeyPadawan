namespace KeyPadawan
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static internal KeyboardInterceptor ki;

        protected override void OnStartup(StartupEventArgs e)
        {
            ki = new KeyboardInterceptor();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ki.Dispose();
        }
    }
}
