using System.Text.Json;
using static cardgames.core.Card;


namespace cardgames.core
{
    internal abstract class NaturalLanguageParser
    {
        // Map File Management
        public Dictionary<string, TEnum> ImportMap<TEnum>(string path) where TEnum : struct, Enum
        {
            Console.WriteLine(Path.GetRelativePath(Directory.GetCurrentDirectory(), path)); // remove when testing finished



            Dictionary<string, TEnum> map = [];
            string jsonMap = null;

            if (File.Exists(path))
            {
                jsonMap = File.ReadAllText(path);
            }
            else
            {
                throw new FileNotFoundException("Invalid file path", path);
            }

            var data = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(jsonMap)!;


            foreach (var (name, aliases) in data)
            {
                if (Enum.TryParse<TEnum>(name, true, out var value))
                {
                    foreach (var alias in aliases)
                    {
                        map[alias] = value;
                    }
                }
            }

            return map;
        } // import map
        protected private void ExportMap<TEnum>(Dictionary<string, TEnum> map, string path) where TEnum : struct, Enum
        {
            var grouped = map.GroupBy(kvp => kvp.Value).ToDictionary(g => g.Key.ToString()!, g => g.Select(x => x.Key).OrderBy(x => x).ToList());

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(grouped, options);

            if (!File.Exists(path))
            {
                File.Create(path);
            }

            File.WriteAllText(path, json);
        } // export map

        // Parsing - Generic
        protected private bool TryFindMatch<T>(string token, Dictionary<string, T> map, out T value)
        {
            if (map.TryGetValue(token, out value)) return true;

            var prefixMatches = map.Keys.Where(k => k.StartsWith(token)).OrderByDescending(k => k.Length).ToList();

            if (prefixMatches.Any())
            {
                value = map[prefixMatches[0]];
                return true;
            }
            else
            {
                return TryFuzzyMatch(token, map, out value);
            }
        }
        protected private bool TryFuzzyMatch<T>(string token, Dictionary<string, T> map, out T value)
        {
            {
                {
                    value = default!;
                    const int TOLERANCE = 2; // can be made higher for more... imagination?

                    string? bestKey = null;
                    int bestDistance = int.MaxValue;

                    foreach (var key in map.Keys)
                    {
                        int distance = Levenshtein(token, key);

                        if (distance < bestDistance)
                        {
                            bestDistance = distance;
                            bestKey = key;
                        }
                    }

                    if (bestKey != null && bestDistance <= TOLERANCE)
                    {
                        value = map[bestKey];
                        return true;
                    }

                    return false;
                }
            }
        }

        // Util
        protected private string NormaliseInput(string input)
        {
            return input.ToLower().Replace("of", "").Replace(",", "").Replace(".", "").Trim();
        } // remove unnecessary characters & standardise formatting
        protected private int Levenshtein(string string1, string string2)
        {
            // Store length of each string in variable; this will be used a lot.
            int length1 = string1.Length;
            int length2 = string2.Length;

            int[,] distanceMatrix = new int[length1 + 1, length2 + 1];

            if (length1 == 0) return length2;
            if (length2 == 0) return length1;

            for (int i = 1; i <= length1; i++)
            {
                distanceMatrix[i, 0] = i;
            }
            for (int j = 1; j <= length2; j++)
            {
                distanceMatrix[0, j] = j;
            }

            int substitutionCost;

            for (int j = 1; j <= length2; j++)
            {
                for (int i = 1; i <= length1; i++)
                {
                    if (string1[i - 1] == string2[j - 1])
                    {
                        substitutionCost = 0;
                    }
                    else
                    {
                        substitutionCost = 1;
                    }

                    distanceMatrix[i, j] = Math.Min(
                        Math.Min(
                            distanceMatrix[i - 1, j] + 1,                   // deletion
                            distanceMatrix[i, j - 1] + 1                    // insertion
                            ),
                        distanceMatrix[i - 1, j - 1] + substitutionCost     // substitution
                        );
                }
            }

            return distanceMatrix[length1, length2];
        } // calculate levenshtein distance

    }
}
