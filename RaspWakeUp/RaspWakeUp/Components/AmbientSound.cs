using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Maker.Media.UniversalMediaEngine;
using RaspWakeUp.Contracts;

namespace RaspWakeUp.Components
{
    public class AmbientSound : IAmbientSound
    {
        private readonly MediaEngine _mediaEngine;

        public AmbientSound(MediaEngine mediaEngine)
        {
            _mediaEngine = mediaEngine;
        }

        public async Task Play()
        {
            Debug.WriteLine("AmbientSound::Play");
            _mediaEngine.MediaStateChanged += MediaEngineOnMediaStateChanged;
            await _mediaEngine.InitializeAsync();
            _mediaEngine.Play("ms-appx:///Content/birds.mp3");
        }

        public async Task Stop()
        {
            Debug.WriteLine("AmbientSound::Stop");
            _mediaEngine.MediaStateChanged -= MediaEngineOnMediaStateChanged;
            _mediaEngine.Stop();
        }

        private void MediaEngineOnMediaStateChanged(MediaState state)
        {
            Debug.WriteLine("AmbientSound :: Media State Changed " + state);
        }
    }
}
