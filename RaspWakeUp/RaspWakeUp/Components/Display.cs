using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maker.Devices.TextDisplay;
using RaspWakeUp.Contracts;

namespace RaspWakeUp.Components
{
    public class Display : IDisplay
    {
        private ITextDisplay _display;

        public async Task Init()
        {
            IEnumerable<ITextDisplay> displays = await TextDisplayManager.GetDisplays();
            _display = displays.First();

            // display gets initialized automagically when calling GetDisplays();
            //await _display.InitializeAsync();

            //await _display.WriteMessageAsync("Much Loves &\nGood Night <3", 0);
            //await Task.Delay(5000);

            //await Task.Factory.StartNew(async () =>
            //{
            //    await _display.WriteMessageAsync("Much Loves &\nGood Night <3", 0);
            //    await Task.Delay(2000);

            //    while (true)
            //    {
            //        await _display.WriteMessageAsync("  Becky & Jens", 0);
            //        await Task.Delay(500);
            //        await _display.WriteMessageAsync("", 0);
            //        await Task.Delay(500);
            //    }
            //});

            //await display.WriteMessageAsync("Much Loves &\nGood Night <3", 0);
            //await display.InitializeAsync();
        }

        public void FirstLine(string text)
        {
            Debug.WriteLine($"Display 1st line: {text}");
            _display.WriteMessageAsync(text, 0);
        }

        public void SecondLine(string text)
        {
            Debug.WriteLine($"Display 2nd line: {text}");
        }
    }
}
