using System;

namespace Apollon.Common.SongAnalizer
{
    public struct AnalizingProgess : IComparable<AnalizingProgess>, IComparable
    {
        public AnalizingProgess(double init, double analizing, double post)
        {
            Initilize = init;
            Analize = analizing;
            PostProcessing = post;
        }

        public double Initilize { get; }
        public double Analize { get; }
        public double PostProcessing { get; }

        public int CompareTo(AnalizingProgess other)
        {
            var erg = this.Initilize.CompareTo(other.Initilize);
            if (erg != 0)
                return erg;

            erg = this.Analize.CompareTo(other.Analize);
            if (erg != 0)
                return erg;
           
            return this.PostProcessing.CompareTo(other.PostProcessing);
        }

        public int CompareTo(object obj)
        {
            if (obj is AnalizingProgess)
                return CompareTo((AnalizingProgess)obj);
            throw new ArgumentException("Cant compare this Type", nameof(obj));
        }

        public static bool operator <(AnalizingProgess p1, AnalizingProgess p2)
            => p1.CompareTo(p2) < 0;

        public static bool operator >(AnalizingProgess p1, AnalizingProgess p2)
            => p1.CompareTo(p2) > 0;

        public static bool operator ==(AnalizingProgess p1, AnalizingProgess p2)
            => p1.Initilize == p2.Initilize
            && p1.Analize == p2.Analize
            && p1.PostProcessing == p2.PostProcessing;
        public static bool operator !=(AnalizingProgess p1, AnalizingProgess p2)
            => !(p1 == p2);
    }
}