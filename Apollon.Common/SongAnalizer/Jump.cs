using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Apollon.Common
{
    public class Jump
    {
        /// <summary>
        /// The Position at when we initiate the jump
        /// </summary>
        [DataMember]
        public TimeSpan Origin { get; set; }

        /// <summary>
        /// The Time to when we jump.
        /// </summary>
        [DataMember]
        public TimeSpan TargetTime { get; set; }
        /// <summary>
        /// The Song to where we jump.
        /// </summary>
        [DataMember]
        public Windows.Storage.StorageFile TargetSong { get; set; }
        /// <summary>
        /// The Time before we hit origin where we start fading in the other Sound
        /// </summary>
        [DataMember]
        public TimeSpan CrossFade { get; set; }

    }
}
