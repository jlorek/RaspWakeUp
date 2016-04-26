using System;
using System.Diagnostics;
using Windows.Devices.Gpio;
using RaspWakeUp.Contracts;

namespace RaspWakeUp.Components
{
    public class Input : IInput
    {
        private const int GpioPinAlarm = 16;
        private const int GpioPinSnooze = 20;
        private const int GpioPinRadio = 21;

        public event Action KeyAlarm = delegate { }; // { get; set; } = delegate { };
        public event Action KeySnooze = delegate { }; // { get; set; } = delegate { };
        public event Action KeyRadio = delegate { }; // { get; set; } = delegate { };

        private readonly GpioPin _alarmPin;
        private readonly GpioPin _snoozePin;
        private readonly GpioPin _radioPin;

        public Input()
        {
            var gpio = GpioController.GetDefault();

            if (gpio != null)
            {
                _alarmPin = SetupPin(gpio, GpioPinAlarm, () => KeyAlarm());
                _snoozePin = SetupPin(gpio, GpioPinSnooze, () => KeySnooze());
                _radioPin = SetupPin(gpio, GpioPinRadio, () => KeyRadio());
            }
            else
            {
                Debugger.Break();
            }
        }

        private GpioPin SetupPin(GpioController gpio, int pin, Action action)
        {
            GpioPin gpioPin = gpio.OpenPin(pin);
            gpioPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            gpioPin.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            gpioPin.ValueChanged += (sender, args) =>
            {
                if (args.Edge == GpioPinEdge.FallingEdge)
                {
                    action();
                }
            };

            return gpioPin;
        }
    }
}
