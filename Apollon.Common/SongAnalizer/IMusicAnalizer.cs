using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollon.Common.SongAnalizer
{
    public interface IMusicAnalizer
    {
        string Name { get; }

        MusicAnalizerConfiguration PrototypeConfiguration { get; }

        Task<IEnumerable<Jump>> Analyze(Song song, MusicAnalizerConfiguration configuration);

    }
}
