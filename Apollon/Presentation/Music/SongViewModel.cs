using Apollon.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intense.Presentation;
using System.Windows.Input;
using System.Runtime.CompilerServices;

namespace Apollon.Presentation.Music
{
    [PropertyChanged.ImplementPropertyChanged]
    class SongViewModel : NotifyPropertyChanged
    {

        public SongViewModel(FileSong song)
        {
            this.Song = song;

            AddJumpCommand = new RelayCommand(AddJump);
            RemoveJumpCommand = new RelayCommand(RemoveJump, CanRemoveJump);

        }

        private bool CanRemoveJump()
            => SelectedJump >= 0
            && SelectedJump < Jumps.Count;

        private void RemoveJump()
        {
            Jumps.RemoveAt(SelectedJump);
            SelectedJump = -1;
        }

        private void AddJump()
        {
            Jumps.Add(new JumpViewModel(this));
            SelectedJump = Jumps.Count - 1;
        }

        public ISong Song { get; }


        public ObservableCollection<JumpViewModel> Jumps { get; } = new ObservableCollection<JumpViewModel>();
        public ICommand AddJumpCommand { get; }
        public ICommand RemoveJumpCommand { get; }

        public int SelectedJump { get; set; } = -1;

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName == nameof(SelectedJump))
                (RemoveJumpCommand as RelayCommand).OnCanExecuteChanged();
            base.OnPropertyChanged(propertyName);
        }
    }
}
