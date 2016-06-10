using System;
using System.Threading.Tasks;
using Windows.Media.Audio;

namespace Apollon.Model
{
    interface ISong
    {
        Task<AudioFileInputNode> CreateNode(AudioGraph graph);

        string Album { get; }
        string Artist { get; }
        TimeSpan Duration { get; }
        string Title { get; }
        uint TrackNumber { get; }

    }
}