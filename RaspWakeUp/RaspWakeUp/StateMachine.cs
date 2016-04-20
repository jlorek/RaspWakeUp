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

        private TimeSpan _snoozeEnd;
        private TimeSpan _ambientEnd;
        private bool _alarmSkip = false;
        private bool _alarmEnabled = true;

        private TimeSpan _fakeTime = new TimeSpan(7, 59, 50);

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

                OnKeyAlarm = () =>
                {
                    _alarmEnabled = !_alarmEnabled;
                },

                ClockTick = (time) =>
                {
                    // reset alarm at midnight
                    if (time >= _config.Alarm)
                    {
                        if (_alarmEnabled && !_alarmSkip)
                        {
                            _alarmSkip = true;
                            CurrentState = _stateAmbient;
                        }
                    }
                }
            };

            _stateAmbient = new State("Ambient")
            {
                OnStateEnter = () =>
                {
                    _ambientEnd = _fakeTime + _config.AmbientDuration;
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
                    if (time >= _ambientEnd)
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
                    _snoozeEnd = _fakeTime + _config.SnoozeDuration;
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
                    if (time >= _snoozeEnd)
                    {
                        CurrentState = _stateRadio;
                    }
                }
            };

            CurrentState = _stateIdle;
        }

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
                    if (_fakeTime.Days == 1)
                    {
                        _fakeTime = new TimeSpan(0, 0, 0);
                    }
                    // TimeSpan.Days is incremented after 23:59:59
                    _display.FirstLine(_fakeTime.ToString(@"hh\:mm\:ss"));
                    CurrentState.ClockTick(_fakeTime);

                    if (_fakeTime.Hours == 0 && _fakeTime.Minutes == 0)
                    {
                        _alarmSkip = false;
                    }

                    if (_fakeTime >= _config.Alarm)
                    {
                        _alarmSkip = true;
                    }

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
