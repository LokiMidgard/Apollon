using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intense.Presentation;
using System.Windows.Input;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Apollon.Presentation.Music
{
    [DataContract(IsReference = true)]
    [PropertyChanged.ImplementPropertyChanged]
    class JumpViewModel : INotifyPropertyChanged
    {

        public JumpViewModel(SongViewModel song) : this()
        {
            Song = song;
        }

        private JumpViewModel()
        {
            TestCommand = new RelayCommand(Test);
            this.PropertyChanged += (sender, e) => Song.Project.PrepareForWrite();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void Test()
        {
            await App.MusicPlayer.Play(Song);
            App.MusicPlayer.NextJump = this;
            var seekPosition = this.Origin - this.CrossFade - TimeSpan.FromSeconds(1);
            if (seekPosition < TimeSpan.Zero)
                seekPosition = TimeSpan.Zero;
            App.MusicPlayer.Seek(seekPosition);
            var millisecondsToWait = (int)this.CrossFade.Add(TimeSpan.FromSeconds(2)).TotalMilliseconds;
            await Task.Delay(millisecondsToWait);
            App.MusicPlayer.Pause();
        }

        public string Name { get; set; }

        [DataMember]
        public SongViewModel Song { get; }

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
        public SongViewModel TargetSong { get; set; }
        /// <summary>
        /// The Time before we hit origin where we start fading in the other Sound
        /// </summary>
        [DataMember]
        public TimeSpan CrossFade { get; set; }

        [DataMember]
        public JumpViewModel NextDefaultJump { get; set; }
        public ICommand TestCommand { get; }
    }
}
