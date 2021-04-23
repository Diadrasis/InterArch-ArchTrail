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

    private static readonly string PREFS_PATHS_KEY = "paths";
    private static readonly string PATHS = "Paths";

    //private static readonly string PATH = "Path";
    private static readonly string AREA_TITLE = "AreaTitle";
    private static readonly string TITLE = "Title";
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

    //add a path point which will happen associated with cRoutePoints.cs
    public void AddPathPoint()
    {

    }

    //Show/load the path in our scene
    public void Show()
    {

    }

    public static void Delete(cPath _pathToDelete)
    {
        OnlineMapsXML xml = GetXML();
        xml.Remove(_pathToDelete.areaTitle + _pathToDelete.title);

        PlayerPrefs.SetString(PREFS_PATHS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }

    private static OnlineMapsXML GetXML()
    {
        // Load xml string from PlayerPrefs
        string xmlData = PlayerPrefs.GetString(PREFS_PATHS_KEY);

        // Load xml document, if null create new
        OnlineMapsXML xml = OnlineMapsXML.Load(xmlData);
        if (xml.isNull)
        {
            xml = new OnlineMapsXML(PATHS);
            //Debug.Log("New PATHS XML");
        }

        return xml;
    }

    public static void Save(cPath _pathToSave)
    {
        // Load xml document, if null create new
        OnlineMapsXML xml = GetXML();

        // Create a new path
        OnlineMapsXML pathNode = xml.Create(_pathToSave.areaTitle + _pathToSave.title); // PATH
        pathNode.Create(AREA_TITLE, _pathToSave.areaTitle);
        pathNode.Create(TITLE, _pathToSave.title);

        // Save xml string to PlayerPrefs
        PlayerPrefs.SetString(PREFS_PATHS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }

    public static void SavePaths(List<cPath> _pathsToSave)
    {
        foreach (cPath pathToSave in _pathsToSave)
        {
            Save(pathToSave);
        }
    }

    public static List<cPath> LoadPathsOfArea(string _areaTitle)
    {
        List<cPath> loadedPaths = LoadPaths(); // TODO: Optimize
        List<cPath> areaPaths = new List<cPath>();

        foreach (cPath path in loadedPaths)
        {
            if (path.areaTitle.Equals(_areaTitle))
                areaPaths.Add(path);
        }

        return areaPaths;
    }

    public static List<cPath> LoadPaths()
    {
        // If the key does not exist, returns.
        if (!PlayerPrefs.HasKey(PREFS_PATHS_KEY))
            return null;

        // Init list of cPath to add paths to
        List<cPath> loadedPaths = new List<cPath>();

        // Load xml string from PlayerPrefs
        string xmlData = PlayerPrefs.GetString(PREFS_PATHS_KEY);

        // Load xml document
        OnlineMapsXML xml = OnlineMapsXML.Load(xmlData);

        // Load paths
        foreach (OnlineMapsXML node in xml)
        {
            string areaTitle = node.Get<string>(AREA_TITLE);
            string title = node.Get<string>(TITLE);

            // Create cPath and add it to loadedPaths list
            cPath loadedPath = new cPath(areaTitle, title);
            loadedPaths.Add(loadedPath);
        }

        return loadedPaths;
    }
    #endregion
}
/*
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
/*
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
}*/

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
/*
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
}*/
