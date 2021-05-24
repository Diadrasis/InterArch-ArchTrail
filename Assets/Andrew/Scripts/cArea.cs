using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class cArea
{
    #region Variables
    public int server_area_id;
    public int local_area_id { get; private set; }
    public string title;
    public Vector2 position; // longitude, latitude (x, y)
    public int zoom;
    public Vector2 areaConstraintsMin; // minLongitude, minLatitude (x, y)
    public Vector2 areaConstraintsMax; // maxLongitude, maxLatitude (x, y)
    public Vector2 viewConstraintsMin; // minLongitude, minLatitude (x, y)
    public Vector2 viewConstraintsMax; // maxLongitude, maxLatitude (x, y)
    //public Vector4 constraints; // minLongitude, minLatitude, maxLongitude, maxLatitude (x, y, z, w)
    public List<cPath> paths = new List<cPath>();

    public static readonly string PREFS_KEY = "areas";
    //public static readonly string UPLOAD_PREFS_KEY = "areasToUpload";
    public static readonly string AREAS_TO_DELETE_PREFS_KEY = "areasToDelete";
    public static readonly string AREAS = "areas";

    public static readonly string AREA = "area";
    public static readonly string SERVER_AREA_ID = "server_area_id";
    public static readonly string LOCAL_AREA_ID = "local_area_id";
    public static readonly string TITLE = "title";
    public static readonly string POSITION = "position";
    public static readonly string ZOOM = "zoom";
    public static readonly string AREA_CONSTRAINTS_MIN = "areaConstraintsMin";
    public static readonly string AREA_CONSTRAINTS_MAX = "areaConstraintsMax";
    public static readonly string VIEW_CONSTRAINTS_MIN = "viewConstraintsMin";
    public static readonly string VIEW_CONSTRAINTS_MAX = "viewConstraintsMax";
    public static readonly string PATHS = "paths";
    #endregion

    #region Methods
    // Constructor for Loading from Player Prefs / Server
    public cArea(int _server_area_id, int _local_area_id, string _title, Vector2 _position, int _zoom, Vector2 _areaConstraintsMin, Vector2 _areaConstraintsMax, Vector2 _viewConstraintsMin, Vector2 _viewConstraintsMax) // TODO: Make private when testing is finished
    {
        server_area_id = _server_area_id;
        local_area_id = _local_area_id;
        title = _title;
        position = _position;
        zoom = _zoom;
        areaConstraintsMin = _areaConstraintsMin;
        areaConstraintsMax = _areaConstraintsMax;
        viewConstraintsMin = _viewConstraintsMin;
        viewConstraintsMax = _viewConstraintsMax;
    }

    /*public cArea(string _title, Vector2 _position, int _zoom, Vector2 _constraintsMin, Vector2 _constraintsMax)
    {
        server_area_id = -1;
        local_area_id = GetAvailableAreaID();
        title = _title;
        position = _position;
        zoom = _zoom;
        //constraints = _constraints;
        areaConstraintsMin = _constraintsMin;
        areaConstraintsMax = _constraintsMax;
    }*/

    // Constructor for creating a new area locally
    public cArea(string _title, Vector2 _centerPosition, Vector2 _areaConstraintsMin, Vector2 _areaConstraintsMax) 
    {
        server_area_id = -1;
        local_area_id = GetAvailableAreaID();
        title = _title;
        position = _centerPosition; //OnlineMaps.instance.position;
        zoom = OnlineMaps.instance.zoom; //MapManager.DEFAULT_ZOOM;
        //constraints = new Vector4(_position.x - MapManager.DEFAULT_POSITION_OFFSET, _position.y - MapManager.DEFAULT_POSITION_OFFSET, _position.x + MapManager.DEFAULT_POSITION_OFFSET, _position.y + MapManager.DEFAULT_POSITION_OFFSET); // for testing
        areaConstraintsMin = _areaConstraintsMin;
        areaConstraintsMax = _areaConstraintsMax;
        viewConstraintsMin = new Vector2((float)OnlineMaps.instance.bounds.left, (float)OnlineMaps.instance.bounds.bottom);
        viewConstraintsMax = new Vector2((float)OnlineMaps.instance.bounds.right, (float)OnlineMaps.instance.bounds.top);
    }

    private static int GetAvailableAreaID()
    {
        // Load xml string from PlayerPrefs
        string xmlData = PlayerPrefs.GetString(PREFS_KEY);

        // Load xml document, if null create new
        OnlineMapsXML xml = OnlineMapsXML.Load(xmlData);

        if (xml.isNull)
            return 0;

        // get all area ids
        HashSet<int> areaIDs = new HashSet<int>();
        
        OnlineMapsXMLList areaIDNodes = xml.FindAll("/"+ AREAS + "/" + AREA + "/" + LOCAL_AREA_ID);

        foreach (OnlineMapsXML node in areaIDNodes)
        {
            int nodeId = node.Get<int>(node.element);
            //Debug.Log("nodeId = " + nodeId);
            areaIDs.Add(nodeId);
        }
        
        int index = 0;

        do
        {
            if (!areaIDs.Contains(index))
                break;
            index++;
        }
        while (true);
        //Debug.Log("index = " + index);
        return index;
    }

    public static void PrintData(string _xpath)
    {
        OnlineMapsXML xml = GetXML();

        OnlineMapsXMLList nodes = xml.FindAll(_xpath);
        Debug.Log("Nodes count = " + nodes.count);
        foreach (OnlineMapsXML node in nodes)
        {
            Debug.Log("Node name = " + node.name);
            Debug.Log("Node Value = " + node.outerXml);
        }
    }

    public static void Delete(int _areaId)
    {
        OnlineMapsXML xml = GetXML();

        OnlineMapsXML areaToDelete = xml.Find("/" + AREAS + "/" + AREA + "[" + LOCAL_AREA_ID + "=" + _areaId + "]");
        if (!areaToDelete.isNull)
            areaToDelete.Remove();

        PlayerPrefs.SetString(PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }
    
    public static void RemoveIdToDelete(int _idToRemove)
    {
        // Load previously saved ids array
        int[] loadedIdsToDelete = PlayerPrefsX.GetIntArray(AREAS_TO_DELETE_PREFS_KEY);

        if (loadedIdsToDelete.Length > 0)
        {
            // Create a new int array based on the loaded ids array
            int[] idsToDelete = new int[loadedIdsToDelete.Length - 1];
            Debug.Log("RemoveIdToDelete, idsToDelete length = " + idsToDelete.Length);
            // Insert all loaded ids to the new ids array except the _idToRemove
            int i = 0;
            foreach (int id in loadedIdsToDelete)
            {
                if (id == _idToRemove)
                {
                    Debug.Log("id == _idToRemove: " + (id == _idToRemove));
                    continue;
                }

                idsToDelete[i] = id;
                Debug.Log("id to delete = " + idsToDelete[i]);
                i++;
            }
            
            // Save new ids array
            PlayerPrefsX.SetIntArray(AREAS_TO_DELETE_PREFS_KEY, idsToDelete);
            PlayerPrefs.Save();
        }
    }

    public static void AddIdToDelete(int _idToDelete)
    {
        // Load previously saved ids array
        int[] loadedIdsToDelete = PlayerPrefsX.GetIntArray(AREAS_TO_DELETE_PREFS_KEY);
        
        // Create a new int array based on the loaded ids array
        int[] idsToDelete = new int[loadedIdsToDelete.Length + 1];
        Debug.Log("AddIdToDelete, idsToDelete length = " + idsToDelete.Length);
        // Insert all loaded ids to the new ids array
        for (int i = 0; i < loadedIdsToDelete.Length; i++)
        {
            idsToDelete[i] = loadedIdsToDelete[i];
            Debug.Log("id to delete = " + idsToDelete[i]);
        }

        // Insert the new id
        idsToDelete[idsToDelete.Length - 1] = _idToDelete;
        Debug.Log("id to delete = " + idsToDelete[idsToDelete.Length - 1]);
        // Save new ids array
        PlayerPrefsX.SetIntArray(AREAS_TO_DELETE_PREFS_KEY, idsToDelete);
        PlayerPrefs.Save();
    }

    public static int[] GetServerIdsToDelete() => PlayerPrefsX.GetIntArray(AREAS_TO_DELETE_PREFS_KEY);

    /*public static List<cArea> GetAreasToUpload()
    {
        // List of areas
        List<cArea> areasToUpload = new List<cArea>();

        // Load previously saved ids array
        int[] idsToUpload = PlayerPrefsX.GetIntArray(UPLOAD_PREFS_KEY);

        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        foreach (int id in idsToUpload)
        {
            // Load Area
            OnlineMapsXML areaNode = xml.Find("/" + AREAS + "/" + AREA + "[" + LOCAL_AREA_ID + "=" + id + "]");
            if (areaNode.isNull)
            {
                Debug.Log("Area with id: " + id + " has been deleted!");
                continue;
            }

            cArea loadedArea = Load(areaNode);

            if (loadedArea != null)
                areasToUpload.Add(loadedArea);
        }

        return areasToUpload;
    }*/

    public static List<cArea> GetAreasToUpload()
    {
        // List of paths
        List<cArea> areasToUpload = new List<cArea>();

        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        // Get paths with server_area_id = -1
        OnlineMapsXMLList areaNodes = xml.FindAll("/" + AREAS + "/" + AREA + "[" + SERVER_AREA_ID + "= -1" + "]");

        foreach (OnlineMapsXML areaNode in areaNodes)
        {
            if (areaNode.isNull)
            {
                Debug.Log("Area has been deleted!");
                continue;
            }

            cArea loadedArea = Load(areaNode);

            if (loadedArea != null)
                areasToUpload.Add(loadedArea);
        }

        //Debug.Log("areasToUpload count = " + areasToUpload.Count);

        return areasToUpload;
    }

    public static OnlineMapsXML GetXML()
    {
        // Load xml string from PlayerPrefs
        string xmlData = PlayerPrefs.GetString(PREFS_KEY);

        // Load xml document, if null create new
        OnlineMapsXML xml = OnlineMapsXML.Load(xmlData);
        if (xml.isNull)
        {
            xml = new OnlineMapsXML(AREAS);
            //Debug.Log("New PATHS XML");
        }

        return xml;
    }

    public static void Save(cArea _areaToSave)
    {
        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        // Check if area is already saved
        OnlineMapsXML areaSaved = xml.Find("/" + AREAS + "/" + AREA + "[" + LOCAL_AREA_ID + "=" + _areaToSave.local_area_id + "]");
        if (!areaSaved.isNull)
        {
            Debug.Log("Area is already saved!");
            return;
        }

        // Create a new area node
        OnlineMapsXML areaNode = xml.Create(AREA);
        areaNode.Create(SERVER_AREA_ID, _areaToSave.server_area_id);
        areaNode.Create(LOCAL_AREA_ID, _areaToSave.local_area_id);
        areaNode.Create(TITLE, _areaToSave.title);
        areaNode.Create(POSITION, _areaToSave.position);
        areaNode.Create(ZOOM, _areaToSave.zoom);
        areaNode.Create(AREA_CONSTRAINTS_MIN, new Vector2(_areaToSave.areaConstraintsMin.x, _areaToSave.areaConstraintsMin.y));
        areaNode.Create(AREA_CONSTRAINTS_MAX, new Vector2(_areaToSave.areaConstraintsMax.x, _areaToSave.areaConstraintsMax.y));
        areaNode.Create(VIEW_CONSTRAINTS_MIN, new Vector2(_areaToSave.viewConstraintsMin.x, _areaToSave.viewConstraintsMin.y));
        areaNode.Create(VIEW_CONSTRAINTS_MAX, new Vector2(_areaToSave.viewConstraintsMax.x, _areaToSave.viewConstraintsMax.y));
        areaNode.Create(PATHS);
        
        // Save xml string to PlayerPrefs
        PlayerPrefs.SetString(PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }

    public static void SaveAreas(List<cArea> _areasToSave)
    {
        foreach (cArea areaToSave in _areasToSave)
        {
            Save(areaToSave);
        }
    }

    public static void Edit(cArea _areaToEdit)
    {
        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        // Get area
        OnlineMapsXML areaNode = xml.Find("/" + AREAS + "/" + AREA + "[" + LOCAL_AREA_ID + "=" + _areaToEdit.local_area_id + "]");
        if (areaNode.isNull)
        {
            Debug.Log("Cannot edit because area is not saved!");
            return;
        }

        // Remove old and Create new values
        areaNode.Remove(TITLE);
        areaNode.Create(TITLE, _areaToEdit.title);
        areaNode.Remove(POSITION);
        areaNode.Create(POSITION, _areaToEdit.position);
        areaNode.Remove(ZOOM);
        areaNode.Create(ZOOM, _areaToEdit.zoom);
        areaNode.Remove(AREA_CONSTRAINTS_MIN);
        areaNode.Create(AREA_CONSTRAINTS_MIN, _areaToEdit.areaConstraintsMin);
        areaNode.Remove(AREA_CONSTRAINTS_MAX);
        areaNode.Create(AREA_CONSTRAINTS_MAX, _areaToEdit.areaConstraintsMax);
        areaNode.Remove(VIEW_CONSTRAINTS_MIN);
        areaNode.Create(VIEW_CONSTRAINTS_MIN, _areaToEdit.viewConstraintsMin);
        areaNode.Remove(VIEW_CONSTRAINTS_MAX);
        areaNode.Create(VIEW_CONSTRAINTS_MAX, _areaToEdit.viewConstraintsMax);

        // Save xml string to PlayerPrefs
        PlayerPrefs.SetString(PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }

    public static void SetServerAreaId(int _local_area_id, int _server_area_id)
    {
        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        // Find path
        OnlineMapsXML areaNode = xml.Find("/" + AREAS + "/" + AREA + "[" + LOCAL_AREA_ID + "=" + _local_area_id + "]");

        // Load path
        cArea loadedArea = Load(areaNode);
        loadedArea.server_area_id = _server_area_id;

        // Edit path
        EditServerAreaId(loadedArea);
    }

    private static void EditServerAreaId(cArea _areaToEdit)
    {
        // Load xml document, if null create new
        OnlineMapsXML xml = GetXML();

        // Create a new path
        OnlineMapsXML areaNode = xml.Find("/" + AREAS + "/" + AREA + "[" + LOCAL_AREA_ID + "=" + _areaToEdit.local_area_id + "]");
        areaNode.Remove(SERVER_AREA_ID);
        areaNode.Create(SERVER_AREA_ID, _areaToEdit.server_area_id);
        Debug.Log("Edited xml = " + xml.outerXml);
        // Save xml string to PlayerPrefs
        PlayerPrefs.SetString(PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }

    /*public static string GetInfoFromXML(string _xpath)
    {
        OnlineMapsXML xml = GetXML();
        XmlNode node = xml.document.SelectSingleNode(_xpath);
        return node.InnerText;
    }*/

    private static cArea Load(OnlineMapsXML _areaNode)
    {
        int server_area_id = _areaNode.Get<int>(SERVER_AREA_ID);
        int local_area_id = _areaNode.Get<int>(LOCAL_AREA_ID);
        string title = _areaNode.Get<string>(TITLE);
        Vector2 position = _areaNode.Get<Vector2>(POSITION);
        int zoom = _areaNode.Get<int>(ZOOM);
        Vector2 areaConstraints_min = _areaNode.Get<Vector2>(AREA_CONSTRAINTS_MIN);
        Vector2 areaConstraints_max = _areaNode.Get<Vector2>(AREA_CONSTRAINTS_MAX);
        Vector2 viewConstraints_min = _areaNode.Get<Vector2>(VIEW_CONSTRAINTS_MIN);
        Vector2 viewConstraints_max = _areaNode.Get<Vector2>(VIEW_CONSTRAINTS_MAX);

        // Create cArea and add it to loadedAreas list
        cArea loadedArea = new cArea(server_area_id, local_area_id, title, position, zoom, areaConstraints_min, areaConstraints_max, viewConstraints_min, viewConstraints_max);
        return loadedArea;
    }

    public static List<cArea> LoadAreas()
    {
        // If the key does not exist, returns.
        if (!PlayerPrefs.HasKey(PREFS_KEY))
            return null;

        // Init list of cArea to add paths to
        List<cArea> loadedAreas = new List<cArea>();

        // Load xml string from PlayerPrefs
        string xmlData = PlayerPrefs.GetString(PREFS_KEY);

        // Load xml document
        OnlineMapsXML xml = OnlineMapsXML.Load(xmlData);

        // Load areas
        foreach (OnlineMapsXML node in xml)
        {
            loadedAreas.Add(Load(node));
        }
        //Debug.Log(xml.outerXml);
        return loadedAreas;
    }

    /*public static void Upload(cArea _areaToUpload)
    {
        AppManager.Instance.serverManager.UploadArea(_areaToUpload);
    }*/

    /*public static void DownloadAreas()
    {
        AppManager.Instance.serverManager.DownloadAreas();
    }*/

    /*public static void DeleteAreaFromServer(int _server_area_idToDelete)
    {
        AppManager.Instance.serverManager.DeleteAreaFromServer(_server_area_idToDelete);
    }*/
    #endregion
}

[Serializable]
public class cAreaData
{
    public int server_area_id;
    public int local_area_id;
    public string title;
    public string position; // longitude, latitude (x, y)
    public int zoom;
    public string areaConstraintsMin; // minLongitude, minLatitude (x, y)
    public string areaConstraintsMax; // maxLongitude, maxLatitude (x, y)
    public string viewConstraintsMin; // minLongitude, minLatitude (x, y)
    public string viewConstraintsMax; // maxLongitude, maxLatitude (x, y)
}