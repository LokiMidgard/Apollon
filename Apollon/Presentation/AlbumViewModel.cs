using System.Linq;
using Apollon.Presentation.Music;
using System.Collections.ObjectModel;

namespace Apollon.Presentation
{
    internal class AlbumViewModel
    {
        public string Title { get; set; }
        public ObservableCollection<SongViewModel> Songs { get; } = new ObservableCollection<SongViewModel>();

        public AlbumViewModel(IGrouping<string, SongViewModel> album)
        {
            this.Title = album.Key;
            foreach (var song in album)
                this.Songs.Add(song);
        }
    }
}