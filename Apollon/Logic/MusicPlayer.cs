using Apollon.Presentation.Music;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Audio;

namespace Apollon.Logic
{
    [PropertyChanged.ImplementPropertyChanged]
    class MusicPlayer : INotifyPropertyChanged
    {
        private AudioGraph graph;
        private AudioDeviceOutputNode outputNode;
        private AudioFileInputNode mainInputNode;
        private SongViewModel mainSong { get; set; }
        private AudioFileInputNode subInputNode;
        private SongViewModel subSong;

        public event PropertyChangedEventHandler PropertyChanged;

        public TimeSpan Position { get; private set; }

        public SongViewModel PlayingSong => mainSong;

        public JumpViewModel NextJump { get; set; }

        public bool Fading { get; private set; }
        public double SubGain { get; private set; }
        public double MainGain { get; private set; }

        public async Task Init()
        {
            var graphResult = await Windows.Media.Audio.AudioGraph.CreateAsync(new Windows.Media.Audio.AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media) { });
            if (graphResult.Status != Windows.Media.Audio.AudioGraphCreationStatus.Success)
                throw new Exception();

            graph = graphResult.Graph;



            var outPutResult = await graph.CreateDeviceOutputNodeAsync();
            if (outPutResult.Status != Windows.Media.Audio.AudioDeviceNodeCreationStatus.Success)
                throw new Exception();

            outputNode = outPutResult.DeviceOutputNode;


            graph.QuantumProcessed += MainGraph_QuantumProcessed;
        }

        private async void MainGraph_QuantumProcessed(AudioGraph sender, object args)
        {
            this.Position = mainInputNode.Position;

            if (NextJump != null && NextJump.Song == mainSong)
            {

                if (Position >= NextJump.Origin - NextJump.CrossFade
                    && Position <= NextJump.Origin
                    && !Fading)
                {
                    Fading = true;
                    subSong = NextJump.TargetSong;
                    subInputNode = await subSong.Song.CreateNode(graph);
                    subInputNode.AddOutgoingConnection(outputNode);

                    subInputNode.StartTime = NextJump.TargetTime - (NextJump.Origin - mainInputNode.Position);
                    subInputNode.OutgoingGain = 0.00000000001;
                    subInputNode.Start();
                }
                if (Fading && subInputNode != null)
                {
                    var fadePosition = (NextJump.Origin - mainInputNode.Position);
                    var fadeTime = NextJump.CrossFade;

                    var percentage = Math.Min(1.0, Math.Max(0.0, fadePosition.TotalSeconds / fadeTime.TotalSeconds));

                    this.SubGain = subInputNode.OutgoingGain = 1.0-percentage;
                    this.MainGain = mainInputNode.OutgoingGain = percentage;

                }
                if (Position > NextJump.Origin
                    && Fading && subInputNode != null)
                {
                    subInputNode.OutgoingGain = 1.0;
                    mainInputNode.Stop();
                    mainInputNode.RemoveOutgoingConnection(outputNode);
                    mainInputNode.Dispose();
                    mainInputNode = subInputNode;
                    subInputNode = null;
                    mainSong = subSong;
                    subSong = null;
                    Fading = false;
                }

            }

        }

        public async Task Play(SongViewModel song)
        {

            mainInputNode = await song.Song.CreateNode(graph);
            mainSong = song;
            NextJump = mainSong.Jumps.FirstOrDefault();
            mainInputNode.AddOutgoingConnection(outputNode);

            graph.Start();
        }

        private async void OnPropertyChanged(string caller)
        {
            await App.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller)));
        }
    }
}
