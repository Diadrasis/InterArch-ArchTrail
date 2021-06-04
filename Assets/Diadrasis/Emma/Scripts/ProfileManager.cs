﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfileManager : MonoBehaviour
{
    public delegate void Profile();
    public static Profile OnContinue, OnInternetError;

    public Button btnSubmit/*, BtnOk*/;
    public GameObject[] demographicOptions;
    public Transform dropdownContainer;
    public Transform inputContainer;
    public Transform dropdownContainerOptionA;
    public Transform toggleContainerOptionA;
    //public GameObject newIdPanel;
    //public Text newIdText, btnNextText, btnOkText;

    public TMP_Dropdown[] dropdowns;
    public TMP_InputField[] inputFields;
    public Toggle[] toggle;
    public TMP_Dropdown optionDropdown;
    int step = 0;

    public static string jsonFile;

    //public GameObject panelSettings;

    //Intro intro;

    private void Awake()
    {
        //BtnOk.onClick.AddListener(() => ContinueToSettings());
        //intro = FindObjectOfType<Intro>();
        btnSubmit.onClick.AddListener(() => Submit());
        dropdowns = dropdownContainer.GetComponentsInChildren<TMP_Dropdown>(true);
        inputFields = inputContainer.GetComponentsInChildren<TMP_InputField>(true);
        dropdowns = dropdownContainerOptionA.GetComponentsInChildren<TMP_Dropdown>(true);
        toggle = toggleContainerOptionA.GetComponentsInChildren<Toggle>(true);
    }

    void Start()
    {
        step = 0;
        
    }

    private void OnEnable()
    {
        //BtnOk.gameObject.SetActive(false);
        //newIdPanel.SetActive(false);
        btnSubmit.gameObject.SetActive(true);
        step = 0;
        foreach (GameObject gb in demographicOptions) gb.gameObject.SetActive(false);
        demographicOptions[0].SetActive(true);
        //btnNextText.text = "Επόμενο";

        //Debug.LogWarning("is new = " + StaticData.isNewVisitor);

        //also reset values?
        /*if (StaticData.isNewVisitor)
        {
            foreach (TMP_Dropdown dd in dropdowns) dd.value = 0;
        }
        else
        {
            //Load old visitor data
            ProfileItem profileItem = JsonUtility.FromJson<ProfileItem>(StaticData.visitorData);
            foreach (TMP_Dropdown dd in dropdowns)
            {
                for (int i = 0; i < dd.options.Count; i++)
                {
                    if (dd.options[i].text == profileItem.Age)
                    {
                        dd.value = i;
                        i = 1000;
                    }
                    else
                    if (dd.options[i].text == profileItem.Sex)
                    {
                        dd.value = i;
                        i = 1000;
                    }
                    else
                    if (dd.options[i].text == profileItem.Education)
                    {
                        dd.value = i;
                        i = 1000;
                    }
                    else
                    if (dd.options[i].text == profileItem.Technology)
                    {
                        dd.value = i;
                        i = 1000;
                    }
                    else
                    if (dd.options[i].text == profileItem.Familiarity)
                    {
                        StaticData.visitorFamiliarity = i.ToString();
                        dd.value = i;
                        i = 1000;
                    }
                    else
                    if (dd.options[i].text == profileItem.VisitReason)
                    {
                        dd.value = i;
                        i = 1000;
                    }
                }
            }
        }*/
    }

    void Submit()
    {
        demographicOptions[step].SetActive(false);

        if (step < demographicOptions.Length - 1)
        {
            //StopCoroutine("DelayShow");
            StartCoroutine(DelayShow());
            
            //if (step == demographicOptions.Length - 1) { btnNextText.text = "Αποστολή"; }
            
            if (step == 4)
            {
                if(optionDropdown.GetComponent<TMP_Dropdown>() != null)
                {
                    CheckValue(optionDropdown);
                    Debug.Log("Here");
                }
                
            }
            else
            {
                step++;
                demographicOptions[step].SetActive(true);
            }
            
        }
        
        //SaveProfile();
        /*else
        {
            SaveProfile();
        }*/
    }
    //for the dropdown which will make the different selections and open/close panels
    public void CheckValue(TMP_Dropdown val)
    {
        if (val.value == 0)
        {
            AppManager.Instance.uIManager.pnlWarningScreen.SetActive(true);
            AppManager.Instance.uIManager.txtWarning.text = "Please enter a valid option";
            Debug.Log("1st Option");
            return;
        }
        else if (val.value == 1)
        {
            step = 0;
            AppManager.Instance.uIManager.pnlOptionA.SetActive(true);
            AppManager.Instance.uIManager.pnlMainQuestions.SetActive(false);
            Debug.Log("2nd Option");
        }
        else if (val.value == 2)
        {
            AppManager.Instance.uIManager.pnlOptionB.SetActive(true);
            AppManager.Instance.uIManager.pnlMainQuestions.SetActive(false);
            Debug.Log("3rd Option");
        }
        else if (val.value == 3)
        {
            AppManager.Instance.uIManager.pnlOptionC1.SetActive(true);
            AppManager.Instance.uIManager.pnlMainQuestions.SetActive(false);
            Debug.Log("4th Option");
        }
        else if (val.value == 4)
        {
            AppManager.Instance.uIManager.pnlOptionC2.SetActive(true);
            AppManager.Instance.uIManager.pnlMainQuestions.SetActive(false);
            Debug.Log("5th Option");
        }

    }
    
    IEnumerator DelayShow()
    {
        btnSubmit.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.25f);
        btnSubmit.gameObject.SetActive(true);
        yield break;
    }

    /*void ContinueToSettings()
    {
        OnContinue?.Invoke();
    }*/

    void SaveProfile()
    {
        ProfileItem profileItem = new ProfileItem();

        foreach (TMP_Dropdown dd in dropdowns)
        {
            if (dd.transform.parent == demographicOptions[0].transform)//sex
            {
                profileItem.Sex = dd.options[dd.value].text;
            }
            else if (dd.transform.parent == demographicOptions[1].transform)//age
            {
                profileItem.Age = dd.options[dd.value].text;
            }
            else if (dd.transform.parent == demographicOptions[2].transform)//education
            {
                profileItem.VisitReason = dd.options[dd.value].text;
                
            }
            /*else if (dd.transform.parent == demographicOptions[3].transform)//tech familiarity
            {
                profileItem.Technology = dd.options[dd.value].text;
                profileItem.TechnologyINT = dd.value + 1;
            }
            else if (dd.transform.parent == demographicOptions[4].transform)//exhibit familiarity
            {
                profileItem.FamiliarityINT = dd.value + 1;
                profileItem.Familiarity = dd.options[dd.value].text;
                StaticData.visitorFamiliarity = dd.value.ToString();
            }
            else if (dd.transform.parent == demographicOptions[5].transform)//visit reason
            {
                profileItem.VisitReason = dd.options[dd.value].text;
            }*/
            //Debug.Log("Here on Save");
        }

        foreach(TMP_InputField fields in inputFields)
        {
            if (fields.transform.parent == demographicOptions[0].transform)//education
            {
                profileItem.Education = fields.onValueChanged.ToString();
            }
            else if (fields.transform.parent == demographicOptions[1].transform)//ethnicity
            {
                profileItem.Ethnicity = fields.onValueChanged.ToString();
            }
        }
        btnSubmit.gameObject.SetActive(false);

        /*string pass = Random.Range(100, 999).ToString();
        string visitorId = SystemInfo.deviceUniqueIdentifier.Substring(0, 3) + pass;*/

        /*if (StaticData.isNewVisitor)
        {
            profileItem.VisitorId = visitorId;
            StaticData.visitorId = visitorId;
            profileItem.VisitorPass = pass;
            StaticData.visitorPass = pass;
        }
        else
        {
            profileItem.VisitorId = StaticData.visitorId;
            profileItem.VisitorPass = StaticData.visitorPass;
        }
*/

        //StopCoroutine("PostVisitorData");
        //StartCoroutine(PostVisitorData(profileItem));
    }

    /*IEnumerator PostVisitorData(ProfileItem profileItem)
    {
       // CoroutineWithData cd = new CoroutineWithData(this, ServerManager.Instance.CreateVisitor(profileItem));// ServerManager.Instance.SendDemographicDataToServer(profileItem));
        yield return cd.coroutine;

        Debug.Log("result is " + cd.result);  //  'success' or 'fail'

        if (cd.result.ToString().Contains("destination host"))
        {
            OnInternetError?.Invoke();
            yield break;
        }
        else
        if (cd.result.ToString() == "error")
        {
            newIdText.text = StaticData.errorOnPostData();
            BtnOk.gameObject.SetActive(false);
        }
        else
        {
            if (StaticData.isNewVisitor)
            {
                newIdText.text = string.Empty;
                newIdText.text = StaticData.newVisitorMessage(profileItem.VisitorId, profileItem.VisitorPass);
                btnOkText.text = "Εντάξει. Τα θυμάμαι.";
            }
            else
            {
                newIdText.text = string.Empty;
                newIdText.text = StaticData.dataUpdated();
                btnOkText.text = "Ξενάγηση";
            }
            BtnOk.gameObject.SetActive(true);
        }

        newIdPanel.SetActive(true);

        //Debug.LogWarning(StaticData.visitorData);

        yield break;
    }*/

    /*public void GetFamiliarity()
    {
        ProfileItem profileItem = JsonUtility.FromJson<ProfileItem>(StaticData.visitorData);
        foreach (TMP_Dropdown dd in dropdowns)
        {
            for (int i = 0; i < dd.options.Count; i++)
            {
                if (dd.options[i].text == profileItem.Familiarity)
                {
                    StaticData.visitorFamiliarity = i.ToString();
                }
            }
        }
    }*/


}
