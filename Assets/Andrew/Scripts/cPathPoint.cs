using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cPathPoint
{
    #region Variables
    public int pathId;
    public int index;
    public Vector2 position; // longitude, latitude (x, y)
    public float time;

    private static readonly string PATHPOINT = "pathPoint";
    private static readonly string PATH_ID = "pathId";
    private static readonly string INDEX = "index";
    private static readonly string POSITION = "position";
    private static readonly string TIME = "time";
    #endregion

    #region Methods
    public cPathPoint(int _pathId, int _index, Vector2 _position, float _time)
    {
        pathId = _pathId;
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

    /*public static void Delete(cPathPoint _pathPointToDelete)
    {
        OnlineMapsXML xml = GetXML();

        string childName = _pathPointToDelete.pathTitle + _pathPointToDelete.index;

        if (xml.HasChild(childName))
            xml.Remove(childName);

        PlayerPrefs.SetString(PREFS_PATHPOINTS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }*/

    public static void Save(OnlineMapsXML _pathPointsNode, cPathPoint _pathPointToSave)
    {
        // Create a new path
        OnlineMapsXML pathPointNode = _pathPointsNode.Create(PATHPOINT);
        pathPointNode.Create(PATH_ID, _pathPointToSave.pathId);
        pathPointNode.Create(INDEX, _pathPointToSave.index);
        pathPointNode.Create(POSITION, _pathPointToSave.position);
        pathPointNode.Create(TIME, _pathPointToSave.time);

        // Save xml string to PlayerPrefs
        //PlayerPrefs.SetString(PREFS_PATHPOINTS_KEY, xml.outerXml);
        //PlayerPrefs.Save();
    }

    public static void SavePathPoints(OnlineMapsXML _pathPointsNode, List<cPathPoint> _pathPointsToSave)
    {
        foreach (cPathPoint pathPointToSave in _pathPointsToSave)
        {
            Save(_pathPointsNode, pathPointToSave);
        }
    }

    private static cPathPoint Load(OnlineMapsXML _pathPointNode)
    {
        int pathId = _pathPointNode.Get<int>(PATH_ID);
        int index = _pathPointNode.Get<int>(INDEX);
        Vector2 position = _pathPointNode.Get<Vector2>(POSITION);
        float time = _pathPointNode.Get<float>(TIME);

        // Create cArea and add it to loadedAreas list
        cPathPoint loadedPathPoint = new cPathPoint(pathId, index, position, time);
        return loadedPathPoint;
    }

    public static List<cPathPoint> LoadPathPointsOfPath(OnlineMapsXML _pathPointsNode)
    {
        List<cPathPoint> loadedpathPoints = new List<cPathPoint>();

        foreach (OnlineMapsXML pathPointNode in _pathPointsNode)
        {
            loadedpathPoints.Add(Load(pathPointNode));
        }

        return loadedpathPoints;
    }
    #endregion
}
