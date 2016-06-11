using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Apollon.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page2 : Page
    {
        public Page2()
        {
            this.InitializeComponent();
            Init();
        }

        private async void Init()
        {
            var projectViewModel = await (App.CurrentProject ?? Task.FromResult<Presentation.Music.ProjectViewModel>(null));
            if(projectViewModel==null)
            {
                projectViewModel = new Presentation.Music.ProjectViewModel();
                App.SetProject(projectViewModel);
            }

            this.DataContext = projectViewModel;
        }


        private async void PlayClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var s = sender as FrameworkElement;
            var song = s?.DataContext as Presentation.Music.SongViewModel;
            if (song != null)
            {
                await App.MusicPlayer.Play(song);
            }
        }

        private void EditJump(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            Frame.Navigate(typeof(Pages.JumpEditor), frameworkElement.DataContext);
        }
    }
}
