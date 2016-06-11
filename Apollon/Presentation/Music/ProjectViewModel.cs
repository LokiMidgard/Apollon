using Intense.Presentation;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.Storage.Pickers;
using System.ComponentModel;

namespace Apollon.Presentation.Music
{
    [DataContract(IsReference = true)]
    [PropertyChanged.ImplementPropertyChanged]
    class ProjectViewModel : INotifyPropertyChanged
    {
        private const int WRITE_DELAY = 5000;

        [DataMember]
        public ObservableCollection<SongViewModel> Songs => songs ?? (songs = new ObservableCollection<SongViewModel>());
        private ObservableCollection<SongViewModel> songs;

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Guid Id { get; private set; } = Guid.NewGuid();
        public ICommand ImportSongCommand { get; private set; }
        private static DataContractSerializer Serelizer => new DataContractSerializer(typeof(ProjectViewModel), new DataContractSerializerSettings()
        {
            SerializeReadOnlyTypes = true,
            PreserveObjectReferences = true,

        });

        public ProjectViewModel()
        {
            OnDeserialized(default(StreamingContext));
        }


        [OnDeserialized]
        void OnDeserialized(StreamingContext c)
        {
            this.ImportSongCommand = new RelayCommand(() => ImportSong());
            this.PropertyChanged += (sender, e) => PrepareForWrite();
            this.Songs.CollectionChanged += (sender, e) => PrepareForWrite();
        }

        private async void ImportSong()
        {

            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".mp3");
            picker.SuggestedStartLocation = PickerLocationId.MusicLibrary;

            var files = await picker.PickMultipleFilesAsync();

            foreach (var file in files)
            {
                try
                {
                    Model.FileSong fileSong = await Model.FileSong.Create(file);
                    var song = new SongViewModel(new Common.Song(fileSong), this);
                    this.Songs.Add(song);
                }
                catch (Exception e)
                {
                    App.Log(e);
                }
            }
        }

        private Task prepareForWriteWaiter;

        public event PropertyChangedEventHandler PropertyChanged;

        public async void PrepareForWrite()
        {
            await App.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                var localTask = Task.Delay(WRITE_DELAY);
                prepareForWriteWaiter = localTask;
                if (Object.ReferenceEquals(prepareForWriteWaiter, localTask))
                {
                    await Save();
                }
            });
        }

        public async Task Save()
        {
            prepareForWriteWaiter = null;
            var file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync($"{Id}.appolonProject", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            using (var stream = await file.OpenTransactedWriteAsync(Windows.Storage.StorageOpenOptions.None))
            {
                Serelizer.WriteObject(stream.Stream.AsStreamForWrite(), this);
                await stream.CommitAsync();
            }
        }

        public static async Task<Guid[]> GetIds()
        {
            var files = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFilesAsync();
            return files
                .Where(x => x.FileType == ".appolonProject")
                .Select(x =>
                   {
                       Guid g;
                       if (!Guid.TryParse(x.DisplayName, out g))
                           return null;
                       return g as Guid?;
                   })
               .Where(x => x.HasValue)
               .Select(x => x.Value)
               .ToArray();
        }
        public static async Task<ProjectViewModel> Load(Guid id)
        {
            var file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync($"{id}.appolonProject");
            using (var stream = await file.OpenReadAsync())
            {
                return (ProjectViewModel)Serelizer.ReadObject(stream.AsStreamForRead());
            }

        }
    }
}
