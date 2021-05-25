using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;

public class ServerManager : MonoBehaviour
{
    #region Variables
    //readonly string getUrl = "http://localhost/UnityWebRequest/date.php?name=";
    //readonly string postUrl = "http://localhost/UnityWebRequest/post.php";

    public delegate void InternetConnection(bool isOn);
    public InternetConnection OnCheckInternetCheckComplete;

    // URL to Post
    readonly string postDiadrasisUrl = "http://diadrasis.net/test_upload.php"; //"http://diadrasis.net/test_form.php"; //"http://diadrasis.net/test_upload.php"
    readonly string diadrasisAreaManagerUrl = "http://diadrasis.net/interarch_area_manager.php";
    //readonly string diadrasisPathManagerUrl = "http://diadrasis.net/interarch_path_manager.php";
    //private string testXMLFileName = "C:/Users/Andrew Xeroudakis/Desktop/testXMLFile.xml";
    public bool postUserData = false; // true;

    public enum PHPActions { Get_Areas, Save_Area, Delete_Area, Get_Paths, Save_Path, Delete_Path, Get_Points, Save_Point, Delete_Point }
    #endregion

    #region UnityMethods
    private void Start()
    {
        postUserData = true;

        // Test ids
        /*cArea.AddIdToDelete(10);
        cArea.AddIdToDelete(11);
        cArea.AddIdToDelete(12);
        cArea.AddIdToDelete(13);

        // Delete all areas from server
        for (int i = 10; i < 14; i++)
        {
            DeleteAreaFromServer(i);
        }*/

        // Delete all areas from server
        /*for (int i = 0; i < 100; i++)
        {
            DeletePathFromServer(i);
        }*/


        //cArea.DeleteAreaFromServer(90);
        /*if (CheckInternet())
        {
            DownloadAreas();
            AppManager.Instance.mapManager.areas = new List<cArea>();
            AppManager.Instance.mapManager.areas = cArea.LoadAreas();
        }*/
            
        //Debug.Log(Enum.GetName(typeof(PHPActions), 0));
        //Debug.Log("dataPath : " + Application.dataPath + "/../sunflowerTest.jpg");
        //Debug.Log(SystemInfo.deviceModel);
        // CreateXMLFile();
        // Debug.Log(cArea.GetXML().outerXml);
        //Texture2D sunflowerTexture = Resources.Load("Sunflower", typeof(Texture2D)) as Texture2D;
        //Debug.Log(sunflowerTexture.);
    }

    private void Update()
    {
        // Check if there is an internet connection
        if (postUserData && CheckInternet())
        {
            // if postUserData is true, uploads the user's data to the server
            // NOTE: The postUserData variable is set to true when opening the application, when the user saves a new area or when a path is added etc.
			//StartCoroutine(UploadUserDataToDiadrasis());
			postUserData = false;
        }
    }
    #endregion

    #region Methods
    /*void CreateXMLFile() // For Testing
    {
        string path = Application.dataPath + "/testXMLFile.xml";
        File.WriteAllText(path, cArea.GetXML().outerXml);
        Debug.Log("Created/Updated file at path: " + path);
    }

    public void GetTest()
    {
        Debug.Log("Pressed");
        Debug.Log("Get Test Server Manager\n" + AppManager.Instance.mapManager.currentArea.title);
        StartCoroutine(GetFile(PlayerPrefs.GetString(cArea.PREFS_KEY)));
    }
    
    IEnumerator GetFile(string areaName)
    {
        string URL = getUrl + areaName ;

        UnityWebRequest webRequest = UnityWebRequest.Get(URL);
        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            // Show results as text
            Debug.Log(webRequest.downloadHandler.text);
            //point.title = areaName;
            // Or retrieve results as binary data
            byte[] results = webRequest.downloadHandler.data;
        }
    }

    public void PostTest()
    {
        StartCoroutine(PostText());
    }

    public IEnumerator PostText()
    {
        string URL = postUrl;
        UnityWebRequest www = UnityWebRequest.Get(URL);
        yield return www.SendWebRequest();
        AppManager.Instance.uIManager.infoText.text = www.downloadHandler.text;
    }*/


