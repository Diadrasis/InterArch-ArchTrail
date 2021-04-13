using System;
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
    public Button /*btnLayer,*/ btnCurrentLoc, btnGPS, btnClose, btnSettings, btnNewMap, btnMessiniMap, btnRec, btnMainMenuHolder, btnSave, btnPrefabToLoad, btnBack, btnLoad, btnCancel, btnNewMarker;//btnRec to record the path and save it
    [Space]
    public TextMeshProUGUI infoText, blackText, settingsText;
    public float time = 3;
    private float angle;
    public TMP_InputField markerName;

    private bool isMovement, isAutoMarkerEnabled, isNewAreaSet, hasPlayed, isRecPath, isMarkerCreated, isMessiniPlace, hasSavedRoutes;
    private Vector2 fromPosition, toPosition, toPositionFinal, toPositionTest;
    
    private double fromTileX, fromTileY, toTileX, toTileY;
    private int moveZoom;
    OnlineMapsLocationService locationService;
    [Space]
    public GameObject blackScreen, settingsScreen, /*markerIns,*/ menuAnim, buttonPanel, holderOnBlackScreen;
    private static string prefsKey = "markers";
    private OnlineMapsVector2d[] points;
    //string currentPath;
    List<string> pathNamesSavedList = new List<string>();
    
    private void Awake()
    {
        Application.runInBackground = true;
        blackScreen.SetActive(false);
        //settingsScreen.SetActive(true);
        buttonPanel.SetActive(false);
        //btnClose.gameObject.SetActive(false);
        //OpenCloseCanvas(false);
    }
    void Start()
    {
        /*#if PLATFORM_ANDROID
                IsAndroidBuild();
        #endif*/
       
        locationService = OnlineMapsLocationService.instance;
        btnGPS.onClick.AddListener(() => OpenNativeAndroidSettings());
        btnClose.onClick.AddListener(() => CloseCanvas());
        btnCancel.onClick.AddListener(() => RemoveMarker());
        btnSettings.onClick.AddListener(() => OpenSettings());
        btnNewMap.onClick.AddListener(() => BeOnNewPlace());
        
        btnLoad.onClick.AddListener(() => LoadState());
        btnMainMenuHolder.onClick.AddListener(() => OpenCloseMenu());
        btnSave.onClick.AddListener(() => SaveState());
        btnCurrentLoc.onClick.AddListener(() => CheckMyLocation());
        
        btnSave.gameObject.SetActive(false);
        btnCancel.gameObject.SetActive(false);
        btnPrefabToLoad.gameObject.SetActive(false);
        btnNewMarker.onClick.AddListener(() => BeOnNewPlace());
        btnBack.gameObject.SetActive(false);
        btnBack.onClick.AddListener(() => CloseCanvas());
        markerName.onEndEdit.AddListener((b) => SaveName(markerName.text));
        if(!isMessiniPlace)InitLocation();
        toPosition = new Vector2(21.91794f, 37.17928f); //correct position for app
        toPositionTest = new Vector2(23.72402f, 37.97994f); //for testing purposes

        markerName.gameObject.SetActive(false);
        
        SaveLoad.TryLoadMarkers(OnlineMaps.instance.labels.ToString());
        CreateListOfButtons(btnPrefabToLoad, label);
        //LoadState();
        //CheckMyLocation();
    }

    void InitLocation()
    {
        isNewAreaSet = false;
        isMessiniPlace = false;
        /*ShowPlaceOnMap(OnlineMaps.instance.position.y,OnlineMaps.instance.position.x,OnlineMaps.instance.zoom);
        locationService = OnlineMapsLocationService.instance;
        settingsScreen.SetActive(true);
        playerMarker = OnlineMapsMarkerManager.CreateItem(locationService.position, null, "Player");*/

        OnlineMaps.instance.mapType = "google.satellite";
       
        if (locationService == null)
        {
            Debug.LogError(
                "Location Service not found.\nAdd Location Service Component (Component / Infinity Code / Online Maps / Plugins / Location Service).");
            return;
        }

        // Subscribe to the change location event.
        /*locationService.OnLocationChanged += OnLocationChanged;
        OnlineMapsControlBase.instance.OnMapClick += OnMapClick;*/

        if (CheckForLocationServices()) return;
    }
    //if we move out of messini in order to go back in the original place and constraints
    void MessiniLocation(Vector2 pos)
    {
        pos = locationService.position; //my position from gps
        float distance = OnlineMapsUtils.DistanceBetweenPoints(toPositionTest, pos).sqrMagnitude;
        isMessiniPlace = true;
        Debug.Log("Messini"+ isMessiniPlace);
        if (isMessiniPlace)
        {
            OnlineMapsControlBase.instance.OnMapClick -= OnMapClick;
        }
        if (distance <= locationService.desiredAccuracy)
        {
            settingsScreen.SetActive(false);
            infoText.text = "You are in the correct area " + pos + " with set position " + toPositionTest;//testing
            blackText.text = "Press the red button to record your route from main menu icon.";
            blackScreen.SetActive(true);
            holderOnBlackScreen.SetActive(false);
            
            hasSavedRoutes = true;
            //for testing purposes hasSavedRoutes will be true
            if(hasSavedRoutes) StartCoroutine(CountdownForNewMessage("Υπάρχουν αποθηκευμένες διαδρομές."));
            OnlineMaps.instance.position = toPosition;
            OnlineMaps.instance.Redraw();
            playerMarker = OnlineMapsMarkerManager.CreateItem(locationService.position, null, "Player");
            OnlineMaps.instance.zoomRange = new OnlineMapsRange(15, 20);//constrain zoom
            OnlineMaps.instance.positionRange = new OnlineMapsPositionRange(37.17f, 21.91f, 37.2f, 21.93f);//constraint to messini area
            
        }
        else
        {
            settingsScreen.SetActive(true);
            //blackText.text = "Please go near the area. Your location is: "+pos+" and the marker is: "+ toPosition; //for final build
            blackText.text = "Please go near the area. Your location is: " + pos + " and the marker is: " + toPositionTest; //testing
           
        }
        isMessiniPlace = false;
    }
    //location services to check if gps is running on android
    private bool CheckForLocationServices()
    {
        if (locationService == null) return false;

        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            if (locationService.useGPSEmulator)
            {
                btnMessiniMap.onClick.AddListener(() => MessiniLocation(locationService.position));
                settingsScreen.SetActive(true);
                settingsText.text = "Επιλέξτε μια περιοχή";
                btnNewMap.gameObject.SetActive(true);
                btnMessiniMap.gameObject.SetActive(true);
                Debug.Log(locationService);
                infoText.text = "Checking location";
                locationService.OnLocationChanged += OnLocationChanged;
                //CheckMyLocation();
                return true;
            }
            else
            {
                IsAndroidBuild();
                settingsScreen.SetActive(true);
                //btnClose.gameObject.SetActive(true); //on build we can remove it
                return true;
            }

        }
        return false;
    }
    void IsAndroidBuild()
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
            settingsScreen.SetActive(true);
            settingsText.text = "Press the gps button to grant the location permission of your mobile";
            btnNewMap.gameObject.SetActive(false);
            btnMessiniMap.gameObject.SetActive(false);
            Debug.Log("Init");
            //locationService.StopLocationService();
        }
        else if (locationService.useGPSEmulator)
        {
            locationService.StartLocationService();
            infoText.text = "Sto android Build sto else";
        }
    }

    void Update()
    {
        //IsAndroidBuild();//on final build uncomment
        if (!isMovement) return;

        // update relative position
        angle += Time.deltaTime / time;

        if (angle > 1)
        {
            // stop movement
            isMovement = false;
            angle = 1;
        }
        if(locationService.TryStartLocationService())
         infoText.text = "Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp;
    }

    public void CheckMyLocation()
    {
        fromPosition = OnlineMaps.instance.position;
        toPositionFinal = locationService.position;
        locationService.OnLocationChanged += OnLocationChanged;
        playerMarker = OnlineMapsMarkerManager.CreateItem(locationService.position, null, "Player");
        // calculates tile positions
        moveZoom = OnlineMaps.instance.zoom;
        OnlineMaps.instance.projection.CoordinatesToTile(fromPosition.x, fromPosition.y, moveZoom, out fromTileX, out fromTileY);
        OnlineMaps.instance.projection.CoordinatesToTile(toPositionFinal.x, toPositionFinal.y, moveZoom, out toTileX, out toTileY);
        //OnlineMaps.instance.projection.CoordinatesToTile(toPosition.x, toPosition.y, moveZoom, out toTileX, out toTileY);

        // if tile offset < 4, then start smooth movement
        if (OnlineMapsUtils.Magnitude(fromTileX, fromTileY, toTileX, toTileY) < 4)
        {
            // set relative position 0
            angle = 0;

            // start movement
            isMovement = true;
            //StartCoroutine(DrawingOnWalking());
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
        else if (map.mapType == "google.terrain")
        {
            map.mapType = "google.relief";
        }
        else
        {
            map.mapType = "google.satelite";
        }

    }
#endregion
    public void OnLocationChanged(Vector2 position)
    {
        position = locationService.position;
        playerMarker.position = position;
    }

    /*void CheckAppLocation(Vector2 loc)
    {
        
    }*/
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
        locationService.OnLocationChanged += OnLocationChanged;
        //CheckMyLocation();
        OnlineMapsControlBase.instance.OnMapClick += OnMapClick;
        settingsScreen.SetActive(false);
        if (playerMarker == null)
            OnlineMapsMarkerManager.RemoveAllItems(m => m != playerMarker);
        //OnlineMapsMarkerManager.RemoveAllItems();
        //btnLayer.onClick.AddListener(() => CheckLayer());
        
        OnlineMaps.instance.zoomRange = new OnlineMapsRange(0, 20);
        OnlineMaps.instance.positionRange = new OnlineMapsPositionRange(-90, -180, 90, 180, OnlineMapsPositionRangeType.center);
        OnlineMaps.instance.Redraw();
        //StartCoroutine(DrawingOnWalking());
    }
    
    void OpenSettings()
    {
        settingsScreen.SetActive(true);
        blackScreen.SetActive(false);
        btnBack.gameObject.SetActive(true);
        btnPrefabToLoad.gameObject.SetActive(true);
        CreateListOfButtons(btnPrefabToLoad, label);
        //LoadState();
    }
    void CloseCanvas()
    {
        if (blackScreen.activeSelf && !settingsScreen.activeSelf)
            blackScreen.SetActive(false);
        else if (settingsScreen.activeSelf && !blackScreen.activeSelf)
            settingsScreen.SetActive(false);
    }
    void RemoveMarker()
    {
        if (blackScreen.activeSelf && isMarkerCreated)
        {
            blackScreen.SetActive(false);
            OnlineMapsMarkerManager.RemoveAllItems(m => m != playerMarker);
            //OnlineMaps.instance.positionRange = new OnlineMapsPositionRange(-90, -180, 90, 180, OnlineMapsPositionRangeType.center);
            OnlineMaps.instance.Redraw();
        }
    }
    //create a marker as gameobject so we can remove on long press
    /*private void OnMarkerLongPress(OnlineMapsMarker marker)
    {
        OnlineMapsMarkerManager.RemoveItem(marker);
    }*/
    string label;
    //to get the input field text and change it dynamically for the label name
    public string SaveName(string tmp)
    {
        return markerName.text = tmp;
    }
    //create new marker and restrict location in map
    private void OnMapClick()
    {
        isNewAreaSet = true;
        isMarkerCreated = true;
        isMessiniPlace = false;
        if (isMarkerCreated && !isMessiniPlace)
        {
            // Get the coordinates under the cursor.
            double lng, lat;
            OnlineMapsControlBase.instance.GetCoords(out lng, out lat);
            markerName.gameObject.SetActive(true);
            blackScreen.SetActive(true);

            
            SaveName(label); //to get the name of the label
            // Create a new marker.
            OnlineMapsMarkerManager.CreateItem(lng, lat, label);
            ShowPlaceOnMap(lng, lat, OnlineMaps.instance.zoom);
            /*OnlineMaps.instance.zoomRange = new OnlineMapsRange(10, 20);
            OnlineMaps.instance.positionRange = new OnlineMapsPositionRange((float)lat, (float)lng, 
            (float)lat*locationService.desiredAccuracy, (float)lng, OnlineMapsPositionRangeType.center);
            OnlineMaps.instance.Redraw();*/


            blackText.text = "Do you want to save the location?";
            btnSave.gameObject.SetActive(true);
            btnCancel.gameObject.SetActive(true);
            holderOnBlackScreen.SetActive(true);

        }
        
    }

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
        SaveLoad.SaveNewMarkersAndArea(prefsKey);
        settingsScreen.SetActive(true);
        
    }
    private void LoadState()
    {
        if (isMessiniPlace)
        {
            BeOnNewPlace();
            //OnlineMaps.instance.Redraw();
            SaveLoad.TryLoadMarkers(prefsKey);
            settingsScreen.SetActive(false);
        }
        else
        {
            SaveLoad.TryLoadMarkers(prefsKey);
            settingsScreen.SetActive(false);
        }
        
    }
#endregion

    Button CreateListOfButtons(Button temp, string label)
    {
        buttonPanel.SetActive(true);
        temp = Instantiate(btnPrefabToLoad);
        temp.GetComponentInChildren<TextMeshProUGUI>().text = label;
        temp.transform.SetParent(buttonPanel.transform, false);

        return temp;
    }
    
    IEnumerator CountdownForNewMessage(string textNew)
    {
        yield return new WaitForSeconds(2f);
        blackScreen.SetActive(true);
        holderOnBlackScreen.SetActive(false);
        blackText.text = textNew;
    }

    void ShowPlaceOnMap(double posLat, double posLng, int zoomSave)
    {
        isMessiniPlace = false;
        OnlineMaps.instance.GetTilePosition(out posLng,out posLat, zoomSave);
        OnlineMaps.instance.SetTilePosition(posLng, posLat, zoomSave);
        OnlineMaps.instance.Redraw();
    }
}
