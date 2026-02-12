using cardgames.core;
using static cardgames.core.Card;

namespace cardgames.games.cheat
{
    internal class CheatParser : NaturalLanguageParser
    {
        private Dictionary<string, Rank> rankMap;
        private Dictionary<string, Suit> suitMap;

        CheatParser()
        {
            rankMap = [];
            suitMap = [];
        }
        public void ImportMaps(string rankMapPath, string suitMapPath)
        {
            rankMap = ImportMap<Rank>(rankMapPath);
            suitMap = ImportMap<Suit>(suitMapPath);
        }

        // Parsing - Specific
        public bool TryParseCard(string input, out Card? card)
        {
            card = null;
            var tokens = NormaliseInput(input).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Rank? rank = null;
            Suit? suit = null;

            foreach (var token in tokens)
            {
                if (token.Length == 1 && !rankMap.ContainsKey(token) && !suitMap.ContainsKey(token))
                {
                    card = null;
                    return false;
                }

                if (rank == null && TryFindMatch(token, rankMap, out Rank foundRank))
                {
                    rank = foundRank;
                    continue;
                }
                if (suit == null && TryFindMatch(token, suitMap, out Suit foundSuit))
                {
                    if (rankMap.ContainsKey(token))
                    {
                        card = null;
                        return false;
                    }

                    suit = foundSuit;
                    continue;
                }
            }

            if (rank == null || suit == null) return false;

            card = new Card(rank.Value, suit.Value);
            return true;

        }
    }
}
