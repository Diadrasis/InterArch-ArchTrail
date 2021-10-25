using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class cSurvey
{
    #region Variables
    public int server_path_id;
    public int local_path_id;
    public List<string> answers;

    public static readonly string PREFS_KEY = "surveys";
    public static readonly string SURVEYS = "surveys";

    public static readonly string SURVEY = "survey";
    public static readonly string SERVER_PATH_ID = "server_path_id";
    public static readonly string LOCAL_PATH_ID = "local_path_id";
    public static readonly string ANSWERS = "answers";
    public static readonly string ANSWER = "answer";
    #endregion

    #region Methods
    // Constructor for Loading from Player Prefs and creating a new survey
    public cSurvey(int _server_path_id, int _local_path_id, List<string> _answers)
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
            xml = new OnlineMapsXML(SURVEYS);
        }

        return xml;
    }

    public static void Save(cSurvey _surveyToSave)
    {
        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        // Check if survey is already saved
        OnlineMapsXML surveySaved = xml.Find("/" + SURVEYS + "/" + SURVEY + "[" + LOCAL_PATH_ID + "=" + _surveyToSave.local_path_id + "]");
        if (!surveySaved.isNull)
        {
            Debug.Log("Survey is already saved!");
            return;
        }

        // Create a new survey node
        OnlineMapsXML surveyNode = xml.Create(SURVEY);
        surveyNode.Create(SERVER_PATH_ID, _surveyToSave.server_path_id);
        surveyNode.Create(LOCAL_PATH_ID, _surveyToSave.local_path_id);
        OnlineMapsXML answersNode = surveyNode.Create(ANSWERS);
        foreach (string answer in _surveyToSave.answers)
        {
            answersNode.Create(ANSWER, answer);
        }

        // Save xml string to PlayerPrefs
        PlayerPrefs.SetString(PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();

        // Debug
        Debug.Log("Saved Survey");
        Debug.Log("Edited xml = " + xml.outerXml);
    }

    public static void SetServerPathId(int _server_path_id, int _local_path_id)
    {
        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        // Find survey
        OnlineMapsXML surveyNode = xml.Find("/" + SURVEYS + "/" + SURVEY + "[" + LOCAL_PATH_ID + "=" + _local_path_id + "]");

        if (!surveyNode.isNull)
        {
            // Load survey
            cSurvey loadedSurvey = Load(surveyNode);

            if (loadedSurvey != null)
            {
                loadedSurvey.server_path_id = _server_path_id;

                // Edit path
                EditServerPathId(loadedSurvey);
            }
        }
    }

    private static void EditServerPathId(cSurvey _surveyToEdit)
    {
        // Load xml document, if null create new
        OnlineMapsXML xml = GetXML();

        // Create a new path
        OnlineMapsXML surveyNode = xml.Find("/" + SURVEYS + "/" + SURVEY + "[" + LOCAL_PATH_ID + "=" + _surveyToEdit.local_path_id + "]");
        surveyNode.Remove(SERVER_PATH_ID);
        surveyNode.Create(SERVER_PATH_ID, _surveyToEdit.server_path_id);
        
        // Save xml string to PlayerPrefs
        PlayerPrefs.SetString(PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }

    public static cSurvey Load(int _local_path_id)
    {
        // Load xml document
        OnlineMapsXML xml = GetXML();

        // Get survey node
        OnlineMapsXML surveyNode = xml.Find("/" + SURVEYS + "/" + SURVEY + "[" + LOCAL_PATH_ID + "=" + _local_path_id + "]");
        if (surveyNode.isNull)
        {
            Debug.Log("Survey with local_path_id: " + _local_path_id + " has been deleted!");
            return null;
        }

        return Load(surveyNode);
    }

    private static cSurvey Load(OnlineMapsXML _surveyNode)
    {
        int server_path_id = _surveyNode.Get<int>(SERVER_PATH_ID);
        int local_path_id = _surveyNode.Get<int>(LOCAL_PATH_ID);

        OnlineMapsXMLList answerNodes = _surveyNode.FindAll("/" + SURVEYS + "/" + SURVEY + "[" + LOCAL_PATH_ID + "=" + local_path_id + "]/" + ANSWERS + "/" + ANSWER);
        List<string> answers = new List<string>();
        foreach (OnlineMapsXML answerNode in answerNodes)
        {
            string answer = answerNode.Get<string>(answerNode.element);
            answers.Add(answer);
        }

        cSurvey loadedSurvey = new cSurvey(server_path_id, local_path_id, answers);
        return loadedSurvey;
    }

    public static void Delete(int _local_path_id)
    {
        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        // Get survey node
        OnlineMapsXML surveyToDelete = xml.Find("/" + SURVEYS + "/" + SURVEY + "[" + LOCAL_PATH_ID + "=" + _local_path_id + "]");
        if (!surveyToDelete.isNull)
            surveyToDelete.Remove();

        PlayerPrefs.SetString(PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }

    public static List<cSurvey> GetSurveysToUpload()
    {
        // List of surveys
        List<cSurvey> surveysToUpload = new List<cSurvey>();

        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        // Get surveys with server_path_id != -1
        OnlineMapsXMLList surveyNodes = xml.FindAll("/" + SURVEYS + "/" + SURVEY + "[" + SERVER_PATH_ID + "=" + (-1) + "]");

        foreach (OnlineMapsXML surveyNode in surveyNodes)
        {
            if (surveyNode.isNull)
            {
                Debug.Log("Survey has been deleted!");
                continue;
            }
            
            // Load survey
            cSurvey loadedSurvey = Load(surveyNode);
            
            if (loadedSurvey != null)
            {
                // Get server_path_id from local path id
                int? serverPathId = cPath.GetServerPathId(loadedSurvey.local_path_id);
                if (serverPathId != null)
                {
                    // Set survey server path id
                    loadedSurvey.server_path_id = (int)serverPathId;

                    // Add this survey to the list
                    surveysToUpload.Add(loadedSurvey);
                }
            }
        }

        // Debug
        //Debug.Log("surveyNodes = " + surveyNodes.count);
        //Debug.Log("Edited xml = " + xml.outerXml);

        return surveysToUpload;
    }
    #endregion
}
