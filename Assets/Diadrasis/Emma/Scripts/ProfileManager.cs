using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    public delegate void Profile();
    public static Profile OnContinue, OnInternetError;

    public Button btnSubmit, BtnOk;
    public GameObject[] demographicOptions;
    public Transform dropdownContainer;

    public GameObject newIdPanel;
    public Text newIdText, btnNextText, btnOkText;

    public Dropdown[] dropdowns;
    int step = 0;

    public static string jsonFile;

    public GameObject panelSettings;

    //Intro intro;

    private void Awake()
    {
        BtnOk.onClick.AddListener(() => ContinueToSettings());
        //intro = FindObjectOfType<Intro>();
        btnSubmit.onClick.AddListener(() => Submit());
        dropdowns = dropdownContainer.GetComponentsInChildren<Dropdown>(true);
    }

    void Start()
    {
        step = 0;
    }

    private void OnEnable()
    {
        BtnOk.gameObject.SetActive(false);
        newIdPanel.SetActive(false);
        btnSubmit.gameObject.SetActive(true);
        step = 0;
        foreach (GameObject gb in demographicOptions) gb.gameObject.SetActive(false);
        demographicOptions[0].SetActive(true);
        btnNextText.text = "Επόμενο";

        Debug.LogWarning("is new = " + StaticData.isNewVisitor);

        //also reset values?
        if (StaticData.isNewVisitor)
        {
            foreach (Dropdown dd in dropdowns) dd.value = 0;
        }
        else
        {
            //Load old visitor data
            ProfileItem profileItem = JsonUtility.FromJson<ProfileItem>(StaticData.visitorData);
            foreach (Dropdown dd in dropdowns)
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
        }
    }

    void Submit()
    {
        demographicOptions[step].SetActive(false);

        if (step < demographicOptions.Length - 1)
        {
            StopCoroutine("DelayShow");
            StartCoroutine(DelayShow());
            step++;
            if (step == demographicOptions.Length - 1) { btnNextText.text = "Αποστολή"; }
            demographicOptions[step].SetActive(true);
        }
        else
        {
            SaveProfile();
        }
    }

    IEnumerator DelayShow()
    {
        btnSubmit.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.25f);
        btnSubmit.gameObject.SetActive(true);
        yield break;
    }

    void ContinueToSettings()
    {
        OnContinue?.Invoke();
    }

    void SaveProfile()
    {
        ProfileItem profileItem = new ProfileItem();

        foreach (Dropdown dd in dropdowns)
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
                profileItem.Education = dd.options[dd.value].text;
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


        StopCoroutine("PostVisitorData");
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




    public void GetFamiliarity()
    {
        ProfileItem profileItem = JsonUtility.FromJson<ProfileItem>(StaticData.visitorData);
        foreach (Dropdown dd in dropdowns)
        {
            for (int i = 0; i < dd.options.Count; i++)
            {
                if (dd.options[i].text == profileItem.Familiarity)
                {
                    StaticData.visitorFamiliarity = i.ToString();
                }
            }
        }
    }

}