    #region InternetConnection
    public bool CheckInternet()
    {
        OnlineMaps.instance.CheckServerConnection(OnCheckConnectionComplete);
        return true;
    }

    public void OnCheckConnectionComplete(bool status)
    {
        if (OnCheckInternetCheckComplete != null) OnCheckInternetCheckComplete(status);

        if (status)
        {
            AppManager.Instance.uIManager.pnlWarningScreen.SetActive(false);
        }
        else
        {
            AppManager.Instance.uIManager.pnlWarningScreen.SetActive(true);
            AppManager.Instance.uIManager.txtWarning.text = "Please check your internet connection";
        }

        // If the test is successful, then allow the user to manipulate the map.
        //OnlineMapsControlBase.instance.allowUserControl = status;
        // Showing test result in console.
        Debug.Log(status ? "Has connection" : "No connection");
    }
    #endregion

    // ============= Andrew ============= //
    public void UploadArea(cArea _areaToUpload)
    {
        // Create a form and add all the fields of the area
        List<IMultipartFormSection> formToPost = new List<IMultipartFormSection>();
        formToPost.Add(new MultipartFormDataSection("action", Enum.GetName(typeof(PHPActions), 1))); // Save_Area
        formToPost.Add(new MultipartFormDataSection("server_area_id", _areaToUpload.server_area_id.ToString()));
        //formToPost.Add(new MultipartFormDataSection("local_area_id", _areaToUpload.local_area_id.ToString()));
        formToPost.Add(new MultipartFormDataSection("title", _areaToUpload.title));
        formToPost.Add(new MultipartFormDataSection("position", _areaToUpload.position.ToString("F6")));
        formToPost.Add(new MultipartFormDataSection("zoom", _areaToUpload.zoom.ToString()));
        formToPost.Add(new MultipartFormDataSection("areaConstraintsMin", _areaToUpload.areaConstraintsMin.ToString("F6")));
        formToPost.Add(new MultipartFormDataSection("areaConstraintsMax", _areaToUpload.areaConstraintsMax.ToString("F6")));
        formToPost.Add(new MultipartFormDataSection("viewConstraintsMin", _areaToUpload.viewConstraintsMin.ToString("F6")));
        formToPost.Add(new MultipartFormDataSection("viewConstraintsMax", _areaToUpload.viewConstraintsMax.ToString("F6")));

        // Uploading data
        StartCoroutine(PostAreaToDiadrasis(formToPost, _areaToUpload.local_area_id));
    }

    public void UploadPath(cPath _pathToUpload)
    {
        // Create a form and add all the fields of the area
        List<IMultipartFormSection> formToPost = new List<IMultipartFormSection>();
        formToPost.Add(new MultipartFormDataSection("action", Enum.GetName(typeof(PHPActions), 4))); // Save_Path
        formToPost.Add(new MultipartFormDataSection("server_area_id", _pathToUpload.server_area_id.ToString()));
        formToPost.Add(new MultipartFormDataSection("server_path_id", _pathToUpload.server_area_id.ToString()));
        //formToPost.Add(new MultipartFormDataSection("local_area_id", _pathToUpload.local_area_id.ToString()));
        //formToPost.Add(new MultipartFormDataSection("local_path_id", _pathToUpload.local_path_id.ToString()));
        formToPost.Add(new MultipartFormDataSection("title", _pathToUpload.title));
        formToPost.Add(new MultipartFormDataSection("date", _pathToUpload.date.ToShortDateString())); // TODO: Change???

        // Uploading data
        StartCoroutine(PostPathToDiadrasis(formToPost, _pathToUpload.server_area_id, _pathToUpload.local_path_id));
    }

