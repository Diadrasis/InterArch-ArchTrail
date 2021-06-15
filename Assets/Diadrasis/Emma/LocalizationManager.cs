using System.Collections;
using System.Collections.Generic;

public class LocalizationManager
{
    public enum Language
    {
        English,
        Greek
    }

    public static Language language = Language.English;

    public static Dictionary<string, string> localisedEN;
    public static Dictionary<string, string> localisedGR;

    public static bool isInit;

    public static void Init()
    {
        CSVLoader cSVLoader = new CSVLoader();
        cSVLoader.LoadCSV();

        localisedEN = cSVLoader.GetDictionaryValues("en");
        localisedGR = cSVLoader.GetDictionaryValues("el");

        isInit = true;
    }

    public static string GetLocalizedValue(string key)
    {
        if (!isInit) { Init(); }

        string value = key;

        switch (language)
        {
            case Language.English:
                localisedEN.TryGetValue(key, out value);
                break;
            case Language.Greek:
                localisedGR.TryGetValue(key, out value);
                break;
        }
        return value;
    }
}
