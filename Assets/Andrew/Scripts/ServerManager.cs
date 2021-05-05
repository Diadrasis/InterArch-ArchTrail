using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ServerManager : MonoBehaviour
{
    readonly string getUrl = "http://localhost/UnityWebRequest/date.php?name=";
    readonly string postUrl = "http://localhost/UnityWebRequest/post.php";

    public void GetTest()
    {
        Debug.Log("Get Test Server Manager\n" + AppManager.Instance.mapManager.currentArea.title);
        StartCoroutine(GetFile(PlayerPrefs.GetString(cArea.PREFS_KEY)));
    }
    
    IEnumerator GetFile(string areaName)
    {
        string URL = getUrl + areaName ;

        UnityWebRequest www = UnityWebRequest.Get(URL);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            //point.title = areaName;
            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
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
        *//*Texture2D txt2D = Stathis.File_Manager.loadImage<Texture2D>(pathName, Stathis.File_Manager.Ext.JPG);

        if (txt2D)
        {
            byte[] bytes = txt2D.EncodeToJPG();

            wwwForm.AddBinaryData("photo", bytes, "hehe.jpg", "image/jpg");
        }

        yield return new WaitForEndOfFrame();
*//*

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
}
