using cardgames.core;

namespace cardgames.games.blackjack
{
    internal class BlackjackParser : NaturalLanguageParser
    {
        private const string CHOICE_MAP_PATH = "..\\..\\..\\games\\blackjack\\maps\\ChoiceMap.json";

        public BlackjackParser() : base()
        {
            ImportMaps();
        }

        public enum Choice
        {
            hit,
            stand
        }

        private Dictionary<string, Choice> choiceMap;
        
        public void ImportMaps()
        {
            choiceMap = ImportMap<Choice>(CHOICE_MAP_PATH);
        }

        //Parsing - Specific
        public bool TryParseChoice(string input, out Choice? choice)
        {
            choice = null;
            var tokens = NormaliseInput(input).Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var token in tokens)
            {
                if (token.Length == 1 && !choiceMap.ContainsKey(token))
                {
                    choice = null;
                    return false;
                }

                if (choice == null && TryFindMatch(token, choiceMap, out Choice foundChoice))
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