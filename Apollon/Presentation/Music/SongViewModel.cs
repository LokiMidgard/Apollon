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

        public SongViewModel(FileSong song)
        {
            this.Song = song;
        }

        public String Title { get; }

        public string Album { get; }

        public ISong Song { get; }

        public TimeSpan Duration { get; }

        public TimeSpan CurrentPosition { get; set; }

        public ObservableCollection<JumpViewModel> Jumps { get;  } = new ObservableCollection<JumpViewModel>();
    }
}
