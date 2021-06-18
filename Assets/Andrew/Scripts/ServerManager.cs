using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;
using System.Globalization;

public class ServerManager : MonoBehaviour
{
    #region Variables
    public delegate void InternetConnection(bool isOn);
    public InternetConnection OnCheckInternetCheckComplete;

    // URL to Post
    readonly string diadrasisAreaManagerUrl = "http://diadrasis.net/interarch_area_manager.php";
    readonly string diadrasisSurveyManagerUrl = "http://diadrasis.net/interarch_survey_manager.php";
    public enum PHPActions { Get_Areas, Save_Area, Delete_Area, Get_Paths, Save_Path, Delete_Path, Get_Points, Save_Point, Delete_Point, Save_Survey }
    public bool postUserData = false; // true;
    public bool getData = false; // true;
    private bool downloadAreas;
    private int downloadAreaId = -1;
    private int downloadPathId = -1;

    // Internet checks
    bool internetChecked;
    public bool hasInternet;

    // Counter
    private float timeToCount = 5f;
    public float timeRemaining = 0f;

    // Tiles
    public TileDownloader tileDownloader;

    // Ui warning
    public bool panelInternetWarning, isShownOnce;
    private float secondsToWaitBeforeWarning = 1f;
    private float minSecondsToDisplayWarning = 1f;
    #endregion

    #region UnityMethods
    private void Start()
    {
        // Initialize variables
        postUserData = true;
        timeRemaining = 0f;
        getData = true;
        isShownOnce = true;
        tileDownloader = OnlineMaps.instance.gameObject.GetComponent<TileDownloader>();
        hasInternet = false;
    }

