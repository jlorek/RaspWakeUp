using System;
using RaspWakeUp.Components;
using RaspWakeUp.States;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Maker.Media.UniversalMediaEngine;
using RaspWakeUp.Contracts;
using RaspWakeUp.Mocks;

namespace RaspWakeUp
{
    public class StateMachine
    {
        private AmbientSound _ambientSound;
        private InternetRadio _internetRadio;
        private PowerSockets _sockets = new PowerSockets();
        private Display _display = new Display();
        private Config _config = new Config();
        private ITimeService _timeService;
        private IInput _input;
        private MediaEngine _mediaEngine;

        private State _state = new State("Empty");

        public async Task SetState(State value)
        {
            Debug.WriteLine($"Leaving State [{_state.Name}]");
            await _state.StateLeave();
            Debug.WriteLine($"Entering State [{value.Name}]");
            await value.StateEnter();
            _state = value;
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
            //Task.Factory.StartNew(async () => await Init());
            Init();
        }

        private async Task Init()
        {
            await InitComponents();
            InitStates();
        }

        private void InitStates()
        {
            _stateIdle = new State("Idle")
            {
                KeyRadio = async () =>
                {
                    await SetState(_stateRadio);
                },

                KeyAlarm = () =>
                {
                    _alarmEnabled = !_alarmEnabled;
                },

                ClockTick = async (time) =>
                {
                    if (time.Hours == _config.Alarm.Hours && time.Minutes == _config.Alarm.Minutes)
                    {
                        if (!_alarmSkip)
                        {
                            _alarmSkip = true;
                            await SetState(_stateAmbient);
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
                StateEnter = async () =>
                {
                    _ambientEnd = _timeService.Now + _config.AmbientDuration;
                    await _ambientSound.Play();
                },

                StateLeave = async () =>
                {
                    await _ambientSound.Stop();
                },

                KeyAlarm = async () =>
                {
                    await SetState(_stateIdle);
                },

                KeyRadio = async () =>
                {
                    await SetState(_stateRadio);
                },

                ClockTick = async (time) =>
                {
                    if (time >= _ambientEnd)
                    {
                        await SetState(_stateRadio);
                    }
                }
            };

            _stateRadio = new State("Radio")
            {
                StateEnter = async () =>
                {
                    await _internetRadio.Play();
                },

                StateLeave = async () =>
                {
                    await _internetRadio.Stop();
                },

                KeyRadio = async () =>
                {
                    await SetState(_stateIdle);
                },

                KeyAlarm = async () =>
                {
                    await SetState(_stateIdle);
                },

                KeySnooze = async () =>
                {
                    await SetState(_stateSnooze);
                }
            };

            _stateSnooze = new State("Snooze")
            {
                StateEnter = async () =>
                {
                    //_snoozeAlarm = DateTime.Now.TimeOfDay + _config.SnoozeDuration;
                    _snoozeEnd = _timeService.Now + _config.SnoozeDuration;
                },

                KeyRadio = async () =>
                {
                    await SetState(_stateRadio);
                },

                KeyAlarm = async () =>
                {
                    await SetState(_stateIdle);
                },

                ClockTick = async (time) =>
                {
                    if (time >= _snoozeEnd)
                    {
                        await SetState(_stateRadio);
                    }
                }
            };

            SetState(_stateIdle);
        }

        private async Task InitComponents()
        {
            _mediaEngine = new MediaEngine();
            _mediaEngine.MediaStateChanged += MediaEngineOnMediaStateChanged; 
            var result = await _mediaEngine.InitializeAsync();

            if (result == MediaEngineInitializationResult.Fail)
            {
                Debugger.Break();
                return;
            }
            else
            {
                //_mediaEngine.Play("http://uwstream1.somafm.com:80");
                //_mediaEngine.Play("http://ice.somafm.com/groovesalad");
                //return;
            }

            _ambientSound = new AmbientSound(_mediaEngine);
            _internetRadio = new InternetRadio(_mediaEngine);

            _timeService = new MockTimeService();
            _timeService.Tick = ClockTick;

            _input = new Input();
            _input.KeyAlarm += OnKeyAlarm;
            _input.KeySnooze += OnKeySnooze;
            _input.KeyRadio += OnKeyRadio;
        }

        private void MediaEngineOnMediaStateChanged(MediaState state)
        {
            Debug.WriteLine("MediaEngine State Changed " + state);
        }

        private void ClockTick(TimeSpan time)
        {
            _display.FirstLine(time.ToString(@"hh\:mm\:ss"));
            _state.ClockTick(time);
        }

        public void OnKeyRadio()
        {
            Debug.WriteLine("StateMachine::KeyRadio");
            _state.KeyRadio();
        }

        public void OnKeyAlarm()
        {
            Debug.WriteLine("StateMachine::KeyAlarm");
            _state.KeyAlarm();
        }

        public void OnKeySnooze()
        {
            Debug.WriteLine("StateMachine::KeySnooze");
            _state.KeySnooze();
        }
    }
}
