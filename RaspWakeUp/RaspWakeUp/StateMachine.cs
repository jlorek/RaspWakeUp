using System;
using RaspWakeUp.Components;
using RaspWakeUp.States;
using System.Threading.Tasks;
using System.Diagnostics;

namespace RaspWakeUp
{
    public class StateMachine
    {
        private AmbientSound _ambient = new AmbientSound();
        private InternetRadio _radio = new InternetRadio();
        private PowerSockets _sockets = new PowerSockets();
        private Display _display = new Display();
        private Config _config = new Config();

        private State _state = new State("Empty");

        public State CurrentState
        {
            get
            {
                return _state;
            }

            set
            {
                if (_state != null)
                {
                    Debug.WriteLine($"Leaving State [{_state.Name}]");
                    _state.OnStateLeave();
                    Debug.WriteLine($"Entering State [{value.Name}]");
                    value.OnStateEnter();
                    _state = value;
                }
            }
        }

        private State _stateIdle;
        private State _stateAmbient;
        private State _stateRadio;
        private State _stateSnooze;

        private void InitStates()
        {
            _stateIdle = new State("Idle")
            {
                OnKeyRadio = () =>
                {
                    CurrentState = _stateRadio;
                },

                ClockTick = (time) =>
                {
                    if (time > _config.Alarm)
                    {
                        CurrentState = _stateAmbient;
                    }
                }
            };

            _stateAmbient = new State("Ambient")
            {
                OnStateEnter = () =>
                {
                    _ambient.Play();
                },

                OnStateLeave = () =>
                {
                    _ambient.Stop();
                },

                OnKeyAlarm = () =>
                {
                    CurrentState = _stateIdle;
                },

                OnKeyRadio = () =>
                {
                    CurrentState = _stateRadio;
                },

                ClockTick = (time) =>
                {
                    if (time > _config.Alarm + _config.AmbientDuration)
                    {
                        CurrentState = _stateRadio;
                    }
                }
            };

            _stateRadio = new State("Radio")
            {
                OnStateEnter = () =>
                {
                    _radio.Play();
                },

                OnStateLeave = () =>
                {
                    _radio.Stop();
                },

                OnKeyRadio = () =>
                {
                    CurrentState = _stateIdle;
                },

                OnKeyAlarm = () =>
                {
                    CurrentState = _stateIdle;
                },

                OnKeySnooze = () =>
                {
                    CurrentState = _stateSnooze;
                }
            };

            _stateSnooze = new State("Snooze")
            {
                OnStateEnter = () =>
                {
                    //_snoozeAlarm = DateTime.Now.TimeOfDay + _config.SnoozeDuration;
                    _snoozeAlarm = _fakeTime + _config.SnoozeDuration;
                },

                OnKeyRadio = () =>
                {
                    CurrentState = _stateRadio;
                },

                OnKeyAlarm = () =>
                {
                    CurrentState = _stateIdle;
                },

                ClockTick = (time) =>
                {
                    if (time > _snoozeAlarm)
                    {
                        CurrentState = _stateRadio;
                    }
                }
            };

            CurrentState = _stateIdle;
        }

        private TimeSpan _snoozeAlarm;
        private TimeSpan _fakeTime = new TimeSpan(7, 59, 0);

        public StateMachine()
        {
            InitStates();
            StartClock();
        }

        private void StartClock()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    //DateTime now = DateTime.Now;
                    //CurrentState.ClockTick(now.TimeOfDay);
                    //_display.FirstLine(now.ToString("HH:mm:ss"));

                    _fakeTime = _fakeTime.Add(TimeSpan.FromSeconds(1));
                    _display.FirstLine(_fakeTime.ToString(@"hh\:mm\:ss"));
                    CurrentState.ClockTick(_fakeTime);

                    await Task.Delay(1000);
                }
            });
        }

        public void OnKeyRadio()
        {
            Debug.WriteLine("StateMachine::OnKeyRadio");
            CurrentState.OnKeyRadio();
        }

        public void OnKeyAlarm()
        {
            Debug.WriteLine("StateMachine::OnKeyAlarm");
            CurrentState.OnKeyAlarm();
        }

        public void OnKeySnooze()
        {
            Debug.WriteLine("StateMachine::OnKeySnooze");
            CurrentState.OnKeySnooze();
        }
    }
}
