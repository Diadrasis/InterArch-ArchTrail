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
    public bool isRecordingPath, isMovement;

    // Create a list of markers to draw the path lines
    [HideInInspector]
    public List<OnlineMapsMarker> markerListCurrPath = new List<OnlineMapsMarker>();

    //createMarker on user position
    public OnlineMapsMarker userMarker;

    private float angle = 0.5f;
    public float time = 10;
    // Move direction
    private int direction = 1, moveZoom;

    //maybe can be deleted later, for now testing purposes
    private Vector2 fromPosition, toPosition;
    private double fromTileX, fromTileY, toTileX, toTileY;

    // Create Area
    public Texture2D markerCreateAreaTexture;
    public float borderWidth = 1;
    private bool createArea = false;
    OnlineMapsMarker[] markersCreateArea = new OnlineMapsMarker[2];
    Vector2[] positionsCreateArea = new Vector2[4];
    private OnlineMapsDrawingPoly polygon;
    private static readonly int DEFAULT_MARKER_SCALE = 2;

    
    #endregion

    #region Unity Functions
    private void Awake()
    {
        //List<cArea> areasToTestSave = GetTestAreas();
        //cArea.SaveAreas(areasToTestSave);
        areas = new List<cArea>();
        areas = cArea.LoadAreas();

        //cArea.PrintData("/areas/area/id"); // /areas/area[title='Sarri']

        //Debug.Log(cArea.GetInfoFromXML("/areas/Μεσσήνη/title"));

        

        //cPath.Delete(new cPath(1, 0));
    }

    private void Start()
    {
        SubscribeToEvents();
        fromPosition = OnlineMaps.instance.position;
        toPosition = OnlineMapsLocationService.instance.position;
        isRecordingPath = false;

        userMarker = OnlineMapsMarkerManager.CreateItem(new Vector2(0, 0), AppManager.Instance.uIManager.userMarker, "User");
        userMarker.SetDraggable(false);
        // Test
        //List<cPath> pathsToTest = GetTestPaths();
        //DisplayPath(pathsToTest[0]);
    }

    private void Update()
    {
        // Checks the position of the markers.
        if (createArea && polygon != null)
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
    }

    private void OnDestroy()
    {
        //PlayerPrefs.DeleteAll(); // TODO: REMOVE!!!
    }
    private void OnDisable()
    {
        AppManager.Instance.serverManager.OnCheckInternetCheckComplete -= AppManager.Instance.androidManager.OnCheckInternetCheckComplete;
    }
    #endregion

    #region Methods
    private List<cArea> GetTestAreas()
    {
        List<cArea> areasFromDatabase = new List<cArea>()
        {
            new cArea(0, "Μεσσήνη", new Vector2(21.9202085525009f, 37.17642261183837f), 17, new Vector2(21.9160667457503f, 37.1700252387224f), new Vector2(21.9227518498302f, 37.178659594564f), new Vector2(21.9160667457503f, 37.1700252387224f), new Vector2(21.9227518498302f, 37.178659594564f)),
            new cArea(1, "Κνωσός", new Vector2(25.16310005634713f, 35.29800050616538f), 19, new Vector2(25.1616718900387f, 35.2958874528396f), new Vector2(25.1645352578472f, 35.3000733065711f), new Vector2(25.1616718900387f, 35.2958874528396f), new Vector2(25.1645352578472f, 35.3000733065711f)),
            new cArea(2, "Σαρρή", new Vector2(23.724021164280998f, 37.979955135461715f), 19, new Vector2(23.72385281512933f, 37.97881236959543f), new Vector2(23.725090676541246f, 37.9802439464203f), new Vector2(23.72385281512933f, 37.97881236959543f), new Vector2(23.725090676541246f, 37.9802439464203f))
        };

        //DisplayAreaDebug(areasFromDatabase[0]);
        //DisplayAreaDebug(areasFromDatabase[1]);
        //DisplayAreaDebug(areasFromDatabase[2]);

        return areasFromDatabase;
    }

    private List<cPath> GetTestPaths()
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
    }

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
        Debug.Log("Id = " + _area.Id);

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
                return path;
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
            cArea.Save(areaToSave);
            areas = cArea.LoadAreas();

            // Upload user data to server
            cArea.Upload(areaToSave);
            //AppManager.Instance.serverManager.postUserData = true;
        }
    }

    public void DeleteArea(int _selectAreaObjectIndex)
    {
        cArea areaSelected = areas[_selectAreaObjectIndex];
        cArea.Delete(areaSelected.Id);
        areas = cArea.LoadAreas();
    }

    public void DeletePath(int _selectPathObjectIndex)
    {
        cPath pathSelected = currentArea.paths[_selectPathObjectIndex];
        Debug.Log(pathSelected.title);
        cPath.Delete(pathSelected);
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
        AppManager.Instance.serverManager.OnCheckInternetCheckComplete += AppManager.Instance.androidManager.OnCheckInternetCheckComplete;
    }

    private void OnMapClick()
    {
        // Create a new area
        if (createArea)
        {
            // if there is no polygon
            if (polygon == null)
            {
                Vector2 centerPosition = OnlineMaps.instance.position;

                // Calculate polygon positions
                Vector2 bottomLeftPosition = new Vector2((float)(OnlineMaps.instance.bounds.left + centerPosition.x) / 2, (float)(OnlineMaps.instance.bounds.bottom + centerPosition.y) / 2);
                Vector2 topLeftposition = new Vector2((float)(OnlineMaps.instance.bounds.left + centerPosition.x) / 2, (float)(centerPosition.y + OnlineMaps.instance.bounds.top) / 2);
                Vector2 topRightPosition = new Vector2((float)(centerPosition.x + OnlineMaps.instance.bounds.right) / 2, (float)(centerPosition.y + OnlineMaps.instance.bounds.top) / 2);
                Vector2 bottomRightPosition = new Vector2((float)(centerPosition.x + OnlineMaps.instance.bounds.right) / 2, (float)(OnlineMaps.instance.bounds.bottom + centerPosition.y) / 2);

                // Create two markers on the specified coordinates.
                OnlineMapsMarker markerMin = OnlineMapsMarkerManager.CreateItem(bottomLeftPosition, markerCreateAreaTexture, "Marker Min");
                markerMin.scale = DEFAULT_MARKER_SCALE;
                markerMin.SetDraggable(true);
                //OnlineMapsMarker testM = OnlineMapsMarkerManager.CreateItem(topLeftposition, markerCreateAreaTexture, "topLeft");
                OnlineMapsMarker markerMax = OnlineMapsMarkerManager.CreateItem(topRightPosition, markerCreateAreaTexture, "Marker Max");
                markerMax.scale = DEFAULT_MARKER_SCALE;
                markerMax.SetDraggable(true);
                //OnlineMapsMarker testM2 = OnlineMapsMarkerManager.CreateItem(bottomRightPosition, markerCreateAreaTexture, "bottomRight");

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

    public void OnLocationChanged(Vector2 position)
    {
        //AppManager.Instance.uIManager.infoText.text = "Location changed to: " + position;

        CreateMarkerOnUserPosition(position);
        CheckUserPosition();
        
        if (isRecordingPath && IsWithinConstraints())
        {
            //AppManager.Instance.uIManager.infoText.text = "Distance changed to: " + distance;
            OnlineMapsLocationService.instance.UpdatePosition();

            float distance = OnlineMapsUtils.DistanceBetweenPoints(position, previousPosition).magnitude;
            //Debug.Log("Distance from previous marker: " + distance);
            if (distance < OnlineMapsLocationService.instance.updateDistance / 1000f)
            {
                //Debug.Log("Minimum distance needed to create marker");
                //OnlineMaps.instance.Redraw();
                return;
            }
            else
            {
                // Creates a new marker
                string label = "Point_" + currentPath.pathPoints.Count + "_" + DateTime.Now.TimeOfDay;
                OnlineMapsMarker marker = OnlineMapsMarkerManager.CreateItem(position,label);
                marker.SetDraggable(false);
                // Creates and Adds a new PathPoint
                currentPath.pathPoints.Add(new cPathPoint(currentPath.Id, currentPath.pathPoints.Count, position, DateTime.Now.TimeOfDay));
                
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
        else if (isRecordingPath && !IsWithinConstraints())
        {
            AppManager.Instance.uIManager.pnlWarningScreen.SetActive(false);
            AppManager.Instance.uIManager.pnlWarningSavePathScreen.SetActive(true);
            isRecordingPath = false;
            //Debug.Log("is recording and not in constraints");
        }
        else if(!isRecordingPath && IsWithinConstraints())
        {
            AppManager.Instance.uIManager.pnlWarningScreen.SetActive(false);
            AppManager.Instance.uIManager.pnlWarningSavePathScreen.SetActive(false);
        }
        else
        {
            AppManager.Instance.uIManager.pnlWarningScreen.SetActive(true);
            //Debug.Log("is not recording and not in constraints");
        }
        //marker.OnPositionChanged += OnMarkerPositionChange;
    }

    public void StartRecordingPath()
    {
        isRecordingPath = true;
        previousPosition = OnlineMapsLocationService.instance.position;

        // Center View 
        OnlineMapsLocationService.instance.UpdatePosition();

        // Create a new marker at the starting position
        OnlineMapsMarker marker = OnlineMapsMarkerManager.CreateItem(previousPosition, "Point_0_" + DateTime.Now.TimeOfDay);

        // Clear and Add new marker to markerListCurrPath
        markerListCurrPath.Clear();
        markerListCurrPath.Add(marker);

        currentPath = new cPath(currentArea.Id);
        currentPath.pathPoints.Add(new cPathPoint(currentPath.Id, currentPath.pathPoints.Count, previousPosition, DateTime.Now.TimeOfDay));
        
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

    public void StopRecordingPath()
    {
        isRecordingPath = false;
    }

    public void SavePath()
    {
        /*Debug.Log("currentPath areaId = " + currentPath.areaId);
        Debug.Log("currentPath Id = " + currentPath.Id);
        Debug.Log("currentPath title = " + currentPath.title);
        Debug.Log("currentPath pathPoints.Count = " + currentPath.pathPoints.Count);*/
        cPath.Save(currentPath);
        currentPath = null;

        // Upload user data to server
        AppManager.Instance.serverManager.postUserData = true;
    }

    public void DisplayPath(cPath _pathToDisplay)
    {
        // Sort points by index
        List<cPathPoint> sortedList = _pathToDisplay.pathPoints.OrderBy(point => point.index).ToList();

        // Create a list of markers to draw the path lines
        List<OnlineMapsMarker> markerListOfCurrentPath = new List<OnlineMapsMarker>();

        for (int i = 0; i < sortedList.Count; i++)
        {
            // Get cPathPoint
            cPathPoint pathPoint = sortedList[i];

            // Create marker
            string label = "Path_" + _pathToDisplay.Id + "_Point_" + pathPoint.index + "_" + pathPoint.time.ToString();
            OnlineMapsMarker marker = OnlineMapsMarkerManager.CreateItem(pathPoint.position, label);
            marker.SetDraggable(false);

            // Add marker to current path marker list
            markerListOfCurrentPath.Add(marker);

            // Set marker scale
            if (pathPoint.index != 0)
            {
                cPathPoint previousPathPoint = sortedList[i - 1];
                TimeSpan timeDuration= (pathPoint.time - previousPathPoint.time);
                marker.scale = 1f + 2f * Mathf.Clamp((float)timeDuration.TotalSeconds/600f, 0f, 1f); // 3 will be max value (timeDuration >= 600 sec(10min)), 1 is min value/default (timeDuration <= 7 sec)
                Debug.Log("Scale = " + marker.scale);
            }
        }

        // Draw lines
        OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingLine(markerListOfCurrentPath.Select(m => m.position).ToArray(), Color.red, 3));
        OnlineMaps.instance.Redraw();
    }

    public List<cPath> GetPaths()
    {
        currentArea.paths = cPath.LoadPathsOfArea(currentArea.Id);
        return currentArea.paths;
    }

    #region Marker
    private void OnMarkerPositionChange(OnlineMapsMarkerBase marker)
    {
        //Debug.Log("OnMarkerPositionChange "+marker.label);
        OnlineMapsMarkerManager.instance.RemoveAll();
        OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingLine(OnlineMapsMarkerManager.instance.Select(m => m.position).ToArray(), Color.red, 3));
        //MarkersManager.CreateLineFromList(OnlineMaps.instance.markers.ToList(), Color.red, 3f);
        OnlineMaps.instance.Redraw();
    }

    public void RemoveMarkersAndLine()
    {
        OnlineMapsMarkerManager.RemoveAllItems(m => m != userMarker);
        OnlineMapsDrawingElementManager.RemoveAllItems(e => e != polygon);
        OnlineMaps.instance.Redraw();
    }

    private void CreateMarkerOnUserPosition(Vector2 position)
    {
        //CheckMyLocation();
        userMarker.position = position;
        OnlineMaps.instance.Redraw();
        SetMapViewToLocation();
        userMarker.scale = 2;
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


    #endregion
}

