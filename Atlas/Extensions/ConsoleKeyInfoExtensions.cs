using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Extensions
{
    internal static class ConsoleKeyInfoExtensions
    {
        public static ConsoleKeyInfo FromKey(this ConsoleKeyInfo keyInfo, string combination)
        {
            char character;
            ConsoleKey key;
            ConsoleModifiers modifiers = 0;

            if (combination.StartsWith("C-"))
            {
                modifiers |= ConsoleModifiers.Control;
                combination = combination[2..];
            }

            if (combination.Length == 1)
            {
                character = combination[0];
                if (char.IsLetter(character))
                {
                    key = (ConsoleKey)Enum.Parse(typeof(ConsoleKey), char.ToUpper(character).ToString());
                }
                else if (char.IsDigit(character))
                {
                    key = (ConsoleKey)Enum.Parse(typeof(ConsoleKey), $"D{character}");
                }
                else
                {
                    // Handle other characters as needed
                    key = (ConsoleKey)character;
                }
            }
            else
            {
                throw new ArgumentException("Invalid key combination format");
            }

            return new ConsoleKeyInfo(character, key, false, false, (modifiers & ConsoleModifiers.Control) != 0);
        }
    }
}
