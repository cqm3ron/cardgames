using System.Runtime.InteropServices;
using static cardgames.core.Language;

namespace cardgames.core
{
    internal static class Util
    {
        private static Thread? loading;
        public static bool IsLoading = false;
        public static bool UserAgrees()
        {
            string[] options = [T("Util.Yes"), T("Util.No")];

            if (GetChoice(options) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
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
        public static ConsoleKey[] scrollLeft = { ConsoleKey.LeftArrow, ConsoleKey.A };
        public static ConsoleKey[] scrollRight = { ConsoleKey.RightArrow, ConsoleKey.D };
        public static ConsoleKey[] scrollUp = { ConsoleKey.UpArrow, ConsoleKey.W };
        public static ConsoleKey[] scrollDown = { ConsoleKey.DownArrow, ConsoleKey.S };


        public static void ResetColor()
        {
            Console.ForegroundColor = DEFAULT_FOREGROUND;
            Console.BackgroundColor = DEFAULT_BACKGROUND;
        }

        public static void ResetColour()
        {
            ResetColor();
        }

        public static void WriteLineBackwards(string input) // error handling non-existent?
        {
            Console.SetCursorPosition(Console.WindowWidth - 1, Console.GetCursorPosition().Top);
            char[] characters = input.ToCharArray();

            (int, int) initialCursorPos = Console.GetCursorPosition();

            for (int c = characters.Length - 1; c >= 0; c--)
            {
                (int, int) cursorPos = Console.GetCursorPosition();
                Console.Write(characters[c]);
                Console.SetCursorPosition(cursorPos.Item1 - 1, cursorPos.Item2);
            }

            Console.SetCursorPosition(0, initialCursorPos.Item2 + 1);
        }

        public static void StartLoading(string reason = "")
        {
            loading = new Thread(() => LoadingBar(reason))
            {
                IsBackground = true
            };
            IsLoading = true;
            loading.Start();
        }
        public static void FinishLoading()
        {
            if (IsLoading)
            {
                IsLoading = false;
                loading!.Join();
            }
            loading = null;
        }
        private static void LoadingBar(string reason = "")
        {
            const int SLEEP_DURATION = 250;

            Queue<char> characters = new();

            (int, int) cursorPos = Console.GetCursorPosition();
            Console.Write(reason + new string(' ', Console.WindowWidth -  reason.Length)); // pad reason with spaces to clear any previous text
            Thread.Sleep(SLEEP_DURATION / 2);
            Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);
            Console.Write(new string(' ', Console.WindowWidth));
            while (IsLoading)
            {
                if (characters.Count == 0)
                {
                    characters.Enqueue('-');
                    characters.Enqueue('/');
                    characters.Enqueue('-');
                    characters.Enqueue('\\');
                }

                Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);
                Console.Write(characters.Dequeue());
                Thread.Sleep(SLEEP_DURATION);
            }
            Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);
            Console.Write("Done!");
            Thread.Sleep(SLEEP_DURATION);
            Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);
        }



        private struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        public static void MaximiseWindow() // Code from https://learn.microsoft.com/en-us/answers/questions/1275773/how-to-resize-a-console-app-in-c-windows-terminal
        {                                   // Only needed because Microsoft changed the default terminal application in Win 11 and school recently updated; Previously Console.SetWindowSize() would have worked.
            // Import the necessary functions from user32.dll
            [DllImport("user32.dll")]
            static extern IntPtr GetForegroundWindow();
            [DllImport("user32.dll")]
            static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
            [DllImport("user32.dll")]
            static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);
            [DllImport("user32.dll")]
            static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);
            // Constants for the ShowWindow function
            const int SW_MAXIMIZE = 3;
            // Get the handle of the console window
            IntPtr consoleWindowHandle = GetForegroundWindow();
            // Maximize the console window
            ShowWindow(consoleWindowHandle, SW_MAXIMIZE);
            // Get the screen size
            Rect screenRect;
            GetWindowRect(consoleWindowHandle, out screenRect);
            // Resize and reposition the console window to fill the screen
            int width = screenRect.Right - screenRect.Left;
            int height = screenRect.Bottom - screenRect.Top;
            MoveWindow(consoleWindowHandle, screenRect.Left, screenRect.Top, width, height, true);
        }
    }
}
