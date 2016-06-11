using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollon.Common
{
    public class SongMetadataEqualaty : IEqualityComparer<ISongData>
    {
        public bool Equals(ISongData x, ISongData y)
        {
            if (x == y)
                return true;
            if (x == null || y == null) // If both would be null see above.
                return false;

            return x.Album == y.Album
                && x.Artist == y.Artist
                && x.Duration == y.Duration
                && x.Title == y.Title
                && x.TrackNumber == y.TrackNumber;
        }

        public int GetHashCode(ISongData obj)
        {
            return unchecked(
                obj.Album.GetHashCode()
                * 397 ^ obj.Artist.GetHashCode()
                * 397 ^ obj.Duration.GetHashCode()
                * 397 ^ obj.Title.GetHashCode()
                * 397 ^ obj.TrackNumber.GetHashCode()
            );
        }
    }
}
