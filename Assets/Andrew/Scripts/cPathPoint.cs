using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cPathPoint
{
    #region Variables
    public string pathTitle;
    public int index;
    public Vector2 position;
    public float time;

    private static readonly string PREFS_PATHPOINTS_KEY = "pathPoints";
    private static readonly string PATHPOINTS = "PathPoints";

    //private static readonly string PATHPOINT = "PathPoint";
    private static readonly string PATH_TITLE = "PathTitle";
    private static readonly string INDEX = "Index";
    private static readonly string POSITION = "Position";
    private static readonly string TIME = "Time";
    #endregion

    #region Methods
    public cPathPoint(string _pathTitle, int _index, Vector2 _position, float _time)
    {
        pathTitle = _pathTitle;
        index = _index;
        position = _position;
        time = _time;
    }

    //create path points when position or time is updated
    /*void Create()
    {

    }*/

    //save each point in a path
    /*void Save()
    {

    }*/

    //show/load path points in a path. Probable will be associated with cPath
    void Show()
    {

    }

    public static void Delete(cPathPoint _pathPointToDelete)
    {
        OnlineMapsXML xml = GetXML();

        string childName = _pathPointToDelete.pathTitle + _pathPointToDelete.index;

        if (xml.HasChild(childName))
            xml.Remove(childName);

        PlayerPrefs.SetString(PREFS_PATHPOINTS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }

    private static OnlineMapsXML GetXML()
    {
        // Load xml string from PlayerPrefs
        string xmlData = PlayerPrefs.GetString(PREFS_PATHPOINTS_KEY);

        // Load xml document, if null create new
        OnlineMapsXML xml = OnlineMapsXML.Load(xmlData);
        if (xml.isNull)
        {
            xml = new OnlineMapsXML(PATHPOINTS);
            //Debug.Log("New PATHS XML");
        }

        return xml;
    }

    public static void Save(cPathPoint _pathPointToSave)
    {
        // Load xml document, if null create new
        OnlineMapsXML xml = GetXML();

        // Create a new path
        OnlineMapsXML pathNode = xml.Create(_pathPointToSave.pathTitle + _pathPointToSave.index); // PATHPOINT
        pathNode.Create(PATH_TITLE, _pathPointToSave.pathTitle);
        pathNode.Create(INDEX, _pathPointToSave.index);
        pathNode.Create(POSITION, _pathPointToSave.position);
        pathNode.Create(TIME, _pathPointToSave.time);

        // Save xml string to PlayerPrefs
        PlayerPrefs.SetString(PREFS_PATHPOINTS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }

    public static void SavePathPoints(List<cPathPoint> _pathPointsToSave)
    {
        foreach (cPathPoint pathPointToSave in _pathPointsToSave)
        {
            Save(pathPointToSave);
        }
    }

    public static List<cPathPoint> LoadPathPointsOfPath(string _pathTitle)
    {
        List<cPathPoint> loadedPathPoints = LoadPathPoints(); // TODO: Optimize
        List<cPathPoint> pathPoints = new List<cPathPoint>();

        foreach (cPathPoint pathPoint in loadedPathPoints)
        {
            if (pathPoint.pathTitle.Equals(_pathTitle))
                pathPoints.Add(pathPoint);
        }

        return pathPoints;
    }

    public static List<cPathPoint> LoadPathPoints()
    {
        // If the key does not exist, returns.
        if (!PlayerPrefs.HasKey(PREFS_PATHPOINTS_KEY))
            return null;

        // Init list of cPathPoints to add paths to
        List<cPathPoint> loadedPathPoints = new List<cPathPoint>();

        // Load xml string from PlayerPrefs
        string xmlData = PlayerPrefs.GetString(PREFS_PATHPOINTS_KEY);

        // Load xml document
        OnlineMapsXML xml = OnlineMapsXML.Load(xmlData);

        // Load path points
        foreach (OnlineMapsXML node in xml)
        {
            string areaTitle = node.Get<string>(PATH_TITLE);
            int index = node.Get<int>(INDEX);
            Vector2 position = node.Get<Vector2>(POSITION);
            float time = node.Get<float>(TIME);

            // Create cPathPoint and add it to loadedPathPoints list
            cPathPoint loadedPathPoint = new cPathPoint(areaTitle, index, position, time);
            loadedPathPoints.Add(loadedPathPoint);
        }

        return loadedPathPoints;
    }
    #endregion
}
