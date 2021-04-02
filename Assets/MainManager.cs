using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stathis.Android;
using UnityEngine.Android;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    private OnlineMapsMarker playerMarker;
    public Button /*btnLayer,*/ btnCurrentLoc, btnGPS, btnClose, btnSettings,btnResetMap,btnOriginalMap, btnRec;//btnRec to record the path and save it
    public Text infoText, blackText;
    public float time = 3;
    private float angle;

    private bool isMovement;
    private Vector2 fromPosition, toPosition;
    private Vector2 toPositionTest;
    private double fromTileX, fromTileY, toTileX, toTileY;
    private int moveZoom;
    OnlineMapsLocationService locationService;
    public GameObject blackScreen, settingsScreen;

    //to create a new area to look around
    /*private List<OnlineMapsMarker> markers = new List<OnlineMapsMarker>();
    private List<Vector2> markerPositions = new List<Vector2>();
    private OnlineMapsDrawingPoly polygon;*/
    private void Awake()
    {
        Application.runInBackground = true;
        blackScreen.SetActive(false);
        settingsScreen.SetActive(false);
        btnClose.gameObject.SetActive(false);
        //OpenCloseCanvas(false);
    }
    void Start()
    {
        locationService = OnlineMapsLocationService.instance;
        btnGPS.onClick.AddListener(() => OpenNativeAndroidSettings());
        btnClose.onClick.AddListener(() => CloseCanvas());
        btnSettings.onClick.AddListener(() => OpenSettings());
        btnResetMap.onClick.AddListener(() => BeOnNewPlace());
        btnOriginalMap.onClick.AddListener(() => MessiniLocation());
        InitLocation();
        toPosition = new Vector2(21.91794f, 37.17928f); //correct position for app
        toPositionTest = new Vector2(23.72402f, 37.97994f); //for testing purposes
        locationService.OnLocationChanged += CheckAppLocation;
        settingsScreen.SetActive(false);
    }
    void InitLocation()
    {
        locationService = OnlineMapsLocationService.instance;
        //playerMarker = locationService.marker2DTexture.LoadImage();
        playerMarker = OnlineMapsMarkerManager.CreateItem(toPosition, null, "Player");
        OnlineMaps.instance.mapType = "google.satellite";
        OnlineMaps.instance.zoomRange = new OnlineMapsRange(15, 20);//constrain zoom
        OnlineMaps.instance.positionRange = new OnlineMapsPositionRange(37.17f, 21.91f, 37.18f, 21.923f);//constraint to messini area
        if (locationService == null)
        {
            Debug.LogError(
                "Location Service not found.\nAdd Location Service Component (Component / Infinity Code / Online Maps / Plugins / Location Service).");
            return;
        }

        // Subscribe to the change location event.
        locationService.OnLocationChanged += OnLocationChanged;

        if (CheckForLocationServices()) return;
    }
    //if we move out of messini in order to go back in the original place and constraints
    void MessiniLocation()
    {
        settingsScreen.SetActive(false);
        OnLocationChanged(toPosition);
        //CheckMyLocation();
        OnlineMaps.instance.position = toPosition;
        OnlineMaps.instance.Redraw();
        //playerMarker = OnlineMapsMarkerManager.CreateItem(toPosition, null, "Player")
        OnlineMaps.instance.zoomRange = new OnlineMapsRange(15, 20);//constrain zoom
        OnlineMaps.instance.positionRange = new OnlineMapsPositionRange(37.17f, 21.91f, 37.18f, 21.92f);//constraint to messini area
        
    }
    private bool CheckForLocationServices()
    {
        if (locationService == null) return false;

        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            if (locationService.useGPSEmulator)
            {
                Debug.Log(locationService);
                infoText.text = "Checking location";
                //CheckAppLocation(toPosition);
                CheckAppLocation(toPositionTest);
                return true;
            }
            else
            {
                isAndroidBuild();
                blackScreen.SetActive(true);
                //btnClose.gameObject.SetActive(true); //on build we can remove it
                blackText.text = "Press the gps button";
                //StartCoroutine(CheckAppLocation());
                return true;
            }
            
        }
        return false;
    }
    void isAndroidBuild()
    {
        infoText.text = "Android prin na dei pws eimai se android";
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            //Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            infoText.text = "Android sto permission";
        }
