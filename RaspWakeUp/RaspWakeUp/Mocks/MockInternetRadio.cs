using System.Diagnostics;
using System.Threading.Tasks;
using RaspWakeUp.Contracts;

namespace RaspWakeUp.Mocks
{
    public class MockInternetRadio : IInternetRadio
    {
        public async Task Play()
        {
            Debug.WriteLine("MockInternetRadio::Play");
        }

        public async Task Stop()
        {
            Debug.WriteLine("MockInternetRadio::Stop");
        }
    }
}
