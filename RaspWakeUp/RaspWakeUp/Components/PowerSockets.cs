using System.Diagnostics;

namespace RaspWakeUp.Components
{
    public class PowerSockets
    {
        public void On()
        {
            Debug.WriteLine("PowerSockets::On");
        }

        public void Off()
        {
            Debug.WriteLine("PowerSockets::Off");
        }
    }
}
