using OnBrick.SourceCodeDictionary.Views;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Globalization;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace OnBrick.SourceCodeDictionary
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();

            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;

            this.Suspending += OnSuspending;
            this.UnhandledException += OnUnhandledException;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            AppShell shell = Window.Current.Content as AppShell ?? new AppShell();
            shell.Language = ApplicationLanguages.Languages[0];
            Window.Current.Content = shell;

            if (shell.AppFrame.Content == null)
            {
                shell.AppFrame.Navigate(typeof(MainPage), null, new SuppressNavigationTransitionInfo());
            }

            Window.Current.Activate();
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            deferral.Complete();
        }

        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            Debug.WriteLine(e.Message);

            e.Handled = true;
        }
    }
}
