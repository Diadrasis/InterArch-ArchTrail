using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cPath
{
    #region Variables
    public string areaTitle;
    public string title;
    //List<cPathPoint> pathPoints = new List<cPathPoint>();

    //private static readonly string PATH_KEY = "_path_";
    private static readonly string PATH_KEY = "path";
    #endregion

    #region Methods
    public cPath(string _areaTitle, string _title) //, List<cPathPoint> _pathPoints
    {
        areaTitle = _areaTitle;
        title = _title;
        //pathPoints = _pathPoints;
    }

    //create a path
    public void Create()
    {

    }

    //delete a path
    public void Delete()
    {

    }

    //add a path point which will happen associated with cRoutePoints.cs
    public void AddPathPoint()
    {

    }

    //Show/load the path in our scene
    public void Show()
    {

    }

    private static int GetAvailablePathIndex(string _areaTitle)
    {
        int index = 0;

        do
        {
            string pathTitle = PlayerPrefs.GetString(_areaTitle + PATH_KEY + index.ToString());
            if (String.IsNullOrEmpty(pathTitle))
                break;
            index++;
        }
        while (index < 1000000);

        return (index < 1000000) ? index : -1;
    }

    /*public static void Save(cPath _pathToSave)
    {
        int availablePathIndex = GetAvailablePathIndex(_pathToSave.areaTitle);
        Debug.Log("available path index = " + availablePathIndex); // TODO: Remove!!!
        if (availablePathIndex == -1)
        {
            Debug.Log("GetAvailablePathIndex method has run out of available indexes");
            return;
        }

        PlayerPrefs.SetString(_pathToSave.areaTitle + PATH_KEY + availablePathIndex, _pathToSave.title);
    }*/
    public static void Save(cPath _pathToSave)
    {
        string pathInstanceKey = _pathToSave.areaTitle + _pathToSave.title;
        PlayerPrefs.SetString(PATH_KEY, pathInstanceKey);
        PlayerPrefs.SetString(pathInstanceKey, _pathToSave.title);
    }

    public static void SavePaths(List<cPath> _pathsToSave)
    {
        foreach (cPath pathToSave in _pathsToSave)
        {
            Save(pathToSave);
        }
    }

    /*public static cPath Load(string _areaTitle, int _pathIndex)
    {
        string pathKey = _areaTitle + PATH_KEY + _pathIndex;
        if (!PlayerPrefs.HasKey(pathKey))
        {
            return null;
        }

        string title = PlayerPrefs.GetString(pathKey);
        //List<cPathPoint> loadedPathPoints = new List<cPathPoint>(); // TODO: Load Path Points?

        cPath loadedPath = new cPath(_areaTitle, title); //, loadedPathPoints
        return loadedPath;
    }*/

    /*public static List<cPath> LoadPaths(string _areaTitle)
    {
        List<cPath> loadedPaths = new List<cPath>();
        cPath loadedPath = null;
        int index = 0;

        do
        {
            loadedPath = Load(_areaTitle, index);
            if (loadedPath != null)
                loadedPaths.Add(loadedPath);
            else
                break;
            index++;
        }
        while (index < 1000000);

        return loadedPaths;
    }*/

    public static string[] LoadAreaPaths(string _areaTitle)
    {
        if (PlayerPrefs.HasKey(_areaTitle))
        {
            return PlayerPrefsX.GetStringArray(_areaTitle);
        }

        return null;
    }

    public static string[] LoadAllPaths()
    {
        if (PlayerPrefs.HasKey(PATH_KEY))
        {
            return PlayerPrefsX.GetStringArray(PATH_KEY);
        }

        return null;
    }
    #endregion
}
