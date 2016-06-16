using System;

namespace RaspWakeUp.Contracts
{
    public interface IInput
    {
        event Action KeyPause;
        event Action KeyTime;
        event Action KeySleep;
        event Action KeyForward;
        event Action KeyFastForward;
    }
}
