using System.Threading.Tasks;

namespace RaspWakeUp.Contracts
{
    public interface IAmbientSound
    {
        Task Play();
        Task Stop();
    }
}
