﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    #region Variables
    [HideInInspector]
    public List<cArea> areas = new List<cArea>();
    //[HideInInspector]
    //public List<cPath> paths = new List<cPath>();
    [HideInInspector]
    public cArea currentArea;
    public cPath currentPath;

    public static readonly int DEFAULT_ZOOM = 19;
    public static readonly float DEFAULT_POSITION_OFFSET = 0.00114180675f; //0.00414180675f; //the commented number is the correct one when build the app
    [HideInInspector]
    public Vector2 previousPosition = Vector2.zero;
    
    [HideInInspector]
    public float minDistanceToPutNewMarker = 10f / 1000f;
    [HideInInspector]
    public bool isRecordingPath, isMovement, isPausePath;

    // Create a list of markers to draw the path lines
    [HideInInspector]
    public List<OnlineMapsMarker> markerListCurrPath = new List<OnlineMapsMarker>();
    //public List<OnlineMapsMarker> markersForDuration = new List<OnlineMapsMarker>();
    public cPath pathToDisplay;

    // createMarker on user position
    public OnlineMapsMarker userMarker;

    private float angle = 0.5f;
    public float time = 10;

    // Move direction
    private int direction = 1, moveZoom;

    //maybe can be deleted later, for now testing purposes
    private Vector2 fromPosition, toPosition;
    private double fromTileX, fromTileY, toTileX, toTileY;

    // Create Area
    public Texture2D markerCreateAreaTextureMin, markerCreateAreaTextureMax;
    public float borderWidth = 1;
    private bool createArea, editArea = false;
    OnlineMapsMarker[] markersCreateArea = new OnlineMapsMarker[2];
    Vector2[] positionsCreateArea = new Vector2[4];
    private OnlineMapsDrawingPoly polygon;
    private static readonly float DEFAULT_MARKER_SCALE = 0.2f;

    // Create Path
    TimeSpan previousPointTime;
    TimeSpan startedPauseTime;
    float pausedDuration = 0f;
    public Texture2D markerForDurationTexture;
    private List<Texture2D> markerDurationTextures;

    //TimeSpan testTime;

    //for markers
    private bool touchedLastUpdate = false;
    int lastTouchCount;
    //private int areaCounter = 0; // TODO: Remove, for testing only
    #endregion

    #region Unity Functions
    private void Awake()
    {
        areas = new List<cArea>();
        areas = cArea.LoadAreas();

        //List<cArea> areasToTestSave = GetTestAreas();
        //cArea.SaveAreas(areasToTestSave);

        //cArea.PrintData("/areas/area/id"); // /areas/area[title='Sarri']

        //Debug.Log(cArea.GetInfoFromXML("/areas/Μεσσήνη/title"));

        //testTime = DateTime.Now.TimeOfDay;
        //Debug.Log("Awake, testTime = " + testTime);
        //cPath.Delete(new cPath(1, 0));
    }

    private void Start()
    {
        // Download areas
        AppManager.Instance.serverManager.DownloadAreas();
        /*if (AppManager.Instance.serverManager.CheckInternet())
        {
            AppManager.Instance.serverManager.DownloadAreas();
            AppManager.Instance.serverManager.DownloadPaths();
            AppManager.Instance.serverManager.DownloadPoints();
        }
        areas = new List<cArea>();
        areas = cArea.LoadAreas();*/

        //AppManager.Instance.uIManager.DisplayAreasScreen();

        SubscribeToEvents();
        fromPosition = OnlineMaps.instance.position;
        toPosition = OnlineMapsLocationService.instance.position;
        isRecordingPath = false;
        isPausePath = false;

        userMarker = OnlineMapsMarkerManager.CreateItem(new Vector2(0, 0), AppManager.Instance.uIManager.userMarker, "User");
        userMarker.SetDraggable(false);
        userMarker.scale = 0.3f;

        // Test
        //List<cPath> pathsToTest = GetTestPaths();
        //DisplayPath(pathsToTest[0]);
    }

    private void Update()
    {
        // Checks the position of the markers.
        if ((createArea || editArea) && polygon != null)
            CheckMarkerPositions();

        if (!isMovement) return;

        angle += Time.deltaTime / time;

        if (angle > 1)
        {
            // stop movement
            isMovement = false;
            angle = 1;
        }

        double px = (toTileX - fromTileX) * angle + fromTileX;
        double py = (toTileY - fromTileY) * angle + fromTileY;

        userMarker.position = Vector2.Lerp(fromPosition, toPosition, angle);
        OnlineMaps.instance.projection.TileToCoordinates(px, py, moveZoom, out px, out py);
        OnlineMaps.instance.SetPosition(px, py);
        /*int touchCount = 0;
        //for input touch to be better in mobile
        bool touched = touchCount > 0;
        if (!touchedLastUpdate && touched)
        {
            OnlineMapsControlBase.instance.UpdateLastPosition();
            OnlineMapsControlBase.instance.GetTile(out lastPositionLng, out lastPositionLat);
        }

        touchedLastUpdate = touched;
        if (OnlineMapsControlBase.instance.dragMarker != null) OnlineMapsControlBase.instance.DragMarker();
        else if (OnlineMapsControlBase.instance.HitTest())
        {
            map.tooltipDrawer.ShowMarkersTooltip(GetInputPosition());
        }
        else
        {
            OnlineMapsTooltipDrawerBase.tooltip = string.Empty;
            OnlineMapsTooltipDrawerBase.tooltipMarker = null;
        }*/
        /*int i = 0;
        while (i < Input.touchCount)
        {
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(marker.position);
            Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began)
            {

                marker.position = touchPosition;
                AppManager.Instance.uIManager.infoText.text = "Began";

            }

            if (touch.phase == TouchPhase.Ended)
            {

                AppManager.Instance.uIManager.infoText.text = "Ended";
            }


            *//*if (touch.phase == TouchPhase.Moved)
            {
                marker.position = touchPosition;
                AppManager.Instance.uIManager.infoText.text = "Moved";
            }*//*
            ++i;
        }*/

    }

    private void OnDestroy()
    {
        //TimeSpan timeDuration = (DateTime.Now.TimeOfDay - testTime);
        //float duration = (float)timeDuration.TotalSeconds;
        //Debug.Log("timeDuration in seconds = " + duration);

        //PlayerPrefs.DeleteAll(); // TODO: REMOVE!!!
    }
    private void OnDisable()
    {
        //AppManager.Instance.serverManager.OnCheckInternetCheckComplete -= AppManager.Instance.androidManager.OnCheckInternetCheckComplete;
    }
    #endregion

    #region Methods
    public void SetMapViewToPoint(Vector2 _positionToView)
    {
        OnlineMaps.instance.SetPositionAndZoom(_positionToView.x, _positionToView.y, DEFAULT_ZOOM);
    }

    public void SetMapViewToLocation()
    {
        ResetMapConstraints();
        Vector2 locationPoint = OnlineMapsLocationService.instance.position; // 23.72413215765034, 37.98021913845082
        SetMapViewToPoint(locationPoint);
        //OnlineMapsLocationService.instance.updatePosition = true; // MUST BE UNCOMMENTED
    }

    public void SetMapViewToArea(cArea _areaToView)
    {
        //if (_areaToView.constraints != null)
        OnlineMaps.instance.positionRange = new OnlineMapsPositionRange(_areaToView.viewConstraintsMin.y, _areaToView.viewConstraintsMin.x, _areaToView.viewConstraintsMax.y, _areaToView.viewConstraintsMax.x);
        OnlineMaps.instance.zoomRange = new OnlineMapsRange(_areaToView.zoom, OnlineMaps.MAXZOOM);
        OnlineMaps.instance.SetPositionAndZoom(_areaToView.position.x, _areaToView.position.y, _areaToView.zoom);
        DisplayArea(_areaToView);
    }

    public cArea GetAreaByTitle(string _areaTitle)
    {
        foreach (cArea area in areas)
        {
            if (area.title.Equals(_areaTitle))
                return area;
        }
        return null;
    }

    public cPath GetPathByTitle(string _pathTitle)
    {
        foreach (cPath path in currentArea.paths)
        {
            if (path.title.Equals(_pathTitle))
            {
                //path.pathPoints = cPathPoint.GetPointsOfPath(path.local_path_id);
                //Debug.Log("GetPathByTitle, path.local_path_id = " + path.local_path_id);
                //Debug.Log("GetPathByTitle, path.pathPoints = " + path.pathPoints.Count);
                return path;
            }
        }

        return null;
    }

    public void SaveArea(string _areaTitle)
    {
        // Get center point
        OnlineMapsUtils.GetCenterPointAndZoom(markersCreateArea, out Vector2 center, out int zoom);

        // Create a new cArea
        cArea areaToSave = new cArea(_areaTitle, center, markersCreateArea[0].position, markersCreateArea[1].position);

        //if (areas != null && !areas.Contains(_areaToSave))
        {
            // Save area locally and reload areas
            cArea.Save(areaToSave);
            areas = cArea.LoadAreas();
            Debug.Log("Saved new area!");
            AppManager.Instance.serverManager.postUserData = true;

            // Upload user data to server
            /*if (areaCounter >= 2) // TODO: For testing only. Remove.
            {
                AppManager.Instance.serverManager.postUserData = true;
                areaCounter = 0;
            }
            else
                areaCounter += 1;*/
        }
    }
    //to save when edits have been made
    public void EditArea(cArea _areaToEdit, string _areaTitle)
    {
        // Get center point
        OnlineMapsUtils.GetCenterPointAndZoom(markersCreateArea, out Vector2 center, out int zoom);

        // Edit area values
        _areaToEdit.title = _areaTitle;
        _areaToEdit.position = center;
        _areaToEdit.zoom = zoom;
        _areaToEdit.areaConstraintsMin = markersCreateArea[0].position;
        _areaToEdit.areaConstraintsMax = markersCreateArea[1].position;
        _areaToEdit.viewConstraintsMin = new Vector2((float)OnlineMaps.instance.bounds.left, (float)OnlineMaps.instance.bounds.bottom);
        _areaToEdit.viewConstraintsMax = new Vector2((float)OnlineMaps.instance.bounds.right, (float)OnlineMaps.instance.bounds.top);

        // Edit area locally and reload areas
        cArea.Edit(_areaToEdit);
        areas = cArea.LoadAreas();
        editArea = false;
		
        // Edit area on server
        if (_areaToEdit.server_area_id != -1)
        {
            Debug.Log(_areaToEdit.title + " has server id = " + _areaToEdit.server_area_id);
            cArea.AddAreaIdToEdit(_areaToEdit.server_area_id);
            AppManager.Instance.serverManager.postUserData = true;
        }
    }

    public void DeleteArea(int _selectAreaObjectIndex)
    {
        // Delete area locally and reload areas
        cArea areaSelected = areas[_selectAreaObjectIndex];
        cArea.Delete(areaSelected.local_area_id);
        areas = cArea.LoadAreas();
        Debug.Log("Delete Area, local_area_id = " + areaSelected.local_area_id + "server_area_id = " + areaSelected.server_area_id);

        // Delete Area from server
        if (areaSelected.server_area_id != -1)
        {
            Debug.Log("AddIdToDelete, area's server id: " + areaSelected.server_area_id);
            cArea.AddIdToDelete(areaSelected.server_area_id);
            AppManager.Instance.serverManager.postUserData = true;
        }

        // TODO: For testing only. Remove.
        /*if (areaSelected.server_area_id != -1)
        {
            cArea.AddIdToDelete(areaSelected.server_area_id);
        }

        if (areaCounter >= 2)
        {
            AppManager.Instance.serverManager.postUserData = true;
            areaCounter = 0;
        }
        else
            areaCounter += 1;*/
    }

    public void DeletePath(int _selectPathObjectIndex)
    {
        cPath pathSelected = currentArea.paths[_selectPathObjectIndex];
        //Debug.Log(pathSelected.title);
        cPath.Delete(pathSelected);
        Debug.Log("Deleted Path, local_path_id = " + pathSelected.local_area_id + "server_path_id = " + pathSelected.server_area_id);

        // Delete Path from server
        if (pathSelected.server_path_id != -1)
        {
            Debug.Log("AddIdToDelete, path's server id: " + pathSelected.server_area_id);
            cPath.AddIdToDelete(pathSelected.server_path_id);
            AppManager.Instance.serverManager.postUserData = true;
        }
    }

    private void ResetMapConstraints()
    {
        OnlineMaps.instance.positionRange = null;
        OnlineMaps.instance.zoomRange = null;
        OnlineMaps.instance.Redraw();
    }

    private void SubscribeToEvents()
    {
        // GPS Events
        OnlineMapsLocationService.instance.OnLocationInited += SetMapViewToLocation;

        // Input Events
        OnlineMapsControlBase.instance.OnMapClick += OnMapClick;
        //OnlineMapsControlBase.instance.OnMapRelease += AppManager.Instance.uIManager.OnMapRelease;

        //Changed Location Event
        OnlineMapsLocationService.instance.OnLocationChanged += OnLocationChanged;

        //Interent Events
        //AppManager.Instance.serverManager.OnDownloadData += ReloadAreasScreen;
        //AppManager.Instance.serverManager.OnCheckInternetCheckComplete += AppManager.Instance.androidManager.OnCheckInternetCheckComplete;
    }

    public void ReloadAreas()
    {
        // Reload Areas
        areas = cArea.LoadAreas();

        // Display Areas
        AppManager.Instance.uIManager.DisplayAreasScreen();
    }

    public void ReloadPaths()
    {
        // Reload Paths
        currentArea.paths = cPath.LoadPathsOfArea(currentArea.local_area_id);

        // Display Paths
        //AppManager.Instance.uIManager.DisplayPathsScreen();
    }

    public void ReloadPoints()
    {
        if (pathToDisplay != null)
        {
            if (pathToDisplay.pathPoints.Count <= 0)
            {
                // Reload Points
                pathToDisplay.pathPoints = cPathPoint.LoadPathPointsOfPath(pathToDisplay.local_path_id);

                // Display Points
                DisplayPath(pathToDisplay);
            }
        }
    }

    private void OnMapClick()
    {
        // Create a new area
        if (createArea)
        {
            // if there is no polygon
            if (polygon == null)
            {
                GeneralAreaCreation();
                // Activate button
                AppManager.Instance.uIManager.btnSaveArea.interactable = true;
            }
        }
    }
    
    public void CreateNewAreaInitialize()
    {
        OnlineMapsDrawingElementManager.RemoveAllItems();
        SetMapViewToLocation();
        createArea = true;
        polygon = null;
    }

    public void CreateNewAreaFinalize()
    {
        RemoveNewArea();
        createArea = false;
    }

    public void RemoveNewArea()
    {
        // Remove Polygon
        if (polygon != null)
        {
            OnlineMapsDrawingElementManager.RemoveItem(polygon);
            polygon = null;
        }

        // Remove Markers
        if (markersCreateArea != null)
        {
            foreach (OnlineMapsMarker marker in markersCreateArea)
            {
                if (marker != null)
                    OnlineMapsMarkerManager.RemoveItem(marker);
            }

            // Reset arrays to default values
            markersCreateArea.Initialize();
            positionsCreateArea.Initialize();
        }
    }

    public void CheckUserPosition()
    {
        if(currentArea != null)
        {
            if (!IsWithinConstraints())
            {
                AppManager.Instance.uIManager.pnlWarningScreen.SetActive(true);
                AppManager.Instance.uIManager.btnAddNewPath.interactable = false;
                
            }
            else
            {   
                AppManager.Instance.uIManager.pnlWarningScreen.SetActive(false);
                AppManager.Instance.uIManager.btnAddNewPath.interactable = true;
            }
        }
    }
    
    private bool IsWithinConstraints()
    {
        if ((currentArea.areaConstraintsMin.x < OnlineMapsLocationService.instance.position.x) && (OnlineMapsLocationService.instance.position.x < currentArea.areaConstraintsMax.x)
            && (currentArea.areaConstraintsMin.y < OnlineMapsLocationService.instance.position.y) && (OnlineMapsLocationService.instance.position.y < currentArea.areaConstraintsMax.y))
        {
            return true;
        }
        return false;
    }

    
    #region Path
    public void OnLocationChanged(Vector2 position)
    {
        SetMarkerOnUserPosition(position);
        CheckUserPosition();

        if (isRecordingPath && !isPausePath && IsWithinConstraints())
        {
            OnlineMapsLocationService.instance.UpdatePosition();

            float distance = OnlineMapsUtils.DistanceBetweenPoints(position, previousPosition).magnitude;
            
            if (distance < OnlineMapsLocationService.instance.updateDistance / 1000f)
            {
                //OnlineMaps.instance.Redraw();
                return;
            }
            else
            {
                // Creates a new marker
                string label = "Point_" + currentPath.pathPoints.Count + "_" + DateTime.Now.TimeOfDay;
                OnlineMapsMarker marker = OnlineMapsMarkerManager.CreateItem(position, label);
                marker.SetDraggable(false);
                marker.align = OnlineMapsAlign.Center;

                // Get new time duration
                TimeSpan timeDuration = (DateTime.Now.TimeOfDay - previousPointTime);
                previousPointTime = DateTime.Now.TimeOfDay;

                // Set duration of previous path point
                if (currentPath.pathPoints.Count > 0)
                    currentPath.pathPoints[currentPath.pathPoints.Count - 1].duration = (float)timeDuration.TotalSeconds - pausedDuration;

                // Reset pausedDuration
                pausedDuration = 0f;

                // Creates and Adds a new PathPoint
                currentPath.pathPoints.Add(new cPathPoint(currentPath.local_path_id, currentPath.pathPoints.Count, position, 0f));

                //marker.label = label;
                //marker.position = position;
                //OnlineMapsMarkerManager.AddItem(marker);

                // Creates a line
                markerListCurrPath.Add(marker);
                OnlineMapsDrawingElementManager.RemoveAllItems(e => e != polygon);
                OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingLine(markerListCurrPath.Select(m => m.position).ToArray(), Color.red, 3)); //OnlineMapsMarkerManager.instance.Select(m => m.position).ToArray(), Color.red, 3) // Average human walk speed is 1.4m/s

                previousPosition = position;
                OnlineMaps.instance.Redraw();
            }
        }
        else if ((isRecordingPath || isPausePath) && !IsWithinConstraints())
        {
            AppManager.Instance.uIManager.pnlWarningScreen.SetActive(false);
            AppManager.Instance.uIManager.pnlWarningSavePathScreen.SetActive(true);
            /*isRecordingPath = false;
            isPausePath = false;*/
            /*Debug.Log("isRecording Path: " + isRecordingPath);
            Debug.Log("isPause Path: " + isPausePath);*/
            //Debug.Log("is recording and not in constraints");
        }
        else if((!isRecordingPath && isPausePath) && IsWithinConstraints())
        {
            AppManager.Instance.uIManager.pnlWarningScreen.SetActive(false);
            AppManager.Instance.uIManager.pnlWarningSavePathScreen.SetActive(true);
           /* Debug.Log("isRecording Path: " + isRecordingPath);
            Debug.Log("isPause Path: " + isPausePath);*/
        }
        else if ((isRecordingPath && !isPausePath) && IsWithinConstraints())
        {
            AppManager.Instance.uIManager.pnlWarningScreen.SetActive(false);
            AppManager.Instance.uIManager.pnlWarningSavePathScreen.SetActive(true);
           /* Debug.Log("isRecording Path: " + isRecordingPath);
            Debug.Log("isPause Path: " + isPausePath);*/
        }
        else if (!isRecordingPath && !isPausePath && IsWithinConstraints())
        {
            AppManager.Instance.uIManager.pnlWarningScreen.SetActive(false);
            AppManager.Instance.uIManager.pnlWarningSavePathScreen.SetActive(false);
            /*Debug.Log("isRecording Path: " + isRecordingPath);
            Debug.Log("isPause Path: " + isPausePath);*/
        }
        else
        {
            AppManager.Instance.uIManager.pnlWarningScreen.SetActive(true);
            /*Debug.Log("isRecording Path: " + isRecordingPath);
            Debug.Log("isPause Path: " + isPausePath);*/
            //Debug.Log("is not recording and not in constraints");
        }
        //marker.OnPositionChanged += OnMarkerPositionChange;
    }

    public void StartRecordingPath()
    {
        isRecordingPath = true;
        isPausePath = false;
        previousPosition = OnlineMapsLocationService.instance.position;

        // Center View 
        OnlineMapsLocationService.instance.UpdatePosition();

        // Create a new marker at the starting position
        OnlineMapsMarker marker = OnlineMapsMarkerManager.CreateItem(previousPosition, "Point_0_" + DateTime.Now.TimeOfDay);
        marker.align = OnlineMapsAlign.Center;
        // Clear and Add new marker to markerListCurrPath
        markerListCurrPath.Clear();
        markerListCurrPath.Add(marker);

        previousPointTime = DateTime.Now.TimeOfDay;
        
        currentPath = new cPath(currentArea.local_area_id);
        currentPath.pathPoints.Add(new cPathPoint(currentPath.local_path_id, currentPath.pathPoints.Count, previousPosition, 0f));

        /* TIMESPAN TESTING
        TimeSpan time = DateTime.Now.TimeOfDay;
        Debug.Log("Time of day = " + time);
        string timeString = time.ToString();
        Debug.Log("timeString = " + time);
        //Debug.Log("Hours = " + time.Hours);
        //Debug.Log("Hours = " + time.Hours);
        TimeSpan dateTime = TimeSpan.Parse(timeString);
        Debug.Log("Parsed string = " + dateTime);*/
    }

    public void PauseRecordingPath()
    {
        startedPauseTime = DateTime.Now.TimeOfDay;
        //Debug.Log("PauseRecordingPath, startedPauseTime = " + startedPauseTime);
        isPausePath = true;
    }

    //when pausing the recording but user hasn't saved the path yet
    public void ResumeRecordingPath()
    {
        // Get pausedDuration and add it to previous pausedDuration
        TimeSpan pausedTimeDuration = (DateTime.Now.TimeOfDay - startedPauseTime);
        pausedDuration += (float)pausedTimeDuration.TotalSeconds;
        //Debug.Log("pausedDuration = " + pausedDuration);
        isRecordingPath = true;
        isPausePath = false;
    }

    //when user stps recording and is not on pause too
    public void StopRecordingPath()
    {
        isRecordingPath = false;
        isPausePath = false;
    }

    public void SavePath()
    {
        // Get pausedDuration and add it to previous pausedDuration
        TimeSpan pausedTimeDuration = (DateTime.Now.TimeOfDay - startedPauseTime);
        pausedDuration += (float)pausedTimeDuration.TotalSeconds;
        //Debug.Log("pausedDuration = " + pausedDuration);
        // Get new time duration
        TimeSpan timeDuration = (DateTime.Now.TimeOfDay - previousPointTime);

        // Set duration of the last path point
        if (currentPath.pathPoints.Count > 0)
            currentPath.pathPoints[currentPath.pathPoints.Count - 1].duration = (float)timeDuration.TotalSeconds - pausedDuration;

        // Debug path
        /*foreach (cPathPoint point in currentPath.pathPoints)
        {
            Debug.Log("point " + point.index + " duration = " + point.duration);
        }*/

        // Save
        cPath.Save(currentPath);
        currentPath = null;

        // Upload user data to server
        AppManager.Instance.serverManager.postUserData = true;
    }

    public void DisplayPath(cPath _pathToDisplay)
    {
        // Get displaying path
        pathToDisplay = _pathToDisplay;

        // Sort points by index
        List<cPathPoint> sortedList = pathToDisplay.pathPoints.OrderBy(point => point.index).ToList();
        
        // Create a list of markers to draw the path lines
        //List<OnlineMapsMarker> markerListOfCurrentPath = new List<OnlineMapsMarker>();
        foreach (OnlineMapsMarker mapsMarker in markerListCurrPath)
        {
            mapsMarker.DestroyInstance();
        }
        markerListCurrPath.Clear();
        markerListCurrPath = new List<OnlineMapsMarker>();

        for (int i = 0; i < sortedList.Count; i++)
        {
            // Get cPathPoint
            cPathPoint pathPoint = sortedList[i];

            // Create marker base
            string label = "Path_" + pathToDisplay.local_path_id + "_Point_" + pathPoint.index + "_" + pathPoint.duration.ToString();
            OnlineMapsMarker marker = OnlineMapsMarkerManager.CreateItem(pathPoint.position, label);
            marker.SetDraggable(false);
            marker.align = OnlineMapsAlign.Center;
            marker.scale = 0.1f;

            // Add marker to current path marker list
            markerListCurrPath.Add(marker);
            //Debug.Log("i = " + i);
            //Debug.Log("Add marker = " + marker.label);
            //markerListOfCurrentPath.Add(marker);

            // Create marker ontop of marker to display duration
            OnlineMapsMarker markerForDuration = OnlineMapsMarkerManager.CreateItem(pathPoint.position, label);
            markerForDuration.SetDraggable(false);
            markerForDuration.align = OnlineMapsAlign.Center;
            markerForDuration.texture = markerForDurationTexture;
            markerForDuration.scale = 0.085f - DEFAULT_MARKER_SCALE * Mathf.Clamp(pathPoint.duration / 600f, 0f, 1f); // 1f, 2f
            //Debug.Log("Marker " + pathPoint.index + ", Scale = " + markerForDuration.scale);

            // Add markerForDuration on markersForDuaration list to remove later
            //markersForDuration.Add(markerForDuration);

            // Set marker scale
            //if (pathPoint.index != 0)
            {
                /*cPathPoint previousPathPoint = sortedList[i - 1];
                TimeSpan timeDuration= (pathPoint.time - previousPathPoint.time);
                marker.scale = 1f + 2f * Mathf.Clamp((float)timeDuration.TotalSeconds/600f, 0f, 1f); // 3 will be max value (timeDuration >= 600 sec(10min)), 1 is min value/default (timeDuration <= 7 sec)
                Debug.Log("Scale = " + marker.scale);*/

                //Debug.Log("Marker " + pathPoint.index + ", Scale = " + marker.scale);
                //Debug.Log("pathPoint.duration = " + pathPoint.duration);

                //markerForDuration.scale = 1f + DEFAULT_MARKER_SCALE * Mathf.Clamp(pathPoint.duration / 600f, 0f, 1f); // 1f, 2f
                //Debug.Log("Marker " + pathPoint.index + ", Scale = " + markerForDuration.scale);
            }
        }

        // Draw lines
        OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingLine(markerListCurrPath.Select(m => m.position).ToArray(), Color.red, 3)); //markerListOfCurrentPath
        OnlineMaps.instance.Redraw();

        // Set map view on path
        if (sortedList.Count > 0)
        {
            OnlineMaps.instance.SetPositionAndZoom(sortedList[0].position.x, sortedList[0].position.y, 20);
        }
    }

    public List<cPath> GetPaths()
    {
        currentArea.paths = cPath.LoadPathsOfArea(currentArea.local_area_id);
        return currentArea.paths;
    }


    #endregion

    #region Marker

    private void OnMarkerLongPress(OnlineMapsMarkerBase marker)
    {
        int touchCount = OnlineMapsControlBase.instance.GetTouchCount();
        
        if (touchCount != lastTouchCount)
        {
            /*if (allowTouchZoom)
            {
                if (touchCount == 1) OnlineMapsControlBase.instance.OnMapBasePress();
                else if (touchCount == 0) OnlineMapsControlBase.instance.OnMapBaseRelease();
            }*/

            if (lastTouchCount == 0) OnlineMapsControlBase.instance.UpdateLastPosition();
            OnlineMapsControlBase.instance.dragMarker = marker;
            OnlineMapsControlBase.instance.isMapDrag = false;
            marker.label = "";
        }
    }
        

    public void RemoveMarkersAndLine()
    {
        OnlineMapsMarkerManager.RemoveAllItems(m => m != userMarker);
        OnlineMapsDrawingElementManager.RemoveAllItems(e => e != polygon);
        OnlineMaps.instance.Redraw();
    }

    private void SetMarkerOnUserPosition(Vector2 position)
    {
        //CheckMyLocation();
        userMarker.position = position;
        OnlineMaps.instance.Redraw();
        SetMapViewToLocation();
    }

    private void CheckMarkerPositions()
    {
        // Check if the markers moved
        if ((markersCreateArea[0].position != positionsCreateArea[0]) || (markersCreateArea[1].position != positionsCreateArea[2]))
        {
            // Check limits and reset marker positions if necessary.
            if ((markersCreateArea[0].position.x > markersCreateArea[1].position.x) || (markersCreateArea[0].position.y > markersCreateArea[1].position.y))
            {
                markersCreateArea[0].position = positionsCreateArea[0];
            }
            if ((markersCreateArea[1].position.x < markersCreateArea[0].position.x) || (markersCreateArea[1].position.y < markersCreateArea[0].position.y))
            {
                markersCreateArea[1].position = positionsCreateArea[2];
            }

            positionsCreateArea[0] = markersCreateArea[0].position;
            positionsCreateArea[1] = new Vector2(markersCreateArea[0].position.x, markersCreateArea[1].position.y);
            positionsCreateArea[2] = markersCreateArea[1].position;
            positionsCreateArea[3] = new Vector2(markersCreateArea[1].position.x, markersCreateArea[0].position.y);
        }
    }

    private OnlineMapsDrawingPoly CreatePolygon(Vector2[] _arrayOfPoints)
    {
        // For points, reference to markerPositions. 
        // If you change the values ​​in markerPositions, value in the polygon will be adjusted automatically.
        OnlineMapsDrawingPoly onlineMapsDrawingPoly = new OnlineMapsDrawingPoly(_arrayOfPoints, Color.black, borderWidth, new Color(1, 1, 1, 0.3f));
        OnlineMapsDrawingElementManager.AddItem(onlineMapsDrawingPoly);
       
        return onlineMapsDrawingPoly;
    }

    public void DisplayArea(cArea _areaToDisplay)
    {
        Vector2[] points = new Vector2[4];

        points[0] = _areaToDisplay.areaConstraintsMin;
        points[1] = new Vector2(_areaToDisplay.areaConstraintsMin.x, _areaToDisplay.areaConstraintsMax.y);
        points[2] = _areaToDisplay.areaConstraintsMax;
        points[3] = new Vector2(_areaToDisplay.areaConstraintsMax.x, _areaToDisplay.areaConstraintsMin.y);

        if (polygon != null)
            OnlineMapsDrawingElementManager.RemoveItem(polygon);
        polygon = CreatePolygon(points); // OnlineMapsDrawingPoly polygonToDisplay = 
    }
    public void OnEditArea(cArea _areaToEdit)
    {
        editArea = true;
        //currentArea = _areaToEdit;
        //DisplayArea(_areaToEdit);
        SetMapViewToPoint(_areaToEdit.position);
    }
    public void EditSelectedArea(cArea _areaToEdit)
    {
        OnEditArea(_areaToEdit);
        
        if (editArea)
        {
            currentArea = _areaToEdit;
            
            if (polygon == null)
            {
                GeneralAreaCreation();
               
                // Activate button
                AppManager.Instance.uIManager.btnSaveEditArea.interactable = true;
                AppManager.Instance.uIManager.btnSaveEditArea.onClick.AddListener(() => AppManager.Instance.uIManager.EnableScreen(AppManager.Instance.uIManager.pnlSaveEditArea,true));
                AppManager.Instance.uIManager.inptFldEditArea.text = _areaToEdit.title;
            }
        }
        //Debug.Log("Edit area bool: " + editArea);
    }

    void GeneralAreaCreation()
    {
        Vector2 centerPosition = OnlineMaps.instance.position;

        // Calculate polygon positions
        Vector2 bottomLeftPosition = new Vector2((float)(OnlineMaps.instance.bounds.left + centerPosition.x) / 2, (float)(OnlineMaps.instance.bounds.bottom + centerPosition.y) / 2);
        Vector2 topLeftposition = new Vector2((float)(OnlineMaps.instance.bounds.left + centerPosition.x) / 2, (float)(centerPosition.y + OnlineMaps.instance.bounds.top) / 2);
        Vector2 topRightPosition = new Vector2((float)(centerPosition.x + OnlineMaps.instance.bounds.right) / 2, (float)(centerPosition.y + OnlineMaps.instance.bounds.top) / 2);
        Vector2 bottomRightPosition = new Vector2((float)(centerPosition.x + OnlineMaps.instance.bounds.right) / 2, (float)(OnlineMaps.instance.bounds.bottom + centerPosition.y) / 2);

        // Create two markers on the specified coordinates.

        //OnlineMapsMarker testM = OnlineMapsMarkerManager.CreateItem(topLeftposition, markerCreateAreaTexture, "topLeft");
        OnlineMapsMarker markerMin = OnlineMapsMarkerManager.CreateItem(bottomLeftPosition, markerCreateAreaTextureMin, "Marker Min");
        markerMin.scale = DEFAULT_MARKER_SCALE;
        markerMin.OnPress += OnMarkerLongPress;
        //markerMin.SetDraggable();
        markerMin.align = OnlineMapsAlign.Center;// so the graphic to be aligned correctly with the rectangle

        //OnlineMapsMarker testM2 = OnlineMapsMarkerManager.CreateItem(bottomRightPosition, markerCreateAreaTexture, "bottomRight");
        OnlineMapsMarker markerMax = OnlineMapsMarkerManager.CreateItem(topRightPosition, markerCreateAreaTextureMax, "Marker Max");
        markerMax.scale = DEFAULT_MARKER_SCALE;
        markerMax.OnPress += OnMarkerLongPress;
        //markerMax.SetDraggable();
        markerMax.align = OnlineMapsAlign.Center;// so the graphic to be aligned correctly with the rectangle
                                                 

        // Set markers and positions.
        markersCreateArea[0] = markerMin;
        markersCreateArea[1] = markerMax;
        positionsCreateArea[0] = bottomLeftPosition; // markerMin position
        positionsCreateArea[1] = topLeftposition;
        positionsCreateArea[2] = topRightPosition; // markerMax position
        positionsCreateArea[3] = bottomRightPosition;

        // Create polygon
        polygon = CreatePolygon(positionsCreateArea);

        // Redraw Map
        OnlineMaps.instance.Redraw();

    }
    #endregion

    #region Screencapture the path
    /*void RecMyPath()
    {
        if (isRecPath)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ScreenCapture.CaptureScreenshot(Application.persistentDataPath + "/" + System.DateTime.Now.ToString("yy'-'MM'-'dd'-'hh'-'mm") + ".png");
                Debug.Log(Application.persistentDataPath + "/" + System.DateTime.Now.ToString("yy'-'MM'-'dd'-'hh'-'mm") + ".png");
                //OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingLine(OnlineMapsMarkerManager.instance.Select(m => m.position).ToArray(), Color.red, 3));
            }

            StartCoroutine(TakeScreenShot(currentPath));

        }

    }
    IEnumerator TakeScreenShot(string pathname)
    {
        *//*if (!serverManager.useScreenShots) { yield break; }
        MarkersManager.CenterZoomOnMarkers();*//*

        yield return new WaitForEndOfFrame();
        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        byte[] bytes = tex.EncodeToJPG(); //Can also encode to jpg, just make sure to change the file extensions down below
        Destroy(tex);
        OnLocationChanged(OnlineMapsLocationService.instance.position);
        yield return new WaitForEndOfFrame();

        //Stathis.File_Manager.saveImage(bytes, pathname, Stathis.File_Manager.Ext.JPG);

        yield break;
    }*/
    #endregion

    #region NotInUse
    private void OnMarkerPositionChange(OnlineMapsMarkerBase marker)
    {
        //Debug.Log("OnMarkerPositionChange "+marker.label);
        OnlineMapsMarkerManager.instance.RemoveAll();
        OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingLine(OnlineMapsMarkerManager.instance.Select(m => m.position).ToArray(), Color.red, 3));
        //MarkersManager.CreateLineFromList(OnlineMaps.instance.markers.ToList(), Color.red, 3f);
        OnlineMaps.instance.Redraw();
    }
    private List<cArea> GetTestAreas()
    {
        List<cArea> areasFromDatabase = new List<cArea>()
        {
            new cArea(-1, 0, "Μεσσήνη", new Vector2(21.9202085525009f, 37.17642261183837f), 17, new Vector2(21.9160667457503f, 37.1700252387224f), new Vector2(21.9227518498302f, 37.178659594564f), new Vector2(21.9160667457503f, 37.1700252387224f), new Vector2(21.9227518498302f, 37.178659594564f)),
            new cArea(-1, 1, "Κνωσός", new Vector2(25.16310005634713f, 35.29800050616538f), 19, new Vector2(25.1616718900387f, 35.2958874528396f), new Vector2(25.1645352578472f, 35.3000733065711f), new Vector2(25.1616718900387f, 35.2958874528396f), new Vector2(25.1645352578472f, 35.3000733065711f)),
            new cArea(-1, 2, "Σαρρή", new Vector2(23.724021164280998f, 37.979955135461715f), 19, new Vector2(23.72385281512933f, 37.97881236959543f), new Vector2(23.725090676541246f, 37.9802439464203f), new Vector2(23.72385281512933f, 37.97881236959543f), new Vector2(23.725090676541246f, 37.9802439464203f))
        };

        //DisplayAreaDebug(areasFromDatabase[0]);
        //DisplayAreaDebug(areasFromDatabase[1]);
        //DisplayAreaDebug(areasFromDatabase[2]);

        return areasFromDatabase;
    }

    /*private List<cPath> GetTestPaths()
    {
        List<cPathPoint> pathPointsToTest0 = new List<cPathPoint>()
        {
            new cPathPoint(0, 0, new Vector2(23.724021164280998f, 37.979955135461715f), DateTime.Now.TimeOfDay),
            new cPathPoint(0, 1, new Vector2(23.7242f, 37.979955135461715f), DateTime.Now.TimeOfDay),
            new cPathPoint(0, 2, new Vector2(23.7244f, 37.9801f), DateTime.Now.TimeOfDay)
        };

        List<cPathPoint> pathPointsToTest1 = new List<cPathPoint>()
        {
            new cPathPoint(1, 0, Vector2.zero, DateTime.Now.TimeOfDay)
        };

        List<cPathPoint> pathPointsToTest2 = new List<cPathPoint>()
        {
            new cPathPoint(2, 0, Vector2.zero, DateTime.Now.TimeOfDay),
            new cPathPoint(2, 1, Vector2.zero, DateTime.Now.TimeOfDay)
        };

        List <cPath> pathsToTest = new List<cPath>()
        {
            new cPath(0, 0, pathPointsToTest0),
            new cPath(0, 1, pathPointsToTest1),
            new cPath(1, 2, pathPointsToTest2)
        };

        return pathsToTest;
    }*/
    private List<cPathPoint> GetTestPathPoints()
    {
        List<cPathPoint> pathPointsToTest = new List<cPathPoint>()
        {
            //new cPathPoint("path_0", ),
            //new cPathPoint("Μεσσήνη", "path_1"),
            //new cPathPoint("Κνωσός", "path_0")
        };

        return pathPointsToTest;
    }

    private void DisplayAreaDebug(cArea _area)
    {
        // ID
        Debug.Log("Id = " + _area.local_area_id);

        // Title
        Debug.Log("Title = " + _area.title);

        // Position
        Debug.Log("Longitude = " + _area.position.x);
        Debug.Log("Latitude = " + _area.position.y);

        // Zoom
        Debug.Log("Zoom = " + _area.zoom);

        // Constraints
        Debug.Log("minLongitude = " + _area.areaConstraintsMin.x);
        Debug.Log("minLatitude = " + _area.areaConstraintsMin.y);
        Debug.Log("maxLongitude = " + _area.areaConstraintsMax.x);
        Debug.Log("maxLatitude = " + _area.areaConstraintsMax.y);
    }
    //can be removed?
    /* public void CheckMyLocation()
     {
         //Debug.Log("CheckMyLocation");
         //CreateMarkerOnUserPosition();
         fromPosition = OnlineMaps.instance.position;
         toPosition = OnlineMapsLocationService.instance.position;

         // calculates tile positions
         moveZoom = OnlineMaps.instance.zoom;
         OnlineMaps.instance.projection.CoordinatesToTile(fromPosition.x, fromPosition.y, moveZoom, out fromTileX, out fromTileY);
         OnlineMaps.instance.projection.CoordinatesToTile(toPosition.x, toPosition.y, moveZoom, out toTileX, out toTileY);

         // if tile offset < 4, then start smooth movement
         if (OnlineMapsUtils.Magnitude(fromTileX, fromTileY, toTileX, toTileY) < 4)
         {
             // set relative position 0
             angle = 0;

             // start movement
             isMovement = true;
         }
         else // too far
         {
             OnlineMaps.instance.position = toPosition;
         }
     }*/

    /*public List<cPathPoint> GetPoints()
    {
        currentPath.pathPoints = cPathPoint.GetPointsOfPath(currentPath.local_path_id);
        return currentPath.pathPoints;
    }*/
    #endregion

    #endregion
}

