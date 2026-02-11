using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.core
{
    internal class YesNoParser : NaturalLanguageParser
    {
        private const string YES_NO_MAP_PATH = "..\\..\\..\\core\\maps\\YesNoMap.json";

        public YesNoParser() : base()
        {
            ImportMaps();
        }

        public enum Choice
        {
            yes,
            no
        }

        private Dictionary<string, Choice> yesNoMap;

        public void ImportMaps()
        {
            yesNoMap = ImportMap<Choice>(YES_NO_MAP_PATH);
        }

        //Parsing - Specific
        public bool TryParseChoice(string input, out Choice? choice)
        {
            choice = null;
            var tokens = NormaliseInput(input).Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var token in tokens)
            {
                if (token.Length == 1 && !yesNoMap.ContainsKey(token))
                {
                    choice = null;
                    return false;
                }

                if (choice == null && TryFindMatch(token, yesNoMap, out Choice foundChoice))
                {
                    choice = foundChoice;
                    continue;
                }
            }

            if (choice == null) return false;
            return true;

        }
    }
}
