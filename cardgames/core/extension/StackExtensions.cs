namespace cardgames.core.extension
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

        public static void RemoveFirstOccurrence<T>(this Stack<T> stack, T itemToRemove) // referenced https://www.w3resource.com/csharp-exercises/stack/csharp-stack-exercise-8.php
        {
            Stack<T> temp = [];
            bool firstOccurrenceRemoved = false;

            while (stack.Count > 0)
            {
                T element = stack.Pop();

                if (!Equals(element, itemToRemove))
                {
                    temp.Push(element);
                }
                else
                {
                    if (!firstOccurrenceRemoved)
                    {
                        firstOccurrenceRemoved = true;
                    }
                    else
                    {
                        temp.Push(element);
                    }
                }
            }

            while (temp.Count > 0)
            {
                stack.Push(temp.Pop());
            }
        }
    }
}
