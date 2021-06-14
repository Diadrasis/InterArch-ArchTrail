using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class QuestionnaireManager : MonoBehaviour
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

    public Toggle[] choiceToggles/*groupB1, groupB2, groupC1, groupC2*/;
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
                    //Debug.Log("Here");
                }
                //step = 5;
                
            }
            else if (step == 9 || step == 15 || step == 21 || step == 31 || step == 25
                || step == 33 || step == 37 || step == 45 || step == 49 || step == 52 || step == 57
                || step == 61 || step == 66 || step == 69)
            {
                //Debug.Log("Step for toggleCheck" + step);
                CheckIfUserHasSelectedOtherOption();
            }
            else if (step == 11 || step == 23 || step == 27 || step == 35 || step == 39
                || step == 47 || step == 59 || step == 64)
            {
                //Debug.Log("Step for toggleCheck Second method: " + step);
                CheckIfUserHasSelectedOtherOptionSecond();
            }
            else if (step == 41 || step == 16 || step == 32 || step == 53 || step == 70)
            {
                step = demographicOptions.Length;
                btnSkip.gameObject.SetActive(false);
            }
            else
            {
                step++;
                demographicOptions[step].SetActive(true);
            }
            
            if (step==4 ||step == 15 || step == 16 || step == 31 || step == 32 || step == 41 || step == 52 || step == 53
            || step == 69 || step == 70)
            {
                btnSkip.gameObject.SetActive(false);
            }
            else
            {
                btnSkip.gameObject.SetActive(true);
            }
            
        }

        // End survey and save
        if (step >= demographicOptions.Length)
        {
            if (currentPath != null)
            {
                SaveQuestionnaire();

                // Test
                /*cQuestionnaire loadedQuestionnaire = cQuestionnaire.Load(currentPath.local_path_id);

                if (loadedQuestionnaire != null)
                {
                    //Debug.Log("Answers:");
                    //Debug.Log("loadedQuestionnaire.answers.Count" + loadedQuestionnaire.answers.Count);
                    foreach (string answer in loadedQuestionnaire.answers)
                    {
                        Debug.Log(answer);
                    }
                }*/
            }
			
            //close questionnairePanel and go to DisplayAreas and reset questionnaire for new user
            AppManager.Instance.uIManager.pnlQuestionnaireScreen.SetActive(false);
            ResetValues();
            AppManager.Instance.uIManager.DisplayAreasScreen();
        }
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
                //Debug.Log("Here");
            }
            
            //step = 5;
        }
        else
        {

            step++;
            demographicOptions[step].SetActive(true);
        }

        if(step==4 || step == 15 || step == 16 || step == 31 || step == 32 || step == 41 || step == 52 || step == 53
            || step == 69 || step == 70)
        {
            btnSkip.gameObject.SetActive(false);
        }
        
        //Debug.Log("Step: " + step);

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
            AppManager.Instance.uIManager.pnlOptionB.SetActive(true);
            AppManager.Instance.uIManager.pnlMainQuestions.SetActive(false);
            //step = 5;
            CheckToggle();
        }
        else if (val.value == 3)
        {
            AppManager.Instance.uIManager.pnlOptionC.SetActive(true);
            AppManager.Instance.uIManager.pnlMainQuestions.SetActive(false);
            //step = 7;
            CheckToggle();
        }

    }
    //when selecting the panels with the options, to see which option is selected and then activate the coresponding panel
    void CheckToggle()
    {
        btnSkip.gameObject.SetActive(false);
        if (choiceToggles[0].isOn)
        {
            AppManager.Instance.uIManager.pnlOptionB1.SetActive(true);
            AppManager.Instance.uIManager.pnlOptionB2.SetActive(false);
            toggleContainerOptionB.gameObject.SetActive(false);
            textB.gameObject.SetActive(false);
            step = 17;
            demographicOptions[step].SetActive(true);
           
        }
        else if (choiceToggles[1].isOn)
        {
            
            AppManager.Instance.uIManager.pnlOptionB1.SetActive(false);
            AppManager.Instance.uIManager.pnlOptionB2.SetActive(true);
            toggleContainerOptionB.gameObject.SetActive(false);
            textB.gameObject.SetActive(false);
            step = 33;
            demographicOptions[step].SetActive(true);

        }
        if (choiceToggles[2].isOn)
        {
            AppManager.Instance.uIManager.pnlOptionC1.SetActive(true);
            AppManager.Instance.uIManager.pnlOptionC2.SetActive(false);
            toggleContainerOptionC.gameObject.SetActive(false);
            textC.gameObject.SetActive(false);
            step = 42;
        }
        else if (choiceToggles[3].isOn)
        {
            AppManager.Instance.uIManager.pnlOptionC2.SetActive(true);
            AppManager.Instance.uIManager.pnlOptionC1.SetActive(false);
            toggleContainerOptionC.gameObject.SetActive(false);
            textC.gameObject.SetActive(false);
            step = 54;
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
        if (step == 15 && questionToggle[2].isOn)
        {
            step = 16;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 15 && !questionToggle[2].isOn)
        {
            step = demographicOptions.Length;
        }

        if (step == 21 && questionToggle[3].isOn)
        {
            step = 22;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 21 && !questionToggle[3].isOn)
        {
            step = 23;
            demographicOptions[step].SetActive(true);
        }

        if(step == 25 && questionToggle[4].isOn)
        {
            step = 26;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 25 && !questionToggle[4].isOn)
        {
            step = 27;
            demographicOptions[step].SetActive(true);
        }
        if (step == 31 && questionToggle[6].isOn)
        {
            step = 32;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 31 && !questionToggle[6].isOn)
        {
            step = demographicOptions.Length;
        }

        if(step == 33 && questionToggle[7].isOn)
        {
            step = 34;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 33 && !questionToggle[7].isOn)
        {
            step = 35;
            demographicOptions[step].SetActive(true);
        }

        if (step == 37 && questionToggle[9].isOn)
        {
            step = 38;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 37 && !questionToggle[9].isOn)
        {
            step = 39;
            demographicOptions[step].SetActive(true);
        }
        if(step == 45 && questionToggle[11].isOn)
        {
            step = 46;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 45 && !questionToggle[11].isOn)
        {
            step = 47;
            demographicOptions[step].SetActive(true);
        }
        if (step == 49 && questionToggle[13].isOn)
        {
            step = 50;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 49 && !questionToggle[13].isOn)
        {
            step = 51;
            demographicOptions[step].SetActive(true);
        }

        if (step == 52 && questionToggle[14].isOn)
        {
            step = 53;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 52 && !questionToggle[14].isOn)
        {
            step = demographicOptions.Length;
        }

        if (step == 57 && questionToggle[15].isOn)
        {
            step = 58;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 58 && !questionToggle[15].isOn)
        {
            step = 59;
            demographicOptions[step].SetActive(true);
        }

        if (step == 61 && questionToggle[17].isOn)
        {
            step = 62;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 61 && !questionToggle[17].isOn)
        {
            step = 63;
            demographicOptions[step].SetActive(true);
        }

        if (step == 66 && questionToggle[19].isOn)
        {
            step = 67;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 66 && !questionToggle[19].isOn)
        {
            step = 68;
            demographicOptions[step].SetActive(true);
        }

        if (step == 69 && questionToggle[20].isOn)
        {
            step = 70;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 69 && !questionToggle[20].isOn)
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

        if (step == 23 && questionToggle[3].isOn)
        {
            step = 24;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 23 && !questionToggle[3].isOn)
        {
            step = 25;
            demographicOptions[step].SetActive(true);
        }

        if (step == 27 && questionToggle[5].isOn)
        {
            step = 28;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 27 && !questionToggle[5].isOn)
        {
            step = 29;
            demographicOptions[step].SetActive(true);
        }

        if(step == 35 && questionToggle[8].isOn)
        {
            step = 36;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 35 && !questionToggle[8].isOn)
        {
            step = 37;
            demographicOptions[step].SetActive(true);
        }

        if (step == 39 && questionToggle[10].isOn)
        {
            step = 40;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 39 && !questionToggle[10].isOn)
        {
            step = 41;
            demographicOptions[step].SetActive(true);
        }
        if (step == 47 && questionToggle[12].isOn)
        {
            step = 48;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 47 && !questionToggle[12].isOn)
        {
            step = 49;
            demographicOptions[step].SetActive(true);
        }
        if (step == 59 && questionToggle[16].isOn)
        {
            step = 60;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 59 && !questionToggle[16].isOn)
        {
            step = 61;
            demographicOptions[step].SetActive(true);
        }

        if (step == 64 && questionToggle[18].isOn)
        {
            step = 65;
            demographicOptions[step].SetActive(true);
        }
        else if (step == 64 && !questionToggle[18].isOn)
        {
            step = 66;
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

    /*void ContinueToSettings()
    {
        OnContinue?.Invoke();
    }*/

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
            *//*else if (dd.transform.parent == demographicOptions[3].transform)//tech familiarity
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
            }*//*
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

        *//*string pass = Random.Range(100, 999).ToString();
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
*//*

        //StopCoroutine("PostVisitorData");
        //StartCoroutine(PostVisitorData(profileItem));
    }*/

    public void SaveQuestionnaire()
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
                answers.Add(dropdown.captionText.text.Equals("<Επιλέξτε από τα παρακάτω>") || dropdown.captionText.text.Equals("<Select>") ? null : dropdown.captionText.text); // TODO: Check for translated
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

        // Save Questionnaire
        cQuestionnaire questionnaireToSave = new cQuestionnaire(currentPath.server_path_id, currentPath.local_area_id, answers);
        cQuestionnaire.Save(questionnaireToSave);

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
        foreach (Toggle toCH in choiceToggles) toCH.isOn = false;
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
        //Debug.Log("Reset "+ choiceToggles.Length.ToString() +" can we see it "+choiceToggles[0].gameObject.activeSelf);

    }
}
