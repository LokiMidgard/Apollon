using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Apollon.Common.SongAnalizer
{
    public interface IMusicAnalize
    {
        string Name { get; }

        MusicAnalizerConfiguration PrototypeConfiguration { get; }

        Task<IEnumerable<Jump>> Analyze(Windows.Storage.StorageFile song, MusicAnalizerConfiguration configuration, IProgress<AnalizingProgess> p = null, CancellationToken canel = default(CancellationToken));

    }
}
