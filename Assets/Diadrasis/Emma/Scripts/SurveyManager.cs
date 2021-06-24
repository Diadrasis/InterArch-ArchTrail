using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;
using System.Linq;

public class SurveyManager : MonoBehaviour
{
    public delegate void Profile();
    public static Profile OnContinue, OnInternetError;

    public Button btnSubmit, btnSkip/*, BtnOk*/;
    public GameObject[] demographicOptions;
    public Transform dropdownContainer;
    public Transform inputContainer, inputContainerOptionA, inputContainerOptionB1, inputContainerOptionB2, inputContainerOptionC1, inputContainerOptionC2;
    public Transform toggleContainerOptionA, toggleContainerOptionB, toggleContainerOptionB1, toggleContainerOptionB2, toggleContainerOptionC, toggleContainerOptionC1, toggleContainerOptionC2;

    //public GameObject newIdPanel;
    //public Text newIdText, btnNextText, btnOkText;
    public TextMeshProUGUI textB, textC;
    public TMP_Dropdown[] dropdownsGeneral;
    public TMP_InputField[] inputFieldsGeneral, inputFieldOptionA, inputFieldOptionB1, inputFieldOptionB2, inputFieldOptionC1, inputFieldOptionC2;
    public Toggle[] toggleOptionA, toggleOptionB1, toggleOptionB2, toggleOptionC1, toggleOptionC2, questionToggle/*, questionToggleB1*/;
    public TMP_Dropdown optionDropdown;

    public Toggle[] choiceTogglesB, choiceTogglesC/*groupB1, groupB2, groupC1, groupC2*/;
    [HideInInspector] public int step = 0;

    public static string jsonFile;

    public cPath currentPath;

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
        toggleOptionA = toggleContainerOptionA.GetComponentsInChildren<Toggle>(true);
        
        toggleOptionB1 = toggleContainerOptionB1.GetComponentsInChildren<Toggle>(true);
        toggleOptionB2 = toggleContainerOptionB2.GetComponentsInChildren<Toggle>(true);
        toggleOptionC1 = toggleContainerOptionC1.GetComponentsInChildren<Toggle>(true);
        toggleOptionC2 = toggleContainerOptionC2.GetComponentsInChildren<Toggle>(true);
        inputFieldOptionA = inputContainerOptionA.GetComponentsInChildren<TMP_InputField>(true);
        inputFieldOptionB1 = inputContainerOptionB1.GetComponentsInChildren<TMP_InputField>(true);
        inputFieldOptionB2 = inputContainerOptionB2.GetComponentsInChildren<TMP_InputField>(true);
        inputFieldOptionC1 = inputContainerOptionC1.GetComponentsInChildren<TMP_InputField>(true);
        inputFieldOptionC2 = inputContainerOptionC2.GetComponentsInChildren<TMP_InputField>(true);

        choiceTogglesB = toggleContainerOptionB.GetComponentsInChildren<Toggle>(true);
        choiceTogglesC = toggleContainerOptionC.GetComponentsInChildren<Toggle>(true);
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

