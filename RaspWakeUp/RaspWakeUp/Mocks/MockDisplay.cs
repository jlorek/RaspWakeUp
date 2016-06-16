using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspWakeUp.Contracts;

namespace RaspWakeUp.Mocks
{
    public class MockDisplay : IDisplay
    {
        public Task Init()
        {
            Debug.WriteLine("MockDisplay: Init");
            return Task.CompletedTask;
        }

        public void FirstLine(string text)
        {
            Debug.WriteLine($"MockDisplay 1st line: {text}");
        }

        public void SecondLine(string text)
        {
            Debug.WriteLine($"MockDisplay 2nd line: {text}");
        }
    }
}