    public void UploadPoint(cPathPoint _pointToUpload)
    {
        // Create a form and add all the fields of the area
        List<IMultipartFormSection> formToPost = new List<IMultipartFormSection>();
        formToPost.Add(new MultipartFormDataSection("action", Enum.GetName(typeof(PHPActions), 7))); // Save_Point
        formToPost.Add(new MultipartFormDataSection("server_path_id", _pointToUpload.server_path_id.ToString()));
        formToPost.Add(new MultipartFormDataSection("server_point_id", _pointToUpload.server_point_id.ToString()));
        //formToPost.Add(new MultipartFormDataSection("local_area_id", _pathToUpload.local_area_id.ToString()));
        //formToPost.Add(new MultipartFormDataSection("local_path_id", _pathToUpload.local_path_id.ToString()));
        formToPost.Add(new MultipartFormDataSection("indexx", _pointToUpload.index.ToString()));
        formToPost.Add(new MultipartFormDataSection("position", _pointToUpload.position.ToString()));
        //formToPost.Add(new MultipartFormDataSection("time", _pointToUpload.time.ToString())); // TODO: Change???
        formToPost.Add(new MultipartFormDataSection("duration", _pointToUpload.duration.ToString()));

        // Uploading data
        StartCoroutine(PostPointToDiadrasis(formToPost, _pointToUpload.server_path_id, _pointToUpload.index));
    }

    IEnumerator UploadUserDataToDiadrasis()
    {
        // ============== Upload Areas ============== //
        Debug.Log("Started uploading user data!");
        // Get areas to upload
        List<cArea> areasToUpload = cArea.GetAreasToUpload();

        if (areasToUpload != null && areasToUpload.Count > 0)
        {
            foreach (cArea areaToUpload in areasToUpload)
            {
                // Create a form and add all the fields of the area
                List<IMultipartFormSection> formToPost = new List<IMultipartFormSection>();
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
                    Debug.Log("Test failed. Error #" + webRequest.error);
                }
                else
                {
                    //Debug.Log("Posted successfully: " + webRequest.uploadHandler.data);
                    Debug.Log("Uploaded area successfully!");
                    Debug.Log("Echo: " + webRequest.downloadHandler.text);
                    
                    // Get database id and set it
                    string echo = webRequest.downloadHandler.text;
                    string server_area_idString = echo.Replace("[{\"max(server_area_id)\":\"", "").Replace("\"}]", "");
                    if (int.TryParse(server_area_idString, out int server_area_id))
                    {
                        cArea.SetServerAreaId(areaToUpload.local_area_id, server_area_id);
                    }
                }
            }
        }

        // ============== Upload Paths ============== //

        // Get paths to upload
        List<cPath> pathsToUpload = cPath.GetPathsToUpload();

