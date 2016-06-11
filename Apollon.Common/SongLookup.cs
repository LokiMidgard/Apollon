using System.Reflection;
using Apollon.Common;
using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollon.Common
{

    public static class SongLookup
    {
        public static ContainerConfiguration Configuration { get; set; }

        public static async Task<bool> PrepareSong(Song song)
        {
            using (var container = Configuration.CreateContainer())
            {
                var lookups = container.GetExports<Common.ISongLookUp>();
                foreach (var lookup in lookups)
                {
                    if (await lookup.UpdateSong(song))
                        return true;
                }
                return false;
            }
        }
    }
}