        if (step < demographicOptions.Length)
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
                    //Debug.Log("Here");
                }
                //step = 5;
            }
            else if (step == 9 || step == 13 || step == 16 || step == 20 || step == 27 || step == 31
                || step == 37 || step == 41 || step == 43 || step == 47 || step == 51 || step == 60
                || step == 64 || step == 67 || step == 71 || step == 76 || step == 80 || step == 84
                || step == 89 || step == 93)
            {
                //Debug.Log("Step for toggleCheck" + step);
                CheckIfUserHasSelectedOtherOption();
            }
            else if (step == 11 || step == 18 || step == 29 || step == 33 || step == 39
            || step == 45 || step == 49 || step == 53 || step == 62 || step == 69 || step == 78 || step == 82
            || step == 86 || step == 91)
            {
                //Debug.Log("Step for toggleCheck Second method: " + step);
                CheckIfUserHasSelectedOtherOptionSecond();
            }
            else if (step == 21 || step == 42 || step == 55 || step == 72 || step == 94)
            {
                step = demographicOptions.Length;
                //btnSkip.gameObject.SetActive(false);
            }
            else if (step == 22 || step == 56)
            {
                btnSkip.gameObject.SetActive(false);
                CheckToggle();
            }
            else
            {
                step++;
                demographicOptions[step].SetActive(true);
                btnSkip.gameObject.SetActive(true);
            }
            
            if (step == 4 || step == 22 || step == 56)
            {
                btnSkip.gameObject.SetActive(false);
            }
            else
            {
                btnSkip.gameObject.SetActive(true);
            }
            Debug.Log("Step: " + step);
        }

        // End survey and save
        if (step >= demographicOptions.Length)
        {
            EndSurvey();
        }
    }

    //because we don't want to save in case user skips a step. Does the same as Submit() but in submit we will save the answers...
    void Skip()
    {
        demographicOptions[step].SetActive(false);

        if (step < demographicOptions.Length)
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
                //Debug.Log("Here");
            }

            //step = 5;
        }
        else if (step == 9 || step == 13 || step == 16 || step == 20 || step == 27
                || step == 31 || step == 37 || step == 41 || step == 43 || step == 47 || step == 51
                || step == 60 || step == 64 || step == 67 || step == 71 || step == 76 || step == 80 || step == 84
                || step == 89 || step == 93)
        {
            //Debug.Log("Step for toggleCheck" + step);
            CheckIfUserHasSelectedOtherOption();
        }
        else if (step == 11 || step == 18 || step == 29 || step == 33 || step == 39
            || step == 45 || step == 49 || step == 53 || step == 62 || step == 69 || step == 78 || step == 82
            || step == 86 || step == 91)
        {
            //Debug.Log("Step for toggleCheck Second method: " + step);
            CheckIfUserHasSelectedOtherOptionSecond();

            //Debug.Log("Step: " + step);

        }
        else if (step == 55)
        {
            step = demographicOptions.Length;
        }
        else
        {
            step++;
            demographicOptions[step].SetActive(true);
        }

        if(step == 4 || step == 22 || step == 56)
        {
            btnSkip.gameObject.SetActive(false);
        }
        else
        {
            btnSkip.gameObject.SetActive(true);
        }

        // End survey and save
        if (step >= demographicOptions.Length)
        {
            EndSurvey();
        }
    }
    private void EndSurvey()
    {
        if (currentPath != null)
        {
            SaveSurvey();

            // Test
            /*cSurvey loadedSurvey = cSurvey.Load(currentPath.local_path_id);

            if (loadedSurvey != null)
            {
                //Debug.Log("Answers:");
                //Debug.Log("loadedSurvey.answers.Count" + loadedSurvey.answers.Count);
                foreach (string answer in loadedSurvey.answers)
                {
                    Debug.Log(answer);
                }
            }*/
        }

        //close questionnairePanel and go to DisplayAreas and reset questionnaire for new user
        AppManager.Instance.uIManager.pnlQuestionnaireScreen.SetActive(false);
        ResetValues();
        AppManager.Instance.uIManager.DisplayAreasScreen();
        AppManager.Instance.uIManager.pnlWarningThankYouScreen.SetActive(true);
    }

    //for the dropdown which will make the different selections and open/close panels
    public void CheckValue(TMP_Dropdown val)
    {
        if (val.value == 0)
        {
            AppManager.Instance.uIManager.pnlWarningScreen.SetActive(true);
            AppManager.Instance.uIManager.txtWarning.text = "Please enter a valid option";
            return;
        }
        else if (val.value == 1)
        {
            //step = 0;
            AppManager.Instance.uIManager.pnlOptionA.SetActive(true);
            AppManager.Instance.uIManager.pnlMainQuestions.SetActive(false);
            step = 5;
        }
        else if (val.value == 2)
        {
            step = 22;
            AppManager.Instance.uIManager.pnlOptionB.SetActive(true);
            AppManager.Instance.uIManager.pnlMainQuestions.SetActive(false);
            btnSkip.gameObject.SetActive(false);
        }
        else if (val.value == 3)
        {
            step = 56;
            AppManager.Instance.uIManager.pnlOptionC.SetActive(true);
            AppManager.Instance.uIManager.pnlMainQuestions.SetActive(false);
            btnSkip.gameObject.SetActive(false);
        }

    }
    //when selecting the panels with the options, to see which option is selected and then activate the coresponding panel
    void CheckToggle()
    {
        btnSkip.gameObject.SetActive(false);

        if (step == 22 && choiceTogglesB[0].isOn)
        {
            AppManager.Instance.uIManager.pnlOptionB1.SetActive(true);
            AppManager.Instance.uIManager.pnlOptionB2.SetActive(false);
            toggleContainerOptionB.gameObject.SetActive(false);
            textB.gameObject.SetActive(false);
            step = 23;
            demographicOptions[step].SetActive(true);
            //Debug.Log("Is first toggle on: "+choiceTogglesB[0].isOn);
           
        }
        else if (step == 22 && choiceTogglesB[1].isOn)
        {
            
            AppManager.Instance.uIManager.pnlOptionB1.SetActive(false);
            AppManager.Instance.uIManager.pnlOptionB2.SetActive(true);
            toggleContainerOptionB.gameObject.SetActive(false);
            textB.gameObject.SetActive(false);
            step = 43;
            demographicOptions[step].SetActive(true);

        }
        if (step == 56 && choiceTogglesC[0].isOn)
        {
            AppManager.Instance.uIManager.pnlOptionC1.SetActive(true);
            AppManager.Instance.uIManager.pnlOptionC2.SetActive(false);
            toggleContainerOptionC.gameObject.SetActive(false);
            textC.gameObject.SetActive(false);
            step = 57;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 56 && choiceTogglesC[1].isOn)
        {
            AppManager.Instance.uIManager.pnlOptionC2.SetActive(true);
            AppManager.Instance.uIManager.pnlOptionC1.SetActive(false);
            toggleContainerOptionC.gameObject.SetActive(false);
            textC.gameObject.SetActive(false);
            step = 73;
            demographicOptions[step].SetActive(true);
        }
    }

    void CheckIfUserHasSelectedOtherOption()
    {
        if (step == 9 && questionToggle[0].isOn)
        {
            step = 10;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 9 && !questionToggle[0].isOn)
        {
            step = 11;
            demographicOptions[step].SetActive(true);
        }
        if (step == 13 && questionToggle[23].isOn)
        {
            step = 14;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 13 && !questionToggle[23].isOn)
        {
            step = 15;
            demographicOptions[step].SetActive(true);
        }

        if (step == 16 && questionToggle[2].isOn)
        {
            step = 17;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 16 && !questionToggle[2].isOn)
        {
            step = 18;
            demographicOptions[step].SetActive(true);
        }

        if(step == 20 && questionToggle[25].isOn)
        {
            step = 21;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 20 && !questionToggle[25].isOn)
        {
            step = demographicOptions.Length;
        }
        if (step == 27 && questionToggle[21].isOn)
        {
            step = 28;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 27 && !questionToggle[21].isOn)
        {
            step = 29;
            demographicOptions[step].SetActive(true);
        }

        if(step == 31 && questionToggle[4].isOn)
        {
            step = 32;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 31 && !questionToggle[4].isOn)
        {
            step = 33;
            demographicOptions[step].SetActive(true);
        }

        if (step == 37 && questionToggle[6].isOn)
        {
            step = 38;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 37 && !questionToggle[6].isOn)
        {
            step = 39;
            demographicOptions[step].SetActive(true);
        }
        if(step == 41 && questionToggle[27].isOn)
        {
            step = 42;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 41 && !questionToggle[27].isOn)
        {
            step = demographicOptions.Length;
        }
        if (step == 43 && questionToggle[7].isOn)
        {
            step = 44;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 43 && !questionToggle[7].isOn)
        {
            step = 45;
            demographicOptions[step].SetActive(true);
        }

        if (step == 47 && questionToggle[9].isOn)
        {
            step = 48;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 47 && !questionToggle[9].isOn)
        {
            step = 49;
            demographicOptions[step].SetActive(true);
        }

        if (step == 51 && questionToggle[28].isOn)
        {
            step = 52;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 51 && !questionToggle[28].isOn)
        {
            step = 53;
            demographicOptions[step].SetActive(true);
        }

        if (step == 60 && questionToggle[11].isOn)
        {
            step = 61;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 60 && !questionToggle[11].isOn)
        {
            step = 62;
            demographicOptions[step].SetActive(true);
        }
        if (step == 64 && questionToggle[13].isOn)
        {
            step = 65;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 64 && !questionToggle[13].isOn)
        {
            step = 66;
            demographicOptions[step].SetActive(true);
        }
        if (step == 67 && questionToggle[14].isOn)
        {
            step = 68;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 67 && !questionToggle[14].isOn)
        {
            step = 69;
            demographicOptions[step].SetActive(true);
        }
        if (step == 71 && questionToggle[31].isOn)
        {
            step = 72;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 71 && !questionToggle[31].isOn)
        {
            step = demographicOptions.Length;
        }

        if (step == 76 && questionToggle[15].isOn)
        {
            step = 77;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 76 && !questionToggle[15].isOn)
        {
            step = 78;
            demographicOptions[step].SetActive(true);
        }
        if (step == 80 && questionToggle[17].isOn)
        {
            step = 81;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 80 && !questionToggle[17].isOn)
        {
            step = 82;
            demographicOptions[step].SetActive(true);
        }
        if (step == 84 && questionToggle[18].isOn)
        {
            step = 85;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 84 && !questionToggle[18].isOn)
        {
            step = 86;
            demographicOptions[step].SetActive(true);
        }
        if (step == 89 && questionToggle[20].isOn)
        {
            step = 90;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 89 && !questionToggle[20].isOn)
        {
            step = 91;
            demographicOptions[step].SetActive(true);
        }
        if (step == 93 && questionToggle[33].isOn)
        {
            step = 94;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 93 && !questionToggle[33].isOn)
        {
            step = demographicOptions.Length;
        }

    }

    void CheckIfUserHasSelectedOtherOptionSecond() 
    {
        if (step == 11 && questionToggle[1].isOn)
        {
            step = 12;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 11 && !questionToggle[1].isOn)
        {
            step = 13;
            demographicOptions[step].SetActive(true);
        }

        if (step == 18 && questionToggle[24].isOn)
        {
            step = 19;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 18 && !questionToggle[24].isOn)
        {
            step = 20;
            demographicOptions[step].SetActive(true);
        }

        if (step == 29 && questionToggle[3].isOn)
        {
            step = 30;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 29 && !questionToggle[3].isOn)
        {
            step = 31;
            demographicOptions[step].SetActive(true);
        }

        if(step == 33 && questionToggle[5].isOn)
        {
            step = 34;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 33 && !questionToggle[5].isOn)
        {
            step = 35;
            demographicOptions[step].SetActive(true);
        }

        if (step == 39 && questionToggle[26].isOn)
        {
            step = 40;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 39 && !questionToggle[26].isOn)
        {
            step = 41;
            demographicOptions[step].SetActive(true);
        }
        if (step == 45 && questionToggle[8].isOn)
        {
            step = 46;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 45 && !questionToggle[8].isOn)
        {
            step = 47;
            demographicOptions[step].SetActive(true);
        }
        if (step == 49 && questionToggle[10].isOn)
        {
            step = 50;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 49 && !questionToggle[10].isOn)
        {
            step = 51;
            demographicOptions[step].SetActive(true);
        }
        if(step == 53 && questionToggle[29].isOn)
        {
            step = 54;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 53 && !questionToggle[29].isOn)
        {
            step = demographicOptions.Length;
        }
        if (step == 62 && questionToggle[12].isOn)
        {
            step = 63;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 62 && !questionToggle[12].isOn)
        {
            step = 64;
            demographicOptions[step].SetActive(true);
        }
        if (step == 69 && questionToggle[30].isOn)
        {
            step = 70;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 69 && !questionToggle[30].isOn)
        {
            step = 71;
            demographicOptions[step].SetActive(true);
        }
        if (step == 78 && questionToggle[16].isOn)
        {
            step = 79;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 78 && !questionToggle[16].isOn)
        {
            step = 80;
            demographicOptions[step].SetActive(true);
        }
        if (step == 82 && questionToggle[22].isOn)
        {
            step = 83;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 82 && !questionToggle[22].isOn)
        {
            step = 84;
            demographicOptions[step].SetActive(true);
        }
        if (step == 86 && questionToggle[19].isOn)
        {
            step = 87;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 86 && !questionToggle[19].isOn)
        {
            step = 88;
            demographicOptions[step].SetActive(true);
        }
        if (step == 91 && questionToggle[32].isOn)
        {
            step = 92;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 91 && !questionToggle[32].isOn)
        {
            step = 93;
            demographicOptions[step].SetActive(true);
        }

    }
    IEnumerator DelayShow()
    {
        btnSubmit.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.25f);
        btnSubmit.gameObject.SetActive(true);
        yield break;
    }

    /*void SaveProfile()
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
            else if (dd.transform.parent == demographicOptions[3].transform)//tech familiarity
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
            }
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

        string pass = Random.Range(100, 999).ToString();
        string visitorId = SystemInfo.deviceUniqueIdentifier.Substring(0, 3) + pass;

    if (StaticData.isNewVisitor)
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


        //StopCoroutine("PostVisitorData");
        //StartCoroutine(PostVisitorData(profileItem));
    }*/

    public void SaveSurvey()
    {
        // Reload current path to get server path id if it was uploaded
        currentPath = cPath.Reload(currentPath);

        // Initialize a list of answers
        List<string> answers = new List<string>();

        // Get answers
        foreach (GameObject gO in demographicOptions)
        {
            // Input Container
            TMP_InputField inputField = gO.GetComponentInChildren<TMP_InputField>();
            if (inputField != null)
            {
                answers.Add(string.IsNullOrEmpty(inputField.text) ? null : inputField.text);
                continue;
            }

            // Dropdown Container
            TMP_Dropdown dropdown = gO.GetComponentInChildren<TMP_Dropdown>();
            if (dropdown != null)
            {
                answers.Add(dropdown.captionText.text.Equals("<Επιλέξτε από τα παρακάτω>") || dropdown.captionText.text.Equals("<Choose Below>") ? null : dropdown.captionText.text);
                continue;
            }

            // Get Toggle Group
            ToggleGroup toggleGroup = gO.GetComponentInChildren<ToggleGroup>();

            // Toggle Container
            Toggle[] toggles = gO.GetComponentsInChildren<Toggle>();
            if (toggles != null && toggles.Length > 0)
            {
                string text = string.Empty;

                foreach (Toggle toggle in toggles)
                {
                    if (toggle.isOn)
                    {
                        if (toggleGroup != null)
                        {
                            text = toggle.GetComponentInChildren<TMP_Text>().text;
                            break;
                        }
                        
                        if (text.Length <= 0)
                            text = toggle.GetComponentInChildren<TMP_Text>().text;
                        else
                            text += ", " + toggle.GetComponentInChildren<TMP_Text>().text;
                    }
                }

                answers.Add(string.IsNullOrEmpty(text) ? null : text);
            }
        }

        Debug.Log("answers count = " + answers.Count);
        Debug.Log("demographicOptions count = " + demographicOptions.Length);
        for (int i = 0; i < answers.Count; i++)
        {
            Debug.Log("demographicOptions " + i + " = " + demographicOptions[i].name);
            Debug.Log("answer " + i + " = " + answers[i]);
        }

        // Save Survey
        cSurvey surveyToSave = new cSurvey(currentPath.server_path_id, currentPath.local_area_id, answers);
        cSurvey.Save(surveyToSave);

        // Upload to server
        AppManager.Instance.serverManager.postUserData = true;
        AppManager.Instance.serverManager.timeRemaining = 0f;
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

    public void ResetValues()
    {
        step = 0;
        foreach (GameObject gb in demographicOptions) gb.SetActive(false);
        demographicOptions[0].SetActive(true);
        foreach (TMP_Dropdown td in dropdownsGeneral) td.value = 0;
        foreach (TMP_InputField ti in inputFieldsGeneral) ti.text = "";
        foreach (TMP_InputField tiA in inputFieldOptionA) tiA.text = "";
        foreach (Toggle toA in toggleOptionA) toA.isOn = false;
        foreach (Toggle tοΒ in toggleOptionB1) tοΒ.isOn = false;
        foreach (Toggle tοΒ in toggleOptionB2) tοΒ.isOn = false;
        foreach (TMP_InputField tiB in inputFieldOptionB1) tiB.text = "";
        foreach (TMP_InputField tiB in inputFieldOptionB2) tiB.text = "";
        foreach (Toggle tοC in toggleOptionC1) tοC.isOn = false;
        foreach (Toggle tοC in toggleOptionC2) tοC.isOn = false;
        foreach (TMP_InputField tiC in inputFieldOptionC1) tiC.text = "";
        foreach (TMP_InputField tiC in inputFieldOptionC2) tiC.text = "";
        foreach (Toggle toQ in questionToggle) toQ.isOn = false;
        foreach (Toggle toCHB in choiceTogglesB) toCHB.isOn = false;
        foreach (Toggle toCHC in choiceTogglesC) toCHC.isOn = false;
        optionDropdown.value = 0;
        toggleContainerOptionB.gameObject.SetActive(true);
        toggleContainerOptionC.gameObject.SetActive(true);
        if (AppManager.Instance.uIManager.pnlOptionA.activeSelf && AppManager.Instance.uIManager.pnlQuestionnaireScreen.activeSelf)
        {
            AppManager.Instance.uIManager.pnlOptionA.SetActive(false);
            AppManager.Instance.uIManager.pnlMainQuestions.SetActive(true);
        }
        else if (AppManager.Instance.uIManager.pnlOptionB1.activeSelf && AppManager.Instance.uIManager.pnlQuestionnaireScreen.activeSelf)
        {
            AppManager.Instance.uIManager.pnlOptionB1.SetActive(false);
            AppManager.Instance.uIManager.pnlOptionB.SetActive(false);
            AppManager.Instance.uIManager.pnlMainQuestions.SetActive(true);
        }

        else if (AppManager.Instance.uIManager.pnlOptionB2.activeSelf && AppManager.Instance.uIManager.pnlQuestionnaireScreen.activeSelf)
        {
            AppManager.Instance.uIManager.pnlOptionB2.SetActive(false);
            AppManager.Instance.uIManager.pnlOptionB.SetActive(false);
            AppManager.Instance.uIManager.pnlMainQuestions.SetActive(true);
        }
        else if (AppManager.Instance.uIManager.pnlOptionC1.activeSelf && AppManager.Instance.uIManager.pnlQuestionnaireScreen.activeSelf)
        {
            AppManager.Instance.uIManager.pnlOptionC1.SetActive(false);
            AppManager.Instance.uIManager.pnlOptionC.SetActive(false);
            AppManager.Instance.uIManager.pnlMainQuestions.SetActive(true);
        }
        else if (AppManager.Instance.uIManager.pnlOptionC2.activeSelf && AppManager.Instance.uIManager.pnlQuestionnaireScreen.activeSelf)
        {
            AppManager.Instance.uIManager.pnlMainQuestions.SetActive(true);
            AppManager.Instance.uIManager.pnlOptionC.SetActive(false);
            AppManager.Instance.uIManager.pnlOptionC2.SetActive(false);
        }
        else
        {
            AppManager.Instance.uIManager.pnlOptionA.SetActive(false);
            AppManager.Instance.uIManager.pnlOptionB1.SetActive(false);
            AppManager.Instance.uIManager.pnlOptionB2.SetActive(false);
            AppManager.Instance.uIManager.pnlOptionB.SetActive(false);
            AppManager.Instance.uIManager.pnlOptionC1.SetActive(false);
            AppManager.Instance.uIManager.pnlOptionC.SetActive(false);
            AppManager.Instance.uIManager.pnlOptionC2.SetActive(false);
        }
    }
}
