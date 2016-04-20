using System;

namespace RaspWakeUp.States
{
    public class State
    {
        public State(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public Action OnStateEnter { get; set; } = delegate { }; 
        public Action OnStateLeave { get; set; } = delegate { }; 
        public Action OnKeyRadio { get; set; } = delegate { }; 
        public Action OnKeyAlarm { get; set; } = delegate { }; 
        public Action OnKeySnooze { get; set; } = delegate { };
        public Action<TimeSpan> ClockTick { get; set; } = delegate { };
    }
}
