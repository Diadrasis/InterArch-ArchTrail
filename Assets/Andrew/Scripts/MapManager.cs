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
    public static readonly float DEFAULT_POSITION_OFFSET = 0.00114180675f;//0.00414180675f; //the commented number is the correct one when build the app
    [HideInInspector]
    public Vector2 previousPosition = Vector2.zero;
    
    [HideInInspector]
    public float minDistanceToPutNewMarker = 10f / 1000f;
    [HideInInspector]
    public bool isRecordingPath, isMovement;
    [HideInInspector]
    public List<OnlineMapsMarker> markerListCurrPath = new List<OnlineMapsMarker>();

    //createMarker on user position and on the path after specific meters
    //OnlineMapsMarker marker = new OnlineMapsMarker();
    private float angle = 0.5f;
    public float time = 10;
    // Move direction
    private int direction = 1, moveZoom;

    //maybe can be deleted later, for now testing purposes
    private Vector2 fromPosition, toPosition;
    private double fromTileX, fromTileY, toTileX, toTileY;
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

        // Test
        //List<cPath> pathsToTest = GetTestPaths();
        //DisplayPath(pathsToTest[0]);
    }
    private void Update()
    {
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
        OnlineMaps.instance.projection.TileToCoordinates(px, py, moveZoom, out px, out py);
        OnlineMaps.instance.SetPosition(px, py);
    }
    private void OnDestroy()
    {
        //PlayerPrefs.DeleteAll(); // TODO: REMOVE!!!
    }
    #endregion

    #region Methods
    private List<cArea> GetTestAreas()
    {
        List<cArea> areasFromDatabase = new List<cArea>()
        {
            new cArea(0, "Μεσσήνη", new Vector2(21.9202085525009f, 37.17642261183837f), 17, new Vector4(21.9160667457503f, 37.1700252387224f , 21.9227518498302f, 37.178659594564f)),
            new cArea(1, "Κνωσός", new Vector2(25.16310005634713f, 35.29800050616538f), 19, new Vector4(25.1616718900387f, 35.2958874528396f , 25.1645352578472f, 35.3000733065711f)),
            new cArea(2, "Σαρρή", new Vector2(23.724021164280998f, 37.979955135461715f),19, new Vector4(23.72385281512933f, 37.97881236959543f , 23.725090676541246f, 37.9802439464203f))
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
        Debug.Log("minLongitude = " + _area.constraints.x);
        Debug.Log("minLatitude = " + _area.constraints.y);
        Debug.Log("maxLongitude = " + _area.constraints.z);
        Debug.Log("maxLatitude = " + _area.constraints.w);
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
        OnlineMaps.instance.positionRange = new OnlineMapsPositionRange(_areaToView.constraints.y, _areaToView.constraints.x, _areaToView.constraints.w, _areaToView.constraints.z);
        OnlineMaps.instance.zoomRange = new OnlineMapsRange(_areaToView.zoom, OnlineMaps.MAXZOOM);
        OnlineMaps.instance.SetPositionAndZoom(_areaToView.position.x, _areaToView.position.y, _areaToView.zoom);
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

    public void SaveArea(cArea _areaToSave)
    {
        //if (areas != null && !areas.Contains(_areaToSave))
        {
            cArea.Save(_areaToSave);
            areas = cArea.LoadAreas();
        }
    }

    public void DeleteArea(int _selectAreaObjectIndex)
    {
        cArea areaSelected = areas[_selectAreaObjectIndex];
        cArea.Delete(areaSelected.Id);
        areas = cArea.LoadAreas();
        //Debug.Log(areas.Count);
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
        OnlineMapsControlBase.instance.OnMapClick += AppManager.Instance.uIManager.OnMapClick;

        //Changed Location Event
        OnlineMapsLocationService.instance.OnLocationChanged += OnLocationChanged;
    }

    public void CheckUserPosition()
    {
        if(currentArea != null)
        {
            if (!IsWithinConstraints())
            {
                AppManager.Instance.uIManager.pnlWarningScreen.SetActive(true);
                AppManager.Instance.uIManager.btnAddNewRoute.interactable = false;
                
            }
            else
            {   
                AppManager.Instance.uIManager.pnlWarningScreen.SetActive(false);
                AppManager.Instance.uIManager.btnAddNewRoute.interactable = true;
            }
            
        }
    }
    
    private bool IsWithinConstraints()
    {
        if ((currentArea.constraints.x < OnlineMapsLocationService.instance.position.x) && (OnlineMapsLocationService.instance.position.x < currentArea.constraints.z)
            && (currentArea.constraints.y < OnlineMapsLocationService.instance.position.y) && (OnlineMapsLocationService.instance.position.y < currentArea.constraints.w))
        {
            return true;

        }
        return false;
    }

    
    //can be removed?
    public void CheckMyLocation()
    {
        Debug.Log("CheckMyLocation");
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
    }

    public void OnLocationChanged(Vector2 position)
    {
        //AppManager.Instance.uIManager.infoText.text = "Location changed to: " + position;

        //CheckMyLocation();

        if (isRecordingPath)
        {
            //AppManager.Instance.uIManager.infoText.text = "Distance changed to: " + distance;
            OnlineMapsLocationService.instance.UpdatePosition();

            float distance = OnlineMapsUtils.DistanceBetweenPoints(position, previousPosition).magnitude;
            Debug.Log("Distance from previous marker: " + distance);
            if (distance < OnlineMapsLocationService.instance.updateDistance/1000f)
            {
                Debug.Log("Minimum distnace needed to create marker");

                //
                
                //OnlineMaps.instance.Redraw();
                return;
            }
            else
            {
                //AppManager.Instance.uIManager.pnlWarningSaveRouteScreen.SetActive(false);
                // Creates a new marker
                string label = "Point_" + currentPath.pathPoints.Count + "_" + DateTime.Now.TimeOfDay;
                OnlineMapsMarker marker = OnlineMapsMarkerManager.CreateItem(position, label);

                // Creates and Adds a new PathPoint
                currentPath.pathPoints.Add(new cPathPoint(currentPath.Id, currentPath.pathPoints.Count, position, DateTime.Now.TimeOfDay));

                //marker.label = label;
                //marker.position = position;
                //OnlineMapsMarkerManager.AddItem(marker);

                // Creates a line
                markerListCurrPath.Add(marker);
                OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingLine(OnlineMapsMarkerManager.instance.Select(m => m.position).ToArray(), Color.red, 3));

                previousPosition = position;
                OnlineMaps.instance.Redraw();
            }
            if((position.x < currentArea.constraints.x+DEFAULT_POSITION_OFFSET) || (position.x > currentArea.constraints.z+DEFAULT_POSITION_OFFSET) 
                ||(position.y < currentArea.constraints.y + DEFAULT_POSITION_OFFSET) ||(position.y > currentArea.constraints.w + DEFAULT_POSITION_OFFSET))
            {
                AppManager.Instance.uIManager.pnlWarningSaveRouteScreen.SetActive(true);
                
            }
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
        OnlineMapsMarkerManager.CreateItem(previousPosition, "Point_0_" + DateTime.Now.TimeOfDay);

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
    }

    public void DisplayPath(cPath _pathToDisplay)
    {
        // Sort points by index
        List<cPathPoint> sortedList = _pathToDisplay.pathPoints.OrderBy(point => point.index).ToList();

        foreach (cPathPoint pathPoint in sortedList)
        {
            Debug.Log("Point " + pathPoint.index); // 0 -
            
            // Create Marker
            string label = "Path_" + _pathToDisplay.Id + "_Point_" + pathPoint.index + "_" + pathPoint.time.ToString();
            OnlineMapsMarker marker = OnlineMapsMarkerManager.CreateItem(pathPoint.position, label);

            // Creates a line
            markerListCurrPath.Add(marker);
            OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingLine(OnlineMapsMarkerManager.instance.Select(m => m.position).ToArray(), Color.red, 3));
        }

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
        OnlineMapsMarkerManager.instance.RemoveAll();
        OnlineMapsDrawingElementManager.RemoveAllItems();
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


    #endregion
}
