using cardgames.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static cardgames.core.Card;

namespace cardgames.games.cheat
{
    internal class CheatParser : NaturalLanguageParser
    {
        private Dictionary<string, Ranks> rankMap;
        private Dictionary<string, Suits> suitMap;

        public CheatParser()
        {
            rankMap = [];
            suitMap = [];
        }

        public void ImportMaps(string rankMapPath, string suitMapPath)
        {
            rankMap = ImportMap<Ranks>(rankMapPath);
            suitMap = ImportMap<Suits>(suitMapPath);
        }

        // Parsing - Specific
        public bool TryParseCard(string input, out Card? card)
        {
            card = null;
            string[] tokens = NormaliseInput(input).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Ranks? rank = null;
            Suits? suit = null;

            foreach (string token in tokens)
            {
                if (token.Length == 1 && !rankMap.ContainsKey(token) && !suitMap.ContainsKey(token))
                {
                    card = null;
                    return false;
                }

                if (rank == null && TryFindMatch(token, rankMap, out Ranks foundRank))
                {
                    rank = foundRank;
                    continue;
                }
                if (suit == null && TryFindMatch(token, suitMap, out Suits foundSuit))
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

            card = new Card(suit.Value, rank.Value);

            return true;
        }

        public bool TryParseRank(string input, out Ranks? rank)
        {
            rank = null;
            string[] tokens = NormaliseInput(input).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var token in tokens)
            {
                if (TryFindMatch(token, rankMap, out Ranks foundRank))
                {
                    rank = foundRank;
                    return true;
                }
            }
            return false;
        }
    }
}