using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class cArea
{
    #region Variables
    //private int id;
    public string title;
    public Vector2 position; // longitude, latitude (x, y)
    public int zoom;
    public Vector4 constraints; // minLongitude, minLatitude, maxLongitude, maxLatitude (x, y, z, w)
    //public List<cPath> paths = new List<cPath>();

    private static readonly string PREFS_AREAS_KEY = "areas";
    private static readonly string AREAS = "areas";

    private static readonly string AREA = "area";
    private static readonly string TITLE = "title";
    private static readonly string POSITION = "position";
    private static readonly string ZOOM = "zoom";
    private static readonly string CONSTRAINTS_MIN = "constraintsMin";
    private static readonly string CONSTRAINTS_MAX = "constraintsMax";
    #endregion

    #region Methods
    public cArea(string _title, Vector2 _position, int _zoom, Vector4 _constraints)
    {
        title = _title;
        position = _position;
        zoom = _zoom;
        constraints = _constraints;
    }

    // FOR TESTING, WILL BE REMOVED
    public cArea(string _title, Vector2 _position)
    {
        title = _title;
        position = _position;
        zoom = MapManager.DEFAULT_ZOOM;
        constraints = new Vector4(_position.x - MapManager.DEFAULT_POSITION_OFFSET, _position.y - MapManager.DEFAULT_POSITION_OFFSET, _position.x + MapManager.DEFAULT_POSITION_OFFSET, _position.y + MapManager.DEFAULT_POSITION_OFFSET);
    }

    void Create()
    {

    }

    void Show()
    {

    }

    public static void Delete(string _areaTitleToDelete)
    {
        OnlineMapsXML xml = GetXML();

        if (xml.HasChild(_areaTitleToDelete))
            xml.Remove(_areaTitleToDelete);

        PlayerPrefs.SetString(PREFS_AREAS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }

    private static OnlineMapsXML GetXML()
    {
        // Load xml string from PlayerPrefs
        string xmlData = PlayerPrefs.GetString(PREFS_AREAS_KEY);

        // Load xml document, if null create new
        OnlineMapsXML xml = OnlineMapsXML.Load(xmlData);
        if (xml.isNull)
        {
            xml = new OnlineMapsXML(AREAS);
            Debug.Log("New PATHS XML");
        }

        return xml;
    }

    public static void Save(cArea _areaToSave)
    {
        // Load xml document, if null create new
        OnlineMapsXML xml = GetXML();

        // Create a new path
        OnlineMapsXML areaNode = xml.Create(_areaToSave.title);
        areaNode.Create(TITLE, _areaToSave.title);
        areaNode.Create(POSITION, _areaToSave.position);
        areaNode.Create(ZOOM, _areaToSave.zoom);
        areaNode.Create(CONSTRAINTS_MIN, new Vector2(_areaToSave.constraints.x, _areaToSave.constraints.y));
        areaNode.Create(CONSTRAINTS_MAX, new Vector2(_areaToSave.constraints.z, _areaToSave.constraints.w));

        // Save xml string to PlayerPrefs
        PlayerPrefs.SetString(PREFS_AREAS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }

    public static void SaveAreas(List<cArea> _areasToSave)
    {
        foreach (cArea areaToSave in _areasToSave)
        {
            Save(areaToSave);
        }
    }

    public static string GetInfoFromXML(string _xpath)
    {
        OnlineMapsXML xml = GetXML();
        XmlNode node = xml.document.SelectSingleNode(_xpath);
        return node.InnerText;
    }

    public static List<cArea> LoadAreas()
    {
        // If the key does not exist, returns.
        if (!PlayerPrefs.HasKey(PREFS_AREAS_KEY))
            return null;

        // Init list of cArea to add paths to
        List<cArea> loadedAreas = new List<cArea>();

        // Load xml string from PlayerPrefs
        string xmlData = PlayerPrefs.GetString(PREFS_AREAS_KEY);

        // Load xml document
        OnlineMapsXML xml = OnlineMapsXML.Load(xmlData);

        // Load areas
        foreach (OnlineMapsXML node in xml)
        {
            string title = node.Get<string>(TITLE);
            Vector2 position = node.Get<Vector2>(POSITION);
            int zoom = node.Get<int>(ZOOM);
            Vector2 constraints_min = node.Get<Vector2>(CONSTRAINTS_MIN);
            Vector2 constraints_max = node.Get<Vector2>(CONSTRAINTS_MAX);

            // Create cArea and add it to loadedAreas list
            cArea loadedArea = new cArea(title, position, zoom, new Vector4(constraints_min.x, constraints_min.y, constraints_max.x, constraints_max.y));
            loadedAreas.Add(loadedArea);
        }
        Debug.Log(xml.outerXml);
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
