﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ProfileManager : MonoBehaviour
{
    public delegate void Profile();
    public static Profile OnContinue, OnInternetError;

    public Button btnSubmit, btnSkip/*, BtnOk*/;
    public GameObject[] demographicOptions;
    public Transform dropdownContainer;
    public Transform inputContainer;
    public Transform dropdownContainerOptionA, dropdownContainerOptionB1, dropdownContainerOptionB2, dropdownContainerOptionC1, dropdownContainerOptionC2;
    public Transform toggleContainerOptionA, toggleContainerOptionB, toggleContainerOptionB1, toggleContainerOptionB2, toggleContainerOptionC, toggleContainerOptionC1, toggleContainerOptionC2;

    //public GameObject newIdPanel;
    //public Text newIdText, btnNextText, btnOkText;

    public TMP_Dropdown[] dropdownsGeneral, dropdownsOptionA, dropdownsOptionB1, dropdownsOptionB2, dropdownsOptionC1, dropdownsOptionC2;
    public TMP_InputField[] inputFieldsGeneral;
    public Toggle[] toggleOptionA, toggleOptionB1, toggleOptionB2,toggleOptionC1,toggleOptionC2;
    public TMP_Dropdown optionDropdown;

    public Toggle groupB1, groupB2, groupC1, groupC2;
    [HideInInspector] public int step=0;

    public static string jsonFile;

    //public GameObject panelSettings;

    //Intro intro;

    private void Awake()
    {
        //BtnOk.onClick.AddListener(() => ContinueToSettings());
        //intro = FindObjectOfType<Intro>();
        btnSubmit.onClick.AddListener(() => Submit());
        btnSkip.onClick.AddListener(() => Skip());
        dropdownsGeneral = dropdownContainer.GetComponentsInChildren<TMP_Dropdown>(true);
        inputFieldsGeneral = inputContainer.GetComponentsInChildren<TMP_InputField>(true);
        dropdownsOptionA = dropdownContainerOptionA.GetComponentsInChildren<TMP_Dropdown>(true);
        toggleOptionA = toggleContainerOptionA.GetComponentsInChildren<Toggle>(true);
        dropdownsOptionB1 = dropdownContainerOptionB1.GetComponentsInChildren<TMP_Dropdown>(true);
        dropdownsOptionB2 = dropdownContainerOptionB2.GetComponentsInChildren<TMP_Dropdown>(true);
        dropdownsOptionC1 = dropdownContainerOptionC1.GetComponentsInChildren<TMP_Dropdown>(true);
        dropdownsOptionC2 = dropdownContainerOptionC2.GetComponentsInChildren<TMP_Dropdown>(true);
        toggleOptionB1 = toggleContainerOptionB1.GetComponentsInChildren<Toggle>(true);
        toggleOptionB2 = toggleContainerOptionB2.GetComponentsInChildren<Toggle>(true);
        toggleOptionC1 = toggleContainerOptionC1.GetComponentsInChildren<Toggle>(true);
        toggleOptionC2 = toggleContainerOptionC2.GetComponentsInChildren<Toggle>(true);
        groupB1 = toggleContainerOptionB.GetComponentInChildren<Toggle>(true);
        groupB2 = toggleContainerOptionB.GetComponentInChildren<Toggle>(true);
        groupC1 = toggleContainerOptionC.GetComponentInChildren<Toggle>(true);
        groupC2 = toggleContainerOptionC.GetComponentInChildren<Toggle>(true);
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
                if (optionDropdown.GetComponent<TMP_Dropdown>() != null)
                {
                    CheckValue(optionDropdown);
                    demographicOptions[step].SetActive(true);
                    Debug.Log("Here");
                }
                //step = 5;
            }
            else
            {

                step++;
                demographicOptions[step].SetActive(true);
            }
            Debug.Log("Step: " + step);
        }

        //SaveProfile();
        /*else
        {
            SaveProfile();
        }*/
    }

    //because we don't want to save in case user skips a step. Does the same as Submit() but in submit we will save the answers...
    void Skip()
    {
        demographicOptions[step].SetActive(false);

        if (step < demographicOptions.Length - 1)
        {
            //StopCoroutine("DelayShow");
            StartCoroutine(DelayShow());
        }
        if (step == 4)
        {
            if (optionDropdown.GetComponent<TMP_Dropdown>() != null)
            {
                CheckValue(optionDropdown);
                demographicOptions[step].SetActive(true);
                Debug.Log("Here");
            }
            //step = 5;
        }
        else
        {

            step++;
            demographicOptions[step].SetActive(true);
        }
        Debug.Log("Step: " + step);

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
            //step = 0;
            AppManager.Instance.uIManager.pnlOptionA.SetActive(true);
            AppManager.Instance.uIManager.pnlMainQuestions.SetActive(false);
            step = 5;
            Debug.Log("2nd Option");
        }
        else if (val.value == 2)
        {
            AppManager.Instance.uIManager.pnlOptionB.SetActive(true);
            AppManager.Instance.uIManager.pnlMainQuestions.SetActive(false);
            //step = 6;
            CheckToggle();
            Debug.Log("3rd Option" + step);
        }
        else if (val.value == 3)
        {
            AppManager.Instance.uIManager.pnlOptionC.SetActive(true);
            AppManager.Instance.uIManager.pnlMainQuestions.SetActive(false);
            //step = 7;
            CheckToggle();
            Debug.Log("4th Option");
        }

    }


    //for testing I'll remove Toggle from parenthesis and leave it as is
    void CheckToggle()
    {
        if (/*id.isOn == */groupB1.isOn)
        {
            AppManager.Instance.uIManager.pnlOptionB1.SetActive(true);
            AppManager.Instance.uIManager.pnlOptionB2.SetActive(false);
            toggleContainerOptionB.gameObject.SetActive(false);
            step = 7;
            Debug.Log("groupB1 will be on");
        }
        else if (/*id.isOn ==*/ groupB2.isOn)
        {
            AppManager.Instance.uIManager.pnlOptionB2.SetActive(true);
            AppManager.Instance.uIManager.pnlOptionB1.SetActive(false);
            toggleContainerOptionB.gameObject.SetActive(false);
            step = 7;
            Debug.Log("groupB2 will be on");
        }
        else if (/*id.isOn ==*/ groupC1.isOn)
        {
            AppManager.Instance.uIManager.pnlOptionC1.SetActive(true);
            AppManager.Instance.uIManager.pnlOptionC2.SetActive(false);
            toggleContainerOptionC.gameObject.SetActive(false);
            step = 7;
            Debug.Log("groupC1 will be on");
        }
        else if (/*id.isOn == */groupC2.isOn)
        {
            AppManager.Instance.uIManager.pnlOptionC2.SetActive(true);
            AppManager.Instance.uIManager.pnlOptionC1.SetActive(false);
            toggleContainerOptionC.gameObject.SetActive(false);
            step = 7;
            Debug.Log("groupC2 will be on");
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

        foreach (TMP_Dropdown dd in dropdownsGeneral)
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

        foreach (TMP_InputField fields in inputFieldsGeneral)
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
