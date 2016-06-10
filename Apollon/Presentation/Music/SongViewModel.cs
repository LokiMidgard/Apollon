using Apollon.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollon.Presentation.Music
{
    class SongViewModel
    {

        private SongViewModel(FileSong song, TimeSpan duration)
        {
            this.Song = song;
            this.Duration = duration;
        }

        public String Title { get; }

        public string Album { get; }

        public ISong Song { get; }

        public TimeSpan Duration { get; }

        public TimeSpan CurrentPosition { get; set; }

        public ObservableCollection<JumpViewModel> Jumps { get; } = new ObservableCollection<JumpViewModel>();

        public static async Task<SongViewModel> Create(FileSong song)
        {

            using (var graph = (await Windows.Media.Audio.AudioGraph.CreateAsync(new Windows.Media.Audio.AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Other))).Graph)
            using (var node = (await song.CreateNode(graph)))
                return new SongViewModel(song, node.Duration);

        }
    }
}
