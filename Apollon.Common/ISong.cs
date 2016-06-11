using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Media.Audio;

namespace Apollon.Common
{
    public interface ISongData
    {
        Task<AudioFileInputNode> CreateNode(AudioGraph graph);

        string Album { get; }
        string Artist { get; }
        TimeSpan Duration { get; }
        string Title { get; }
        uint TrackNumber { get; }
    }
}