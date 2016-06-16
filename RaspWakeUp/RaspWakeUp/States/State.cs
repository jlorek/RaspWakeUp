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

        public Func<Task> StateEnter = () => Task.CompletedTask; 
        public Func<Task> StateLeave = () => Task.CompletedTask;

        public Action KeyRadio { get; set; } = delegate { }; 
        public Action KeyAlarm { get; set; } = delegate { }; 
        public Action KeySnooze { get; set; } = delegate { };

        public Func<TimeSpan, Task> ClockTick = (time) => Task.CompletedTask;
    }
}