#endif
        if (!locationService.TryStartLocationService())
        {
            blackScreen.SetActive(true);
            blackText.text = "Press the gps button to grant the location permission of your mobile";
            //locationService.StopLocationService();
        }
        else
        {
            //CheckAppLocation(toPosition);
            infoText.text = "Sto android Build sto else";
            CheckAppLocation(OnlineMaps.instance.position);
        }
    }

    void Update()
    {
       //OnLocationChanged(new Vector2(23.8f, 38.1f));
        //isAndroidBuild();//on final build uncomment
        if (!isMovement) return;

        // update relative position
        angle += Time.deltaTime / time;

        if (angle > 1)
        {
            // stop movement
            isMovement = false;
            angle = 1;
        }

        // Set new position
        //OnlineMaps.instance.zoomRange = 18f;
        //double px = (toTileX - fromTileX) * angle + fromTileX;
        //double py = (toTileY - fromTileY) * angle + fromTileY;
        //OnlineMaps.instance.projection.TileToCoordinates(px, py, moveZoom, out px, out py);
        //OnlineMaps.instance.SetPosition(px, py);
        infoText.text = "Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp;
    }
   
    void CheckMyLocation()
    {
        fromPosition = OnlineMaps.instance.position;

        // calculates tile positions
        moveZoom = OnlineMaps.instance.zoom;
        OnlineMaps.instance.projection.CoordinatesToTile(fromPosition.x, fromPosition.y, moveZoom, out fromTileX, out fromTileY);
        OnlineMaps.instance.projection.CoordinatesToTile(toPositionTest.x, toPositionTest.y, moveZoom, out toTileX, out toTileY);
        //OnlineMaps.instance.projection.CoordinatesToTile(toPosition.x, toPosition.y, moveZoom, out toTileX, out toTileY);

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
            OnlineMaps.instance.position = toPositionTest;
        }
    }
    #region Layers
    void CheckLayer()
    {
        OnlineMaps map = OnlineMaps.instance;
        //map.mapType = "google.satelite";
        if (map.mapType == "google.satelite")
        {
            map.mapType = "google.terrain";
        }
        else if (map.mapType =="google.terrain")
        {
            map.mapType = "google.relief";
        }
        else
        {
            map.mapType = "google.satelite";
        }
        
    }
    #endregion
    private void OnLocationChanged(Vector2 position)
    {
        position = locationService.position;
        playerMarker.position = position;
    }

    void CheckAppLocation(Vector2 loc)
    {
        loc = OnlineMaps.instance.position;//new Vector2(23.8f, 38.1f);//new Vector2(Input.location.lastData.longitude, Input.location.lastData.latitude);
        //float distance = OnlineMapsUtils.DistanceBetweenPoints(toPosition, loc).sqrMagnitude;//correct for final build
        float distance = OnlineMapsUtils.DistanceBetweenPoints(toPositionTest, loc).sqrMagnitude;//testing
        if (distance <= locationService.desiredAccuracy)
        {
            //infoText.text = "You are in the correct area "+loc+" with set position "+ toPosition; //correct for final build
            infoText.text = "You are in the correct area " + loc + " with set position " + toPositionTest;//testing
            btnCurrentLoc.onClick.AddListener(() => CheckMyLocation());
            blackScreen.SetActive(false);
            //OpenCloseCanvas(false);
            //btnLayer.onClick.AddListener(() => CheckLayer());

        }
        else
        {
            blackScreen.SetActive(true);
            btnClose.gameObject.SetActive(true);
            //blackText.text = "Please go near the area. Your location is: "+loc+" and the marker is: "+ toPosition; //for final build
            blackText.text = "Please go near the area. Your location is: " + loc + " and the marker is: " + toPositionTest; //testing
            btnCurrentLoc.onClick.AddListener(() => CheckMyLocation());
            //btnLayer.onClick.AddListener(() => CheckLayer());
            //locationService.StopLocationService();            
        }
    }

    public void OpenNativeAndroidSettings()
    {
        AndroidBridge.OpenIntent(IntentNames.GPS_SETTINGS);
    }

    private void OnApplicationQuit()
    {
        locationService.StopLocationService();
    }

    void BeOnNewPlace()
    {
        settingsScreen.SetActive(false);
        OnLocationChanged(locationService.position);
        CheckMyLocation();
        //btnLayer.onClick.AddListener(() => CheckLayer());
        btnCurrentLoc.onClick.AddListener(() => CheckMyLocation());
        OnlineMaps.instance.zoomRange = new OnlineMapsRange(5, 20);
        OnlineMaps.instance.positionRange = new OnlineMapsPositionRange(-90 ,-180 ,90 ,180 , OnlineMapsPositionRangeType.center);
        OnlineMaps.instance.Redraw();
    }
     void OpenSettings()
    {
        settingsScreen.SetActive(true);
        //infoText.text = "Enter the new location you want to explore ";
    }
    void CloseCanvas()
    {
        blackScreen.SetActive(false);
    }
}
