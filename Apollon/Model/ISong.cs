using System.Threading.Tasks;
using Windows.Media.Audio;

namespace Apollon.Model
{
    interface ISong
    {
        Task<AudioFileInputNode> CreateNode(AudioGraph graph);
    }
}