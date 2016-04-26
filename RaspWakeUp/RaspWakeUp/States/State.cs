using System;
using System.Threading.Tasks;

namespace RaspWakeUp.States
{
    public class State
    {
        public State(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public Func<Task> StateEnter = async () => { }; 
        public Func<Task> StateLeave = async () => { };

        public Action KeyRadio { get; set; } = delegate { }; 
        public Action KeyAlarm { get; set; } = delegate { }; 
        public Action KeySnooze { get; set; } = delegate { };

        public Func<TimeSpan, Task> ClockTick = async (time) => { };
    }
}
