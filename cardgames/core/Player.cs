using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using static cardgames.core.Language;

namespace cardgames.core
{
    internal class Player
    {
        private const string USER_FOLDER_PATH = "..\\..\\..\\data\\user\\"; // In a real production environment, this data would be stored in %appdata% or similar location, in order to prevent people tampering with the user data or breaking things accidentally. However, for the purposes of this project, the data will be stored in a folder within the project directory, to make it easier to access and manage during development and testing.
        private static readonly Money DEFAULT_BALANCE = 1500;

        private protected string name;
        private protected string uname;
        private protected Money balance;
        private protected int rechargeCount;
        public Money Bet { get; private set; }
        private protected List<Card> hand = [];

        #region GETTERS & SETTERS

        // Getters
        public string GetName() => name;
        public string GetUsername() => uname;
        public Money GetBalance() => balance;

        #endregion

        #region CONSTRUCTORS

        // Constructors
        public Player(bool create = false)
        {
            if (create)
            {
                SetPreferredName();
                SetUsername();
                SetPassword();
                balance = DEFAULT_BALANCE;
                SaveUserData();
            }
        }
        public Player(string name)
        {
            this.name = name;
            SetUsername();
            SetPassword();
            balance = DEFAULT_BALANCE;
            rechargeCount = 0;
            SaveUserData();
        }
        public Player(string _name, string _uname, decimal _balance, int _recharges) // for logging in
        {
            name = _name;
            uname = _uname;
            balance = _balance;
            rechargeCount = _recharges;
        }
        private Player(string _name, string _uname)
        {
            name = _name;
            uname = _uname;
            balance = DEFAULT_BALANCE; // reset balance if failed to parse
            rechargeCount = 0; // reset recharges with balance since this effectively purges user data. This shouldn't ever happen unless they tamper with the file and then that serves them right for trying to cheat.
        }

        #endregion

        #region AUTHENTICATION

        // Authentication Credential Initialisation
        private void SetPreferredName()
        {
            string? input = "";
            Console.Write(T("Auth.Name.Preferred"));
            while (input == "")
            {
                input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    name = input;
                }
                else
                {
                    input = "";
                }
            }
        }
        private void SetUsername()
        {
            string path = USER_FOLDER_PATH;

            string[] users = Directory.GetFiles(path);

            string[] userNames = new string[users.Length];

            for (int i = 0; i < users.Length; i++)
            {
                userNames[i] = Path.GetFileNameWithoutExtension(users[i]);
            }

            if (!userNames.Contains(name.ToLower()))
            {
                Console.WriteLine(T("Auth.Username.NotTaken", ("name", name.ToLower())));
                if (Util.UserAgrees())
                {
                    uname = name.ToLower();
                    return;
                }
            }

            string? input = "";
            Console.Write(T("Auth.Username.Input"));

            while (input == "")
            {
                input = Console.ReadLine().ToLower();
                if (!string.IsNullOrEmpty(input))
                {
                    if (userNames.Contains(input))
                    {
                        Console.WriteLine(T("Auth.Username.Taken", ("username", input)));
                        input = "";
                    }
                    else
                    {
                        uname = input;
                    }
                }
                else
                {
                    input = "";
                }

            }

        }
        private void SetPassword()
        {
            // --- PASSWORD REQUIREMENTS ---

            int minChars = 8;
            int maxChars = 64;
            int minUppercase = 1;
            int minLowercase = 1;
            int minDigits = 1;
            int minSpecialChars = 1;

            // --- END OF PASSWORD REQUIREMENTS ---

            string? input = "";
            while (input == "")
            {
                Console.WriteLine(T("Auth.Password.Requirements", ("minChars", minChars.ToString()), ("maxChars", maxChars.ToString()), ("minUpperCase", minUppercase.ToString()), ("minLowerCase", minLowercase.ToString()), ("minDigits", minDigits.ToString()), ("minSpecialChars", minSpecialChars.ToString())));

                Console.Write(T("Auth.Password.Enter") + " >>> ");
                input = Util.GetPassword();
                if (!string.IsNullOrEmpty(input))
                {
                    int length = input.Length;
                    int uppercaseCount = input.Count(char.IsUpper);
                    int lowercaseCount = input.Count(char.IsLower);
                    int digitCount = input.Count(char.IsDigit);
                    int specialCharCount = input.Count(ch => !char.IsLetterOrDigit(ch));

                    if (length >= minChars && length <= maxChars && uppercaseCount >= minUppercase && lowercaseCount >= minLowercase && digitCount >= minDigits && specialCharCount >= minSpecialChars)
                    { // that line was fun to write
                        SavePassword(input);
                    }
                    else
                    {
                        input = "";
                        Console.WriteLine(T("Err.PasswordNotSecure"));
                    }
                }
            }
        }
        private void SavePassword(string password)
        {
            string path = USER_FOLDER_PATH;
            path = path + uname + ".userdata";

            int iterations = Random.Shared.Next(10000, 100000); // Number of iterations for hashing to increase security against brute-force attacks
            byte[] saltBytes = RandomNumberGenerator.GetBytes(16); // 128-bit salt for added security

            string dataToWrite = iterations + ":" + Convert.ToBase64String(saltBytes) + ":" + ComputeHash(password, saltBytes, iterations);


            if (!File.Exists(path))
            {
                File.WriteAllText(path, dataToWrite);
            }
            else
            {
                string[] fileContents = File.ReadAllLines(path);
                fileContents[0] = dataToWrite; // Set the first line to the new password
                File.WriteAllLines(path, fileContents); // Keep the rest intact
            }
        }

