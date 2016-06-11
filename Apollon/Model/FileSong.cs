using Apollon.Common;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Apollon.Model
{
    [Equals]
    [DataContract]
    class FileSong : ISongData
    {
        private IStorageFile file;
        [DataMember]
        public string Album { get; }
        [DataMember]
        public string Artist { get; }
        [DataMember]
        public TimeSpan Duration { get; }
        [DataMember]
        public string Title { get; }
        [DataMember]
        public uint TrackNumber { get; }



        private FileSong(StorageFile file, string album, string artist, TimeSpan duration, string title, uint trackNumber)
        {
            this.file = file;
            this.Album = album;
            this.Artist = artist;
            this.Duration = duration;
            this.Title = title;
            this.TrackNumber = trackNumber;
        }

        public async Task<AudioFileInputNode> CreateNode(AudioGraph graph)
        {
            var result = await graph.CreateFileInputNodeAsync(file);
            if (result.Status != AudioFileNodeCreationStatus.Success)
                throw new Exception("Faild To Create InputNode");
            return result.FileInputNode;
        }

        public static async Task<FileSong> Create(StorageFile file)
        {
            var musicPropertys = await file.Properties.GetMusicPropertiesAsync();
            var album = musicPropertys.Album;
            var artist = musicPropertys.Artist;
            var duration = musicPropertys.Duration;
            var title = musicPropertys.Title;
            var trackNumber = musicPropertys.TrackNumber;
            return new FileSong(file, album, artist, duration, title, trackNumber);
        }

        [Export(typeof(ISongLookUp))]
        private class Poplator : ISongLookUp
        {
            private Dictionary<ISongData, FileSong> lookup;
            TaskCompletionSource<object> waiter;
            private async Task Init()
            {
                if (waiter != null)
                {
                    await waiter.Task;
                    return;
                }
                waiter = new TaskCompletionSource<object>();

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

                var songs = await Task.WhenAll(mp3List
                                            .AsParallel()
                                            .Select(Model.FileSong.Create));
                this.lookup = songs.ToDictionary(x => x, x => x, new SongMetadataEqualaty());

                this.waiter.SetResult(lookup);
            }




            public async Task<bool> UpdateSong(Song song)
            {
                if (song.IsReady)
                    return true;
                await Init();
                if (lookup.ContainsKey(song))
                {
                    song.Update(lookup[song]);
                    return true;
                }

                return false;
            }

        }
    }
}
