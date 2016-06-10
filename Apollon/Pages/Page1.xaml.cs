using System;
using Windows.UI.Xaml.Controls;

namespace Apollon.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page1 : Page
    {
        public Page1()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var path = @"C:\Users\Patrick\OneDrive\Musik\Sinsekai [OST]\01. 陰の伝承歌 第一部.mp3";

            var storageFile = await Windows.Storage.KnownFolders.MusicLibrary.GetFileAsync(@"Sinsekai [OST]\01. 陰の伝承歌 第一部.mp3");

            var song = new Model.FileSong(storageFile);

            var vm = new Presentation.Music.SongViewModel(song);

            vm.Jumps.Add(new Presentation.Music.JumpViewModel(vm)
            {
                CrossFade = TimeSpan.FromSeconds(5),
                Origin = TimeSpan.FromSeconds(30),
                TargetTime = TimeSpan.FromSeconds(10),
                TargetSong = vm,
            });

            var player = new Logic.MusicPlayer();
            this.DataContext = player;
            await player.Init();
            await player.Play(vm);
        }
    }
}
