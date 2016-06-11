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
using System.Runtime.Serialization;
using Apollon.Common;
using System.ComponentModel;

namespace Apollon.Presentation.Music
{
    [DataContract(IsReference = true)]
    [PropertyChanged.ImplementPropertyChanged]
    class SongViewModel : INotifyPropertyChanged
    {

        public SongViewModel(Song song, ProjectViewModel project)
        {
            this.Song = song;
            this.Project = project;
            OnDeserialized(default(StreamingContext));
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext c)
        {
            AddJumpCommand = new RelayCommand(AddJump);
            RemoveJumpCommand = new RelayCommand(RemoveJump, CanRemoveJump);

            this.PropertyChanged += (sender, e) => Project.PrepareForWrite();
            this.Jumps.CollectionChanged += (sender, e) => Project.PrepareForWrite();
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        [DataMember]
        public Song Song { get; private set; }

        [DataMember]
        public ProjectViewModel Project { get; private set; }


        [DataMember]
        public ObservableCollection<JumpViewModel> Jumps => jumps ?? (jumps = new ObservableCollection<JumpViewModel>());
        private ObservableCollection<JumpViewModel> jumps;
        public ICommand AddJumpCommand { get; private set; }
        public ICommand RemoveJumpCommand { get; private set; }

        public int SelectedJump { get; set; } = -1;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName == nameof(SelectedJump))
                (RemoveJumpCommand as RelayCommand).OnCanExecuteChanged();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
