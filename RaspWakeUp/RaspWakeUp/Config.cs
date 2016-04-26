using System;

namespace RaspWakeUp
{
    public class Config
    {
        public TimeSpan Alarm { get; set; }
        public TimeSpan AmbientDuration { get; set; }
        public TimeSpan SnoozeDuration { get; set; }

        public Config()
        {
            Alarm = new TimeSpan(8, 0, 0);
            AmbientDuration = new TimeSpan(0, 0, 15);
            SnoozeDuration = new TimeSpan(0, 0, 5);
        }
    }
}
