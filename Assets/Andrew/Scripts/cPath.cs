using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cPath
{
    #region Variables
    // TODO: server_area_id
    public int server_path_id;
    public int local_area_id;
    public int local_path_id { get; private set; }
    public string title;
    public DateTime date;
    public List<cPathPoint> pathPoints;

    public static readonly string PATH = "path";
    public static readonly string SERVER_PATH_ID = "server_path_id";
    public static readonly string LOCAL_AREA_ID = "local_area_id";
    public static readonly string ID = "id"; // TODO: Change name
    public static readonly string TITLE = "title";
    public static readonly string DATE = "date";
    public static readonly string PATH_POINTS = "pathPoints";
    #endregion

    #region Methods
    /*public cPath( int _areaId, int _id, List<cPathPoint> _pathPoints) // TODO: REMOVE WHEN TESTING IS FINISHED
    {
        areaId = _areaId;
        Id = _id;
        title = "path_" + Id;
        pathPoints = _pathPoints;
    }*/

    public cPath(int _local_area_id) // For Creating
    {
        server_path_id = -1;
        local_area_id = _local_area_id;
        local_path_id = GetAvailablePathID();
        date = DateTime.Now; //DateTime.Today.Date;
        title = "path_" + local_path_id + "_" + date.ToString("yyyy-MM-dd");
        pathPoints = new List<cPathPoint>();
    }

    private cPath(int _server_path_id, int _local_area_id, int _id, string _title, DateTime _date, List<cPathPoint> _pathPoints) // For Load
    {
        server_path_id = _server_path_id;
        local_area_id = _local_area_id;
        local_path_id = _id;
        title = _title;
        date = _date;
        pathPoints = _pathPoints;
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

    private static int GetAvailablePathID()
    {
        // Load xml document, if null create new
        OnlineMapsXML xml = cArea.GetXML();

        if (xml.isNull)
            return 0;

        // Get all area ids
        HashSet<int> pathIDs = new HashSet<int>();

        OnlineMapsXMLList pathIDNodes = xml.FindAll("/" + cArea.AREAS + "/" + cArea.AREA + "/" + cArea.PATHS + "/" + PATH + "/" + ID); //"/areas/area/paths/path/id"

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
        OnlineMapsXML pathToDelete = xml.Find("/" + cArea.AREAS + "/" + cArea.AREA + "[" + cArea.LOCAL_AREA_ID + "=" + _pathToDelete.local_area_id + "]/" + cArea.PATHS + "/" + PATH + "[" + ID + "=" + _pathToDelete.local_path_id + "]"); // /areas/area[id=0]/paths/path[id=0]
        if (!pathToDelete.isNull)
            pathToDelete.Remove();

        PlayerPrefs.SetString(cArea.PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }

    public static void Save(cPath _pathToSave)
    {
        // Load xml document, if null create new
        OnlineMapsXML xml = cArea.GetXML();

        // Create a new path
        OnlineMapsXML pathsNode = xml.Find("/" + cArea.AREAS + "/" + cArea.AREA + "[" + cArea.LOCAL_AREA_ID + "=" + _pathToSave.local_area_id + "]/" + cArea.PATHS);
        OnlineMapsXML pathNode = pathsNode.Create(PATH);
        pathNode.Create(SERVER_PATH_ID, _pathToSave.server_path_id);
        pathNode.Create(LOCAL_AREA_ID, _pathToSave.local_area_id);
        pathNode.Create(ID, _pathToSave.local_path_id);
        pathNode.Create(TITLE, _pathToSave.title);
        pathNode.Create(DATE, _pathToSave.date.ToString());
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

    private static void EditServerPathId(cPath _pathToEdit)
    {
        // Load xml document, if null create new
        OnlineMapsXML xml = cArea.GetXML();

        // Create a new path
        OnlineMapsXML pathNode = xml.Find("/" + cArea.AREAS + "/" + cArea.AREA + "[" + cArea.LOCAL_AREA_ID + "=" + _pathToEdit.local_area_id + "]/" + cArea.PATHS + "/" + PATH + "[" + ID + "=" + _pathToEdit.local_path_id + "]");
        pathNode.Remove(SERVER_PATH_ID);
        pathNode.Create(SERVER_PATH_ID, _pathToEdit.server_path_id);

        // Save xml string to PlayerPrefs
        PlayerPrefs.SetString(cArea.PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }

    private static cPath Load(OnlineMapsXML _pathNode)
    {
        int server_path_id = _pathNode.Get<int>(SERVER_PATH_ID);
        int local_area_id = _pathNode.Get<int>(LOCAL_AREA_ID);
        int id = _pathNode.Get<int>(ID);
        string title = _pathNode.Get<string>(TITLE);
        string dateString = _pathNode.Get<string>(DATE);
        DateTime date = DateTime.Parse(dateString);
        List<cPathPoint> pathPoints = cPathPoint.LoadPathPointsOfPath(_pathNode[PATH_POINTS]);

        // Create cArea and add it to loadedAreas list
        cPath loadedPath = new cPath(server_path_id, local_area_id, id, title, date, pathPoints);
        return loadedPath;
    }

    public static List<cPath> LoadPathsOfArea(int _local_area_id)
    {
        // If the key does not exist, returns.
        if (!PlayerPrefs.HasKey(cArea.PREFS_KEY))
            return null;

        // Load xml document
        OnlineMapsXML xml = cArea.GetXML();

        OnlineMapsXML pathsNode = xml.Find("/" + cArea.AREAS + "/" + cArea.AREA + "[" + cArea.LOCAL_AREA_ID + "=" + _local_area_id + "]/" + cArea.PATHS);

        List<cPath> areaPaths = new List<cPath>();

        foreach (OnlineMapsXML pathNode in pathsNode)
        {
            areaPaths.Add(Load(pathNode));
        }

        return areaPaths;
    }

    public static List<cPath> GetPathsToUpload()
    {
        // List of paths
        List<cPath> pathsToUpload = new List<cPath>();

        // Load xml document, if null creates new
        OnlineMapsXML xml = cArea.GetXML();

        // Get paths with database id = -1
        OnlineMapsXMLList pathNodes = xml.FindAll("/" + cArea.AREAS + "/" + cArea.AREA + "/" + cArea.PATHS + "/" + PATH + "[" + SERVER_PATH_ID + "= -1" + "]");

        foreach (OnlineMapsXML pathNode in pathNodes)
        {
            if (pathNode.isNull)
            {
                Debug.Log("Path has been deleted!");
                continue;
            }

            cPath loadedPath = Load(pathNode);

            if (loadedPath != null)
                pathsToUpload.Add(loadedPath);
        }

        return pathsToUpload;
    }

    public static void SetServerPathId(int _id, int _server_path_id)
    {
        // Load xml document, if null creates new
        OnlineMapsXML xml = cArea.GetXML();

        // Find path
        OnlineMapsXML pathNode = xml.Find("/" + cArea.AREAS + "/" + cArea.AREA + "/" + cArea.PATHS + "/" + PATH + "[" + ID + "=" + _id + "]");

        // Load path
        cPath loadedPath = Load(pathNode);
        loadedPath.server_path_id = _server_path_id;

        // Edit path
        EditServerPathId(loadedPath);
    }
    #endregion
}