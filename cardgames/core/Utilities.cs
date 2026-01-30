namespace cardgames.core;

internal static class Utilities
{
    public static char ConvertSuitToSymbol(Card.Suit suit)
    {
        if (true) // need to fix, set to ifUnicode = true eventually
        {
            switch (suit)
            {
                case Card.Suit.heart:
                    return '♥';
                case Card.Suit.diamond:
                    return '♦';
                case Card.Suit.spade:
                    return '♠';
                case Card.Suit.club:
                    return '♣';
            }
        }
        else
        {
            switch (suit)
            {
                case Card.Suit.heart:
                    return 'H';
                case Card.Suit.diamond:
                    return 'D';
                case Card.Suit.spade:
                    return 'S';
                case Card.Suit.club:
                    return 'C';
            }
        }
        return '?';
    } // ascii representation unused; fix later️™️

    public static string ConvertRankToSymbol(Card.Rank rank)
    {
        switch (rank)
        {
            case Card.Rank.ace:
                return "A";
            case Card.Rank.two:
                return "2";
            case Card.Rank.three:
                return "3";
            case Card.Rank.four:
                return "4";
            case Card.Rank.five:
                return "5";
            case Card.Rank.six:
                return "6";
            case Card.Rank.seven:
                return "7";
            case Card.Rank.eight:
                return "8";
            case Card.Rank.nine:
                return "9";
            case Card.Rank.ten:
                return "10";
            case Card.Rank.jack:
                return "J";
            case Card.Rank.queen:
                return "Q";
            case Card.Rank.king:
                return "K";
            default:
                return "?";
        }
    }
    public static T ConvertStringToEnum<T>(string input) where T : struct, Enum
    {
        return Enum.Parse<T>(input, true);
    }

    public static string StripCurrencySymbols(string input)
    {
        foreach (char c in input)
        {
            if (!char.IsDigit(c) && c != '.' && c != '-' && c != ',')
            {
                input = input.Replace(c.ToString(), "");
            }
        }
        return input;
    }
}
