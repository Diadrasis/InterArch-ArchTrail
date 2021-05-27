using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class cPath
{
    #region Variables
    public int server_area_id;
    public int local_area_id;
    public int server_path_id;
    public int local_path_id { get; private set; }
    public string title;
    public DateTime date;
    public List<cPathPoint> pathPoints;

    public static readonly string PATHS_TO_DELETE_PREFS_KEY = "pathsToDelete";

    public static readonly string PATH = "path";
    public static readonly string SERVER_PATH_ID = "server_path_id";
    public static readonly string LOCAL_AREA_ID = "local_area_id";
    public static readonly string LOCAL_PATH_ID = "local_path_id";
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

    // Constructor for creating locally
    public cPath(int _local_area_id)
    {
        Debug.Log(DateTime.Now.ToString("d"));
        server_area_id = -1;
        server_path_id = -1;
        local_area_id = _local_area_id;
        local_path_id = GetAvailablePathID();
        date = DateTime.ParseExact(DateTime.Now.ToString("d"), "d/M/yyyy", CultureInfo.InvariantCulture);
        title = "path_" + local_path_id + "_" + date.ToString("dd/MM/yyyy");
        pathPoints = new List<cPathPoint>();
    }

    // Constructor for Loading from Player Prefs
    private cPath(int _server_area_id, int _server_path_id, int _local_area_id, int _local_path_id, string _title, DateTime _date, List<cPathPoint> _pathPoints)
    {
        server_area_id = _server_area_id;
        local_area_id = _local_area_id;
        server_path_id = _server_path_id;
        local_path_id = _local_path_id;
        title = _title;
        date = _date;
        pathPoints = _pathPoints;
    }

    // Constructor for Downloading from server
    public cPath(int _server_area_id, int _server_path_id, string _title, DateTime _date) // List<cPathPoint> _pathPoints
    {
        server_area_id = _server_area_id;
        local_area_id = -1;
        server_path_id = _server_path_id;
        local_path_id = -1;
        title = _title;
        date = _date;
        pathPoints = null; // _pathPoints;
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

        OnlineMapsXMLList pathIDNodes = xml.FindAll("/" + cArea.AREAS + "/" + cArea.AREA + "/" + cArea.PATHS + "/" + PATH + "/" + LOCAL_PATH_ID); //"/areas/area/paths/path/id"

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
        OnlineMapsXML pathToDelete = xml.Find("/" + cArea.AREAS + "/" + cArea.AREA + "[" + cArea.LOCAL_AREA_ID + "=" + _pathToDelete.local_area_id + "]/" + cArea.PATHS + "/" + PATH + "[" + LOCAL_PATH_ID + "=" + _pathToDelete.local_path_id + "]"); // /areas/area[id=0]/paths/path[id=0]
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
        pathNode.Create(cArea.SERVER_AREA_ID, _pathToSave.server_area_id);
        pathNode.Create(LOCAL_AREA_ID, _pathToSave.local_area_id);
        pathNode.Create(SERVER_PATH_ID, _pathToSave.server_path_id);
        pathNode.Create(LOCAL_PATH_ID, _pathToSave.local_path_id);
        pathNode.Create(TITLE, _pathToSave.title);
        pathNode.Create(DATE, _pathToSave.date.ToString());
        cPathPoint.SavePathPoints(pathNode.Create(PATH_POINTS), _pathToSave.pathPoints);

        // Save xml string to PlayerPrefs
        PlayerPrefs.SetString(cArea.PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }

    public static void SaveFromServer(cPath _pathToSave)
    {
        // Load xml document, if null create new
        OnlineMapsXML xml = cArea.GetXML();

        // Check if path is already downloaded
        OnlineMapsXML pathSaved = xml.Find("/" + cArea.AREAS + "/" + cArea.AREA + "[" + cArea.SERVER_AREA_ID + "=" + _pathToSave.server_area_id + "]/" + cArea.PATHS + "/" + PATH + "[" + SERVER_PATH_ID + "=" + _pathToSave.server_path_id + "]");
        if (!pathSaved.isNull)
        {
            Debug.Log("Path is already downloaded!");
            // Get local area id
            int localArea_id = pathSaved.Get<int>(LOCAL_AREA_ID);

            // Get local path id
            int localPath_id = pathSaved.Get<int>(LOCAL_PATH_ID);

            // Set local values
            _pathToSave.local_area_id = localArea_id;
            _pathToSave.local_path_id = localPath_id;

            // Edit point
            Edit(_pathToSave);
            return;
        }

        // Get local area id from path's server_area_id
        OnlineMapsXML areaNode = xml.Find("/" + cArea.AREAS + "/" + cArea.AREA + "[" + cArea.SERVER_AREA_ID + "=" + _pathToSave.server_area_id + "]");
        int local_area_id = areaNode.Get<int>(LOCAL_AREA_ID);

        if (local_area_id >= 0)
        {
            // Create a new path
            OnlineMapsXML pathsNode = xml.Find("/" + cArea.AREAS + "/" + cArea.AREA + "[" + cArea.SERVER_AREA_ID + "=" + _pathToSave.server_area_id + "]/" + cArea.PATHS);
            OnlineMapsXML pathNode = pathsNode.Create(PATH);
            pathNode.Create(cArea.SERVER_AREA_ID, _pathToSave.server_area_id);
            pathNode.Create(LOCAL_AREA_ID, local_area_id);
            pathNode.Create(SERVER_PATH_ID, _pathToSave.server_path_id);
            pathNode.Create(LOCAL_PATH_ID, GetAvailablePathID());
            pathNode.Create(TITLE, _pathToSave.title);
            pathNode.Create(DATE, _pathToSave.date.ToString());
            pathNode.Create(PATH_POINTS);
            //cPathPoint.SavePathPoints(pathNode.Create(PATH_POINTS), _pathToSave.pathPoints);

            // Save xml string to PlayerPrefs
            PlayerPrefs.SetString(cArea.PREFS_KEY, xml.outerXml);
            PlayerPrefs.Save();
        }
    }

    public static void SavePaths(List<cPath> _pathsToSave)
    {
        foreach (cPath pathToSave in _pathsToSave)
        {
            Save(pathToSave);
        }
    }

    public static void Edit(cPath _pathToEdit)
    {
        // Load xml document, if null creates new
        OnlineMapsXML xml = cArea.GetXML();

        // Get path
        OnlineMapsXML pathNode = xml.Find("/" + cArea.AREAS + "/" + cArea.AREA + "[" + cArea.LOCAL_AREA_ID + "=" + _pathToEdit.local_area_id + "]/" + cArea.PATHS + "/" + PATH + "[" + LOCAL_PATH_ID + "=" + _pathToEdit.local_path_id + "]");
        if (pathNode.isNull)
        {
            Debug.Log("Cannot edit because path is not saved!");
            return;
        }

        // Remove old and Create new values
        pathNode.Remove(TITLE);
        pathNode.Create(TITLE, _pathToEdit.title);
        pathNode.Remove(DATE);
        pathNode.Create(DATE, _pathToEdit.date.ToString());

        // Save xml string to PlayerPrefs
        PlayerPrefs.SetString(cArea.PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }

    private static void EditServerPathId(cPath _pathToEdit)
    {
        // Load xml document, if null create new
        OnlineMapsXML xml = cArea.GetXML();

        // Get path from xml and Edit values
        OnlineMapsXML pathNode = xml.Find("/" + cArea.AREAS + "/" + cArea.AREA + "[" + cArea.LOCAL_AREA_ID + "=" + _pathToEdit.local_area_id + "]/" + cArea.PATHS + "/" + PATH + "[" + LOCAL_PATH_ID + "=" + _pathToEdit.local_path_id + "]");
        pathNode.Remove(cArea.SERVER_AREA_ID);
        pathNode.Create(cArea.SERVER_AREA_ID, _pathToEdit.server_area_id);
        pathNode.Remove(SERVER_PATH_ID);
        pathNode.Create(SERVER_PATH_ID, _pathToEdit.server_path_id);

        Debug.Log("Edited xml = " + xml.outerXml);
        // Save xml string to PlayerPrefs
        PlayerPrefs.SetString(cArea.PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }

    private static cPath Load(OnlineMapsXML _pathNode)
    {
        int server_area_id = _pathNode.Get<int>(cArea.SERVER_AREA_ID);
        int local_area_id = _pathNode.Get<int>(LOCAL_AREA_ID);
        int server_path_id = _pathNode.Get<int>(SERVER_PATH_ID);
        int local_path_id = _pathNode.Get<int>(LOCAL_PATH_ID);
        string title = _pathNode.Get<string>(TITLE);
        string dateString = _pathNode.Get<string>(DATE);
        DateTime date = DateTime.Parse(dateString);
        List<cPathPoint> pathPoints = cPathPoint.LoadPathPointsOfPath(_pathNode[PATH_POINTS]);

        // Create cArea and add it to loadedAreas list
        cPath loadedPath = new cPath(server_area_id, server_path_id, local_area_id, local_path_id, title, date, pathPoints);
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

    public static void RemoveIdToDelete(int _idToRemove)
    {
        // Load previously saved ids array
        int[] loadedIdsToDelete = PlayerPrefsX.GetIntArray(PATHS_TO_DELETE_PREFS_KEY);

        // Create a new int array based on the loaded ids array
        int[] idsToDelete = new int[loadedIdsToDelete.Length - 1];

        // Insert all loaded ids to the new ids array except the _idToRemove
        int i = 0;
        foreach (int id in loadedIdsToDelete)
        {
            if (id == _idToRemove)
                continue;

            idsToDelete[i] = id;
            i++;
        }
        //Debug.Log("idsToDelete length = " + idsToDelete.Length);
        // Save new ids array
        PlayerPrefsX.SetIntArray(PATHS_TO_DELETE_PREFS_KEY, idsToDelete);
        PlayerPrefs.Save();
    }

    public static void AddIdToDelete(int _idToDelete)
    {
        // Load previously saved ids array
        int[] loadedIdsToDelete = PlayerPrefsX.GetIntArray(PATHS_TO_DELETE_PREFS_KEY);

        // Create a new int array based on the loaded ids array
        int[] idsToDelete = new int[loadedIdsToDelete.Length + 1];

        // Insert all loaded ids to the new ids array
        for (int i = 0; i < loadedIdsToDelete.Length; i++)
        {
            idsToDelete[i] = loadedIdsToDelete[i];
        }

        // Insert the new id
        idsToDelete[idsToDelete.Length - 1] = _idToDelete;
        //Debug.Log("idsToDelete length = " + idsToDelete.Length);
        // Save new ids array
        PlayerPrefsX.SetIntArray(PATHS_TO_DELETE_PREFS_KEY, idsToDelete);
        PlayerPrefs.Save();
    }

    public static int[] GetServerIdsToDelete() => PlayerPrefsX.GetIntArray(PATHS_TO_DELETE_PREFS_KEY);

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
            {
                // Get server_area_id
                OnlineMapsXML areaNode = xml.Find("/" + cArea.AREAS + "/" + cArea.AREA + "[" + LOCAL_AREA_ID + "=" + loadedPath.local_area_id + "]/" + cArea.SERVER_AREA_ID);
                string server_area_idString = areaNode.Value();
                if (int.TryParse(server_area_idString, out int server_area_id))
                //Debug.Log("area's server_area_id = " + server_area_id);
                // If server_area_id is -1 then the area is not uploaded yet so don't upload its paths yet.
                if (server_area_id != -1)
                {
                    // Set path's server_area_id
                    if (loadedPath.server_area_id == -1)
                        loadedPath.server_area_id = server_area_id;

                    pathsToUpload.Add(loadedPath);
                }
            }
        }

        return pathsToUpload;
    }

    public static void SetServerAreaAndPathId(int _server_area_id, int _server_path_id, int _local_path_id)
    {
        // Load xml document, if null creates new
        OnlineMapsXML xml = cArea.GetXML();

        // Find path
        OnlineMapsXML pathNode = xml.Find("/" + cArea.AREAS + "/" + cArea.AREA + "/" + cArea.PATHS + "/" + PATH + "[" + LOCAL_PATH_ID + "=" + _local_path_id + "]");

        // Load path
        cPath loadedPath = Load(pathNode);
        if (loadedPath.server_area_id == -1)
            loadedPath.server_area_id = _server_area_id;
        loadedPath.server_path_id = _server_path_id;

        // Edit path
        EditServerPathId(loadedPath);
    }
    #endregion
}

[Serializable]
public class cPathData
{
    public int server_path_id;
    public int server_area_id;
    public string title;
    public string date;
}