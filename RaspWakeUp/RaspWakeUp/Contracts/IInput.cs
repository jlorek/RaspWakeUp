using System;

namespace RaspWakeUp.Contracts
{
    public interface IInput
    {
        event Action KeyAlarm;// { get; set; }
        event Action KeyRadio;// { get; set; }
        event Action KeySnooze;// { get; set; }
    }
}
