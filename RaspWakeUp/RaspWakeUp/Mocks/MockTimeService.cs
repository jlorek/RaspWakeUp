using System;
using System.Threading.Tasks;
using RaspWakeUp.Contracts;

namespace RaspWakeUp.Mocks
{
    public class MockTimeService : ITimeService
    {
        public Action<TimeSpan> Tick { get; set; } = delegate { };

        public TimeSpan Now => _fakeTime;

        private TimeSpan _fakeTime = new TimeSpan(8, 0, 0);

        public MockTimeService()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    _fakeTime = _fakeTime.Add(TimeSpan.FromSeconds(1));
                    if (_fakeTime.Days == 1)
                    {
                        _fakeTime = new TimeSpan(0, 0, 0);
                    }

                    if (Tick != null)
                    {
                        Tick(_fakeTime);
                    }

                    await Task.Delay(500);
                }
            });
        }
    }
}
