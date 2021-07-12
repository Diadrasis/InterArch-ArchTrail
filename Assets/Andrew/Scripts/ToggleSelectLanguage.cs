using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class ToggleSelectLanguage : MonoBehaviour
{
    public Toggle toggle;
    public TextMeshProUGUI txtDisplay;

    private void Awake()
    {
        if (!LocalizationSettings.InitializationOperation.IsDone)
            StartCoroutine(InitializeLocalizationSettings());
    }

    private void OnEnable()
    {
        if (LocalizationSettings.InitializationOperation.IsDone)
            ChangeTextDisplay();
    }

    IEnumerator InitializeLocalizationSettings()
    {
        // Wait for the localization system to initialize, loading Locales, preloading etc.
        yield return LocalizationSettings.InitializationOperation;

        // Initialize language at English
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
        txtDisplay.text = "EN";
    }

    public void ChangeTextDisplay()
    {
        if (LocalizationSettings.SelectedLocale.Equals(LocalizationSettings.AvailableLocales.Locales[0])) // Greek
            txtDisplay.text = "EL";
        else
            txtDisplay.text = "EN";
    }

    public void ChangeLanguage()
    {
        // Change language
        if (LocalizationSettings.SelectedLocale.Equals(LocalizationSettings.AvailableLocales.Locales[0])) // Greek
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
        else
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];

        // CHange text
        ChangeTextDisplay();
    }
}
