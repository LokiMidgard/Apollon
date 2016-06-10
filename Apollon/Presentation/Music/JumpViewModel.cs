using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollon.Presentation.Music
{
    class JumpViewModel
    {

        public JumpViewModel(SongViewModel song)
        {
            Song = song;
        }

        public string Name { get; set; }

        public SongViewModel Song { get; }

        /// <summary>
        /// The Position at when we initiate the jump
        /// </summary>
        public TimeSpan Origin { get; set; }

        /// <summary>
        /// The Time to when we jump.
        /// </summary>
        public TimeSpan TargetTime { get; set; }
        /// <summary>
        /// The Song to where we jump.
        /// </summary>
        public SongViewModel TargetSong { get; set; }
        /// <summary>
        /// The Time before we hit origin where we start fading in the other Sound
        /// </summary>
        public TimeSpan CrossFade { get; set; }

        public JumpViewModel NextDefaultJump { get; set; }
    }
}