        // Authentication Routine
        public static Player? LogIn(string userName, string password)
        {
            userName = userName.Trim().ToLower();

            string path = USER_FOLDER_PATH;

            path = path + userName + ".userdata";

            if (!File.Exists(path))
            {
                return null; // return null if the username does not exist.
            }

            string[] fileData = File.ReadAllLines(path);

            string[] passwordData = fileData[0].Split(':');
            int iterations = int.Parse(passwordData[0]);
            byte[] saltBytes = Convert.FromBase64String(passwordData[1]);
            string storedHash = passwordData[2];
            int playerRechargeCount;

            if (ComputeHash(password, saltBytes, iterations) == storedHash)
            {
                string name = fileData[1];
                string balanceFromFile = fileData[2];
                if (int.TryParse(fileData[3], out int temp))
                {
                    playerRechargeCount = int.Parse(fileData[3]);
                }
                else
                {
                    playerRechargeCount = -1;
                }

                if (decimal.TryParse(balanceFromFile, out decimal balance) || playerRechargeCount == -1)
                {
                    Console.WriteLine(T("Auth.Login.PlayerFound", ("name", name), ("userName", userName), ("balance", balance.ToString())));
                    return new Player(name, userName, balance, playerRechargeCount);
                }
                else
                {
                    Console.WriteLine(T("Auth.Login.PlayerFoundNoBalance", ("name", name), ("userName", userName)));
                    Console.WriteLine(T("Auth.Login.BalanceNotLoaded", ("DEFAULT_BALANCE", DEFAULT_BALANCE.ToString())));
                    return new Player(name, userName);
                }

            }
            else
            {
                Console.WriteLine(T("Auth.Login.PlayerNotFound"));
                return null;
            }
        }

