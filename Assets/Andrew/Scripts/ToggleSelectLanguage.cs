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
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
        txtDisplay.text = "EN";
    }

    public void ChangeTextDisplay()
    {
        if (toggle.isOn)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
            txtDisplay.text = "EN";
        }
        else
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
            txtDisplay.text = "EL";
        }
    }
}
