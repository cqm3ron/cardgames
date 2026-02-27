using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.core.extensions
{
    internal static class StackExtensions
    {
        public static void Shuffle<T>(this Stack<T> stack)
        {
            if (stack == null)
            {
                throw new ArgumentNullException(nameof(stack));
            }

            var list = new List<T>(stack);
            var rng = new Random();
            int j;
            T temp;

            for (int i = 0; i < list.Count - 2; i++) // Performs Fisher-Yates shuffle
            {
                j = rng.Next(i, list.Count);
                temp = list[j];
                list[j] = list[i];
                list[i] = temp;
            }

            stack.Clear();

            foreach (T item in list)
            {
                stack.Push(item);
            }
        }

    }
}
