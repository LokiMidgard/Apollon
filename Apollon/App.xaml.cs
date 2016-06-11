using System;
using System.Reflection;
using System.Threading.Tasks;
using Apollon.Presentation.Music;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Metadata;
using Windows.Phone.UI.Input;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Apollon
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private const string SAVED_STATE = "SavedState";

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        public static CoreDispatcher Dispatcher { get; private set; }
        internal static Logic.MusicPlayer MusicPlayer => App.Current.Resources["musicPlayer"] as Logic.MusicPlayer;

        internal static Task<Presentation.Music.ProjectViewModel> CurrentProject { get; private set; }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            MusicPlayer.Init();

            Common.SongLookup.Configuration = new System.Composition.Hosting.ContainerConfiguration()
         .WithAssembly(this.GetType().GetTypeInfo().Assembly);
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // disabled, obscures the hamburger button, enable if you need it
                //this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            var shell = Window.Current.Content as Shell;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (shell == null)
            {
                // Create a Shell which navigates to the first page
                shell = new Shell();

                // hook-up shell root frame navigation events
                shell.RootFrame.NavigationFailed += OnNavigationFailed;
                shell.RootFrame.Navigated += OnNavigated;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {

                    //TODO: Load state from previously suspended application
                }

                var guid = Windows.Storage.ApplicationData.Current.LocalSettings.Values[SAVED_STATE] as Guid?;

                if (guid.HasValue)
                {
                    var task = Presentation.Music.ProjectViewModel.Load(guid.Value);
                    task = task.ContinueWith(x =>
                      {
                          if (x.IsFaulted)
                          {
                              Log(x.Exception);
                              return null;
                          }
                          return x.Result;
                      });
                    CurrentProject = task;
                }


                // set the Shell as content
                Window.Current.Content = shell;

                // listen for back button clicks (both soft- and hardware)
                SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

                if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                {
                    HardwareButtons.BackPressed += OnBackPressed;
                }

                UpdateBackButtonVisibility();
            }
            Dispatcher = Window.Current.Dispatcher;
            // Ensure the current window is active
            Window.Current.Activate();
        }

        // handle hardware back button press
        void OnBackPressed(object sender, BackPressedEventArgs e)
        {
            var shell = (Shell)Window.Current.Content;
            if (shell.RootFrame.CanGoBack)
            {
                e.Handled = true;
                shell.RootFrame.GoBack();
            }
        }

        // handle software back button press
        void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            var shell = (Shell)Window.Current.Content;
            if (shell.RootFrame.CanGoBack)
            {
                e.Handled = true;
                shell.RootFrame.GoBack();
            }
        }

        void OnNavigated(object sender, NavigationEventArgs e)
        {
            UpdateBackButtonVisibility();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        internal static void Log(Exception e)
        {
            System.Diagnostics.Debug.Write(e);
        }

        internal static void Log(string e)
        {
            System.Diagnostics.Debug.Write(e);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            if (CurrentProject != null)
            {
                var p = await CurrentProject;
                await (p?.Save() ?? Task.Delay(0));
                Windows.Storage.ApplicationData.Current.LocalSettings.Values[SAVED_STATE] = p?.Id;
            }
            deferral.Complete();
        }

        private void UpdateBackButtonVisibility()
        {
            var shell = (Shell)Window.Current.Content;

            var visibility = AppViewBackButtonVisibility.Collapsed;
            if (shell.RootFrame.CanGoBack)
            {
                visibility = AppViewBackButtonVisibility.Visible;
            }

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = visibility;
        }

        internal static void SetProject(ProjectViewModel projectViewModel)
        {
            var t = new TaskCompletionSource<ProjectViewModel>();
            t.SetResult(projectViewModel);
            CurrentProject = t.Task;
        }
    }
}
