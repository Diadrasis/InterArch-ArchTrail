using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cArea
{
    #region Variables
    private int id;
    public string title;
    public Vector2 position; // longitude, latitude (x, y)
    public int zoom;
    public Vector4 constraints; // minLongitude, minLatitude, maxLongitude, maxLatitude (x, y, z, w)
    //public List<cPath> paths = new List<cPath>();

    private static readonly string AREA_KEY = "area_";
    //private static readonly string TITLE_KEY = "_title";
    private static readonly string POSITION_KEY = "_position";
    private static readonly string ZOOM_KEY = "_zoom";
    private static readonly string CONSTRAINTS_KEY = "_constraints";
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
    #endregion
}