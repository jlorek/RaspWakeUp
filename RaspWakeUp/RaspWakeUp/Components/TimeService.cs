using System;
using System.Threading.Tasks;
using RaspWakeUp.Contracts;

namespace RaspWakeUp.Components
{
    public class TimeService : ITimeService
    {
        public Action<TimeSpan> Tick { get; set; } = delegate { };

        public TimeSpan Now => DateTime.Now.TimeOfDay;

        public TimeService()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    DateTime now = DateTime.Now;
                    Tick(now.TimeOfDay);
                    await Task.Delay(500);
                }
            });
        }
    }
}
