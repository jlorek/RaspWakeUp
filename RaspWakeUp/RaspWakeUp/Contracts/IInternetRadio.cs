using System.Threading.Tasks;

namespace RaspWakeUp.Contracts
{
    public interface IInternetRadio
    {
        Task Play();
        Task Stop();
    }
}
