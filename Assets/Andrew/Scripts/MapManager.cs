﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    #region Variables
    private List<cArea> areasToTestSave = new List<cArea>(); // TEST

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
    public bool isDrawLineOnEveryPoint, isMovement;
    [HideInInspector]
    public List<OnlineMapsMarker> markerListCurrPath = new List<OnlineMapsMarker>();

    //createMarker on user position and on the path after specific meters
    OnlineMapsMarker marker = new OnlineMapsMarker();
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
        //areasToTestSave = GetTestAreas();
        //cArea.SaveAreas(areasToTestSave);

        areas = cArea.LoadAreas();
        
    }

    private void Start()
    {
        SubscribeToEvents();
        fromPosition = OnlineMaps.instance.position;
        toPosition = OnlineMapsLocationService.instance.position;
        isDrawLineOnEveryPoint = false;
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
            new cArea("Μεσσήνη", new Vector2(21.9202085525009f, 37.17642261183837f), 17, new Vector4(21.9160667457503f, 37.1700252387224f , 21.9227518498302f, 37.178659594564f)),
            new cArea("Κνωσός", new Vector2(25.16310005634713f, 35.29800050616538f), 19, new Vector4(25.1616718900387f, 35.2958874528396f , 25.1645352578472f, 35.3000733065711f)),
            //new cArea("Σαρρή", new Vector2(23.724021164280998f, 37.979955135461715f),19, new Vector4(23.724021164280998f - 1, 37.97881236959543f , 23.725090676541246f, 37.9802439464203f))
            new cArea("Σαρρή", new Vector2(23.724021164280998f, 37.979955135461715f),19, new Vector4(23.72385281512933f, 37.97881236959543f , 23.725090676541246f, 37.9802439464203f))
        };

        //DisplayAreaDebug(0);
        //DisplayAreaDebug(1);

        return areasFromDatabase;
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
        marker.position = locationPoint;
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
            Debug.Log(areas.Count);
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

            //Debug.Log("true");
            return true;

        }
        //Debug.Log("false");
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
        Debug.Log("On Location Changed");
        //position = OnlineMapsLocationService.instance.position;
        Vector3 pos = OnlineMapsTileSetControl.instance.GetWorldPosition(position);
        OnlineMapsLocationService.instance.position = pos;
        marker.position = position;
        CheckMyLocation();
        float distance = OnlineMapsUtils.DistanceBetweenPoints(position, previousPosition).magnitude;
        if(distance < minDistanceToPutNewMarker)
        {
            OnlineMaps.instance.Redraw();
            return;
        }
        else
        {
            previousPosition = position;
        }

        
        marker.position = position;
        OnlineMapsMarkerManager.AddItem(marker);

        markerListCurrPath.Add(marker);

        if (isDrawLineOnEveryPoint && AppManager.Instance.uIManager.imgRecord.GetBool("isPlaying"))
        {

            double lng, lat;
            OnlineMapsControlBase.instance.GetCoords(out lng, out lat);

            position = new Vector2((float)lng, (float)lat);

            // Calculate the distance in km between locations.
            

            string label = "Marker " + (OnlineMaps.instance.markers.Length + 1);


            marker.label = label;
            marker.SetPosition(lng, lat);


            OnlineMapsMarkerManager.CreateItem(lng, lat, label);
            AppManager.Instance.mapManager.markerListCurrPath.Add(marker);


            /*OnlineMapsMarkerManager.RemoveItem(marker);
            //OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingLine(OnlineMapsMarkerManager.instance.Select(m => m.position).ToArray(), Color.red, 3));
            OnlineMapsDrawingLine route = new OnlineMapsDrawingLine(markerListCurrPath, Color.red, 3);
            OnlineMapsDrawingElementManager.AddItem(route);*/
            Debug.Log(marker.label);
            OnlineMapsMarkerManager.instance.Remove(marker);
            OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingLine(OnlineMapsMarkerManager.instance.Select(m => m.position).ToArray(), Color.red, 3));
            OnlineMaps.instance.Redraw();
        }
        /*if (minDistanceToPutNewMarker < 0.005f)
        {
            minDistanceToPutNewMarker = 5f / 1000f;  //5 meters to km
        }*/
        
        // Create a new marker.
       

        

    }

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