    private void Update()
    {
        // Check if there is internet connection and postUserData is true, uploads the user's data to the server
        // NOTE: The postUserData bool is set to true when opening the application, when the user saves a new area or when a path is saved.
        if (!internetChecked)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                if (postUserData)
                {
                    OnlineMaps.instance.CheckServerConnection(OnCheckConnectionCompleteUpload);
                }
                
                CheckInternet();
                timeRemaining = timeToCount;
            }
        }

        if (internetChecked)
        {
            // Post user data
            if (postUserData)
            {
                StartCoroutine(UploadUserDataToDiadrasis());
                postUserData = false;
            }

            internetChecked = false;
        }
    }
    #endregion

    #region Methods
    #region InternetConnection
    public void CheckInternet()
    {
        OnlineMaps.instance.CheckServerConnection(OnCheckConnectionComplete);
    }
    
    public void OnCheckConnectionCompleteUpload(bool status)
    {
        internetChecked = status;

        if (!status && panelInternetWarning)
        {
            panelInternetWarning = false;
        }
        
        if (status)
        {
            panelInternetWarning = true;
            
        }
    }

    public void OnCheckConnectionComplete(bool status)
    {
        internetChecked = status;
        hasInternet = status;
        //if (OnCheckInternetCheckComplete != null) OnCheckInternetCheckComplete(status);
        if (status)
        {
            AppManager.Instance.uIManager.pnlWarningInternetScreen.SetActive(false);
            AppManager.Instance.uIManager.imgLoading.color = Color.green;
            isShownOnce = true;
        }
        else
        {
            if (isShownOnce)
            {
                AppManager.Instance.uIManager.pnlWarningInternetScreen.SetActive(true);
                isShownOnce = false;
            }

            AppManager.Instance.uIManager.imgLoading.color = Color.red;
        }
    }

    public void OnCheckConnectionCompleteDownload(bool status)
    {
        if (status)
        {
            // Has connection
            AppManager.Instance.uIManager.imgLoading.color = Color.green;

            // Download Areas
            if (downloadAreas)
            {
                StartCoroutine(GetAreas());
                downloadAreas = false;
            }

            // Download Paths
            if (downloadAreaId != -1)
            {
                StartCoroutine(GetPaths(downloadAreaId));
                downloadAreaId = -1;
            }

            // Download Points
            if (downloadPathId != -1)
            {
                StartCoroutine(GetPoints(downloadPathId));
                downloadPathId = -1;
            }
        }
        else
        {
            AppManager.Instance.uIManager.pnlWarningServerScreen.SetActive(false);
        }
    }
    #endregion

    IEnumerator UploadUserDataToDiadrasis()
    {
        // ============== Upload Areas ============== //
        // Calculate seconds to upload
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();

        // Get areas to upload
        List<cArea> areasToUpload = cArea.GetAreasToUpload();

        if (areasToUpload != null && areasToUpload.Count > 0)
        {
            foreach (cArea areaToUpload in areasToUpload)
            {
                // If the area's tiles are not downloaded ask user and download tiles Locally
                tileDownloader.SetValues(areaToUpload.areaConstraintsMin.x, areaToUpload.areaConstraintsMax.y, areaToUpload.areaConstraintsMax.x, areaToUpload.areaConstraintsMin.y, OnlineMaps.MAXZOOM, OnlineMaps.MAXZOOM);
                if (!tileDownloader.HasTiles())
                {
                    tileDownloader.Calculate();

                    // Activate panel warning
                    AppManager.Instance.uIManager.pnlWarningDownloadTilesScreen.SetActive(true);
                    if (AppManager.Instance.uIManager.LanguageIsEnglish())
                        AppManager.Instance.uIManager.txtWarningDownloadTiles.text = "Would you like to download " + areaToUpload.title + "'s tiles?\nSize: " + (tileDownloader.totalSize / 1000) + " ΜB";
                    else
                        AppManager.Instance.uIManager.txtWarningDownloadTiles.text = "Θα θέλατε να κατεβάσετε το χάρτη της περιοχής \"" + areaToUpload.title + "\";\nΜέγεθος: " + (tileDownloader.totalSize / 1000) + " ΜB";

                    // Wait for user input
                    while (AppManager.Instance.uIManager.pnlWarningDownloadTilesScreen.activeSelf)
                    {
                        yield return null;
                    }

                    // Check user input (yes or no)
                    if (AppManager.Instance.uIManager.downloadTiles)
                    {
                        tileDownloader.Download();
                        while (tileDownloader.isDownloading)
                        {
                            // Show warning panel
                            if (stopWatch.Elapsed.TotalSeconds > secondsToWaitBeforeWarning)
                            {
                                int percentage = Mathf.RoundToInt((float)(((double)tileDownloader.downloadedTiles / (double)tileDownloader.countTiles) * 100));
                                if (AppManager.Instance.uIManager.LanguageIsEnglish())
                                    AppManager.Instance.uIManager.txtWarningServer.text = "Downloading tiles... \n" + percentage + "%";
                                else
                                    AppManager.Instance.uIManager.txtWarningServer.text = "Λήψη χάρτη... \n" + percentage + "%";
                                AppManager.Instance.uIManager.pnlWarningServerScreen.SetActive(true);
                            }

                            yield return null;
                        }

                        AppManager.Instance.uIManager.downloadTiles = false;
                    }
                }

                // Show warning panel
                if (stopWatch.Elapsed.TotalSeconds > secondsToWaitBeforeWarning)
                {
                    if (AppManager.Instance.uIManager.LanguageIsEnglish())
                        AppManager.Instance.uIManager.txtWarningServer.text = "Uploading area...";
                    else
                        AppManager.Instance.uIManager.txtWarningServer.text = "Μεταφόρτωση περιοχής...";
                    AppManager.Instance.uIManager.pnlWarningServerScreen.SetActive(true);
                }

                // Create a form and add all the fields of the area
                List <IMultipartFormSection> formToPost = new List<IMultipartFormSection>();
                formToPost.Add(new MultipartFormDataSection("action", Enum.GetName(typeof(PHPActions), 1))); // Save_Area
                formToPost.Add(new MultipartFormDataSection("server_area_id", areaToUpload.server_area_id.ToString()));
                formToPost.Add(new MultipartFormDataSection("title", areaToUpload.title));
                formToPost.Add(new MultipartFormDataSection("position", areaToUpload.position.ToString("F6")));
                formToPost.Add(new MultipartFormDataSection("zoom", areaToUpload.zoom.ToString()));
                formToPost.Add(new MultipartFormDataSection("areaConstraintsMin", areaToUpload.areaConstraintsMin.ToString("F6")));
                formToPost.Add(new MultipartFormDataSection("areaConstraintsMax", areaToUpload.areaConstraintsMax.ToString("F6")));
                formToPost.Add(new MultipartFormDataSection("viewConstraintsMin", areaToUpload.viewConstraintsMin.ToString("F6")));
                formToPost.Add(new MultipartFormDataSection("viewConstraintsMax", areaToUpload.viewConstraintsMax.ToString("F6")));

                UnityWebRequest webRequest = UnityWebRequest.Post(diadrasisAreaManagerUrl, formToPost);

                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    Debug.Log("Web request failed. Error #" + webRequest.error);
                }
                else
                {
                    //Debug.Log("Posted successfully: " + webRequest.uploadHandler.data);
                    //Debug.Log("Uploaded area successfully!");
                    //Debug.Log("Echo: " + webRequest.downloadHandler.text);
                    
                    // Get database id and set it
                    string echo = webRequest.downloadHandler.text;
                    string server_area_idString = echo.Replace("[{\"max(server_area_id)\":\"", "").Replace("\"}]", "");
                    if (int.TryParse(server_area_idString, out int server_area_id))
                    {
                        cArea.SetServerAreaId(areaToUpload.local_area_id, server_area_id);

                        // remove the area's server id if the area was included in the edited areas array
                        cArea.RemoveAreaIdToEdit(areaToUpload.server_area_id);
                    }
                }
            }
        }
       
        // ============== Upload Paths ============== //

        // Get paths to upload
        List<cPath> pathsToUpload = cPath.GetPathsToUpload();

        if (pathsToUpload != null && pathsToUpload.Count > 0)
        {
            // Show warning panel
            if (stopWatch.Elapsed.TotalSeconds > secondsToWaitBeforeWarning)
            {
                if (AppManager.Instance.uIManager.LanguageIsEnglish())
                    AppManager.Instance.uIManager.txtWarningServer.text = "Uploading paths...";
                else
                    AppManager.Instance.uIManager.txtWarningServer.text = "Μεταφόρτωση διαδρομών...";
                AppManager.Instance.uIManager.pnlWarningServerScreen.SetActive(true);
            }

            foreach (cPath pathToUpload in pathsToUpload)
            {
                // Create a form and add all the fields of the area
                List<IMultipartFormSection> formToPost = new List<IMultipartFormSection>();
                formToPost.Add(new MultipartFormDataSection("action", Enum.GetName(typeof(PHPActions), 4))); // Save_Path
                formToPost.Add(new MultipartFormDataSection("server_area_id", pathToUpload.server_area_id.ToString()));
                formToPost.Add(new MultipartFormDataSection("server_path_id", pathToUpload.server_path_id.ToString()));
                //formToPost.Add(new MultipartFormDataSection("local_area_id", _pathToUpload.local_area_id.ToString()));
                //formToPost.Add(new MultipartFormDataSection("local_path_id", _pathToUpload.local_path_id.ToString()));
                formToPost.Add(new MultipartFormDataSection("title", pathToUpload.title));
                formToPost.Add(new MultipartFormDataSection("date", pathToUpload.date)); //pathToUpload.date.ToShortDateString()

                UnityWebRequest webRequest = UnityWebRequest.Post(diadrasisAreaManagerUrl, formToPost);

                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    Debug.Log("Web request failed. Error #" + webRequest.error);
                }
                else
                {
                    Debug.Log("Echo: " + webRequest.downloadHandler.text);
                    //Debug.Log("Uploaded path successfully!");
                    //AppManager.Instance.uIManager.txtLoading.text = "Uploading...";
                    // Get database id and set it
                    string echo = webRequest.downloadHandler.text;
                    string server_path_idString = echo.Replace("[{\"max(server_path_id)\":\"", "").Replace("\"}]", "");
                    //string server_path_idString = cleanString.Replace(" ", "");
                    //Debug.Log("databaseId = " + server_path_idString);
                    if (int.TryParse(server_path_idString, out int server_path_id))
                    {
                        cPath.SetServerAreaAndPathId(pathToUpload.server_area_id, server_path_id, pathToUpload.local_path_id);
                        cSurvey.SetServerPathId(server_path_id, pathToUpload.local_path_id); // for offline
                    }
                }
            }
        }

        // ============== Upload Points ============== //

        // Get points to upload
        List<cPathPoint> pointsToUpload = cPathPoint.GetPointsToUpload();

        if (pointsToUpload != null && pointsToUpload.Count > 0)
        {
            // Show warning panel
            if (stopWatch.Elapsed.TotalSeconds > secondsToWaitBeforeWarning)
            {
                if (AppManager.Instance.uIManager.LanguageIsEnglish())
                    AppManager.Instance.uIManager.txtWarningServer.text = "Uploading points...";
                else
                    AppManager.Instance.uIManager.txtWarningServer.text = "Μεταφόρτωση σημείων...";
                AppManager.Instance.uIManager.pnlWarningServerScreen.SetActive(true);
            }

            foreach (cPathPoint pointToUpload in pointsToUpload)
            {
                /*Debug.Log("Point to upload: server_path_id = " + pointToUpload.server_path_id.ToString());
                Debug.Log("server_point_id = " + pointToUpload.server_point_id.ToString());
                Debug.Log("index = " + pointToUpload.index.ToString());
                Debug.Log("duration = " + pointToUpload.duration.ToString("F6"));*/

                // Create a form and add all the fields of the area
                List<IMultipartFormSection> formToPost = new List<IMultipartFormSection>();
                formToPost.Add(new MultipartFormDataSection("action", Enum.GetName(typeof(PHPActions), 7))); // Save_Point
                formToPost.Add(new MultipartFormDataSection("server_path_id", pointToUpload.server_path_id.ToString()));
                formToPost.Add(new MultipartFormDataSection("server_point_id", pointToUpload.server_point_id.ToString()));
                //formToPost.Add(new MultipartFormDataSection("local_area_id", pointToUpload.local_area_id.ToString()));
                //formToPost.Add(new MultipartFormDataSection("local_path_id", pointToUpload.local_path_id.ToString()));
                formToPost.Add(new MultipartFormDataSection("index", pointToUpload.index.ToString()));
                formToPost.Add(new MultipartFormDataSection("position", pointToUpload.position.ToString("F6")));
                //formToPost.Add(new MultipartFormDataSection("time", pointToUpload.time.ToString())); // TODO: Change???
                formToPost.Add(new MultipartFormDataSection("duration", pointToUpload.duration.ToString("F6")));

                UnityWebRequest webRequest = UnityWebRequest.Post(diadrasisAreaManagerUrl, formToPost);

                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    Debug.Log("Web request failed. Error #" + webRequest.error);
                }
                else
                {
                    //Debug.Log("Uploaded point successfully!");
                    Debug.Log("Echo: " + webRequest.downloadHandler.text);
                    //AppManager.Instance.uIManager.txtLoading.text = "Uploading...";
                    // Get database id and set it
                    string echo = webRequest.downloadHandler.text;
                    string server_point_idString = echo.Replace("[{\"max(server_point_id)\":\"", "").Replace("\"}]", "");
                    //string server_path_idString = cleanString.Replace(" ", "");
                    //Debug.Log("databaseId = " + server_path_idString);
                    if (int.TryParse(server_point_idString, out int server_point_id))
                        cPathPoint.SetServerPathAndPointId(pointToUpload.server_path_id, server_point_id, pointToUpload.index);
                }
            }
        }

        // ============== Upload Questionnaires ============== //

        // Get questionnaires to upload
        List<cSurvey> questionnairesToUpload = cSurvey.GetSurveysToUpload();

        if (questionnairesToUpload != null && questionnairesToUpload.Count > 0)
        {
            // Show warning panel
            if (stopWatch.Elapsed.TotalSeconds > secondsToWaitBeforeWarning)
            {
                if (AppManager.Instance.uIManager.LanguageIsEnglish())
                    AppManager.Instance.uIManager.txtWarningServer.text = "Uploading surveys...";
                else
                    AppManager.Instance.uIManager.txtWarningServer.text = "Μεταφόρτωση ερωτηματολογίων...";
                AppManager.Instance.uIManager.pnlWarningServerScreen.SetActive(true);
            }

            foreach (cSurvey questionnaireToUpload in questionnairesToUpload)
            {
                // Create a form and add all the fields of the area
                List<IMultipartFormSection> formToPost = new List<IMultipartFormSection>();
                formToPost.Add(new MultipartFormDataSection("action", Enum.GetName(typeof(PHPActions), 9))); // Save_Survey
                formToPost.Add(new MultipartFormDataSection("server_path_id", questionnaireToUpload.server_path_id.ToString()));
                for (int i = 0; i < questionnaireToUpload.answers.Count; i++)
                {
                    formToPost.Add(new MultipartFormDataSection("answer" + i, questionnaireToUpload.answers[i] == null ? " " : questionnaireToUpload.answers[i]));
                }

                UnityWebRequest webRequest = UnityWebRequest.Post(diadrasisSurveyManagerUrl, formToPost);

                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    Debug.Log("Web request failed. Error #" + webRequest.error);
                }
                else
                {
                    //Debug.Log("Uploaded point successfully!");
                    Debug.Log("Echo: " + webRequest.downloadHandler.text);
                    //AppManager.Instance.uIManager.txtLoading.text = "Uploading...";
                    // Get database id and set it
                    string echo = webRequest.downloadHandler.text;

                    // Delete questionnaire
                    if (echo.Equals("uploaded survey successfully"))
                        cSurvey.Delete(questionnaireToUpload.local_path_id);
                }
            }
        }

        // ============== Delete Areas ============== //

        // Get areas to delete
        int[] areasToDelete = cArea.GetServerIdsToDelete();

        if (areasToDelete != null && areasToDelete.Length > 0)
        {
            // Show warning panel
            if (stopWatch.Elapsed.TotalSeconds > secondsToWaitBeforeWarning)
            {
                if (AppManager.Instance.uIManager.LanguageIsEnglish())
                    AppManager.Instance.uIManager.txtWarningServer.text = "Deleting areas...";
                else
                    AppManager.Instance.uIManager.txtWarningServer.text = "Διαγραφή περιοχών...";
                AppManager.Instance.uIManager.pnlWarningServerScreen.SetActive(true);
            }

            foreach (int server_area_idToDelete in areasToDelete)
            {
                WWWForm formToPost = new WWWForm();
                formToPost.AddField("action", Enum.GetName(typeof(PHPActions), 2)); // Delete_Area
                formToPost.AddField("server_area_id", server_area_idToDelete);

                UnityWebRequest webRequest = UnityWebRequest.Post(diadrasisAreaManagerUrl, formToPost);

                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    Debug.Log("Delete area with server id " + server_area_idToDelete + " failed. Error #" + webRequest.error);
                }
                else
                {
                    //Debug.Log("Posted successfully: " + webRequest.uploadHandler.data);
                    Debug.Log("Deleted area from server successfully!: " + server_area_idToDelete);
                    Debug.Log("Echo: " + webRequest.downloadHandler.text);
                    //AppManager.Instance.uIManager.pnlWarningServerScreen.SetActive(false);
                    //AppManager.Instance.uIManager.LoadingScreen("Updating...",1,3);
                    //AppManager.Instance.uIManager.imgLoading.fillAmount -= timeToCount / 10;
                    //Debug.Log("Data length: " + webRequest.downloadHandler.data);

                    // Remove id to delete from player prefs
                    cArea.RemoveIdToDelete(server_area_idToDelete);
                }
            }
        }

        // ============== Delete Paths ============== //

        // Get paths to delete
        int[] pathsToDelete = cPath.GetServerIdsToDelete();

        if (pathsToDelete != null && pathsToDelete.Length > 0)
        {
            // Show warning panel
            if (stopWatch.Elapsed.TotalSeconds > secondsToWaitBeforeWarning)
            {
                if (AppManager.Instance.uIManager.LanguageIsEnglish())
                    AppManager.Instance.uIManager.txtWarningServer.text = "Deleting paths...";
                else
                    AppManager.Instance.uIManager.txtWarningServer.text = "Διαγραφή διαδρομών...";
                AppManager.Instance.uIManager.pnlWarningServerScreen.SetActive(true);
            }

            foreach (int server_path_idToDelete in pathsToDelete)
            {
                WWWForm formToPost = new WWWForm();
                formToPost.AddField("action", Enum.GetName(typeof(PHPActions), 5)); // Delete_Path
                formToPost.AddField("server_path_id", server_path_idToDelete);

                UnityWebRequest webRequest = UnityWebRequest.Post(diadrasisAreaManagerUrl, formToPost);

                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    Debug.Log("Delete path with server id " + server_path_idToDelete + " failed. Error #" + webRequest.error);
                }
                else
                {
                    //Debug.Log("Posted successfully: " + webRequest.uploadHandler.data);
                    Debug.Log("Deleted path from server successfully!: " + server_path_idToDelete);
                    //AppManager.Instance.uIManager.LoadingScreen("Updating...", 1, 3);
                    //AppManager.Instance.uIManager.imgLoading.fillAmount -= timeToCount / 10;
                    Debug.Log("Echo: " + webRequest.downloadHandler.text);
                    //Debug.Log("Data length: " + webRequest.downloadHandler.data);

                    // Remove id to delete from player prefs
                    cPath.RemoveIdToDelete(server_path_idToDelete);
                }
            }
        }

        // Deactivate warning panel
        stopWatch.Stop();
        TimeSpan timeSpan = stopWatch.Elapsed;
        float secondsToWait = ((secondsToWaitBeforeWarning + minSecondsToDisplayWarning) - (float)timeSpan.TotalSeconds);
        yield return new WaitForSeconds(timeSpan.TotalSeconds > (secondsToWaitBeforeWarning + minSecondsToDisplayWarning) ? 0f : secondsToWait);
        AppManager.Instance.uIManager.pnlWarningServerScreen.SetActive(false);
    }

    public void DownloadAreas()
    {
        downloadAreas = true;
        OnlineMaps.instance.CheckServerConnection(OnCheckConnectionCompleteDownload);
    }

    IEnumerator GetAreas()
    {
        // Calculate seconds to Download
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();

        WWWForm formToPostGetAreas = new WWWForm();
        formToPostGetAreas.AddField("action", Enum.GetName(typeof(PHPActions), 0)); // Get_Areas

        UnityWebRequest webRequest = UnityWebRequest.Post(diadrasisAreaManagerUrl, formToPostGetAreas);

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.Log("Web request failed. Error #" + webRequest.error);
        }
        else
        {
            //Debug.Log("Posted successfully: " + webRequest.uploadHandler.data);
            //Debug.Log("Echo: " + webRequest.downloadHandler.text);
            //Debug.Log("Data length: " + webRequest.downloadHandler.data);

            // Get Area byte[] data
            byte[] areasData = webRequest.downloadHandler.data;

            if (areasData != null)
            {
                // Create a Json string from byte[]
                string json = System.Text.Encoding.UTF8.GetString(areasData);
                //Debug.Log("areas --> Json string = " + json);
                
                // Create a cAreasData from json string
                cAreaData[] areasDataFromJSON = MethodHelper.FromJson<cAreaData>(MethodHelper.SetupJson(json));

                if (areasDataFromJSON != null && areasDataFromJSON.Length > 0)
                {
                    // Show warning panel
                    if (stopWatch.Elapsed.TotalSeconds > secondsToWaitBeforeWarning)
                    {
                        if (AppManager.Instance.uIManager.LanguageIsEnglish())
                            AppManager.Instance.uIManager.txtWarningServer.text = "Updating areas...";
                        else
                            AppManager.Instance.uIManager.txtWarningServer.text = "Ενημέρωση περιοχών...";
                        AppManager.Instance.uIManager.pnlWarningServerScreen.SetActive(true);
                    }

                    foreach (cAreaData areaData in areasDataFromJSON)
                    {
                        // Create an area from areaData
                        cArea areaToSave = new cArea(
                            areaData.server_area_id,
                            //areaData.local_area_id,
                            areaData.title,
                            MethodHelper.ToVector2(areaData.position),
                            areaData.zoom,
                            MethodHelper.ToVector2(areaData.areaConstraintsMin),
                            MethodHelper.ToVector2(areaData.areaConstraintsMax),
                            MethodHelper.ToVector2(areaData.viewConstraintsMin),
                            MethodHelper.ToVector2(areaData.viewConstraintsMax));

                        // Debug
                        /*Debug.Log("downloadedArea databaseId = " + areaToSave.server_area_id);
                        Debug.Log("downloadedArea id = " + areaToSave.local_area_id);
                        Debug.Log("downloadedArea title = " + areaToSave.title);
                        Debug.Log("downloadedArea position = " + areaToSave.position);
                        Debug.Log("downloadedArea zoom = " + areaToSave.zoom);
                        Debug.Log("downloadedArea areaConstraintsMin = " + areaToSave.areaConstraintsMin);
                        Debug.Log("downloadedArea areaConstraintsMax = " + areaToSave.areaConstraintsMax);
                        Debug.Log("downloadedArea viewConstraintsMin = " + areaToSave.viewConstraintsMin);
                        Debug.Log("downloadedArea viewConstraintsMax = " + areaToSave.viewConstraintsMax);*/

                        // Save to Player Prefs
                        cArea.SaveFromServer(areaToSave);
                    }
                }
            }
        }

        // Reload areas
        AppManager.Instance.mapManager.ReloadAreas();

        // Deactivate warning panel
        stopWatch.Stop();
        TimeSpan timeSpan = stopWatch.Elapsed;
        yield return new WaitForSeconds(timeSpan.TotalSeconds > (secondsToWaitBeforeWarning + minSecondsToDisplayWarning) ? 0f : ((secondsToWaitBeforeWarning + minSecondsToDisplayWarning) - (float)timeSpan.TotalSeconds));
        AppManager.Instance.uIManager.pnlWarningServerScreen.SetActive(false);
    }

    public void DownloadPaths(int _server_area_id)
    {
        downloadAreaId = _server_area_id;
        OnlineMaps.instance.CheckServerConnection(OnCheckConnectionCompleteDownload);

        //StartCoroutine(GetPaths(_server_area_id));
    }

    IEnumerator GetPaths(int _server_area_id)
    {
        // Calculate seconds to Download
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();

        // Get area
        cArea selectedArea = AppManager.Instance.mapManager.currentArea;

        if (selectedArea != null)
        {
            // Download Area Tiles Locally
            /*tileDownloader.SetValues(selectedArea.areaConstraintsMin.x, selectedArea.areaConstraintsMax.y, selectedArea.areaConstraintsMax.x, selectedArea.areaConstraintsMin.y, OnlineMaps.MAXZOOM, OnlineMaps.MAXZOOM);
            //tileDownloader.Calculate();
            tileDownloader.Download();

            AppManager.Instance.uIManager.pnlWarningServerScreen.SetActive(true);

            while (tileDownloader.isDownloading)
            {
                // Update panel
                int percentage = Mathf.RoundToInt((float)(((double)tileDownloader.downloadedTiles / (double)tileDownloader.countTiles) * 100));
                AppManager.Instance.uIManager.txtWarningServer.text = "Downloading tiles... \n" + percentage + "%";

                yield return null;
            }*/

            // Download Tiles Locally
            tileDownloader.SetValues(selectedArea.areaConstraintsMin.x, selectedArea.areaConstraintsMax.y, selectedArea.areaConstraintsMax.x, selectedArea.areaConstraintsMin.y, OnlineMaps.MAXZOOM, OnlineMaps.MAXZOOM);
            if (!tileDownloader.HasTiles())
            {
                tileDownloader.Calculate();

                // Activate panel warning
                AppManager.Instance.uIManager.pnlWarningDownloadTilesScreen.SetActive(true);
                if (AppManager.Instance.uIManager.LanguageIsEnglish())
                    AppManager.Instance.uIManager.txtWarningDownloadTiles.text = "Would you like to download the area's tiles?\nSize: " + (tileDownloader.totalSize / 1000) + " ΜB";
                else
                    AppManager.Instance.uIManager.txtWarningDownloadTiles.text = "Θα θέλατε να κατεβάσετε το χάρτη της περιοχής;\nΜέγεθος: " + (tileDownloader.totalSize / 1000) + " ΜB";

                // Wait for user input
                while (AppManager.Instance.uIManager.pnlWarningDownloadTilesScreen.activeSelf)
                {
                    yield return null;
                }

                if (AppManager.Instance.uIManager.downloadTiles)
                {
                    tileDownloader.Download();
                    while (tileDownloader.isDownloading)
                    {
                        // Update panel
                        AppManager.Instance.uIManager.pnlWarningServerScreen.SetActive(true);
                        int percentage = Mathf.RoundToInt((float)(((double)tileDownloader.downloadedTiles / (double)tileDownloader.countTiles) * 100));
                        if (AppManager.Instance.uIManager.LanguageIsEnglish())
                            AppManager.Instance.uIManager.txtWarningServer.text = "Downloading tiles... \n" + percentage + "%";
                        else
                            AppManager.Instance.uIManager.txtWarningServer.text = "Λήψη χάρτη... \n" + percentage + "%";

                        yield return null;
                    }

                    AppManager.Instance.uIManager.downloadTiles = false;
                }
            }
        }

        WWWForm formToPostGetPaths = new WWWForm();
        formToPostGetPaths.AddField("action", Enum.GetName(typeof(PHPActions), 3)); // Get_Paths
        formToPostGetPaths.AddField("server_area_id", _server_area_id);

        UnityWebRequest webRequestPaths = UnityWebRequest.Post(diadrasisAreaManagerUrl, formToPostGetPaths);

        yield return webRequestPaths.SendWebRequest();

        if (webRequestPaths.isNetworkError || webRequestPaths.isHttpError)
        {
            Debug.Log("Web request failed. Error #" + webRequestPaths.error);
        }
        else
        {
            //Debug.Log("Posted successfully: " + webRequest.uploadHandler.data);
            //Debug.Log("Echo: " + webRequest.downloadHandler.text);
            //Debug.Log("Data length: " + webRequest.downloadHandler.data);

            // Get Area byte[] data
            byte[] pathsData = webRequestPaths.downloadHandler.data;

            if (pathsData != null)
            {
                // Create a Json string from byte[]
                string json = System.Text.Encoding.UTF8.GetString(pathsData);
                //Debug.Log("paths --> Json string = " + json);
                // Create a cAreasData from json string
                cPathData[] pathsDataFromJSON = MethodHelper.FromJson<cPathData>(MethodHelper.SetupJson(json));

                if (pathsDataFromJSON != null && pathsDataFromJSON.Length > 0)
                {
                    // Show warning panel
                    if (stopWatch.Elapsed.TotalSeconds > secondsToWaitBeforeWarning)
                    {
                        if (AppManager.Instance.uIManager.LanguageIsEnglish())
                            AppManager.Instance.uIManager.txtWarningServer.text = "Updating paths...";
                        else
                            AppManager.Instance.uIManager.txtWarningServer.text = "Ενημέρωση διαδρομών...";
                        AppManager.Instance.uIManager.pnlWarningServerScreen.SetActive(true);
                    }

                    foreach (cPathData pathData in pathsDataFromJSON)
                    {
                        // Fix date
                        /*string dateString = pathData.date.Replace("\\", "");
                        Debug.Log("dateString = " + dateString);
                        DateTime dateFromData = DateTime.ParseExact(dateString, "d/M/yyyy", CultureInfo.InvariantCulture);*/

                        // Create a path from pathData
                        cPath pathToSave = new cPath(
                            pathData.server_area_id,
                            pathData.server_path_id,
                            pathData.title,
                            pathData.date); //dateFromData

                        // Debug
                        /*Debug.Log("downloadedPath server_area_id = " + pathToSave.server_area_id);
                        Debug.Log("downloadedPath server_path_id = " + pathToSave.server_path_id);
                        Debug.Log("downloadedPath title = " + pathToSave.title);
                        Debug.Log("downloadedPath date = " + pathToSave.date);*/

                        // Save to Player Prefs
                        cPath.SaveFromServer(pathToSave);
                    }
                }
            }
        }

        // Reload Paths
        AppManager.Instance.mapManager.ReloadPaths();

        // Deactivate warning panel
        stopWatch.Stop();
        TimeSpan timeSpan = stopWatch.Elapsed;
        yield return new WaitForSeconds(timeSpan.TotalSeconds > (secondsToWaitBeforeWarning + minSecondsToDisplayWarning) ? 0f : ((secondsToWaitBeforeWarning + minSecondsToDisplayWarning) - (float)timeSpan.TotalSeconds));
        AppManager.Instance.uIManager.pnlWarningServerScreen.SetActive(false);
    }

    public void DownloadPoints(int _server_path_id)
    {
        downloadPathId = _server_path_id;
        OnlineMaps.instance.CheckServerConnection(OnCheckConnectionCompleteDownload);
    }

    IEnumerator GetPoints(int _server_path_id)
    {
        // Calculate seconds to Download
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();

        WWWForm formToPostGetPoints = new WWWForm();
        formToPostGetPoints.AddField("action", Enum.GetName(typeof(PHPActions), 6)); // Get_Points
        formToPostGetPoints.AddField("server_path_id", _server_path_id);

        UnityWebRequest webRequestPoints = UnityWebRequest.Post(diadrasisAreaManagerUrl, formToPostGetPoints);

        yield return webRequestPoints.SendWebRequest();

        if (webRequestPoints.isNetworkError || webRequestPoints.isHttpError)
        {
            Debug.Log("Web request failed. Error #" + webRequestPoints.error);
        }
        else
        {
            //Debug.Log("Posted successfully: " + webRequest.uploadHandler.data);
            //Debug.Log("Echo: " + webRequest.downloadHandler.text);
            //Debug.Log("Data length: " + webRequest.downloadHandler.data);

            // Get Area byte[] data
            byte[] pointsData = webRequestPoints.downloadHandler.data;

            if (pointsData != null)
            {
                // Create a Json string from byte[]
                string json = System.Text.Encoding.UTF8.GetString(pointsData);
                //Debug.Log("points --> Json string = " + json);
                // Create a cAreasData from json string
                cPointData[] pointsDataFromJSON = MethodHelper.FromJson<cPointData>(MethodHelper.SetupJson(json));

                if (pointsDataFromJSON != null && pointsDataFromJSON.Length > 0)
                {
                    // Show warning panel
                    if (stopWatch.Elapsed.TotalSeconds > secondsToWaitBeforeWarning)
                    {
                        if (AppManager.Instance.uIManager.LanguageIsEnglish())
                            AppManager.Instance.uIManager.txtWarningServer.text = "Updating points...";
                        else
                            AppManager.Instance.uIManager.txtWarningServer.text = "Ενημέρωση σημείων...";
                        AppManager.Instance.uIManager.pnlWarningServerScreen.SetActive(true);
                    }

                    foreach (cPointData pointData in pointsDataFromJSON)
                    {
                        // Create a path from pathData
                        cPathPoint pointToSave = new cPathPoint(
                            pointData.server_path_id,
                            pointData.server_point_id,
                            pointData.indexx,
                            MethodHelper.ToVector2(pointData.position),
                            pointData.duration);

                        // Debug
                        /*Debug.Log("downloadedPoint server_path_id = " + pointToSave.server_path_id);
                        Debug.Log("downloadedPoint server_point_id = " + pointToSave.server_point_id);
                        Debug.Log("downloadedPoint index = " + pointToSave.index);
                        Debug.Log("downloadedPoint position = " + pointToSave.position);
                        Debug.Log("downloadedPoint duration = " + pointToSave.duration);*/

                        // Save to Player Prefs
                        cPathPoint.SaveFromServer(pointToSave);
                    }
                }
            }
        }

        // Reload points
        AppManager.Instance.mapManager.ReloadPoints();

        // Deactivate warning panel
        stopWatch.Stop();
        TimeSpan timeSpan = stopWatch.Elapsed;
        yield return new WaitForSeconds(timeSpan.TotalSeconds > (secondsToWaitBeforeWarning + minSecondsToDisplayWarning) ? 0f : ((secondsToWaitBeforeWarning + minSecondsToDisplayWarning) - (float)timeSpan.TotalSeconds));
        AppManager.Instance.uIManager.pnlWarningServerScreen.SetActive(false);
    }

    public void DeleteAreaFromServer(int _server_area_idToDelete)
    {
        // Downloading data
        StartCoroutine(DeleteArea(_server_area_idToDelete));
    }

    IEnumerator DeleteArea(int _server_area_idToDelete)
    {
        WWWForm formToPost = new WWWForm();
        formToPost.AddField("action", Enum.GetName(typeof(PHPActions), 2)); // Delete_Area
        formToPost.AddField("server_area_id", _server_area_idToDelete);

        UnityWebRequest webRequest = UnityWebRequest.Post(diadrasisAreaManagerUrl, formToPost);

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.Log("Delete area with server id " + _server_area_idToDelete + " failed. Error #" + webRequest.error);
        }
        else
        {
            Debug.Log("Posted successfully: " + webRequest.uploadHandler.data);
            Debug.Log("Echo: " + webRequest.downloadHandler.text);
            //Debug.Log("Data length: " + webRequest.downloadHandler.data);

            // Remove id to delete from player prefs
            cArea.RemoveIdToDelete(_server_area_idToDelete);
        }
    }

    public void DeletePathFromServer(int _server_path_idToDelete)
    {
        // Downloading data
        StartCoroutine(DeletePath(_server_path_idToDelete));
    }

    IEnumerator DeletePath(int _server_path_idToDelete)
    {
        WWWForm formToPost = new WWWForm();
        formToPost.AddField("action", Enum.GetName(typeof(PHPActions), 5)); // Delete_Path
        formToPost.AddField("server_path_id", _server_path_idToDelete);

        UnityWebRequest webRequest = UnityWebRequest.Post(diadrasisAreaManagerUrl, formToPost);

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.Log("Delete path with server id " + _server_path_idToDelete + " failed. Error #" + webRequest.error);
        }
        else
        {
            Debug.Log("Posted successfully: " + webRequest.uploadHandler.data);
            Debug.Log("Echo: " + webRequest.downloadHandler.text);
            //Debug.Log("Data length: " + webRequest.downloadHandler.data);

            // Remove id to delete from player prefs
            cPath.RemoveIdToDelete(_server_path_idToDelete);
        }
    }
    #endregion
}