using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using TMPro;
using UnityEngine.UI;

public class LocaleDropdown : MonoBehaviour
{
    public TMP_Dropdown buttonUI;
    //public Button buttonUI;
    IEnumerator Start()
    {
        // Wait for the localization system to initialize, loading Locales, preloading etc.
        yield return LocalizationSettings.InitializationOperation;

        
        // Generate list of available Locales
        var options = new List<TMP_Dropdown.OptionData>();
        int selected = 0;
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            if (LocalizationSettings.SelectedLocale == locale)
            {
                selected = i;

                /*if (locale.Identifier.Code == "en")
                {
                    buttonUI.GetComponentInChildren<TextMeshProUGUI>().text = locale.Identifier.Code.ToUpper();
                    buttonUI.onClick.AddListener(()=>LocaleSelected(selected));
                }
                else if (locale.Identifier.Code == "el")
                {
                    buttonUI.GetComponentInChildren<TextMeshProUGUI>().text = locale.Identifier.Code.ToUpper();
                    buttonUI.onClick.AddListener(() => LocaleSelected(selected));
                }*/
                //Debug.Log("Locale identifier = "+locale.Identifier.Code);

            }

           options.Add(new TMP_Dropdown.OptionData(locale.Identifier.Code.ToUpper()));
        }
       buttonUI.options = options;

        //buttonUI.GetComponentInChildren<TextMeshProUGUI>().text = selected.ToString();
        buttonUI.onValueChanged.AddListener(LocaleSelected);
        if (Application.systemLanguage == SystemLanguage.Greek)
        {
            
            //Outputs into console that the system is French
            Debug.Log("This system is in Greek. ");
        }
        //Otherwise, if the system is English, output the message in the console
        else if (Application.systemLanguage == SystemLanguage.English)
        {
            Debug.Log("This system is in English. ");
        }

    }

    public void LocaleSelected(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        AppManager.Instance.uIManager.CheckLanguage();
        Debug.Log("Index num: "+index);
    }
}