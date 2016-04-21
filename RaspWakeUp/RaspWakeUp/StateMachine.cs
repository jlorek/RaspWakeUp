using System;
using RaspWakeUp.Components;
using RaspWakeUp.States;
using System.Diagnostics;
using RaspWakeUp.Contracts;
using RaspWakeUp.Mocks;

namespace RaspWakeUp
{
    public class StateMachine
    {
        private AmbientSound _ambient = new AmbientSound();
        private InternetRadio _radio = new InternetRadio();
        private PowerSockets _sockets = new PowerSockets();
        private Display _display = new Display();
        private Config _config = new Config();
        private ITimeService _timeService;

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

        private State _stateIdle;
        private State _stateAmbient;
        private State _stateRadio;
        private State _stateSnooze;

        public StateMachine()
        {
            InitComponents();
            InitStates();

            _timeService.Tick = ClockTick;
        }

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
                    if (time.Hours == _config.Alarm.Hours && time.Minutes == _config.Alarm.Minutes)
                    {
                        if (!_alarmSkip)
                        {
                            _alarmSkip = true;
                            CurrentState = _stateAmbient;
                        }
                    }
                    else
                    {
                        _alarmSkip = false;
                    }
                }
            };

            _stateAmbient = new State("Ambient")
            {
                OnStateEnter = () =>
                {
                    _ambientEnd = _timeService.Now + _config.AmbientDuration;
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
                    _snoozeEnd = _timeService.Now + _config.SnoozeDuration;
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

        private void InitComponents()
        {
            _timeService = new MockTimeService();
        }

        private void ClockTick(TimeSpan time)
        {
            _display.FirstLine(time.ToString(@"hh\:mm\:ss"));
            CurrentState.ClockTick(time);
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
