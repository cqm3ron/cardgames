using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static cardgames.core.Language;

namespace cardgames.core
{
    internal static class Util
    {
        public static bool UserAgrees()
        {
            string[] options = [T("Util.Yes"), T("Util.No")];

            if (GetChoice(options) == 0) return true;
            else return true;
        }

        public static int GetChoice(string[] options, int selected = 0)
        {
            Console.CursorVisible = false;
            (int, int) cursorPos = (Console.CursorLeft, Console.CursorTop);

            while (!Console.KeyAvailable)
            {
                Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selected)
                    {
                        Console.ForegroundColor = SELECTED_FOREGROUND;
                    }
                    else
                    {
                        Console.ForegroundColor = DEFAULT_FOREGROUND;
                    }
                    Console.WriteLine($"> {options[i]}");
                }

                Util.ResetColor();

                ConsoleKeyInfo key = Console.ReadKey(true);
                if (previousOptions.Contains(key.Key) || (key.Key == ConsoleKey.Tab && key.Modifiers.HasFlag(ConsoleModifiers.Shift)))
                {
                    selected = (selected - 1 + options.Length) % options.Length;
                }
                else if (nextOptions.Contains(key.Key))
                {
                    selected = (selected + 1) % options.Length;
                }
                else if (affirmatives.Contains(key.Key))
                {
                    Console.CursorVisible = true;
                    return selected;
                }
            }
            Console.CursorVisible = true;
            return selected;
        }

        public static string GetPassword()
        {
            string password = "";
            ConsoleKeyInfo keyInfo;

            while (true)
            {
                keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password = password.Substring(0, password.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    password += keyInfo.KeyChar;
                    Console.Write('*');
                }
            }

            return password;
        }



        private static readonly ConsoleColor DEFAULT_FOREGROUND = ConsoleColor.White;
        private static readonly ConsoleColor DEFAULT_BACKGROUND = ConsoleColor.Black;
        private static readonly ConsoleColor SELECTED_FOREGROUND = ConsoleColor.Cyan;

        public static ConsoleKey[] affirmatives = { ConsoleKey.Enter, ConsoleKey.Spacebar, ConsoleKey.Z };
        public static ConsoleKey[] nextOptions = { ConsoleKey.DownArrow, ConsoleKey.PageDown, ConsoleKey.Tab, ConsoleKey.S };
        public static ConsoleKey[] previousOptions = { ConsoleKey.UpArrow, ConsoleKey.PageUp, ConsoleKey.W };

        public static void ResetColor()
        {
            Console.ForegroundColor = DEFAULT_FOREGROUND;
            Console.BackgroundColor = DEFAULT_BACKGROUND;
        }

        public static void ResetColour()
        {
            ResetColor();
        }

        public static void WriteLineBackwards(string input)
        {
            Console.SetCursorPosition(Console.WindowWidth - 1, Console.GetCursorPosition().Top);
            char[] characters = input.ToCharArray();

            (int, int) initialCursorPos = Console.GetCursorPosition();

            //if (characters.Length > (Console.WindowWidth - Console.GetCursorPosition().Left)) throw new IndexOutOfRangeException("Not enough space to write!");

            for (int c = characters.Length - 1; c >= 0; c--)
            {
                (int, int) cursorPos = Console.GetCursorPosition();
                Console.Write(characters[c]);
                Console.SetCursorPosition(cursorPos.Item1 - 1, cursorPos.Item2);
            }

            Console.SetCursorPosition(0, initialCursorPos.Item2 + 1);
        }
    }
}
