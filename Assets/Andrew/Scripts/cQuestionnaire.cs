using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class cQuestionnaire : MonoBehaviour
{
    #region Variables
    public int local_path_id;
    /*public string Age;
    public string Sex;
    public string VisitReason;
    public string Education;
    public string Familiarity;
    public string Technology;
    public string Ethnicity;*/

    public static readonly string PREFS_KEY = "questionnaires";
    public static readonly string QUESTIONNAIRES = "questionnaires";

    public static readonly string QUESTIONNAIRE = "questionnaire";
    public static readonly string LOCAL_PATH_ID = "local_area_id";
    #endregion

    #region Methods
    // Constructor for Loading from Player Prefs and creating a new questionnaire
    private cQuestionnaire(int _local_path_id)
    {
        local_path_id = _local_path_id;
    }

    public static OnlineMapsXML GetXML()
    {
        // Load xml string from PlayerPrefs
        string xmlData = PlayerPrefs.GetString(PREFS_KEY);

        // Load xml document, if null create new
        OnlineMapsXML xml = OnlineMapsXML.Load(xmlData);
        if (xml.isNull)
        {
            xml = new OnlineMapsXML(QUESTIONNAIRES);
        }

        return xml;
    }

    public static void Save(cQuestionnaire _questionnaireToSave)
    {
        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        // Check if area is already saved
        OnlineMapsXML questionnaireSaved = xml.Find("/" + QUESTIONNAIRES + "/" + QUESTIONNAIRE + "[" + LOCAL_PATH_ID + "=" + _questionnaireToSave.local_path_id + "]");
        if (!questionnaireSaved.isNull)
        {
            Debug.Log("Questionnaire is already saved!");
            return;
        }

        // Create a new area node
        OnlineMapsXML questionnaireNode = xml.Create(QUESTIONNAIRE);
        questionnaireNode.Create(LOCAL_PATH_ID, _questionnaireToSave.local_path_id);

        // Save xml string to PlayerPrefs
        PlayerPrefs.SetString(PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }

    public static cQuestionnaire Load(int _local_path_id)
    {
        // Load xml document
        OnlineMapsXML xml = GetXML();

        // Get questionnaire node
        OnlineMapsXML questionnaireNode = xml.Find("/" + QUESTIONNAIRES + "/" + QUESTIONNAIRE + "[" + LOCAL_PATH_ID + "=" + _local_path_id + "]");
        if (questionnaireNode.isNull)
        {
            Debug.Log("Questionnaire with local_path_id: " + _local_path_id + " has been deleted!");
            return null;
        }

        return Load(questionnaireNode);
    }

    private static cQuestionnaire Load(OnlineMapsXML _questionnaireNode)
    {
        int local_path_id = _questionnaireNode.Get<int>(LOCAL_PATH_ID);
        //string title = _questionnaireNode.Get<string>(TITLE);

        cQuestionnaire loadedQuestionnaire = new cQuestionnaire(local_path_id);
        return loadedQuestionnaire;
    }

    public static void Delete(int _local_path_id)
    {
        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        // Get Questionnaire node
        OnlineMapsXML questionnaireToDelete = xml.Find("/" + QUESTIONNAIRES + "/" + QUESTIONNAIRE + "[" + LOCAL_PATH_ID + "=" + _local_path_id + "]");
        if (!questionnaireToDelete.isNull)
            questionnaireToDelete.Remove();
        Debug.Log("XML after deleting area: " + xml.outerXml);
        PlayerPrefs.SetString(PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }

    public static List<cQuestionnaire> GetQuestionnairesToUpload()
    {
        // List of areas
        List<cQuestionnaire> questionnairesToUpload = new List<cQuestionnaire>();

        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        // Get paths with server_area_id = -1
        OnlineMapsXMLList questionnaireNodes = xml.FindAll("/" + QUESTIONNAIRES + "/" + QUESTIONNAIRE);

        foreach (OnlineMapsXML questionnaireNode in questionnaireNodes)
        {
            if (questionnaireNode.isNull)
            {
                Debug.Log("Questionnaire has been deleted!");
                continue;
            }

            cQuestionnaire loadedQuestionnaire = Load(questionnaireNode);

            if (loadedQuestionnaire != null)
                questionnairesToUpload.Add(loadedQuestionnaire);
        }

        return questionnairesToUpload;
    }
    #endregion
}