        // Authentication Helpers
        private static string ComputeHash(string data, byte[] saltBytes, int iterations)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data); // Convert the input into bytes
            byte[] saltedData = new byte[saltBytes.Length + dataBytes.Length]; // Combine the salt and the data

            for (int i = 0; i < saltBytes.Length; i++)
            {
                saltedData[i] = saltBytes[i];
            }
            for (int i = 0; i < dataBytes.Length; i++)
            {
                saltedData[saltBytes.Length + i] = dataBytes[i];
            } // Merges the salt & data into one array of bytes

            byte[] hashedData = SHA512.HashData(saltedData); // Hashes the data using the SHA512 algorithm

            for (int i = 1; i < iterations; i++)
            {
                hashedData = SHA512.HashData(hashedData); // Repeats the hashing a given number of times
            }

            return Convert.ToBase64String(hashedData); // Returns the hashed data & salt as a base-64 string so it is shorter but still as secure
        }

        #endregion

        // Importing Players (For Leaderboard)

        public static Dictionary<string, Money> GetLeaderboardPlayerData()
        {
            string[] userFiles = Directory.GetFiles(USER_FOLDER_PATH);
            string[] userData; string name; Money balance; int recharges;

            Dictionary<string, Money> leaderboardData = [];

            foreach (string file in userFiles)
            {
                userData = null;
                name = null;
                balance = -1;
                recharges = -1;
                try
                {
                    userData = File.ReadAllLines(file);
                    name = file.Replace(USER_FOLDER_PATH, "").Replace("userdata", "");
                    balance = Convert.ToDecimal(userData[2]);
                    recharges = Convert.ToInt32(userData[3]);
                }
                catch (Exception ex)
                {
                    // error in importing data; do not show this player on the leaderboard
                }

                balance -= DEFAULT_BALANCE * recharges;

                leaderboardData.Add(name, balance);
            }

            var sortedLeaderboardData = leaderboardData.OrderByDescending(val => val.Value);

            return sortedLeaderboardData.ToDictionary();
        }

        // Player Data Saving

        public void SaveUserData()
        {
            string path = USER_FOLDER_PATH + uname + ".userdata";
            string[] newData;

            if (File.Exists(path))
            {
                string[] data = [.. File.ReadAllLines(path)];
                newData = [data[0], name, balance.ToString(), rechargeCount.ToString()];
                File.WriteAllText(path, string.Empty); // empty existing file
                File.WriteAllLines(path, newData); // write new data
            }
            else
            {
                Console.WriteLine(T("Err.UserDataNotFound"));
            }
        }

        public static void SavePlayers(List<Player> players)
        {
            if (players.Count == 0)
            {
                return;
            }

            Util.StartLoading(T("Info.SavingData")); // TODO: LANG

            foreach (Player player in players)
            {
                player.SaveUserData();
            }

            Util.FinishLoading();
        }

        // Balance & Betting

        public void PlaceBet(Money amountToBet)
        {
            Bet += amountToBet;
        }

        public void DoubleBet()
        {
            Bet = new Money(Bet.Value * 2);
        }

        public void AddToBalance(Money amountToAdd)
        {
            balance += amountToAdd;
        }

        public void DeductFromBalance(Money amountToDeduct)
        {
            if (amountToDeduct > 0)
            {
                balance -= amountToDeduct;
            }
        }

        public Money DeductBetFromBalance()
        {
            Money bet = Bet;
            balance -= Bet;
            Bet = 0;
            return bet;
        }

        public Money AddBetToBalance()
        {
            Money bet = Bet;
            balance += Bet;
            Bet = 0;
            return bet;
        }

        public void RechargeBalance()
        {
            balance = 1500m;
            rechargeCount++;
        }

        // Gameplay
        public List<Card> GetHand()
        {
            return hand;
        }

        public void AddToHand(Card card) => hand.Add(card);
        public void AddToHand(List<Card> cards)
        {
            foreach (Card card in cards)
            {
                hand.Add(card);
            }
        }
        public void AddToHand(Deck deck)
        {
            foreach (Card card in deck.GetCards())
            {
                hand.Add(card);
            }
        }

        public Card PlayCard(Card card)
        {
            if (hand.Remove(card)) return card;
            throw new InvalidOperationException("Card not found in hand");
        }

        public List<Card> PlayCards(List<Card> cards)
        {
            List<Card> playedCards = [];
            foreach (Card card in cards)
            {
                if (hand.Remove(card))
                {
                    playedCards.Add(card);
                }
            }
            if (playedCards.Count > 0)
            {
                return playedCards;
            }
            throw new InvalidOperationException("None of the specified cards were found in hand");
        }

        // Static methods for sorting hands
        public void SortHandBySuit() {
            hand = [..hand.OrderBy(c => c.Suit).ThenBy(c => c.Rank)] ;
        }
        public void SortHandByRank() {
            hand = [..hand.OrderBy(c => c.Rank).ThenBy(c => c.Suit)] ;
        }

        // Selection Logic
        public void DeselectAllCards()
        {
            foreach (Card card in hand) card.Deselect();
        }
    }
}
