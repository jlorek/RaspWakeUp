using System.Diagnostics;

namespace RaspWakeUp.Components
{
    public class Display
    {
        public void FirstLine(string text)
        {
            Debug.WriteLine($"Display 1st line: {text}");
        }

        public void SecondLine(string text)
        {
            Debug.WriteLine($"Display 2nd line: {text}");
        }
    }
}
