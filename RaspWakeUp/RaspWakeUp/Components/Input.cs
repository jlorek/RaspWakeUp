using System;
using System.Diagnostics;
using Windows.Devices.Gpio;
using RaspWakeUp.Contracts;

namespace RaspWakeUp.Components
{
    public class Input : IInput
    {
        private const int GpioPinPause = 18;
        private const int GpioPinTime = 23;
        private const int GpioPinSleep = 24;
        private const int GpioPinForward = 25;
        private const int GpioPinFastForward = 12;

        public event Action KeyPause = delegate { };
        public event Action KeyTime = delegate { };
        public event Action KeySleep = delegate { };
        public event Action KeyForward = delegate { };
        public event Action KeyFastForward = delegate { };

        private readonly GpioPin _pinPause;
        private readonly GpioPin _pinTime;
        private readonly GpioPin _pinSleep;
        private readonly GpioPin _pinForward;
        private readonly GpioPin _pinFastForward;

        public Input()
        {
            var gpio = GpioController.GetDefault();

            if (gpio != null)
            {
                _pinPause = SetupPin(gpio, GpioPinPause, () => KeyPause());
                _pinTime = SetupPin(gpio, GpioPinTime, () => KeyTime());
                _pinSleep = SetupPin(gpio, GpioPinSleep, () => KeySleep());
                _pinForward = SetupPin(gpio, GpioPinForward, () => KeyForward());
                _pinFastForward = SetupPin(gpio, GpioPinFastForward, () => KeyFastForward());
            }
            else
            {
                Debugger.Break();
            }
        }

        private GpioPin SetupPin(GpioController gpio, int pin, Action action)
        {
            GpioPin gpioPin = gpio.OpenPin(pin);
            gpioPin.SetDriveMode(GpioPinDriveMode.InputPullDown);
            gpioPin.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            gpioPin.ValueChanged += (sender, args) =>
            {
                if (args.Edge == GpioPinEdge.RisingEdge)
                {
                    action();
                }
            };

            return gpioPin;
        }
    }
}
