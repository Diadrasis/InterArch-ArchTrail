using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cPath
{
    #region Variables
    public int areaId;
    public int Id { get; private set; }
    public string title;
    public List<cPathPoint> pathPoints;

    public static readonly string PATH = "path";
    public static readonly string AREA_ID = "areaId";
    public static readonly string ID = "id";
    public static readonly string TITLE = "title";
    public static readonly string PATH_POINTS = "pathPoints";
    #endregion

    #region Methods
    public cPath( int _areaId, int _id, List<cPathPoint> _pathPoints) // TODO: REMOVE WHEN TESTING IS FINISHED
    {
        areaId = _areaId;
        Id = _id;
        title = "path_" + Id;
        pathPoints = _pathPoints;
    }

    public cPath(int _areaId) // For Creating
    {
        areaId = _areaId;
        Id = GetAvailablePathID();
        title = "path_" + Id; // It should get it's name from the current area's path count
        pathPoints = new List<cPathPoint>();
    }

    private cPath(int _areaId, int _id, string _title, List<cPathPoint> _pathPoints) // For Load
    {
        areaId = _areaId;
        Id = _id;
        title = _title;
        pathPoints = _pathPoints;
    }

    //create a path
    public void Create()
    {

    }

    //add a path point which will happen associated with cRoutePoints.cs
    /*public void AddPathPoint(cPathPoint _pathPointToAdd)
    {
        pathPoints.Add(_pathPointToAdd);
    }

    public void RemovePathPoint(cPathPoint _pathPointToRemove)
    {
        pathPoints.Remove(_pathPointToRemove);
    }*/

    //Show/load the path in our scene
    public void Show()
    {

    }

    private static int GetAvailablePathID()
    {
        // Load xml document, if null create new
        OnlineMapsXML xml = cArea.GetXML();

        if (xml.isNull)
            return 0;

        // get all area ids
        HashSet<int> pathIDs = new HashSet<int>();

        OnlineMapsXMLList pathIDNodes = xml.FindAll("/areas/area/paths/path/id");

        foreach (OnlineMapsXML node in pathIDNodes)
        {
            int nodeId = node.Get<int>(node.element);
            //Debug.Log("nodeId = " + nodeId);
            pathIDs.Add(nodeId);
        }

        int index = 0;

        do
        {
            if (!pathIDs.Contains(index))
                break;
            index++;
        }
        while (true);
        //Debug.Log("index = " + index);
        return index;
    }

    public static void Delete(cPath _pathToDelete)
    {
        OnlineMapsXML xml = cArea.GetXML();
        //xml.Remove(_pathToDelete.areaTitle + _pathToDelete.title);
        OnlineMapsXML pathToDelete = xml.Find("/" + cArea.AREAS + "/" + cArea.AREA + "[" + cArea.ID + "=" + _pathToDelete.areaId + "]/" + cArea.PATHS + "/" + PATH + "[" + ID + "=" + _pathToDelete.Id + "]"); // /areas/area[id=0]/paths/path[id=0]
        if (!pathToDelete.isNull)
            pathToDelete.Remove();

        PlayerPrefs.SetString(cArea.PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }

    /*private static OnlineMapsXML GetXML()
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
    }*/

    public static void Save(cPath _pathToSave)
    {
        // Load xml document, if null create new
        OnlineMapsXML xml = cArea.GetXML();

        // Create a new path
        OnlineMapsXML pathsNode = xml.Find("/" + cArea.AREAS + "/" + cArea.AREA + "[" + cArea.ID + "=" + _pathToSave.areaId + "]/" + cArea.PATHS);
        OnlineMapsXML pathNode = pathsNode.Create(PATH);
        pathNode.Create(AREA_ID, _pathToSave.areaId);
        pathNode.Create(ID, _pathToSave.Id);
        pathNode.Create(TITLE, _pathToSave.title);
        cPathPoint.SavePathPoints(pathNode.Create(PATH_POINTS), _pathToSave.pathPoints);

        // Save xml string to PlayerPrefs
        PlayerPrefs.SetString(cArea.PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }

    public static void SavePaths(List<cPath> _pathsToSave)
    {
        foreach (cPath pathToSave in _pathsToSave)
        {
            Save(pathToSave);
        }
    }

    private static cPath Load(OnlineMapsXML _pathNode)
    {
        int areaId = _pathNode.Get<int>(AREA_ID);
        int id = _pathNode.Get<int>(ID);
        string title = _pathNode.Get<string>(TITLE);
        List<cPathPoint> pathPoints = cPathPoint.LoadPathPointsOfPath(_pathNode[PATH_POINTS]);

        // Create cArea and add it to loadedAreas list
        cPath loadedPath = new cPath(areaId, id, title, pathPoints);
        return loadedPath;
    }

    public static List<cPath> LoadPathsOfArea(int _areaId)
    {
        // If the key does not exist, returns.
        if (!PlayerPrefs.HasKey(cArea.PREFS_KEY))
            return null;

        // Load xml document
        OnlineMapsXML xml = cArea.GetXML();

        OnlineMapsXML pathsNode = xml.Find("/" + cArea.AREAS + "/" + cArea.AREA + "[" + cArea.ID + "=" + _areaId + "]/" + cArea.PATHS);

        List<cPath> areaPaths = new List<cPath>();

        foreach (OnlineMapsXML pathNode in pathsNode)
        {
            areaPaths.Add(Load(pathNode));
        }

        return areaPaths;
    }

    /*public static List<cPath> LoadPaths()
    {
        // If the key does not exist, returns.
        if (!PlayerPrefs.HasKey(cArea.PREFS_KEY))
            return null;

        // Load xml document
        OnlineMapsXML xml = cArea.GetXML();

        // Init list of cPath to add paths to
        List<cPath> loadedPaths = new List<cPath>();

        // Load paths
        foreach (OnlineMapsXML node in xml)
        {
            string areaTitle = node.Get<string>(AREA_ID);
            string title = node.Get<string>(TITLE);

            // Create cPath and add it to loadedPaths list
            cPath loadedPath = new cPath(areaTitle, title);
            loadedPaths.Add(loadedPath);
        }

        return loadedPaths;
    }*/
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
