using System.Diagnostics;

namespace RaspWakeUp.Components
{
    public class InternetRadio
    {
        public void Play()
        {
            Debug.WriteLine("Radio::Play");
        }

        public void Stop()
        {
            Debug.WriteLine("Radio::Stop");
        }
    }
}
