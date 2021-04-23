using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    #region Variables
    [HideInInspector]
    public List<cArea> areas = new List<cArea>();
    [HideInInspector]
    public cArea currentArea;

    public static readonly int DEFAULT_ZOOM = 19;
    public static readonly float DEFAULT_POSITION_OFFSET = 0.00414180675f;
    [HideInInspector]
    public Vector2 previousPosition = Vector2.zero;
    
    [HideInInspector]
    public float minDistanceToPutNewMarker = 10f / 1000f;
    [HideInInspector]
    public bool isRecording, isMovement;
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
        List<cArea> areasToTestSave = GetTestAreas();
        cArea.SaveAreas(areasToTestSave);

        areas = cArea.LoadAreas();

        Debug.Log(cArea.GetInfoFromXML("/areas/Μεσσήνη/title"));

        /*List<cPath> pathsToTest = GetTestPaths();
        //cPath.SavePaths(pathsToTest);

        foreach (cPath path in pathsToTest)
        {
            cPath.Save(path);
        }

        cPath.Delete(new cPath("Μεσσήνη", "path_0"));

        foreach (cPath path in cPath.LoadPaths())
        {
            Debug.Log(path.areaTitle);
            Debug.Log(path.title);
        }*/
    }

    private void Start()
    {
        SubscribeToEvents();
        fromPosition = OnlineMaps.instance.position;
        toPosition = OnlineMapsLocationService.instance.position;
        isRecording = false;
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
        PlayerPrefs.DeleteAll(); // TODO: REMOVE!!!
    }
    #endregion

    #region Methods
    private List<cArea> GetTestAreas()
    {
        List<cArea> areasFromDatabase = new List<cArea>()
        {
            new cArea("Μεσσήνη", new Vector2(21.9202085525009f, 37.17642261183837f), 17, new Vector4(21.9160667457503f, 37.1700252387224f , 21.9227518498302f, 37.178659594564f)),
            new cArea("Κνωσός", new Vector2(25.16310005634713f, 35.29800050616538f), 19, new Vector4(25.1616718900387f, 35.2958874528396f , 25.1645352578472f, 35.3000733065711f)),
            //new cArea("Σαρρή", new Vector2(23.724021164280998f, 37.979955135461715f),19, new Vector4(23.724021164280998f - 1, 37.97881236959543f , 23.725090676541246f, 37.9802439464203f))
            new cArea("Σαρρή", new Vector2(23.724021164280998f, 37.979955135461715f),19, new Vector4(23.72385281512933f, 37.97881236959543f , 23.725090676541246f, 37.9802439464203f))
        };

        //DisplayAreaDebug(0);
        //DisplayAreaDebug(1);

        return areasFromDatabase;
    }

    private List<cPath> GetTestPaths()
    {
        List<cPath> pathsToTest = new List<cPath>()
        {
            new cPath("Μεσσήνη", "path_0"),
            new cPath("Μεσσήνη", "path_1"),
            new cPath("Κνωσός", "path_0")
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

    private void DisplayAreaDebug(int _index)
    {
        // Title
        Debug.Log("Title = " + areas[_index].title);

        // Position
        Debug.Log("Longitude = " + areas[_index].position.x);
        Debug.Log("Latitude = " + areas[_index].position.y);

        // Zoom
        Debug.Log("Zoom = " + areas[_index].zoom);

        // Constraints
        Debug.Log("minLongitude = " + areas[_index].constraints.x);
        Debug.Log("minLatitude = " + areas[_index].constraints.y);
        Debug.Log("maxLongitude = " + areas[_index].constraints.z);
        Debug.Log("maxLatitude = " + areas[_index].constraints.w);
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

    public void SaveArea(cArea _areaToSave)
    {
        if (!areas.Contains(_areaToSave))
        {
            cArea.Save(_areaToSave);
            areas = cArea.LoadAreas();
        }
    }

    public void DeleteArea(string _areaTitle)
    {
        //if (!areas.Contains(_areaToDelete))
        {
            cArea.Delete(_areaTitle);
            areas = cArea.LoadAreas();
            //Debug.Log(areas.Count);
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
                
            }
            else
            {
                
                AppManager.Instance.uIManager.pnlWarningScreen.SetActive(false);
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

        if (isRecording && AppManager.Instance.uIManager.imgRecord.GetBool("isPlaying")) // IsRecordingPath
        {
            //AppManager.Instance.uIManager.infoText.text = "Distance changed to: " + distance;
            OnlineMapsLocationService.instance.UpdatePosition();

            float distance = OnlineMapsUtils.DistanceBetweenPoints(position, previousPosition).magnitude;
            Debug.Log("Distance from previous marker: " + distance);
            if (distance < OnlineMapsLocationService.instance.updateDistance/1000f)
            {
                //OnlineMaps.instance.Redraw();
                return;
            }
            else
            {
                // Creates a new marker
                string label = "Marker " + (OnlineMapsMarkerManager.instance.Count + 1);
                OnlineMapsMarker marker = OnlineMapsMarkerManager.CreateItem(position, label);

                //marker.label = label;
                //marker.position = position;
                //OnlineMapsMarkerManager.AddItem(marker);

                markerListCurrPath.Add(marker); // TODO: 
                OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingLine(OnlineMapsMarkerManager.instance.Select(m => m.position).ToArray(), Color.red, 3));

                previousPosition = position;
                OnlineMaps.instance.Redraw();
            }
        }

        //marker.OnPositionChanged += OnMarkerPositionChange;
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
