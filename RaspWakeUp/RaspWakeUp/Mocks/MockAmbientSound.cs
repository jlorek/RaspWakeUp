using System.Diagnostics;
using System.Threading.Tasks;
using RaspWakeUp.Contracts;

namespace RaspWakeUp.Mocks
{
    public class MockAmbientSound : IAmbientSound
    {
        public async Task Play()
        {
            Debug.WriteLine("MockAmbientSound::Play");
        }

        public async Task Stop()
        {
            Debug.WriteLine("MockAmbientSound::Stop");
        }
    }
}
