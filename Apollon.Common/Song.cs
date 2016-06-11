using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Audio;

namespace Apollon.Common
{
    [DataContract]
    public class Song : ISongData
    {

        public Song()
        {

        }

        public Song(ISongData data)
        {
            Update(data);
        }

        public ISongData Data { get; private set; }

        [DataMember]
        public string Album { get; private set; }
        [DataMember]
        public string Artist { get; private set; }

        [DataMember]
        public TimeSpan Duration { get; private set; }

        [DataMember]
        public string Title { get; private set; }

        [DataMember]
        public uint TrackNumber { get; private set; }

        public bool IsReady => this.Data != null;

        public void Update(ISongData data)
        {
            this.Data = data;
            this.Album = data.Album;
            this.Artist = data.Artist;
            this.Duration = data.Duration;
            this.Title = data.Title;
            this.TrackNumber = data.TrackNumber;
        }
        public async Task<AudioFileInputNode> CreateNode(AudioGraph graph)
        {
            if (IsReady)
                return await Data.CreateNode(graph);
            if (await SongLookup.PrepareSong(this))
                return await Data.CreateNode(graph);
            throw new Exception("Could Not find SongData.");
        }
    }
}
