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
    readonly string getUrl = "http://localhost/UnityWebRequest/date.php?name=";
    readonly string postUrl = "http://localhost/UnityWebRequest/post.php";

    public delegate void InternetConnection(bool isOn);
    public InternetConnection OnCheckInternetCheckComplete;

    // URL to Post
    readonly string postDiadrasisUrl = "http://diadrasis.net/test_upload.php"; //"http://diadrasis.net/test_form.php"; //"http://diadrasis.net/test_upload.php"
    readonly string diadrasisAreaManagerUrl = "http://diadrasis.net/interarch_area_manager.php";
    private string testXMLFileName = "C:/Users/Andrew Xeroudakis/Desktop/testXMLFile.xml";
    public bool postUserData = true;

    public enum PHPActions {Save, Get, Delete, Edit}
    private cArea downloadedArea;
    #endregion

    #region UnityMethods
    private void Start()
    {
        //cArea.DeleteAreaFromServer(15);
        //cArea.DownloadAreas();
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
        //if ()
        {
            // if postUserData is true, uploads the user's data to the server
            // NOTE: The postUserData variable is set to true when opening the application, when the user saves a new area or when a path is added etc.
            if (postUserData)
            {
                //PostUserDataToDiadrasis();
                postUserData = false;
            }
        }
    }
    #endregion

    #region Methods
    void CreateXMLFile() // For Testing
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
    }

    /*public IEnumerator UploadFileData(string areaKey)
    {
        List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
        areaKey = AppManager.Instance.mapManager.currentArea.ToString();
        //if we change correctly the areakey string variable when the button that is assigned with this method, is pressed, then the xml file(in the server) will be updated
        wwwForm.Add(new MultipartFormDataSection("areaKey",areaKey));

        UnityWebRequest www = UnityWebRequest.Post(postUrl, wwwForm);

        yield return www.SendWebRequest();

        if(www.isHttpError || www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("File has uploaded");
        }
    }*/
    //from Stathis
    /*public void UploadPathsToServer()
    {
        Debug.Log("UPLOADING....");
        //pathManager.ShowMessage("UPLOADING....", 2f);
        StopAllCoroutines();
        StartCoroutine(PostSavedPaths());
    }

    IEnumerator PostSavedPaths()
    {
        List<string> savedPathsList = new List<string>();
        Debug.Log(savedPathsList);
        if (PlayerPrefs.HasKey(cPath.ID))
        {
            savedPathsList = PlayerPrefsX.GetStringArray(cPath.ID).ToList();
            Debug.Log(savedPathsList);
        }
        else
        {
            yield break;
        }
        for (int i = 0; i < savedPathsList.Count; i++)
        {

            if (savedPathsList[i].Contains("---"))
            {
                continue;
            }

            Debug.Log(savedPathsList[i]);

            // Load xml string from PlayerPrefs
            string xmlData = PlayerPrefs.GetString(savedPathsList[i]);
            Debug.Log(xmlData);
            //yield return StartCoroutine(PostPath(pathNamesSavedList[i], xmlData));
            yield return StartCoroutine(SaveXml(savedPathsList[i], xmlData));
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator SaveXml(string pathName, string xmlData)
    {
        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("action", "save");
        wwwForm.AddField("path_name", UnityWebRequest.EscapeURL(pathName));
        wwwForm.AddField("xml_file", xmlData);
        //wwwForm.AddField("timestamp", SystemInfo.deviceUniqueIdentifier); //TimeUtilities.GetUTCUnixTimestamp().ToString());
                                                                          //wwwForm.AddBinaryData("photo", myPhoto.EncodeToJPG());

        yield return new WaitForEndOfFrame();
        Texture2D txt2D = Stathis.File_Manager.loadImage<Texture2D>(pathName, Stathis.File_Manager.Ext.JPG);

        if (txt2D)
        {
            byte[] bytes = txt2D.EncodeToJPG();

            wwwForm.AddBinaryData("photo", bytes, "hehe.jpg", "image/jpg");
        }

        yield return new WaitForEndOfFrame();


        UnityWebRequest www = UnityWebRequest.Get(getUrl);

        // Wait until the request has been sent and a response from the server been recieved:
        yield return www;

        if (www.error == null)
        {
            // Success!
            // Debug.Log(www.text);
        }
        else
        {
            Debug.Log(www.isNetworkError);
            //pathManager.ShowMessage(www.error, 3f);
        }

        yield break;
    }*/

    #region InternetConnection
    public void CheckInternet()
    {
        OnlineMaps.instance.CheckServerConnection(OnCheckConnectionComplete);
    }

    public void OnCheckConnectionComplete(bool status)
    {
        // If the test is successful, then allow the user to manipulate the map.
        //OnlineMapsControlBase.instance.allowUserControl = status;

        // Showing test result in console.
        // Debug.Log(status ? "Has connection" : "No connection");

        if (OnCheckInternetCheckComplete != null) OnCheckInternetCheckComplete(status);
    }
    #endregion

    // ============= Andrew ============= //
    public void UploadArea(cArea _areaToUpload)
    {
        // Create a form and add all the fields of the area
        List<IMultipartFormSection> formToPost = new List<IMultipartFormSection>();
        formToPost.Add(new MultipartFormDataSection("action", Enum.GetName(typeof(PHPActions), 0))); // Save
        formToPost.Add(new MultipartFormDataSection("id", _areaToUpload.Id.ToString()));
        formToPost.Add(new MultipartFormDataSection("title", _areaToUpload.title));
        formToPost.Add(new MultipartFormDataSection("position", _areaToUpload.position.ToString("F6")));
        formToPost.Add(new MultipartFormDataSection("zoom", _areaToUpload.zoom.ToString()));
        formToPost.Add(new MultipartFormDataSection("areaConstraintsMin", _areaToUpload.areaConstraintsMin.ToString("F6")));
        formToPost.Add(new MultipartFormDataSection("areaConstraintsMax", _areaToUpload.areaConstraintsMax.ToString("F6")));
        formToPost.Add(new MultipartFormDataSection("viewConstraintsMin", _areaToUpload.viewConstraintsMin.ToString("F6")));
        formToPost.Add(new MultipartFormDataSection("viewConstraintsMax", _areaToUpload.viewConstraintsMax.ToString("F6")));
        
        /*formToPost.Add(new MultipartFormDataSection("areaConstraintsMinX", _areaToUpload.areaConstraintsMin.x.ToString()));
        formToPost.Add(new MultipartFormDataSection("areaConstraintsMinY", _areaToUpload.areaConstraintsMin.y.ToString()));
        formToPost.Add(new MultipartFormDataSection("areaConstraintsMaxX", _areaToUpload.areaConstraintsMax.x.ToString()));
        formToPost.Add(new MultipartFormDataSection("areaConstraintsMaxY", _areaToUpload.areaConstraintsMax.y.ToString()));
        formToPost.Add(new MultipartFormDataSection("viewConstraintsMinX", _areaToUpload.viewConstraintsMin.x.ToString()));
        formToPost.Add(new MultipartFormDataSection("viewConstraintsMinY", _areaToUpload.viewConstraintsMin.y.ToString()));
        formToPost.Add(new MultipartFormDataSection("viewConstraintsMaxX", _areaToUpload.viewConstraintsMax.x.ToString()));
        formToPost.Add(new MultipartFormDataSection("viewConstraintsMaxY", _areaToUpload.viewConstraintsMax.y.ToString()));*/

        // Uploading data
        StartCoroutine(PostToDiadrasisAreaManager(formToPost));
    }

    IEnumerator PostToDiadrasisAreaManager(List<IMultipartFormSection> _formToPost)
    {
        UnityWebRequest webRequest = UnityWebRequest.Post(diadrasisAreaManagerUrl, _formToPost);

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.Log("Test failed. Error #" + webRequest.error);
        }
        else
        {
            Debug.Log("Posted successfully: " + webRequest.uploadHandler.data);
            Debug.Log("Echo: " + webRequest.downloadHandler.text);
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
        formToPost.AddField("action", Enum.GetName(typeof(PHPActions), 1)); // Get

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
                Debug.Log("Json string = " + json);

                // Create a cAreasData from json string
                cAreaData[] areasDataFromJSON = MethodHelper.FromJson<cAreaData>(MethodHelper.SetupJson(json));

                if (areasDataFromJSON != null)
                {
                    foreach (cAreaData areaData in areasDataFromJSON)
                    {
                        // Create an area from json string
                        cArea areaToSave = new cArea(
                            //areaData.area_id
                            areaData.id,
                            areaData.title,
                            MethodHelper.ToVector2(areaData.position),
                            areaData.zoom,
                            MethodHelper.ToVector2(areaData.areaConstraintsMin),
                            MethodHelper.ToVector2(areaData.areaConstraintsMax),
                            MethodHelper.ToVector2(areaData.viewConstraintsMin),
                            MethodHelper.ToVector2(areaData.viewConstraintsMax));

                        areaToSave.area_id = areaData.area_id; // TODO: Remove and add it to Constructor

                        // Debug
                        //Debug.Log("areaDataFromJSON area_id = " + areasDataFromJSON[0].area_id);
                        Debug.Log("downloadedArea id = " + areaToSave.Id);
                        Debug.Log("downloadedArea title = " + areaToSave.title);
                        Debug.Log("downloadedArea position = " + areaToSave.position);
                        Debug.Log("downloadedArea zoom = " + areaToSave.zoom);
                        Debug.Log("downloadedArea areaConstraintsMin = " + areaToSave.areaConstraintsMin);
                        Debug.Log("downloadedArea areaConstraintsMax = " + areaToSave.areaConstraintsMax);
                        Debug.Log("downloadedArea viewConstraintsMin = " + areaToSave.viewConstraintsMin);
                        Debug.Log("downloadedArea viewConstraintsMax = " + areaToSave.viewConstraintsMax);

                        // Save to Player Prefs
                        cArea.Save(areaToSave);
                    }
                }
            }
        }
    }

    public void DeleteAreaFromServer(int _areaIdToDelete)
    {
        // Downloading data
        StartCoroutine(DeleteArea(_areaIdToDelete));
    }

    IEnumerator DeleteArea(int _areaIdToDelete)
    {
        WWWForm formToPost = new WWWForm();
        formToPost.AddField("action", Enum.GetName(typeof(PHPActions), 2)); // Delete
        formToPost.AddField("area_id", _areaIdToDelete);

        UnityWebRequest webRequest = UnityWebRequest.Post(diadrasisAreaManagerUrl, formToPost);

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.Log("Test failed. Error #" + webRequest.error);
        }
        else
        {
            Debug.Log("Posted successfully: " + webRequest.uploadHandler.data);
            Debug.Log("Echo: " + webRequest.downloadHandler.text);
            Debug.Log("Data length: " + webRequest.downloadHandler.data);
        }
    }

    private void PostUserDataToDiadrasis()
    {
        // Uploading data
        StartCoroutine(PostJPGFileToDiadrasis()); // PostXMLFileToDiadrasis()
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

    IEnumerator PostJPGFileToDiadrasis()
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
    }

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