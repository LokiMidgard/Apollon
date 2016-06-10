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
    class MusicPlayer : INotifyPropertyChanged, IDisposable
    {
        private AudioGraph graph;
        private AudioDeviceOutputNode outputNode;
        private AudioFileInputNode mainInputNode;
        private SongViewModel mainSong { get; set; } // Property so PropertyChanged.Fody can do its stuff.
        private AudioFileInputNode subInputNode;
        private SongViewModel subSong;

        public event PropertyChangedEventHandler PropertyChanged;


        public TimeSpan? Position { get; private set; }

        public SongViewModel PlayingSong => mainSong;

        public TimeSpan? Duration => PlayingSong?.Song.Duration;

        public JumpViewModel NextJump { get; set; }

        public bool IsPlaying { get; private set; }
        public bool IsFading { get; private set; }

        public async Task Init()
        {
            var graphResult = await Windows.Media.Audio.AudioGraph.CreateAsync(new Windows.Media.Audio.AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media) { });
            if (graphResult.Status != Windows.Media.Audio.AudioGraphCreationStatus.Success)
                throw new Exception("Faild to Create Audio Graph");

            graph = graphResult.Graph;



            var outPutResult = await graph.CreateDeviceOutputNodeAsync();
            if (outPutResult.Status != Windows.Media.Audio.AudioDeviceNodeCreationStatus.Success)
                throw new Exception("Faild To Create DeviceOutput");

            outputNode = outPutResult.DeviceOutputNode;


            graph.QuantumProcessed += MainGraph_QuantumProcessed;
        }

        private async void MainGraph_QuantumProcessed(AudioGraph sender, object args)
        {
            try
            {
                this.Position = mainInputNode.Position;

                if (NextJump != null && NextJump.Song == mainSong)
                {

                    if (NextJump != null
                        && Position >= NextJump.Origin - NextJump.CrossFade
                        && Position <= NextJump.Origin
                        && !IsFading)
                    {
                        IsFading = true;
                        subSong = NextJump.TargetSong ?? NextJump.Song;
                        subInputNode = await subSong.Song.CreateNode(graph);
                        subInputNode.AddOutgoingConnection(outputNode);

                        subInputNode.StartTime = NextJump.TargetTime - (NextJump.Origin - mainInputNode.Position);
                        subInputNode.OutgoingGain = 0;
                        subInputNode.Start();
                    }
                    if (IsFading && subInputNode != null)
                    {
                        var fadePosition = (NextJump.Origin - mainInputNode.Position);
                        var fadeTime = NextJump.CrossFade;

                        var percentage = Math.Min(1.0, Math.Max(0.0, fadePosition.TotalSeconds / fadeTime.TotalSeconds));

                        subInputNode.OutgoingGain = 1.0 - percentage;
                        mainInputNode.OutgoingGain = percentage;

                    }
                    if (Position > NextJump.Origin
                        && IsFading && subInputNode != null)
                    {
                        subInputNode.OutgoingGain = 1.0;
                        mainInputNode.Stop();
                        mainInputNode.RemoveOutgoingConnection(outputNode);
                        var tempNode = mainInputNode;
                        mainInputNode = subInputNode;
                        tempNode.Dispose();
                        subInputNode = null;
                        mainSong = subSong;
                        subSong = null;
                        NextJump = NextJump.NextDefaultJump ?? mainSong.Jumps.FirstOrDefault(x => mainInputNode.Position < x.Origin);
                        IsFading = false;
                    }

                }

            }
            catch (Exception e)
            {
                App.Log(e);
                Recover();
            }
        }

        private void Recover()
        {
            App.Log("Recover MediaPlayer");
            graph.Stop();
            try
            {
                mainInputNode.Dispose();
            }
            catch (Exception) { }
            try
            {
                subInputNode.Dispose();
            }
            catch (Exception) { }
            try
            {
                outputNode.Dispose();
            }
            catch (Exception) { }
            mainInputNode = null;
            subInputNode = null;
            outputNode = null;
            mainSong = null;
            subSong = null;

            try
            {
                graph.Dispose();
            }
            catch (Exception) { }

            graph = null;
            Init();

        }

        public void Pause()
        {
            graph.Stop();
            IsPlaying = false;
        }

        public void Seek(TimeSpan position)
        {
            if (subInputNode != null)
                throw new InvalidOperationException("Can't Seek while Fading");
            if (mainInputNode != null)
                mainInputNode.Seek(position);
        }
        public async Task Play(SongViewModel song)
        {
            if (mainInputNode == null)
            {
                mainInputNode = await song.Song.CreateNode(graph);
                mainSong = song;

                NextJump = mainSong.Jumps.FirstOrDefault();
                mainInputNode.AddOutgoingConnection(outputNode);
                graph.Start();
                IsPlaying = true;
            }
            else if (IsPlaying)
            {
                var jump = new JumpViewModel(mainSong)
                {
                    Origin = mainInputNode.Position + TimeSpan.FromSeconds(5),
                    TargetSong = song,
                    TargetTime = TimeSpan.FromSeconds(5),
                    CrossFade = TimeSpan.FromSeconds(5)
                };
                if (mainInputNode.Duration < jump.Origin)
                {
                    jump.Origin = mainInputNode.Duration;
                    jump.CrossFade = mainInputNode.Duration - mainInputNode.Position;
                    jump.TargetTime = jump.CrossFade;
                }
                NextJump = jump;
            }
            else
            {
                if (mainInputNode != null)
                {
                    mainInputNode.RemoveOutgoingConnection(outputNode);
                    mainInputNode.Dispose();
                    mainInputNode = null;
                    mainSong = null;
                }
                if (subInputNode != null)
                {
                    subInputNode.RemoveOutgoingConnection(outputNode);
                    subInputNode.Dispose();
                    subInputNode = null;
                    subSong = null;
                }
                IsFading = false;

                await Play(song);
            }
        }

        private async void OnPropertyChanged(string caller)
        {
            await App.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller)));
        }

        #region IDisposable Support
        private bool disposedValue = false; // Dient zur Erkennung redundanter Aufrufe.

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    graph.Stop();
                    outputNode.Dispose();
                    mainInputNode?.Dispose();
                    subInputNode?.Dispose();
                    graph.Dispose();
                }


                disposedValue = true;
            }
        }


        // Dieser Code wird hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
            Dispose(true);
        }
        #endregion
    }
}
