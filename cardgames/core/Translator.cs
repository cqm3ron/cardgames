using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace cardgames.core
{
    public static class Translator
    {
        private static Dictionary<string, string> translations = [];
        private const string LANGUAGE_DIRECTORY_PATH = "..\\..\\..\\lang\\";
        private const string DEFAULT_LANGUAGE_CODE = "en-GB";

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
            string file = LANGUAGE_DIRECTORY_PATH + languageCode + ".json";
            string json = File.ReadAllText(file);
            translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        }

        public static string T(string key, Dictionary<string, string>? parameters = null)
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

    }
}
