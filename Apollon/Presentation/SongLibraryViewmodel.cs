using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollon.Presentation
{
    [PropertyChanged.ImplementPropertyChanged]
    class SongLibraryViewmodel
    {

        public ObservableCollection<AlbumViewModel> Albums { get; } = new ObservableCollection<AlbumViewModel>();

        public SongLibraryViewmodel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                Init();
        }

        private async void Init()
        {
            var musicLib = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Music);

            var folderQueue = new Queue<Windows.Storage.StorageFolder>(musicLib.Folders);
            var mp3List = new List<Windows.Storage.StorageFile>();
            while (folderQueue.Any())
            {
                var f = folderQueue.Dequeue();
                foreach (var subFolder in await f.GetFoldersAsync())
                    folderQueue.Enqueue(subFolder);

                foreach (var file in await f.GetFilesAsync())
                {
                    if (file.FileType != ".mp3")
                        continue;
                    mp3List.Add(file);
                }
            }

            var songviewmodels = await Task.WhenAll(mp3List
                                        .AsParallel()
                                        .Select(Model.FileSong.Create)
                                        .Select(async x => new Music.SongViewModel(await x)));
            var albumGroups = songviewmodels.GroupBy(x => x.Song.Album);

            foreach (var album in albumGroups.OrderBy(x => x.Key))
                this.Albums.Add(new AlbumViewModel(album));
        }
    }
}
