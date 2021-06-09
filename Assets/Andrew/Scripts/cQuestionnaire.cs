using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class cQuestionnaire
{
    #region Variables
    public int server_path_id;
    public int local_path_id;
    public List<string> answers;

    public static readonly string PREFS_KEY = "questionnaires";
    public static readonly string QUESTIONNAIRES = "questionnaires";

    public static readonly string QUESTIONNAIRE = "questionnaire";
    public static readonly string SERVER_PATH_ID = "server_path_id";
    public static readonly string LOCAL_PATH_ID = "local_path_id";
    public static readonly string ANSWERS = "answers";
    public static readonly string ANSWER = "answer";
    #endregion

    #region Methods
    // Constructor for Loading from Player Prefs and creating a new questionnaire
    public cQuestionnaire(int _server_path_id, int _local_path_id, List<string> _answers)
    {
        server_path_id = _server_path_id; // Probably always equals -1
        local_path_id = _local_path_id;
        answers = _answers;
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
        questionnaireNode.Create(SERVER_PATH_ID, _questionnaireToSave.server_path_id);
        questionnaireNode.Create(LOCAL_PATH_ID, _questionnaireToSave.local_path_id);
        OnlineMapsXML answersNode = questionnaireNode.Create(ANSWERS);
        foreach (string answer in _questionnaireToSave.answers)
        {
            answersNode.Create(ANSWER, answer);
        }

        Debug.Log(xml.outerXml);
        // Save xml string to PlayerPrefs
        PlayerPrefs.SetString(PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }

    public static void SetServerPathId(int _server_path_id, int _local_path_id)
    {
        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        // Find questionnaire
        OnlineMapsXML questionnaireNode = xml.Find("/" + QUESTIONNAIRES + "/" + QUESTIONNAIRE + "[" + LOCAL_PATH_ID + "=" + _local_path_id + "]");

        // Load questionnaire
        cQuestionnaire loadedQuestionnaire = Load(questionnaireNode);
        loadedQuestionnaire.server_path_id = _server_path_id;

        // Edit path
        EditServerPathId(loadedQuestionnaire);
    }

    private static void EditServerPathId(cQuestionnaire _questionnaireToEdit)
    {
        // Load xml document, if null create new
        OnlineMapsXML xml = GetXML();

        // Create a new path
        OnlineMapsXML questionnaireNode = xml.Find("/" + QUESTIONNAIRES + "/" + QUESTIONNAIRE + "[" + LOCAL_PATH_ID + "=" + _questionnaireToEdit.local_path_id + "]");
        questionnaireNode.Remove(SERVER_PATH_ID);
        questionnaireNode.Create(SERVER_PATH_ID, _questionnaireToEdit.server_path_id);
        Debug.Log("Edited xml = " + xml.outerXml);
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
        int server_path_id = _questionnaireNode.Get<int>(SERVER_PATH_ID);
        int local_path_id = _questionnaireNode.Get<int>(LOCAL_PATH_ID);

        OnlineMapsXMLList answerNodes = _questionnaireNode.FindAll("/" + QUESTIONNAIRES + "/" + QUESTIONNAIRE + "[" + LOCAL_PATH_ID + "=" + local_path_id + "]/" + ANSWERS + "/" + ANSWER);
        List<string> answers = new List<string>();
        foreach (OnlineMapsXML answerNode in answerNodes)
        {
            string answer = answerNode.Get<string>(answerNode.element);
            answers.Add(answer);
        }

        cQuestionnaire loadedQuestionnaire = new cQuestionnaire(server_path_id, local_path_id, answers);
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

        PlayerPrefs.SetString(PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }

    public static List<cQuestionnaire> GetQuestionnairesToUpload()
    {
        // List of questionnaires
        List<cQuestionnaire> questionnairesToUpload = new List<cQuestionnaire>();

        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        // Get questionnaires with server_path_id != -1
        OnlineMapsXMLList questionnaireNodes = xml.FindAll("/" + QUESTIONNAIRES + "/" + QUESTIONNAIRE + "[" + SERVER_PATH_ID + "!=" + (-1) + "]");

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
