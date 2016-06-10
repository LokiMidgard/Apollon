using System;
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
    }
}
