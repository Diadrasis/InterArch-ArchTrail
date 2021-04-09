﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stathis.Android;
using UnityEngine.Android;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class MainManager : MonoBehaviour
{
    private OnlineMapsMarker playerMarker;
    public Button /*btnLayer,*/ btnCurrentLoc, btnGPS, btnClose, btnSettings,btnResetMap,btnOriginalMap, btnRec, btnMainHolder, btnSave, btnDefault, btnBack, btnLoad, btnCancel;//btnRec to record the path and save it
    [Space]
    public TextMeshProUGUI infoText, blackText;
    public float time = 3;
    private float angle;
    public TMP_InputField markerName;

    private bool isMovement, isAutoMarkerEnabled, isNewAreaSet, hasPlayed, isRecPath, isMarkerCreated;
    private Vector2 fromPosition, toPosition;
    private Vector2 toPositionTest;
    private double fromTileX, fromTileY, toTileX, toTileY;
    private int moveZoom;
    OnlineMapsLocationService locationService;
    [Space]
    public GameObject blackScreen, settingsScreen, /*markerIns,*/ menuAnim, buttonPanel;
    private static string prefsKey = "markers";
    private OnlineMapsVector2d[] points;
    string currentPath;
    List<string> pathNamesSavedList = new List<string>();
    private static MainManager _instance;

    public static MainManager instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        Application.runInBackground = true;
        blackScreen.SetActive(false);
        settingsScreen.SetActive(false);
        buttonPanel.SetActive(false);
        //btnClose.gameObject.SetActive(false);
        //OpenCloseCanvas(false);
    }
    void Start()
    {
        locationService = OnlineMapsLocationService.instance;
        btnGPS.onClick.AddListener(() => OpenNativeAndroidSettings());
        btnClose.onClick.AddListener(() => CloseCanvas());
        btnCancel.onClick.AddListener(() => RemoveMarker());
        btnSettings.onClick.AddListener(() => OpenSettings());
        btnResetMap.onClick.AddListener(() => BeOnNewPlace());
        btnOriginalMap.onClick.AddListener(() => MessiniLocation());
        btnLoad.onClick.AddListener(() => LoadState());
        btnMainHolder.onClick.AddListener(() => OpenCloseMenu());
        btnSave.onClick.AddListener(() => SaveState());
        btnSave.gameObject.SetActive(false);
        btnCancel.gameObject.SetActive(false);
        btnDefault.gameObject.SetActive(false);
        btnBack.gameObject.SetActive(false);
        btnBack.onClick.AddListener(() => CloseCanvas());
        markerName.onEndEdit.AddListener((b)=>SaveName(markerName));
        InitLocation();
        toPosition = new Vector2(21.91794f, 37.17928f); //correct position for app
        toPositionTest = new Vector2(23.72402f, 37.97994f); //for testing purposes
        locationService.OnLocationChanged += CheckAppLocation;
        //markerName.text = PlayerPrefs.GetString("markers");
        markerName.gameObject.SetActive(false);
        //settingsScreen.SetActive(false);
        SaveLoad.TryLoadMarkers(OnlineMaps.instance.labels.ToString());
    }

    void InitLocation()
    {
        isNewAreaSet = false;
        isMarkerCreated = false;
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
        OnlineMapsControlBase.instance.OnMapClick -= OnMapClick;
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
        //playerMarker = OnlineMapsMarkerManager.CreateItem(toPosition, null, "Player");
        OnlineMaps.instance.zoomRange = new OnlineMapsRange(15, 20);//constrain zoom
        OnlineMaps.instance.positionRange = new OnlineMapsPositionRange(37.17f, 21.90f, 37.18f, 21.92f);//constraint to messini area
        
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
            StartCoroutine(DrawingOnWalking());
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
        loc = toPositionTest;//OnlineMaps.instance.position;//new Vector2(23.8f, 38.1f);
        //float distance = OnlineMapsUtils.DistanceBetweenPoints(toPosition, loc).sqrMagnitude;//correct for final build
        float distance = OnlineMapsUtils.DistanceBetweenPoints(toPositionTest, loc).sqrMagnitude;//testing
        if (distance <= locationService.desiredAccuracy)
        {
            //infoText.text = "You are in the correct area "+loc+" with set position "+ toPosition; //correct for final build
            infoText.text = "You are in the correct area " + loc + " with set position " + toPositionTest;//testing
            blackText.text = "Press the red button to record your route from main menu icon.";
            btnCurrentLoc.onClick.AddListener(() => CheckMyLocation());
            blackScreen.SetActive(true);
            //btnRec.onClick.AddListener(() => RecMyPath());
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
        }
    }
    //to screencapture the path
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

        //StartCoroutine(TakeScreenShot(currentPath));

        }

    }
    IEnumerator TakeScreenShot(string pathname)
    {
        if (!serverManager.useScreenShots) { yield break; }
        MarkersManager.CenterZoomOnMarkers();

        yield return new WaitForEndOfFrame();
        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        byte[] bytes = tex.EncodeToJPG(); //Can also encode to jpg, just make sure to change the file extensions down below
        Destroy(tex);
        OnLocationChanged(locationService.position);
        yield return new WaitForEndOfFrame();

        //Stathis.File_Manager.saveImage(bytes, pathname, Stathis.File_Manager.Ext.JPG);

        yield break;
    }*/
    void OpenNativeAndroidSettings()
    {
        AndroidBridge.OpenIntent(IntentNames.GPS_SETTINGS);
    }

    void OnApplicationQuit()
    {
        locationService.StopLocationService();
    }

    void BeOnNewPlace()
    {
        OnlineMapsControlBase.instance.OnMapClick += OnMapClick;
        settingsScreen.SetActive(false);
        OnLocationChanged(locationService.position);
        CheckMyLocation();
        OnlineMapsMarkerManager.RemoveAllItems();
        //btnLayer.onClick.AddListener(() => CheckLayer());
        btnCurrentLoc.onClick.AddListener(() => CheckMyLocation());
        OnlineMaps.instance.zoomRange = new OnlineMapsRange(5, 20);
        OnlineMaps.instance.positionRange = new OnlineMapsPositionRange(-90 ,-180 ,90 ,180 , OnlineMapsPositionRangeType.center);
        OnlineMaps.instance.Redraw();
        //StartCoroutine(DrawingOnWalking());
    }
    private void OnPress(OnlineMapsMarkerBase marker)
    {
        Debug.Log("OnPress");
    }
    void OpenSettings()
    {
        settingsScreen.SetActive(true);
        blackScreen.SetActive(false);
        btnBack.gameObject.SetActive(true);
        CreateListOfButtons(btnDefault, label);
        //LoadState();
    }
    void CloseCanvas()
    {
        if(blackScreen.activeSelf && !settingsScreen.activeSelf)
        blackScreen.SetActive(false);
        else if(settingsScreen.activeSelf && !blackScreen.activeSelf)
        settingsScreen.SetActive(false);
    }
    void RemoveMarker()
    {
        if (blackScreen.activeSelf && isMarkerCreated)
        {
            blackScreen.SetActive(false);
            OnlineMapsMarkerManager.RemoveAllItems();
            OnlineMaps.instance.positionRange = new OnlineMapsPositionRange(-90, -180, 90, 180, OnlineMapsPositionRangeType.center);
            OnlineMaps.instance.Redraw();
        }
    }
    string label;

    public void SaveName(TMP_InputField tmp)
    {
        if (tmp.text.Length > 0)
        {
            Debug.Log(tmp.text);
        }
    }
    //create new marker and restrict location in map
    private void OnMapClick()
    {
        isNewAreaSet = true;
        isMarkerCreated = true;
        if (isMarkerCreated)
        {
            // Get the coordinates under the cursor.
            double lng, lat;
            OnlineMapsControlBase.instance.GetCoords(out lng, out lat);
            markerName.gameObject.SetActive(true);
            blackScreen.SetActive(true);
            foreach (OnlineMapsMarker marker in OnlineMapsMarkerManager.instance)
            {
                SaveName(markerName);
                label = markerName.GetComponentInChildren<TextMeshProUGUI>().text;
                marker.label = label;
                btnDefault.gameObject.SetActive(true);
                btnDefault.GetComponentInChildren<TextMeshProUGUI>().text = label;
                SaveLoad.SaveNewMarkersAndArea(label);
                //btnLoad.onClick.AddListener(() => SaveLoad.TryLoadMarkers(label));
            }

            // Create a new marker.
            OnlineMapsMarkerManager.CreateItem(lng, lat, label);
            OnlineMaps.instance.zoomRange = new OnlineMapsRange(10, 20);
            OnlineMaps.instance.positionRange = new OnlineMapsPositionRange(locationService.position.y, locationService.position.x, (float)lat, (float)lng, OnlineMapsPositionRangeType.center);
            OnlineMaps.instance.Redraw();


            blackText.text = "Do you want to save the location?";
            btnSave.gameObject.SetActive(true);
            btnCancel.gameObject.SetActive(true);
            
        }
        //Instantiate(markerIns, new Vector2((float)lng, (float)lat), Quaternion.identity);//instantiate marker as gameObject and not the one on top of map.
        //OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingLine(OnlineMapsMarkerManager.instance.Select(m => m.position).ToArray(),Color.red,3)); //draw a line between markers and restrict an area according to those markers
    }
    //in order to show the marker after saving it and show it's label, create a new method where you can save the info and show marker after save button is pressed
   
    void OpenCloseMenu()
    {
        if (!hasPlayed)
        {
            menuAnim.GetComponent<Animator>().SetBool("Open", true);
            hasPlayed = true;
        }
        else
        {
            hasPlayed = false;   
            menuAnim.GetComponent<Animator>().SetBool("Open", false);
        }
       
    }
    #region Save-Load
    private void SaveState()
    {
        OnlineMaps map = OnlineMaps.instance;

        OnlineMapsXML prefs = new OnlineMapsXML("Map");
        label = markerName.text;
        
        Debug.Log("SaveState " + label);

        // Save position and zoom
        OnlineMapsXML generalSettings = prefs.Create("General");
        generalSettings.Create("Coordinates", map.position);
        generalSettings.Create("Zoom", map.zoom);
        generalSettings.Create("ID", map.mapType);

        // Save 2D markers
        map.SaveMarkers(prefs);

        // Save 3D markers
        //OnlineMapsControlBase3D.instance.SaveMarkers3D(prefs);
        
        // Save settings to PlayerPrefs
        PlayerPrefs.SetString(prefsKey, prefs.outerXml);
        blackScreen.SetActive(false);

        //PlayerPrefs.SetString("mapTypeName", mapTypeName);
    }
    private void LoadState()
    {
        if (!PlayerPrefs.HasKey(prefsKey)) return;

        OnlineMaps map = OnlineMaps.instance;

        OnlineMapsXML prefs = OnlineMapsXML.Load(PlayerPrefs.GetString(prefsKey));

        OnlineMapsXML generalSettings = prefs["General"];
        map.position = generalSettings.Get<Vector2>("Coordinates");
        map.zoom = generalSettings.Get<int>("Zoom");
        
        List<OnlineMapsMarker> markers = new List<OnlineMapsMarker>();
        OnlineMapsMarkerManager.SetItems(markers);
        settingsScreen.SetActive(false);
        //map.markers = markers.ToArray();
    }
    #endregion

    Button CreateListOfButtons(Button temp, string label)
    {
        buttonPanel.SetActive(true);
        temp = Instantiate(btnDefault);
        temp.GetComponentInChildren<TextMeshProUGUI>().text = label;
        temp.transform.SetParent(buttonPanel.transform, false);
        
        return temp;
    }
    IEnumerator DrawingOnWalking()
    {
        OnLocationChanged(locationService.position);
        CheckMyLocation();
        //double distance = OnlineMapsUtils.DistanceBetweenPoints(locationService.position, locationService.);
        if (locationService.allowUpdatePosition)
        {
            OnlineMapsMarkerManager.CreateItem(locationService.position.y,locationService.position.x);
            OnlineMapsDrawingLine line = new OnlineMapsDrawingLine(OnlineMapsMarkerManager.instance.items.Select(m => m.position).ToArray(), Color.red, 5);
            line.followRelief = true;
            OnlineMapsDrawingElementManager.AddItem(line);
        }
        yield return new WaitForSeconds(5f);
        
    }
}
