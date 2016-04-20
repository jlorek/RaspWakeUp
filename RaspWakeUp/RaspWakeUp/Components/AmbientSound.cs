using System.Diagnostics;

namespace RaspWakeUp.Components
{
    public class AmbientSound
    {
        public void Play()
        {
            Debug.WriteLine("AmbientSound::Play");
        }

        public void Stop()
        {
            Debug.WriteLine("AmbientSound::Stop");
        }
    }
}
