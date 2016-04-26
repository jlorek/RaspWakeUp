using System;

namespace RaspWakeUp.Contracts
{
    public interface ITimeService
    {
        Action<TimeSpan> Tick { get; set; }

        TimeSpan Now { get; }
    }
}