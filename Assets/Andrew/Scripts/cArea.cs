using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cArea
{
    #region Variables
    //private int id; // We probably need to add an id to find areas with because the user might use the same name for two areas
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

    void Delete()
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
        Debug.Log(availableAreaIndex);
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

    public static cArea Load(string _areaKey)
    {
        if (!PlayerPrefs.HasKey(_areaKey))
        {
            return null;
        }

        string title = PlayerPrefs.GetString(_areaKey);
        Vector2 position = PlayerPrefsX.GetVector2(title + POSITION_KEY, Vector2.zero);
        int zoom = PlayerPrefs.GetInt(title + ZOOM_KEY);
        Vector4 constraints = PlayerPrefsX.GetVector4(title + CONSTRAINTS_KEY);

        cArea loadedArea = new cArea(title, position, zoom, constraints);
        return loadedArea;
    }

    public static List<cArea> LoadAreas()
    {
        List<cArea> loadedAreas = new List<cArea>();
        cArea loadedArea = null;
        int index = 0;

        do
        {
            loadedArea = Load(AREA_KEY + index.ToString());
            if (loadedArea != null)
                loadedAreas.Add(loadedArea);
            else
                break;
            index++;
        }
        while (index < 1000000);

        return loadedAreas;
    }
    #endregion
}