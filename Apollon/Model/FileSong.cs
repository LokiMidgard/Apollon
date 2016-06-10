using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Apollon.Model
{
    class FileSong : ISong
    {
        public readonly IStorageFile file;
        public string Album { get; }
        public string Artist { get; }
        public TimeSpan Duration { get; }
        public string Title { get; }
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
    }
}
