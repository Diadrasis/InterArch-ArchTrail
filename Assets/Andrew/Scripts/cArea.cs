using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class cArea
{
    #region Variables
    public int databaseId;
    public int Id { get; private set; }
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
    public static readonly string UPLOAD_PREFS_KEY = "areasToUpload";
    public static readonly string AREAS = "areas";

    public static readonly string AREA = "area";
    public static readonly string DATABASE_ID = "databaseId";
    public static readonly string ID = "id";
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
    public cArea(int _databaseId, int _id, string _title, Vector2 _position, int _zoom, Vector2 _areaConstraintsMin, Vector2 _areaConstraintsMax, Vector2 _viewConstraintsMin, Vector2 _viewConstraintsMax) // TODO: Make private when testing is finished
    {
        databaseId = _databaseId;
        Id = _id;
        title = _title;
        position = _position;
        zoom = _zoom;
        areaConstraintsMin = _areaConstraintsMin;
        areaConstraintsMax = _areaConstraintsMax;
        viewConstraintsMin = _viewConstraintsMin;
        viewConstraintsMax = _viewConstraintsMax;
    }

    public cArea(string _title, Vector2 _position, int _zoom, Vector2 _constraintsMin, Vector2 _constraintsMax)
    {
        databaseId = -1;
        Id = GetAvailableAreaID();
        title = _title;
        position = _position;
        zoom = _zoom;
        //constraints = _constraints;
        areaConstraintsMin = _constraintsMin;
        areaConstraintsMax = _constraintsMax;
    }

    // Constructor for creating a new area locally
    public cArea(string _title, Vector2 _centerPosition, Vector2 _areaConstraintsMin, Vector2 _areaConstraintsMax) 
    {
        databaseId = -1;
        Id = GetAvailableAreaID();
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
        
        OnlineMapsXMLList areaIDNodes = xml.FindAll("/"+ AREAS + "/" + AREA + "/" + ID);

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

        OnlineMapsXML areaToDelete = xml.Find("/" + AREAS + "/" + AREA + "[" + ID + "=" + _areaId + "]");
        if (!areaToDelete.isNull)
            areaToDelete.Remove();

        PlayerPrefs.SetString(PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }

    public static void RemoveIdToUpload(int _idToRemove)
    {
        // Load previously saved ids array
        int[] loadedIds = PlayerPrefsX.GetIntArray(UPLOAD_PREFS_KEY);
        Debug.Log("loadedIds length = " + loadedIds.Length);
        // Create a new int array based on the loaded ids array
        int[] idsToUpload = new int[loadedIds.Length - 1];

        // Insert all loaded ids to the new ids array except the _idToRemove
        int i = 0;
        foreach (int id in loadedIds)
        {
            if (id == _idToRemove)
                continue;

            idsToUpload[i] = id;
            i++;
        }

        // Save new ids array
        PlayerPrefsX.SetIntArray(UPLOAD_PREFS_KEY, idsToUpload);
    }

    public static void AddIdToUpload(int _idToUpload)
    {
        // Load previously saved ids array
        int[] loadedIds = PlayerPrefsX.GetIntArray(UPLOAD_PREFS_KEY);
        Debug.Log("loadedIds length = " + loadedIds.Length);
        // Create a new int array based on the loaded ids array
        int[] idsToUpload = new int[loadedIds.Length + 1];

        // Insert all loaded ids to the new ids array
        for (int i = 0; i < loadedIds.Length; i++)
        {
            idsToUpload[i] = loadedIds[i];
        }

        // Insert the new id
        idsToUpload[idsToUpload.Length - 1] = _idToUpload;

        // Save new ids array
        PlayerPrefsX.SetIntArray(UPLOAD_PREFS_KEY, idsToUpload);
    }

    public static List<cArea> GetAreasToUpload()
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
            OnlineMapsXML areaNode = xml.Find("/" + AREAS + "/" + AREA + "[" + ID + "=" + id + "]");
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
        OnlineMapsXML areaSaved = xml.Find("/" + AREAS + "/" + AREA + "[" + ID + "=" + _areaToSave.Id + "]");
        if (!areaSaved.isNull)
        {
            Debug.Log("Area is already saved!");
            return;
        }

        // Create a new area node
        OnlineMapsXML areaNode = xml.Create(AREA);
        areaNode.Create(DATABASE_ID, _areaToSave.databaseId);
        areaNode.Create(ID, _areaToSave.Id);
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

    /*public static string GetInfoFromXML(string _xpath)
    {
        OnlineMapsXML xml = GetXML();
        XmlNode node = xml.document.SelectSingleNode(_xpath);
        return node.InnerText;
    }*/

    private static cArea Load(OnlineMapsXML _areaNode)
    {
        int databaseId = _areaNode.Get<int>(DATABASE_ID);
        int id = _areaNode.Get<int>(ID);
        string title = _areaNode.Get<string>(TITLE);
        Vector2 position = _areaNode.Get<Vector2>(POSITION);
        int zoom = _areaNode.Get<int>(ZOOM);
        Vector2 areaConstraints_min = _areaNode.Get<Vector2>(AREA_CONSTRAINTS_MIN);
        Vector2 areaConstraints_max = _areaNode.Get<Vector2>(AREA_CONSTRAINTS_MAX);
        Vector2 viewConstraints_min = _areaNode.Get<Vector2>(VIEW_CONSTRAINTS_MIN);
        Vector2 viewConstraints_max = _areaNode.Get<Vector2>(VIEW_CONSTRAINTS_MAX);

        // Create cArea and add it to loadedAreas list
        cArea loadedArea = new cArea(databaseId, id, title, position, zoom, areaConstraints_min, areaConstraints_max, viewConstraints_min, viewConstraints_max);
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

    public static void Upload(cArea _areaToUpload)
    {
        AppManager.Instance.serverManager.UploadArea(_areaToUpload);
    }

    public static void DownloadAreas()
    {
        AppManager.Instance.serverManager.DownloadAreas();
    }

    public static void DeleteAreaFromServer(int _areaIdToDelete)
    {
        AppManager.Instance.serverManager.DeleteAreaFromServer(_areaIdToDelete);
    }
    #endregion
}

[Serializable]
public class cAreaData
{
    public int databaseId;
    public int id;
    public string title;
    public string position; // longitude, latitude (x, y)
    public int zoom;
    public string areaConstraintsMin; // minLongitude, minLatitude (x, y)
    public string areaConstraintsMax; // maxLongitude, maxLatitude (x, y)
    public string viewConstraintsMin; // minLongitude, minLatitude (x, y)
    public string viewConstraintsMax; // maxLongitude, maxLatitude (x, y)
}




/*
 * private static readonly string AREA_KEY = "area_";
    //private static readonly string TITLE_KEY = "_title";
    private static readonly string POSITION_KEY = "_position";
    private static readonly string ZOOM_KEY = "_zoom";
    private static readonly string CONSTRAINTS_KEY = "_constraints";
 * 
 * 
 * 
    private static int GetAvailableAreaIndex()
    {
        int index = 0;

        do
        {
            string areaTitle = PlayerPrefs.GetString(AREA_KEY + index.ToString());
            if (String.IsNullOrEmpty(areaTitle))
                break;
            index++;
        }
        while (index < 1000000);

        return (index < 1000000) ? index : -1;
    }

    public static void Save(cArea _areaToSave)
    {
        int availableAreaIndex = GetAvailableAreaIndex();
        Debug.Log("available area index = " + availableAreaIndex); // TODO: Remove!!!
        if (availableAreaIndex == -1)
        {
            Debug.Log("GetAvailableAreaIndex method has run out of available indexes");
            return;
        }

        PlayerPrefs.SetString(AREA_KEY + availableAreaIndex, _areaToSave.title); // area_0, Mεσσήνη
        PlayerPrefs.SetInt(_areaToSave.title + ZOOM_KEY, _areaToSave.zoom); // Mεσσήνη_zoom, 19
        PlayerPrefsX.SetVector2(_areaToSave.title + POSITION_KEY, _areaToSave.position); // Mεσσήνη_position, Vector2
        PlayerPrefsX.SetVector4(_areaToSave.title + CONSTRAINTS_KEY, _areaToSave.constraints);
    }

    public static void SaveAreas(List<cArea> _areasToSave)
    {
        foreach (cArea areaToSave in _areasToSave)
        {
            Save(areaToSave);
        }
    }

    private static cArea Load(int _areaId)
    {
        string areaKey = AREA_KEY + _areaId.ToString();

        if (!PlayerPrefs.HasKey(areaKey))
        {
            return null;
        }

        string title = PlayerPrefs.GetString(areaKey);
        Vector2 position = PlayerPrefsX.GetVector2(title + POSITION_KEY, Vector2.zero);
        int zoom = PlayerPrefs.GetInt(title + ZOOM_KEY);
        Vector4 constraints = PlayerPrefsX.GetVector4(title + CONSTRAINTS_KEY);

        cArea loadedArea = new cArea(title, position, zoom, constraints);
        loadedArea.id = _areaId;

        return loadedArea;
    }

    public static List<cArea> LoadAreas()
    {
        List<cArea> loadedAreas = new List<cArea>();
        cArea loadedArea = null;
        int index = 0;

        do
        {
            loadedArea = Load(index);
            if (loadedArea != null)
                loadedAreas.Add(loadedArea);
            index++;
        }
        while (index < 100); // TODO: This value means that the user can only save 100 areas, we must make a new system to save and load.

        /*do
        {
            loadedArea = Load(index);
            if (loadedArea != null)
                loadedAreas.Add(loadedArea);
            else
                break;
            index++;
        }
        while (index < 1000000); */
/*
return loadedAreas;
}

private static cArea GetAreaByTitle(string _areaTitle)
{
cArea loadedArea = null;
int index = 0;

do
{
loadedArea = Load(index);
if (loadedArea != null && loadedArea.title.Equals(_areaTitle))
{
    loadedArea.id = index;
    break;
}

index++;
}
while (index < 100); // TODO: This value means that the user can only save 100 areas, we must make a new system to save and load.

return loadedArea;
}

private static void Delete(cArea _areaToDelete)
{
PlayerPrefs.DeleteKey(AREA_KEY + _areaToDelete.id); // Deletes title
PlayerPrefs.DeleteKey(_areaToDelete.title + POSITION_KEY); // Deletes position
PlayerPrefs.DeleteKey(_areaToDelete.title + ZOOM_KEY); // Deletes zoom
PlayerPrefs.DeleteKey(_areaToDelete.title + CONSTRAINTS_KEY); // Deletes constraints
                                                          //Debug.Log("HasKey = " + PlayerPrefs.HasKey(_areaToDelete.title + POSITION_KEY)); // check if it is deleted.
}

// Deletes all keys related to this area. Paths, Points etc.
public static void Delete(string _areaTitle)
{
//Debug.Log(GetAreaByTitle(_areaTitle).title);
Delete(GetAreaByTitle(_areaTitle));
//cPath.Delete(_areaTitle);
}
*/
