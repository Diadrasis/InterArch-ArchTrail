﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cPathPoint
{
    #region Variables
    public int server_path_id;
    public int server_point_id;
    public int local_path_id;
    public int index;
    public Vector2 position; // longitude, latitude (x, y)
    //public TimeSpan time;
    public float duration; // time duartion in seconds

    private static readonly string PATHPOINT = "pathPoint";

    private static readonly string SERVER_POINT_ID = "server_point_id";
    private static readonly string INDEX = "index";
    private static readonly string POSITION = "position";
    //private static readonly string TIME = "time";
    private static readonly string DURATION = "duration";
    #endregion

    #region Methods
    // Constructor for Create
    public cPathPoint(int _local_path_id, int _index, Vector2 _position, float _duration) //TimeSpan _time
    {
        server_path_id = -1;
        local_path_id = _local_path_id;
        server_point_id = -1;
        index = _index;
        position = _position;
        //time = _time;
        duration = _duration;
    }

    // Constructor for Load
    public cPathPoint(int _server_path_id, int _local_path_id, int _server_point_id, int _index, Vector2 _position, float _duration) // TimeSpan _time
    {
        server_path_id = _server_path_id;
        local_path_id = _local_path_id;
        server_point_id = _server_point_id;
        index = _index;
        position = _position;
        duration = _duration;
        //time = _time;
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
        pathPointNode.Create(cPath.SERVER_PATH_ID, _pathPointToSave.server_path_id);
        pathPointNode.Create(cPath.LOCAL_PATH_ID, _pathPointToSave.local_path_id);
        pathPointNode.Create(SERVER_POINT_ID, _pathPointToSave.server_point_id);
        pathPointNode.Create(INDEX, _pathPointToSave.index);
        pathPointNode.Create(POSITION, _pathPointToSave.position);
        pathPointNode.Create(DURATION, _pathPointToSave.duration);
        //pathPointNode.Create(TIME, _pathPointToSave.time.ToString());
    }

    public static void SavePathPoints(OnlineMapsXML _pathPointsNode, List<cPathPoint> _pathPointsToSave)
    {
        foreach (cPathPoint pathPointToSave in _pathPointsToSave)
        {
            Save(_pathPointsNode, pathPointToSave);
        }
    }

    private static void EditServerPointId(cPathPoint _pointToEdit)
    {
        // Load xml document, if null create new
        OnlineMapsXML xml = cArea.GetXML();

        // Get point from xml and Edit values
        OnlineMapsXML pointNode = xml.Find("/" + cArea.AREAS + "/" + cArea.AREA + "/" + cArea.PATHS + "/" + cPath.PATH + "[" + cPath.LOCAL_PATH_ID + "=" + _pointToEdit.local_path_id + "]/" + cPath.PATH_POINTS + "/" + PATHPOINT + "[" + INDEX + "=" + _pointToEdit.index + "]");
        pointNode.Remove(cPath.SERVER_PATH_ID);
        pointNode.Create(cPath.SERVER_PATH_ID, _pointToEdit.server_path_id);
        pointNode.Remove(SERVER_POINT_ID);
        pointNode.Create(SERVER_POINT_ID, _pointToEdit.server_point_id);

        // Save xml string to PlayerPrefs
        PlayerPrefs.SetString(cArea.PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }

    private static cPathPoint Load(OnlineMapsXML _pathPointNode)
    {
        int server_path_id = _pathPointNode.Get<int>(cPath.SERVER_PATH_ID);
        int local_path_id = _pathPointNode.Get<int>(cPath.LOCAL_PATH_ID);
        int server_point_id = _pathPointNode.Get<int>(SERVER_POINT_ID);
        int index = _pathPointNode.Get<int>(INDEX);
        Vector2 position = _pathPointNode.Get<Vector2>(POSITION);
        float duration = _pathPointNode.Get<float>(DURATION);
        //string timeString = _pathPointNode.Get<string>(TIME);
        //TimeSpan time = TimeSpan.Parse(timeString);

        // Create cArea and add it to loadedAreas list
        cPathPoint loadedPathPoint = new cPathPoint(server_path_id, local_path_id, server_point_id, index, position, duration);
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

    public static List<cPathPoint> GetPointsToUpload()
    {
        // List of points
        List<cPathPoint> pointsToUpload = new List<cPathPoint>();

        // Load xml document, if null creates new
        OnlineMapsXML xml = cArea.GetXML();

        // Get points with database id = -1
        OnlineMapsXMLList pointNodes = xml.FindAll("/" + cArea.AREAS + "/" + cArea.AREA + "/" + cArea.PATHS + "/" + cPath.PATH + "/" + cPath.PATH_POINTS + "/" + PATHPOINT + "[" + SERVER_POINT_ID + "= -1" + "]");

        foreach (OnlineMapsXML pointNode in pointNodes)
        {
            if (pointNode.isNull)
            {
                Debug.Log("Point has been deleted!");
                continue;
            }

            cPathPoint loadedPoint = Load(pointNode);

            if (loadedPoint != null)
            {
                // Get server_path_id
                OnlineMapsXML pathNode = xml.Find("/" + cArea.AREAS + "/" + cArea.AREA + "/" + cArea.PATHS + "/" + cPath.PATH + "[" + cPath.LOCAL_PATH_ID + "=" + loadedPoint.local_path_id + "]/" + cPath.SERVER_PATH_ID);
                string server_path_idString = pathNode.Value();
                if (int.TryParse(server_path_idString, out int server_path_id))
                    Debug.Log("path's server_path_id = " + server_path_id);
                // If server_path_id is -1 then the area is not uploaded yet so don't upload its points yet.
                if (server_path_id != -1)
                {
                    // Set path's server_area_id
                    if (loadedPoint.server_path_id == -1)
                        loadedPoint.server_path_id = server_path_id;

                    pointsToUpload.Add(loadedPoint);
                }
            }
        }

        return pointsToUpload;
    }

    public static void SetServerPathAndPointId(int _server_path_id, int _server_point_id, int _index)
    {
        // Load xml document, if null creates new
        OnlineMapsXML xml = cArea.GetXML();

        // Find point
        OnlineMapsXML pointNode = xml.Find("/" + cArea.AREAS + "/" + cArea.AREA + "/" + cArea.PATHS + "/" + cPath.PATH + "/" + cPath.PATH_POINTS + "/" + PATHPOINT + "[" + INDEX + "=" + _index + "]");

        // Load point
        cPathPoint loadedPoint = Load(pointNode);
        if (loadedPoint.server_path_id == -1)
            loadedPoint.server_path_id = _server_path_id;
        loadedPoint.server_point_id = _server_point_id;

        // Edit path
        EditServerPointId(loadedPoint);
    }
    #endregion
}
