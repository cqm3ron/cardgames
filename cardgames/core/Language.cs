using cardgames.core.extension;
using System.Text.Json;

namespace cardgames.core
{
    internal static class Language
    {
        private static Dictionary<string, string> translations = [];
        private const string LANGUAGE_DIRECTORY_PATH = "..\\..\\..\\lang\\";
        private const string DEFAULT_LANGUAGE_CODE = "en-GB";
        public static string CurrentLanguage { get; private set; } = DEFAULT_LANGUAGE_CODE;

        public static void SelectLanguage()
        {
            Console.WriteLine($"=== {T("Lang.Select")} ===");
            string[] languages = DetectLanguages();
            int choice = Util.GetChoice(languages);
            Load(languages[choice]);
            Console.WriteLine(T("Lang.NowUsing"));
        }

        private static string[] DetectLanguages()
        {
            string[] languages = [];
            languages = Directory.GetFiles(LANGUAGE_DIRECTORY_PATH);
            for (int i = 0; i < languages.Length; i++)
            {
                string language = languages[i];
                language = language.Replace(LANGUAGE_DIRECTORY_PATH, "");
                language = language.Replace(".json", "");
                languages[i] = language;
            }
            return languages;
        }

        public static void Load(string languageCode = DEFAULT_LANGUAGE_CODE)
        {
            CurrentLanguage = languageCode;
            string file = LANGUAGE_DIRECTORY_PATH + languageCode + ".json";
            string json = File.ReadAllText(file);
            translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        }

        public static void LoadGame(string gameName)
        {
            string locale = CurrentLanguage;
            string gameLangDirectoryPath = $"..\\..\\..\\game\\{gameName.ToLower()}\\lang\\{locale}.json";

            string json = File.ReadAllText(gameLangDirectoryPath);
            Dictionary<string, string> importedTranslations = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            translations.AddRange(importedTranslations);

        }

        public static void UnloadGame(string gameName)
        {
            string locale = CurrentLanguage;
            string gameLangDirectoryPath = $"..\\..\\..\\game\\{gameName.ToLower()}\\lang\\{locale}.json";

            string json = File.ReadAllText(gameLangDirectoryPath);
            Dictionary<string, string> importedTranslations = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            translations.RemoveRange(importedTranslations);
        }

        private static string T(string key, Dictionary<string, string>? parameters = null) // internal parameter system
        {
            if (!translations.TryGetValue(key, out string? value))
            {
                return $"[{key}]";
            }

            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    value = value.Replace($"{{{p.Key}}}", p.Value);
                }
            }

            return value;
        }

        public static string T(string key, params (string Key, string Value)[] parameters) // public parameter system
        {
            Dictionary<string, string> paramDict = parameters.ToDictionary(p => p.Key, p => p.Value);
            return T(key, paramDict);
        }
    }
}
