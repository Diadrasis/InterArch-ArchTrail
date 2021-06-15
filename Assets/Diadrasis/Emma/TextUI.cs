using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextUI : MonoBehaviour
{
    TextMeshProUGUI textField;

    public string key;
    // Start is called before the first frame update
    void Start()
    {
        textField = GetComponent<TextMeshProUGUI>();
        string value = LocalizationManager.GetLocalizedValue(key);
        textField.text = value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
