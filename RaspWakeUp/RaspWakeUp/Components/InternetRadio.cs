using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Maker.Media.UniversalMediaEngine;
using RaspWakeUp.Contracts;

namespace RaspWakeUp.Components
{
    public class InternetRadio : IInternetRadio
    {
        //private const string StreamUrl = "http://www.dradio.de/streaming/dradiowissen.m3u";
        //private const string StreamUrl = "http://dradio_mp3_dwissen_m.akacast.akamaistream.net/7/728/142684/v1/gnl.akacast.akamaistream.net/dradio_mp3_dwissen_m";
        //private const string StreamUrl = "http://stream.dradio.de/7/728/142684/v1/gnl.akacast.akamaistream.net/dradio_mp3_dwissen_m";
        private const string StreamUrl = "http://ice.somafm.com/groovesalad";

        private readonly MediaEngine _mediaEngine;

        public InternetRadio(MediaEngine mediaEngine)
        {
            _mediaEngine = mediaEngine;
        }

        public async Task Play()
        {
            Debug.WriteLine("InternetRadio::Play");
            _mediaEngine.MediaStateChanged += MediaEngineOnMediaStateChanged;
            await _mediaEngine.InitializeAsync();
            _mediaEngine.Play(StreamUrl);
        }

        public async Task Stop()
        {
            Debug.WriteLine("InternetRadio::Stop");
            _mediaEngine.MediaStateChanged -= MediaEngineOnMediaStateChanged;
            _mediaEngine.Stop();
        }

        private void MediaEngineOnMediaStateChanged(MediaState state)
        {
            Debug.WriteLine("AmbientSound :: Media State Changed " + state);
        }
    }
}
