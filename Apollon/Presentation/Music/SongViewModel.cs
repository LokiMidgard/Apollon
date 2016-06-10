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


        public ISong Song { get; }


        public ObservableCollection<JumpViewModel> Jumps { get; } = new ObservableCollection<JumpViewModel>();
    }
}
