using System;
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
    public cPath pathToDisplay;

    // createMarker on user position
    public OnlineMapsMarker userMarker;

    private float angle = 0.5f;
    public float time = 10;

    // Move direction
    private int moveZoom;

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
    private static readonly float MARKER_SCALE = 0.1f;
    private static readonly float AREA_MARKER_SCALE = 0.2f;
    private static readonly float MARKERFORDURATION_SCALE = 0.085f;
    private static readonly float MAX_DURATION = 300f; // 5 min //600f; // 10 min

    // Create Path
    DateTime previousPointTime;
    TimeSpan startedPauseTime;
    float pausedDuration = 0f;
    public Texture2D markerForDurationTexture;
    OnlineMapsMarker currentMarkerForDuration;

    public bool isShown;
    //for markers
    private bool touchedLastUpdate = false;
    int lastTouchCount;
    #endregion

    #region Unity Functions
    private void Awake()
    {
        // Enable never sleep for android
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        areas = new List<cArea>();
        areas = cArea.LoadAreas();
    }

    private void Start()
    {
        // Download areas
        AppManager.Instance.serverManager.DownloadAreas();
        isShown = false;

        SubscribeToEvents();
        fromPosition = OnlineMaps.instance.position;
        toPosition = OnlineMapsLocationService.instance.position;
        isRecordingPath = false;
        isPausePath = false;

        CreateUserMarker();
    }

    private void Update()
    {
        // Change marker size while recording path
        if (isRecordingPath)
        {
            TimeSpan timeDuration = (DateTime.Now.TimeOfDay - previousPointTime.TimeOfDay);
            float totalDuration = (float)timeDuration.TotalSeconds + pausedDuration;
            float currentDuration = totalDuration - pausedDuration;
            float scale = MARKERFORDURATION_SCALE - (MARKERFORDURATION_SCALE * Mathf.Clamp(currentDuration / 50, 0f, 1f));
            currentMarkerForDuration.scale = MARKERFORDURATION_SCALE - (MARKERFORDURATION_SCALE * Mathf.Clamp(currentDuration / MAX_DURATION, 0f, 1f));
            OnlineMaps.instance.Redraw();
        }

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
        //PlayerPrefs.DeleteAll(); // TODO: REMOVE!!!

        // Deactivate never sleep for android
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
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
        // Set position range
        OnlineMaps.instance.positionRange = new OnlineMapsPositionRange(_areaToView.viewConstraintsMin.y, _areaToView.viewConstraintsMin.x, _areaToView.viewConstraintsMax.y, _areaToView.viewConstraintsMax.x);

        // Set zoom range
        OnlineMaps.instance.zoomRange = new OnlineMapsRange(_areaToView.zoom, OnlineMaps.MAXZOOM);

        // Set user's position and zoom depending on internet connection
        if (AppManager.Instance.serverManager.hasInternet || !IsWithinConstraints())
            OnlineMaps.instance.SetPositionAndZoom(_areaToView.position.x, _areaToView.position.y, _areaToView.zoom);
        else
            OnlineMaps.instance.SetPositionAndZoom(OnlineMapsLocationService.instance.position.x, OnlineMapsLocationService.instance.position.y, OnlineMaps.MAXZOOM);

        // Display area
        DisplayArea(_areaToView);
    }

    public cPath GetPathByIndex(int _index)
    {
        return currentArea.paths[_index];
    }

    public cArea GetAreaByIndex(int _index)
    {
        return areas[_index];
    }

    public void SaveArea(string _areaTitleGreek, string _areaTitleEnglish)
    {
        // Get center point
        OnlineMapsUtils.GetCenterPointAndZoom(markersCreateArea, out Vector2 center, out int zoom);

        // Create a new cArea
        cArea areaToSave = new cArea(_areaTitleGreek, _areaTitleEnglish, center, markersCreateArea[0].position, markersCreateArea[1].position);

        // Save area locally and reload areas
        cArea.Save(areaToSave);
        areas = cArea.LoadAreas();

        //Debug.Log("Saved new area!");
        AppManager.Instance.serverManager.postUserData = true;
        AppManager.Instance.serverManager.timeRemaining = 0f;

        // Clear Cache (For testing)
        //OnlineMapsCache.instance.ClearAllCaches();
    }

    //to save when edits have been made
    public void EditArea(cArea _areaToEdit, string _areaTitle, string _areaTitleEnglish)
    {
        StartCoroutine(EditEnumerator(_areaToEdit, _areaTitle, _areaTitleEnglish));
    }

    IEnumerator EditEnumerator(cArea _areaToEdit, string _areaTitle, string _areaTitleEnglish)
    {
        TileDownloader tileDownloader = AppManager.Instance.serverManager.tileDownloader;

        if (AppManager.Instance.serverManager.hasInternet)
        {
            // DeleteTiles
            AppManager.Instance.serverManager.tileDownloader.DeleteTiles(_areaToEdit);

            while (tileDownloader.isDeleting)
            {
                yield return null;
            }
        }

        // Get center point
        OnlineMapsUtils.GetCenterPointAndZoom(markersCreateArea, out Vector2 center, out int zoom);

        // Edit area values
        _areaToEdit.title = _areaTitle;
        _areaToEdit.titleEnglish = _areaTitleEnglish;
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
            AppManager.Instance.serverManager.timeRemaining = 0f;
        }
    }

    public void DeleteArea(int _selectAreaObjectIndex)
    {
        // Delete area locally and reload areas
        cArea areaSelected = areas[_selectAreaObjectIndex];
        cArea.Delete(areaSelected.local_area_id);
        areas = cArea.LoadAreas();
        //Debug.Log("Delete Area, local_area_id = " + areaSelected.local_area_id + "server_area_id = " + areaSelected.server_area_id);

        // DeleteTiles
        AppManager.Instance.serverManager.tileDownloader.DeleteTiles(areaSelected);

        // Delete Area from server
        if (areaSelected.server_area_id != -1)
        {
            //Debug.Log("AddIdToDelete, area's server id: " + areaSelected.server_area_id);
            cArea.AddIdToDelete(areaSelected.server_area_id);
            AppManager.Instance.serverManager.postUserData = true;
            AppManager.Instance.serverManager.timeRemaining = 0f;
        }
    }

    public void DeletePath(int _selectPathObjectIndex)
    {
        cPath pathSelected = currentArea.paths[_selectPathObjectIndex];
        //Debug.Log(pathSelected.title);
        cPath.Delete(pathSelected);
        //Debug.Log("Deleted Path, local_path_id = " + pathSelected.local_area_id + "server_path_id = " + pathSelected.server_area_id);

        // Delete Path from server
        if (pathSelected.server_path_id != -1)
        {
            //Debug.Log("AddIdToDelete, path's server id: " + pathSelected.server_area_id);
            cPath.AddIdToDelete(pathSelected.server_path_id);
            AppManager.Instance.serverManager.postUserData = true;
            AppManager.Instance.serverManager.timeRemaining = 0f;
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

        //Internet Events
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
        AppManager.Instance.uIManager.DisplayPathsScreen();
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
                GeneralAreaCreation(null);
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
        if (currentArea != null)
        {
            if (!IsWithinConstraints() /*&& !isShown*/)
            {
                if (!isShown)
                {
                    AppManager.Instance.uIManager.pnlWarningScreen.SetActive(true);
                    AppManager.Instance.uIManager.SetWarningTxtOnCheckUserPosition();
                }

                if(isRecordingPath || isPausePath)
                {
                    AppManager.Instance.uIManager.pnlWarningSavePathScreen.SetActive(true);
                    
                    AppManager.Instance.uIManager.btnContinue.interactable = false;
                    AppManager.Instance.uIManager.IsInRecordingPath(false);
                }

                AppManager.Instance.uIManager.btnAddNewPath.interactable = false;
                isShown = true;
                //Debug.Log("Out of area true");
            }
            else
            {   
                AppManager.Instance.uIManager.pnlWarningScreen.SetActive(false);
                AppManager.Instance.uIManager.btnAddNewPath.interactable = true;
                AppManager.Instance.uIManager.btnContinue.interactable = true;
                isShown = false;
                //Debug.Log("Out of area false");
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
        // Set user marker to user's location
        SetMarkerOnUserPosition(position);

        // Check constraints
        CheckUserPosition();

        if (isRecordingPath && !isPausePath && IsWithinConstraints() && AppManager.Instance.androidManager.HasGPS())
        {
            // Set map view to to user's location with max zoom
            OnlineMaps.instance.SetPositionAndZoom(position.x, position.y, OnlineMaps.MAXZOOM);

            float distance = OnlineMapsUtils.DistanceBetweenPoints(position, previousPosition).magnitude;
            
            if (distance < OnlineMapsLocationService.instance.updateDistance / 500f) //1000f
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
                //marker.texture = markerForDurationTexture;
                marker.scale = 0.1f;

                // Create marker ontop of marker to display duration
                currentMarkerForDuration = OnlineMapsMarkerManager.CreateItem(position, label);
                currentMarkerForDuration.SetDraggable(false);
                currentMarkerForDuration.align = OnlineMapsAlign.Center;
                currentMarkerForDuration.texture = markerForDurationTexture;
                currentMarkerForDuration.scale = MARKERFORDURATION_SCALE;
                
                // Get new time duration
                //Debug.Log("DateTime.Now.TimeOfDay = " + DateTime.Now.TimeOfDay);
                //Debug.Log("previousPointTime.TimeOfDay = " + previousPointTime.TimeOfDay);
                TimeSpan timeDuration = (DateTime.Now.TimeOfDay - previousPointTime.TimeOfDay);
                previousPointTime = DateTime.Now;

                // Set duration of previous path point
                if (currentPath.pathPoints.Count > 0)
                {
                    float totalDuration = (float)timeDuration.TotalSeconds + pausedDuration;
                    currentPath.pathPoints[currentPath.pathPoints.Count - 1].duration = totalDuration - pausedDuration;
                    //Debug.Log("totalDuration = " + totalDuration);
                    //Debug.Log("pausedDuration = " + pausedDuration);
                    //Debug.Log("duration = " + currentPath.pathPoints[currentPath.pathPoints.Count - 1].duration);
                }

                // Reset pausedDuration
                pausedDuration = 0f;

                // Creates and Adds a new PathPoint
                currentPath.pathPoints.Add(new cPathPoint(currentPath.local_path_id, currentPath.pathPoints.Count, position, 0f));

                // Creates a line
                markerListCurrPath.Add(marker);
                OnlineMapsDrawingElementManager.RemoveAllItems(e => e != polygon);
                OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingLine(markerListCurrPath.Select(m => m.position).ToArray(), Color.red, 3)); //OnlineMapsMarkerManager.instance.Select(m => m.position).ToArray(), Color.red, 3) // Average human walk speed is 1.4m/s
                
                if (markerListCurrPath.Count > 1)
                {
                    for (int i = 1; i < markerListCurrPath.Count; i++)
                    {
                        OnlineMapsDrawingElementManager.AddItem(CreateTriangle(markerListCurrPath[i-1].position, markerListCurrPath[i].position));
                    }
                }

                //OnlineMapsUtils.
                // Set user marker on top
                CreateUserMarker();

                previousPosition = position;
                OnlineMaps.instance.Redraw();
            }
            isShown = false;
        }
        else if ((isRecordingPath || isPausePath) && !IsWithinConstraints() && !isShown)
        {
            /*AppManager.Instance.uIManager.pnlWarningScreen.SetActive(true);
            AppManager.Instance.uIManager.txtWarning.text = "You are out of the Active Area";
            Debug.Log("Out of area and recording");*/
            AppManager.Instance.uIManager.pnlWarningSavePathScreen.SetActive(true);
            AppManager.Instance.uIManager.btnContinue.interactable = false;
            isShown = true;
        }
        else if(!isRecordingPath && isPausePath && IsWithinConstraints())
        {
            AppManager.Instance.uIManager.pnlWarningScreen.SetActive(false);
            AppManager.Instance.uIManager.pnlWarningSavePathScreen.SetActive(true);
            isShown = false;
        }
        else if (isRecordingPath && !isPausePath && IsWithinConstraints())
        {
            AppManager.Instance.uIManager.pnlWarningScreen.SetActive(false);
            AppManager.Instance.uIManager.pnlWarningSavePathScreen.SetActive(true);
            isShown = false;
        }
        else if (!isRecordingPath && !isPausePath && IsWithinConstraints())
        {
            AppManager.Instance.uIManager.pnlWarningScreen.SetActive(false);
            AppManager.Instance.uIManager.pnlWarningSavePathScreen.SetActive(false);
            isShown = false;
        }
    }

    public void StartRecordingPath()
    {
        isRecordingPath = true;
        isPausePath = false;
        previousPosition = OnlineMapsLocationService.instance.position;

        // Center View
        OnlineMaps.instance.SetPositionAndZoom(previousPosition.x, previousPosition.y, OnlineMaps.MAXZOOM);
        //OnlineMapsLocationService.instance.UpdatePosition();

        // Create a new marker at the starting position
        string label = "Point_0_" + DateTime.Now.TimeOfDay;
        OnlineMapsMarker marker = OnlineMapsMarkerManager.CreateItem(previousPosition, label);
        marker.align = OnlineMapsAlign.Center;
        //marker.texture = markerForDurationTexture;
        marker.scale = 0.1f;

        // Create marker ontop of marker to display duration
        currentMarkerForDuration = OnlineMapsMarkerManager.CreateItem(previousPosition, label);
        currentMarkerForDuration.SetDraggable(false);
        currentMarkerForDuration.align = OnlineMapsAlign.Center;
        currentMarkerForDuration.texture = markerForDurationTexture;
        currentMarkerForDuration.scale = MARKERFORDURATION_SCALE;

        // Clear and Add new marker to markerListCurrPath
        markerListCurrPath.Clear();
        markerListCurrPath.Add(marker);

        previousPointTime = DateTime.Now;
        
        currentPath = new cPath(currentArea.local_area_id);
        currentPath.pathPoints.Add(new cPathPoint(currentPath.local_path_id, currentPath.pathPoints.Count, previousPosition, 0f));

        // Set user marker on top
        CreateUserMarker();
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
        //Debug.Log("DateTime.Now.TimeOfDay = " + DateTime.Now.TimeOfDay);
        //Debug.Log("previousPointTime.TimeOfDay = " + previousPointTime.TimeOfDay);
        TimeSpan timeDuration = (DateTime.Now.TimeOfDay - previousPointTime.TimeOfDay);

        // Set duration of the last path point
        if (currentPath.pathPoints.Count > 0)
        {
            float totalDuration = (float)timeDuration.TotalSeconds + pausedDuration;
            currentPath.pathPoints[currentPath.pathPoints.Count - 1].duration = totalDuration - pausedDuration;
            //Debug.Log("totalDuration = " + totalDuration);
            //Debug.Log("pausedDuration = " + pausedDuration);
            //Debug.Log("duration = " + currentPath.pathPoints[currentPath.pathPoints.Count - 1].duration);
        }

        // Debug path
        /*foreach (cPathPoint point in currentPath.pathPoints)
        {
            Debug.Log("point " + point.index + " duration = " + point.duration);
        }*/

        // Save
        cPath.Save(currentPath);

        // Set current path of questionnaire
        AppManager.Instance.questionnaireManager.currentPath = currentPath;

        // Reset current path
        currentPath = null;

        // Upload user data to server
        AppManager.Instance.serverManager.postUserData = true;
        AppManager.Instance.serverManager.timeRemaining = 0f;
    }

    public void DisplayPath(cPath _pathToDisplay)
    {
        // Remove user marker
        if (userMarker != null)
            OnlineMapsMarkerManager.instance.items.Remove(userMarker);

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
            marker.scale = MARKER_SCALE;

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
            markerForDuration.scale = MARKERFORDURATION_SCALE - (MARKERFORDURATION_SCALE * Mathf.Clamp(pathPoint.duration / MAX_DURATION, 0f, 1f));
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

        // Draw triangles
        if (markerListCurrPath.Count > 1)
        {
            for (int i = 1; i < markerListCurrPath.Count; i++)
            {
                OnlineMapsDrawingElementManager.AddItem(CreateTriangle(markerListCurrPath[i - 1].position, markerListCurrPath[i].position));
            }
        }

        OnlineMaps.instance.Redraw();

        // Set map view on path
        if (sortedList.Count > 0)
        {
            OnlineMaps.instance.SetPositionAndZoom(sortedList[0].position.x, sortedList[0].position.y, 20);
        }
    }

    public List<cPath> GetPaths()
    {
        List<cPath> loadedPaths = cPath.LoadPathsOfArea(currentArea.local_area_id);
        currentArea.paths = loadedPaths.OrderBy(path => path.date).ToList();
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
        userMarker.position = position;
        OnlineMaps.instance.Redraw();
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
        OnlineMapsDrawingPoly onlineMapsDrawingPoly = new OnlineMapsDrawingPoly(_arrayOfPoints, new Color(1, 1, 1, 0.3f), 0f, new Color(1, 1, 1, 0.3f));
        //OnlineMapsDrawingPoly onlineMapsDrawingPoly = new OnlineMapsDrawingPoly(_arrayOfPoints, Color.black, borderWidth, new Color(1, 1, 1, 0.3f));
        OnlineMapsDrawingElementManager.AddItem(onlineMapsDrawingPoly);
       
        return onlineMapsDrawingPoly;
    }

    private OnlineMapsDrawingPoly CreateTriangle(Vector2 _startPos, Vector2 _endPos)
    {
        Vector2[] arrayOfPoints = new Vector2[3];

        // Get points
        Vector2 direction = (_endPos - _startPos).normalized;
        Vector2 midPoint = (_startPos + _endPos) * 0.5f;
        float radius = 0.000005f;  //OnlineMapsUtils.DistanceBetweenPoints(_startPos, _endPos).magnitude;
        /*int angle = 120;
        float xB = (float)(radius * 10 * Math.Cos(angle * Mathf.Deg2Rad)); // 3
        float yB = (float)(radius * 10 * Math.Sin(angle * Mathf.Deg2Rad));
        float xC = (float)(radius * 10 * Math.Cos(-angle * Mathf.Deg2Rad));
        float yC = (float)(radius * 10 * Math.Sin(-angle * Mathf.Deg2Rad));*/
        //float angleDir = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

        Vector2 directionAToB = DirFromAngle(90, direction * 3);
        Vector2 directionAToC = DirFromAngle(-90, direction * 3);

        /*Debug.Log("direction = " + direction);
        Debug.Log("directionAToB = " + directionAToB);
        Debug.Log("directionAToC = " + directionAToC);*/

        Vector2 pointA = midPoint + (direction * radius);
        Vector2 pointB = midPoint + (directionAToB * radius); //new Vector2(pointA.x + xB, pointA.y + yB);
        Vector2 pointC = midPoint + (directionAToC * radius); //new Vector2(pointA.x + xC, pointA.y + yC);

        arrayOfPoints[0] = pointA;
        arrayOfPoints[1] = pointB;
        arrayOfPoints[2] = pointC;

        OnlineMapsDrawingPoly triangle = new OnlineMapsDrawingPoly(arrayOfPoints, Color.white, 1f, Color.white);

        return triangle;
    }

    /*private Vector2 DirFromAngle(float _angleInDegrees, Transform _origin)
    {
        if (_origin != null)
            _angleInDegrees += _origin.eulerAngles.y;

        return new Vector2(Mathf.Sin(_angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(_angleInDegrees * Mathf.Deg2Rad));
    }*/

    private Vector2 DirFromAngle(float _angleInDegrees, Vector2 _direction)
    {
        return new Vector2(_direction.x * Mathf.Cos(_angleInDegrees) - _direction.y * Mathf.Sin(_angleInDegrees), _direction.x * Mathf.Sin(_angleInDegrees) + _direction.y * Mathf.Cos(_angleInDegrees));
    }

    public void DisplayArea(cArea _areaToDisplay)
    {
        // Remove Paths
        RemoveMarkersAndLine(); // Does this create any problems?

        // Set user marker on top
        CreateUserMarker();

        /*Vector2[] points = new Vector2[4];

        points[0] = _areaToDisplay.areaConstraintsMin;
        points[1] = new Vector2(_areaToDisplay.areaConstraintsMin.x, _areaToDisplay.areaConstraintsMax.y);
        points[2] = _areaToDisplay.areaConstraintsMax;
        points[3] = new Vector2(_areaToDisplay.areaConstraintsMax.x, _areaToDisplay.areaConstraintsMin.y);*/

        Vector2[] points = new Vector2[10];

        /*points[0] = _areaToDisplay.areaConstraintsMin;
        points[1] = new Vector2(_areaToDisplay.areaConstraintsMin.x, _areaToDisplay.areaConstraintsMax.y);
        points[2] = _areaToDisplay.areaConstraintsMax;
        points[3] = new Vector2(_areaToDisplay.areaConstraintsMax.x, _areaToDisplay.areaConstraintsMin.y);*/

        points[0] = _areaToDisplay.viewConstraintsMin;
        points[1] = new Vector2(_areaToDisplay.viewConstraintsMin.x, _areaToDisplay.viewConstraintsMax.y);
        points[2] = _areaToDisplay.viewConstraintsMax;
        points[3] = new Vector2(_areaToDisplay.viewConstraintsMax.x, _areaToDisplay.viewConstraintsMin.y);
        points[4] = _areaToDisplay.viewConstraintsMin;
        points[5] = _areaToDisplay.areaConstraintsMin;
        points[6] = new Vector2(_areaToDisplay.areaConstraintsMin.x, _areaToDisplay.areaConstraintsMax.y);
        points[7] = _areaToDisplay.areaConstraintsMax;
        points[8] = new Vector2(_areaToDisplay.areaConstraintsMax.x, _areaToDisplay.areaConstraintsMin.y);
        points[9] = _areaToDisplay.areaConstraintsMin;

        if (polygon != null)
            OnlineMapsDrawingElementManager.RemoveItem(polygon);
        
        polygon = CreatePolygon(points); // OnlineMapsDrawingPoly polygonToDisplay = 
    }

    public void StartEditArea(cArea _areaToEdit)
    {
        // Set current area
        currentArea = _areaToEdit;

        // Remove Polygon if it exists
        if (polygon != null)
            OnlineMapsDrawingElementManager.RemoveItem(polygon);

        // Recreate area to edit
        GeneralAreaCreation(_areaToEdit);

        // Center map view to area and reset view constraints
        ResetMapConstraints();
        OnlineMaps.instance.SetPositionAndZoom(_areaToEdit.position.x, _areaToEdit.position.y, _areaToEdit.zoom);

        // Start editing
        editArea = true;
    }

    private void GeneralAreaCreation(cArea _areaToRecreate)
    {
        Vector2 centerPosition = _areaToRecreate == null ? OnlineMaps.instance.position : _areaToRecreate.position;

        // Calculate polygon positions
        Vector2 bottomLeftPosition = _areaToRecreate == null ? new Vector2((float)(OnlineMaps.instance.bounds.left + centerPosition.x) / 2, (float)(OnlineMaps.instance.bounds.bottom + centerPosition.y) / 2) : _areaToRecreate.areaConstraintsMin;
        Vector2 topLeftposition = _areaToRecreate == null ? new Vector2((float)(OnlineMaps.instance.bounds.left + centerPosition.x) / 2, (float)(centerPosition.y + OnlineMaps.instance.bounds.top) / 2) : new Vector2(_areaToRecreate.areaConstraintsMin.x, _areaToRecreate.areaConstraintsMax.y);
        Vector2 topRightPosition = _areaToRecreate == null ? new Vector2((float)(centerPosition.x + OnlineMaps.instance.bounds.right) / 2, (float)(centerPosition.y + OnlineMaps.instance.bounds.top) / 2) : _areaToRecreate.areaConstraintsMax;
        Vector2 bottomRightPosition = _areaToRecreate == null ? new Vector2((float)(centerPosition.x + OnlineMaps.instance.bounds.right) / 2, (float)(OnlineMaps.instance.bounds.bottom + centerPosition.y) / 2) : new Vector2(_areaToRecreate.areaConstraintsMax.x, _areaToRecreate.areaConstraintsMin.y);

        // Create two markers on the specified coordinates.
        //OnlineMapsMarker testM = OnlineMapsMarkerManager.CreateItem(topLeftposition, markerCreateAreaTexture, "topLeft");
        OnlineMapsMarker markerMin = OnlineMapsMarkerManager.CreateItem(bottomLeftPosition, markerCreateAreaTextureMin, "Marker Min");
        markerMin.scale = AREA_MARKER_SCALE;
        markerMin.OnPress += OnMarkerLongPress;
        //markerMin.SetDraggable();
        markerMin.align = OnlineMapsAlign.Center;// so the graphic to be aligned correctly with the rectangle

        //OnlineMapsMarker testM2 = OnlineMapsMarkerManager.CreateItem(bottomRightPosition, markerCreateAreaTexture, "bottomRight");
        OnlineMapsMarker markerMax = OnlineMapsMarkerManager.CreateItem(topRightPosition, markerCreateAreaTextureMax, "Marker Max");
        markerMax.scale = AREA_MARKER_SCALE;
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

    private void CreateUserMarker()
    {
        // Remove Marker
        if (userMarker != null)
            OnlineMapsMarkerManager.instance.items.Remove(userMarker);

        // Create Marker
        userMarker = OnlineMapsMarkerManager.CreateItem(new Vector2(OnlineMapsLocationService.instance.position.x, OnlineMapsLocationService.instance.position.y), AppManager.Instance.uIManager.userMarker, "User");
        userMarker.SetDraggable(false);
        userMarker.scale = 0.15f; //0.3f;
    }
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
            new cArea(-1, 0, "Μεσσήνη", "Messene", new Vector2(21.9202085525009f, 37.17642261183837f), 17, new Vector2(21.9160667457503f, 37.1700252387224f), new Vector2(21.9227518498302f, 37.178659594564f), new Vector2(21.9160667457503f, 37.1700252387224f), new Vector2(21.9227518498302f, 37.178659594564f)),
            new cArea(-1, 1, "Κνωσός", "Knosos", new Vector2(25.16310005634713f, 35.29800050616538f), 19, new Vector2(25.1616718900387f, 35.2958874528396f), new Vector2(25.1645352578472f, 35.3000733065711f), new Vector2(25.1616718900387f, 35.2958874528396f), new Vector2(25.1645352578472f, 35.3000733065711f)),
            new cArea(-1, 2, "Σαρρή", "Sarri", new Vector2(23.724021164280998f, 37.979955135461715f), 19, new Vector2(23.72385281512933f, 37.97881236959543f), new Vector2(23.725090676541246f, 37.9802439464203f), new Vector2(23.72385281512933f, 37.97881236959543f), new Vector2(23.725090676541246f, 37.9802439464203f))
        };

        //DisplayAreaDebug(areasFromDatabase[0]);
        //DisplayAreaDebug(areasFromDatabase[1]);
        //DisplayAreaDebug(areasFromDatabase[2]);

        return areasFromDatabase;
    }
    #endregion
    #endregion
}