        if (pathsToUpload != null && pathsToUpload.Count > 0)
        {
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
                formToPost.Add(new MultipartFormDataSection("date", pathToUpload.date.ToShortDateString()));

                UnityWebRequest webRequest = UnityWebRequest.Post(diadrasisAreaManagerUrl, formToPost);

                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    Debug.Log("Test failed. Error #" + webRequest.error);
                }
                else
                {
                    Debug.Log("Echo: " + webRequest.downloadHandler.text);
                    Debug.Log("Uploaded path successfully!");
                    // Get database id and set it
                    string echo = webRequest.downloadHandler.text;
                    string server_path_idString = echo.Replace("[{\"max(server_path_id)\":\"", "").Replace("\"}]", "");
                    //string server_path_idString = cleanString.Replace(" ", "");
                    //Debug.Log("databaseId = " + server_path_idString);
                    if (int.TryParse(server_path_idString, out int server_path_id))
                        cPath.SetServerAreaAndPathId(pathToUpload.server_area_id, server_path_id, pathToUpload.local_path_id);
                }
            }
        }

        // ============== Upload Points ============== //

        // Get points to upload
        List<cPathPoint> pointsToUpload = cPathPoint.GetPointsToUpload();

        if (pointsToUpload != null && pointsToUpload.Count > 0)
        {
            foreach (cPathPoint pointToUpload in pointsToUpload)
            {
                // Create a form and add all the fields of the area
                List<IMultipartFormSection> formToPost = new List<IMultipartFormSection>();
                formToPost.Add(new MultipartFormDataSection("action", Enum.GetName(typeof(PHPActions), 7))); // Save_Point
                formToPost.Add(new MultipartFormDataSection("server_path_id", pointToUpload.server_path_id.ToString()));
                formToPost.Add(new MultipartFormDataSection("server_point_id", pointToUpload.server_point_id.ToString()));
                //formToPost.Add(new MultipartFormDataSection("local_area_id", pointToUpload.local_area_id.ToString()));
                //formToPost.Add(new MultipartFormDataSection("local_path_id", pointToUpload.local_path_id.ToString()));
                formToPost.Add(new MultipartFormDataSection("index", pointToUpload.index.ToString()));
                formToPost.Add(new MultipartFormDataSection("position", pointToUpload.position.ToString()));
                //formToPost.Add(new MultipartFormDataSection("time", pointToUpload.time.ToString())); // TODO: Change???
                formToPost.Add(new MultipartFormDataSection("duration", pointToUpload.duration.ToString()));

                UnityWebRequest webRequest = UnityWebRequest.Post(diadrasisAreaManagerUrl, formToPost);

                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    Debug.Log("Test failed. Error #" + webRequest.error);
                }
                else
                {
                    Debug.Log("Uploaded point successfully!");
                    Debug.Log("Echo: " + webRequest.downloadHandler.text);

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

        // ============== Delete Areas ============== //

        // Get areas to delete
        int[] areasToDelete = cArea.GetServerIdsToDelete();

        if (areasToDelete != null && areasToDelete.Length > 0)
        {
            Debug.Log("Has areas to delete!");
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
                    //Debug.Log("Data length: " + webRequest.downloadHandler.data);

                    // Remove id to delete from player prefs
                    cArea.RemoveIdToDelete(server_area_idToDelete);
                }
            }
        }

        // ============== Delete Paths ============== //

        // Get paths to delete
        int[] pathsToDelete = cPath.GetServerIdsToDelete();

        if (pathsToDelete != null)
        {
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
                    Debug.Log("Echo: " + webRequest.downloadHandler.text);
                    //Debug.Log("Data length: " + webRequest.downloadHandler.data);

                    // Remove id to delete from player prefs
                    cPath.RemoveIdToDelete(server_path_idToDelete);
                }
            }
        }
    }

    private void PostUserDataToDiadrasis()
    {
        // Debug
        Debug.Log("Started posting user data!");

        // Get areas to upload
        List<cArea> areasToUpload = cArea.GetAreasToUpload();

        if (areasToUpload != null && areasToUpload.Count > 0)
        {
            foreach (cArea areaToUpload in areasToUpload)
            {
                UploadArea(areaToUpload);
            }
        }

        // Get paths to upload
        /*List<cPath> pathsToUpload = cPath.GetPathsToUpload();

        if (pathsToUpload != null && pathsToUpload.Count > 0)
        {
            foreach (cPath pathToUpload in pathsToUpload)
            {
                UploadPath(pathToUpload);
            }
        }

        // Get paths to upload
        List<cPathPoint> pointsToUpload = cPathPoint.GetPointsToUpload();

        if (pointsToUpload != null && pointsToUpload.Count > 0)
        {
            foreach (cPathPoint pointToUpload in pointsToUpload)
            {
                UploadPoint(pointToUpload);
            }
        }*/

        // Get areas to delete
        int[] areasToDelete = cArea.GetServerIdsToDelete();

        if (areasToDelete != null && areasToDelete.Length > 0)
        {
            Debug.Log("Has areas to delete!");
            foreach (int areaToDelete in areasToDelete)
            {
                DeleteAreaFromServer(areaToDelete);
            }
        }

        // Get paths to delete ??????
        /*int[] pathsToDelete = cPath.GetServerIdsToDelete();

        if (pathsToDelete != null)
        {
            foreach (int pathToDelete in pathsToDelete)
            {
                DeletePathFromServer(pathToDelete);
            }
        }*/
    }

    IEnumerator PostAreaToDiadrasis(List<IMultipartFormSection> _formToPost, int _local_area_id)
    {
        UnityWebRequest webRequest = UnityWebRequest.Post(diadrasisAreaManagerUrl, _formToPost);

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.Log("Test failed. Error #" + webRequest.error);
        }
        else
        {
            //Debug.Log("Posted successfully: " + webRequest.uploadHandler.data);
            Debug.Log("Echo: " + webRequest.downloadHandler.text);

            // Get database id and set it
            string echo = webRequest.downloadHandler.text;
            string server_area_idString = echo.Replace("[{\"max(server_area_id)\":\"", "").Replace("\"}]", "");
            // Debug.Log("_" + server_area_idString + "_");
            // Debug.Log("length = " + server_area_idString.Length);
            if (int.TryParse(server_area_idString, out int server_area_id))
            {
                //Debug.Log("server_area_id = " + server_area_id);
                cArea.SetServerAreaId(_local_area_id, server_area_id);
            }
                
            /*string echo = webRequest.downloadHandler.text;
            string cleanString = echo.Substring(echo.LastIndexOf('=') + 1);
            string databaseIdString = cleanString.Replace(" ", "");
            Debug.Log("databaseId = " + databaseIdString);
            if (int.TryParse(databaseIdString, out int databaseId))
                cArea.SetDatabaseId(_areaId, databaseId);*/
            //cArea.RemoveIdToUpload(result);
        }
    }

    IEnumerator PostPathToDiadrasis(List<IMultipartFormSection> _formToPost, int _server_area_id, int _local_path_id)
    {
        UnityWebRequest webRequest = UnityWebRequest.Post(diadrasisAreaManagerUrl, _formToPost);

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.Log("Test failed. Error #" + webRequest.error);
        }
        else
        {
            Debug.Log("Echo: " + webRequest.downloadHandler.text);

            // Get database id and set it
            string echo = webRequest.downloadHandler.text;
            string server_path_idString = echo.Replace("[{\"max(server_path_id)\":\"", "").Replace("\"}]", "");
            //string server_path_idString = cleanString.Replace(" ", "");
            //Debug.Log("databaseId = " + server_path_idString);
            if (int.TryParse(server_path_idString, out int server_path_id))
                cPath.SetServerAreaAndPathId(_server_area_id, server_path_id, _local_path_id);
        }
    }

    IEnumerator PostPointToDiadrasis(List<IMultipartFormSection> _formToPost, int _server_path_id, int _index)
    {
        UnityWebRequest webRequest = UnityWebRequest.Post(diadrasisAreaManagerUrl, _formToPost);

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.Log("Test failed. Error #" + webRequest.error);
        }
        else
        {
            Debug.Log("Echo: " + webRequest.downloadHandler.text);

            // Get database id and set it
            string echo = webRequest.downloadHandler.text;
            string server_point_idString = echo.Replace("[{\"max(server_point_id)\":\"", "").Replace("\"}]", "");
            //string server_path_idString = cleanString.Replace(" ", "");
            //Debug.Log("databaseId = " + server_path_idString);
            if (int.TryParse(server_point_idString, out int server_point_id))
                cPathPoint.SetServerPathAndPointId(_server_path_id, server_point_id, _index);
        }
    }

    public void DownloadAreas()
    {
        // Downloading data
        StartCoroutine(GetAreas());
    }

    IEnumerator GetAreas()
    {
        WWWForm formToPost = new WWWForm();
        formToPost.AddField("action", Enum.GetName(typeof(PHPActions), 0)); // Get_Areas

        UnityWebRequest webRequest = UnityWebRequest.Post(diadrasisAreaManagerUrl, formToPost);

        yield return webRequest.SendWebRequest();
        
        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.Log("Test failed. Error #" + webRequest.error);
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
                //Debug.Log("Json string = " + json);

                // Create a cAreasData from json string
                cAreaData[] areasDataFromJSON = MethodHelper.FromJson<cAreaData>(MethodHelper.SetupJson(json));

                if (areasDataFromJSON != null)
                {
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
                        Debug.Log("downloadedArea databaseId = " + areaToSave.server_area_id);
                        Debug.Log("downloadedArea id = " + areaToSave.local_area_id);
                        Debug.Log("downloadedArea title = " + areaToSave.title);
                        Debug.Log("downloadedArea position = " + areaToSave.position);
                        Debug.Log("downloadedArea zoom = " + areaToSave.zoom);
                        Debug.Log("downloadedArea areaConstraintsMin = " + areaToSave.areaConstraintsMin);
                        Debug.Log("downloadedArea areaConstraintsMax = " + areaToSave.areaConstraintsMax);
                        Debug.Log("downloadedArea viewConstraintsMin = " + areaToSave.viewConstraintsMin);
                        Debug.Log("downloadedArea viewConstraintsMax = " + areaToSave.viewConstraintsMax);

                        // Save to Player Prefs
                        cArea.SaveFromServer(areaToSave);
                    }
                }
            }
        }
    }

    public void DownloadPaths()
    {
        // Downloading data
        StartCoroutine(GetPaths());
    }

    IEnumerator GetPaths()
    {
        WWWForm formToPost = new WWWForm();
        formToPost.AddField("action", Enum.GetName(typeof(PHPActions), 3)); // Get_Paths

        UnityWebRequest webRequest = UnityWebRequest.Post(diadrasisAreaManagerUrl, formToPost);

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.Log("Test failed. Error #" + webRequest.error);
        }
        else
        {
            //Debug.Log("Posted successfully: " + webRequest.uploadHandler.data);
            //Debug.Log("Echo: " + webRequest.downloadHandler.text);
            //Debug.Log("Data length: " + webRequest.downloadHandler.data);

            // Get Area byte[] data
            byte[] pathsData = webRequest.downloadHandler.data;

            if (pathsData != null)
            {
                // Create a Json string from byte[]
                string json = System.Text.Encoding.UTF8.GetString(pathsData);
                //Debug.Log("Json string = " + json);

                // Create a cAreasData from json string
                cPathData[] pathsDataFromJSON = MethodHelper.FromJson<cPathData>(MethodHelper.SetupJson(json));

                if (pathsDataFromJSON != null)
                {
                    foreach (cPathData pathData in pathsDataFromJSON)
                    {
                        // Create a path from pathData
                        cPath pathToSave = new cPath(
                            pathData.server_area_id,
                            pathData.server_path_id,
                            pathData.title,
                            Convert.ToDateTime(pathData.date));

                        // Debug
                        Debug.Log("downloadedPath server_area_id = " + pathToSave.server_area_id);
                        Debug.Log("downloadedPath server_path_id = " + pathToSave.server_path_id);
                        Debug.Log("downloadedPath title = " + pathToSave.title);
                        Debug.Log("downloadedPath date = " + pathToSave.date);

                        // Save to Player Prefs
                        cPath.SaveFromServer(pathToSave);
                    }
                }
            }
        }
    }

    public void DownloadPoints()
    {
        // Downloading data
        StartCoroutine(GetPoints());
    }

    IEnumerator GetPoints()
    {
        WWWForm formToPost = new WWWForm();
        formToPost.AddField("action", Enum.GetName(typeof(PHPActions), 6)); // Get_Points

        UnityWebRequest webRequest = UnityWebRequest.Post(diadrasisAreaManagerUrl, formToPost);

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.Log("Test failed. Error #" + webRequest.error);
        }
        else
        {
            //Debug.Log("Posted successfully: " + webRequest.uploadHandler.data);
            //Debug.Log("Echo: " + webRequest.downloadHandler.text);
            //Debug.Log("Data length: " + webRequest.downloadHandler.data);

            // Get Area byte[] data
            byte[] pointsData = webRequest.downloadHandler.data;

            if (pointsData != null)
            {
                // Create a Json string from byte[]
                string json = System.Text.Encoding.UTF8.GetString(pointsData);
                //Debug.Log("Json string = " + json);

                // Create a cAreasData from json string
                cPointData[] pointsDataFromJSON = MethodHelper.FromJson<cPointData>(MethodHelper.SetupJson(json));

                if (pointsDataFromJSON != null)
                {
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
                        Debug.Log("downloadedPoint server_path_id = " + pointToSave.server_path_id);
                        Debug.Log("downloadedPoint server_point_id = " + pointToSave.server_point_id);
                        Debug.Log("downloadedPoint index = " + pointToSave.index);
                        Debug.Log("downloadedPoint position = " + pointToSave.position);
                        Debug.Log("downloadedPoint duration = " + pointToSave.duration);

                        // Save to Player Prefs
                        cPathPoint.SaveFromServer(pointToSave);
                    }
                }
            }
        }
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

    IEnumerator PostXMLFileToDiadrasis()
    {
        // Create web form and add data to it
        string xmlData = cArea.GetXML().outerXml;
        List<IMultipartFormSection> webForm = new List<IMultipartFormSection>();
        //webForm.Add(new MultipartFormFileSection(xmlData, "areasData.xml")); // webForm.Add(new MultipartFormFileSection("my file data", "myfile.txt"));
        webForm.Add(new MultipartFormDataSection("area_no", "100"));
        webForm.Add(new MultipartFormDataSection("area_name", "Sarri"));
        webForm.Add(new MultipartFormDataSection("area_xml", xmlData)); // SystemInfo.deviceModel
        //webForm.Add
        //Debug.Log(xmlData);

        UnityWebRequest webRequest = UnityWebRequest.Post(postDiadrasisUrl, webForm);

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.Log("Test failed. Error #" + webRequest.error);
        }
        else
        {
            //webRequest.downloadHandler.text;
            Debug.Log("Posted successfully: " + webRequest.uploadHandler.data);
        }

        //yield return new WaitForSeconds(1);

        //CreateXMLFile();
    }

    /*IEnumerator PostJPGFileToDiadrasis()
    {
        // Get jpg from resources folder
        Texture2D sunflowerTexture = Resources.Load("Sunflower", typeof(Texture2D)) as Texture2D;

        // Encode texture to JPG
        byte[] textureBytes = sunflowerTexture.EncodeToJPG();
        Debug.Log("array length = " + textureBytes.Length);
        WWWForm form = new WWWForm();
        form.AddBinaryData("sunflower", textureBytes, "sunflower.jpg", "image/jpg");
        // Create web form and add data to it
        //List<IMultipartFormSection> webForm = new List<IMultipartFormSection>();
        //webForm.Add(new MultipartFormFileSection("sunflower2.jpg", textureBytes)); // webForm.Add(new MultipartFormFileSection("my file data", "myfile.txt"));
        //webForm.Add(new MultipartFormDataSection("area_no", "100"));
        //webForm.Add(new MultipartFormDataSection("area_name", "Sarri"));
        //webForm.Add(new MultipartFormDataSection("area_xml", xmlData)); // SystemInfo.deviceModel
        //Debug.Log(xmlData);

        // For testing purposes, also write to a file in the project folder
        File.WriteAllBytes(Application.dataPath + "/../sunflowerTest.jpg", textureBytes);

        UnityWebRequest webRequest = UnityWebRequest.Post(postDiadrasisUrl, form); //UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path)

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.Log("Test failed. Error #" + webRequest.error);
        }
        else
        {
            //webRequest.downloadHandler.text;
            Debug.Log("Posted successfully: " + webRequest.uploadHandler.data);
            Debug.Log("Echo: " + webRequest.downloadHandler.text);
        }

        //yield return new WaitForSeconds(1);

        //CreateXMLFile();
    }*/

    /*IEnumerator GetText()
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // Get downloaded asset bundle
                t = DownloadHandlerTexture.GetContent(uwr);
                skybox.SetTexture("_MainTex", t);
            }
        }
    }*/
    #endregion
}