using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class cArea
{
    #region Variables
    public int Id { get; private set; }
    public string title;
    public Vector2 position; // longitude, latitude (x, y)
    public int zoom;
    public Vector2 constraintsMin; // minLongitude, minLatitude, maxLongitude, maxLatitude (x, y, z, w)
    public Vector2 constraintsMax; // minLongitude, minLatitude, maxLongitude, maxLatitude (x, y, z, w)
    //public Vector4 constraints; // minLongitude, minLatitude, maxLongitude, maxLatitude (x, y, z, w)
    public List<cPath> paths = new List<cPath>();

    public static readonly string PREFS_KEY = "areas";
    public static readonly string AREAS = "areas";

    public static readonly string AREA = "area";
    public static readonly string ID = "id";
    public static readonly string TITLE = "title";
    public static readonly string POSITION = "position";
    public static readonly string ZOOM = "zoom";
    public static readonly string CONSTRAINTS_MIN = "constraintsMin"; // "areaConstraintsMin";
    public static readonly string CONSTRAINTS_MAX = "constraintsMax"; // "areaConstraintsMax";
    // public static readonly string VIEW_CONSTRAINTS_MIN = "viewConstraintsMin";
    // public static readonly string VIEW_CONSTRAINTS_MAX = "viewConstraintsMax";
    public static readonly string PATHS = "paths";
    #endregion

    #region Methods
    public cArea(int _id, string _title, Vector2 _position, int _zoom, Vector2 _constraintsMin, Vector2 _constraintsMax) // TODO: Make private when testing is finished
    {
        Id = _id;
        title = _title;
        position = _position;
        zoom = _zoom;
        //constraints = _constraints;
        constraintsMin = _constraintsMin;
        constraintsMax = _constraintsMax;
    }

    public cArea(string _title, Vector2 _position, int _zoom, Vector2 _constraintsMin, Vector2 _constraintsMax)
    {
        Id = GetAvailableAreaID();
        title = _title;
        position = _position;
        zoom = _zoom;
        //constraints = _constraints;
        constraintsMin = _constraintsMin;
        constraintsMax = _constraintsMax;
    }

    public cArea(string _title) // for creating a new area on map click
    {
        Id = GetAvailableAreaID();
        title = _title;
        position = OnlineMaps.instance.position;
        zoom = OnlineMaps.instance.zoom; //MapManager.DEFAULT_ZOOM;
        //constraints = new Vector4(_position.x - MapManager.DEFAULT_POSITION_OFFSET, _position.y - MapManager.DEFAULT_POSITION_OFFSET, _position.x + MapManager.DEFAULT_POSITION_OFFSET, _position.y + MapManager.DEFAULT_POSITION_OFFSET);
        constraintsMin = new Vector2((float)OnlineMaps.instance.bounds.left, (float)OnlineMaps.instance.bounds.bottom);
        constraintsMax = new Vector2((float)OnlineMaps.instance.bounds.right, (float)OnlineMaps.instance.bounds.top);
    }

    /*void Create()
    {

    }

    void Show()
    {

    }*/

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
        // Load xml document, if null create new
        OnlineMapsXML xml = GetXML();

        // Create a new path
        OnlineMapsXML areaNode = xml.Create(AREA);
        areaNode.Create(ID, _areaToSave.Id);
        areaNode.Create(TITLE, _areaToSave.title);
        areaNode.Create(POSITION, _areaToSave.position);
        areaNode.Create(ZOOM, _areaToSave.zoom);
        areaNode.Create(CONSTRAINTS_MIN, new Vector2(_areaToSave.constraintsMin.x, _areaToSave.constraintsMin.y));
        areaNode.Create(CONSTRAINTS_MAX, new Vector2(_areaToSave.constraintsMax.x, _areaToSave.constraintsMax.y));
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
        int id = _areaNode.Get<int>(ID);
        string title = _areaNode.Get<string>(TITLE);
        Vector2 position = _areaNode.Get<Vector2>(POSITION);
        int zoom = _areaNode.Get<int>(ZOOM);
        Vector2 constraints_min = _areaNode.Get<Vector2>(CONSTRAINTS_MIN);
        Vector2 constraints_max = _areaNode.Get<Vector2>(CONSTRAINTS_MAX);

        // Create cArea and add it to loadedAreas list
        cArea loadedArea = new cArea(id, title, position, zoom, constraints_min, constraints_max);
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
    #endregion
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
