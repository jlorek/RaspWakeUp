using System.Threading.Tasks;

namespace RaspWakeUp.Contracts
{
    public interface IDisplay
    {
        Task Init();
        void FirstLine(string text);
        void SecondLine(string text);
    }
}
