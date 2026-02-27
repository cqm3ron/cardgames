using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static cardgames.core.Translator;

namespace cardgames.core
{
    internal static class Util
    {
        public static bool UserAgrees()
        {
            int selected = 0;
            string[] options = [T("Util.Yes"), T("Util.No")];

            (int, int) cursorPos = (Console.CursorLeft, Console.CursorTop);

            while (!Console.KeyAvailable)
            {
                Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selected)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine($"> {options[i]}");
                }

                Util.ResetColor();

                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.W)
                {
                    selected = (selected - 1 + options.Length) % options.Length;
                }
                else if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.S)
                {
                    selected = (selected + 1) % options.Length;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    return selected == 0;
                }
            }

            return false;
        }

        public static int GetChoice(string[] options, int selected = 0)
        {
            (int, int) cursorPos = (Console.CursorLeft, Console.CursorTop);

            while (!Console.KeyAvailable)
            {
                Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selected)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine($"> {options[i]}");
                }

                Util.ResetColor();

                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.W)
                {
                    selected = (selected - 1 + options.Length) % options.Length;
                }
                else if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.S)
                {
                    selected = (selected + 1) % options.Length;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    return selected;
                }
            }

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



        private static readonly ConsoleColor DefaultForeground = ConsoleColor.White;
        private static readonly ConsoleColor DefaultBackground = ConsoleColor.Black;

        public static void ResetColor()
        {
            Console.ForegroundColor = DefaultForeground;
            Console.BackgroundColor = DefaultBackground;
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
