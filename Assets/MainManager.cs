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
    public Button btnLayer, btnCurrentLocation, btnGPSPermission, btnCloseWarningScreen, btnOpenSettingsScreen, 
                btnNewAreaOnMap, btnMessiniMap, btnRecord, btnMainMenuHolder, btnSave, btnPrefabToLoad, btnBackFromSettingsScreen, 
                btnLoad, btnCancelMarker, btnNewMarker, btnNewRoute, btnSaveRoute, btnCancelRoute;//btnRec to record the path and save it, don't forget to move it to UIManager too
    [Space]
    public Toggle tglDrawLine, tglAutoSavePoint, tglGPSOn, tpgManualMode;
    [Space]
    public TextMeshProUGUI txtInfo, txtBlack, txtSettings;
    private float time = 3;
    private float angle;

    private bool isMovement, isNewAreaSet, hasPlayed, isRecPath, isMarkerCreated, isMessiniPlace, hasSavedRoutes, isDrawingLine, isAutoSave, isGPSOn, isManualMode, isCancelPressed;
    private Vector2 fromPosition, toPosition, toPositionFinal, toPositionTest;

    private double fromTileX, fromTileY, toTileX, toTileY;
    private int moveZoom;

    [Space]
    public GameObject warningScreen, settingsScreen, menuAnim, pnlButtonPrefabs, holderOnBlackScreen, routeScreen, pnlNewRouteSave;
    public Sprite sprSaveRoute, sprAddRoute;
    private void Awake()
    {
        Application.runInBackground = true;
        warningScreen.SetActive(false);

        pnlButtonPrefabs.SetActive(false);
    }
    void Start()
    {

        btnCloseWarningScreen.onClick.AddListener(() => CloseScreenPanels());
        btnCancelMarker.onClick.AddListener(() => RemoveMarker());
        btnOpenSettingsScreen.onClick.AddListener(() => OpenSettingsScreen());
        btnNewAreaOnMap.onClick.AddListener(() => BeOnNewPlace());
        btnMainMenuHolder.onClick.AddListener(() => OpenCloseMenu());
        btnLayer.onClick.AddListener(() => CheckLayer());
        btnCurrentLocation.onClick.AddListener(() => CheckMyLocation());

        btnSave.gameObject.SetActive(false);
        btnCancelMarker.gameObject.SetActive(false);
        btnPrefabToLoad.gameObject.SetActive(false);
        btnNewMarker.onClick.AddListener(() => BeOnNewPlace());
        btnBackFromSettingsScreen.gameObject.SetActive(false);
        btnBackFromSettingsScreen.onClick.AddListener(() => CloseScreenPanels());

        btnCancelRoute.onClick.AddListener(() => CloseScreenPanels());
        btnSaveRoute.onClick.AddListener(() => SaveUIButton());

        btnNewRoute.onClick.AddListener(() => AddNewRoute());

        routeScreen.SetActive(false);
        pnlNewRouteSave.SetActive(false);

        /*tglDrawLine.isOn = false;
        tglAutoSavePoint.isOn = false;
        tglGPSOn.isOn = false;
        tpgManualMode.isOn = false;

        tglAutoSavePoint.onValueChanged.AddListener((b) => SetToggle(tglAutoSavePoint));
        tglDrawLine.onValueChanged.AddListener((b) => SetToggle(tglDrawLine));
        tglGPSOn.onValueChanged.AddListener((b) => SetToggle(tglGPSOn));
        tpgManualMode.onValueChanged.AddListener((b) => SetToggle(tpgManualMode));*/

        //the below variables can be deleted after finishing with the app
        toPosition = new Vector2(21.91794f, 37.17928f); //correct position for app
        toPositionTest = new Vector2(23.72402f, 37.97994f); //for testing purposes

        
        OnlineMapsLocationService.instance.OnLocationChanged += OnLocationChanged;
        btnMessiniMap.onClick.AddListener(() => MessiniLocation());
    }

    #region AREASFORTESTINGPURPOSES
    //if we move out of messini in order to go back in the original place and constraints
    void MessiniLocation()
    {
        Vector2 pos;
        pos = OnlineMapsLocationService.instance.position; //my position from gps
        float distance = OnlineMapsUtils.DistanceBetweenPoints(toPositionTest, pos).sqrMagnitude;
        isMessiniPlace = true;

        Debug.Log("Messini" + isMessiniPlace);
        if (isMessiniPlace)
        {
            OnlineMapsControlBase.instance.OnMapClick -= OnMapClick;
        }

        if (distance <= OnlineMapsLocationService.instance.desiredAccuracy)
        {
            settingsScreen.SetActive(false);
            txtInfo.text = "You are in the correct area " + pos + " with set position " + toPositionTest;//testing
            txtBlack.text = "Press the red button to record your route from main menu icon.";
            warningScreen.SetActive(true);
            holderOnBlackScreen.SetActive(false);

            OnlineMaps.instance.position = toPosition;
            OnlineMaps.instance.Redraw();
            //playerMarker = OnlineMapsMarkerManager.CreateItem(locationService.position, null, "Player");
            OnlineMaps.instance.zoomRange = new OnlineMapsRange(15, 20);//constrain zoom
            OnlineMaps.instance.positionRange = new OnlineMapsPositionRange(37.17f, 21.91f, 37.2f, 21.93f);//constraint to messini area
            OpenNewRoutesScreen();
        }
        else
        {
            settingsScreen.SetActive(true);
            //blackText.text = "Please go near the area. Your location is: "+pos+" and the marker is: "+ toPosition; //for final build
            txtBlack.text = "Please go near the area. Your location is: " + pos + " and the marker is: " + toPositionTest; //testing

        }
        isMessiniPlace = false;
    }

    //this method is used in order to go to the current location. Won't zoom in on the place user is in, but will redraw the map in oder to be in the center of the view
    public void CheckMyLocation()
    {
        fromPosition = OnlineMaps.instance.position;
        toPositionFinal = OnlineMapsLocationService.instance.position;

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
            OnlineMaps.instance.position = toPositionTest;
        }
    }

    public void OnLocationChanged(Vector2 position)
    {
        position = OnlineMapsLocationService.instance.position;
        
        //playerMarker.position = position;
    }

    //when pressing btnNewMapArea this method works
    void BeOnNewPlace()
    {

        OnlineMapsControlBase.instance.OnMapClick += OnMapClick;
        settingsScreen.SetActive(false);
        if (playerMarker == null)
            OnlineMapsMarkerManager.RemoveAllItems(m => m != playerMarker);

        OnlineMaps.instance.zoomRange = new OnlineMapsRange(0, 20);
        OnlineMaps.instance.positionRange = new OnlineMapsPositionRange(-90, -180, 90, 180, OnlineMapsPositionRangeType.center);
        OnlineMaps.instance.Redraw();
    }

    #endregion
    void Update()
    {
        if (!isMovement) return;

        // update relative position
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

    #region Layers
    //to change layers on map
    void CheckLayer()
    {
        OnlineMaps map = OnlineMaps.instance;
        //map.mapType = "google.satelite";
        if (map.mapType == "google.satelite")
        {
            map.mapType = "google.terrain";
            Debug.Log("terrain");
        }
        else if (map.mapType == "google.terrain")
        {
            map.mapType = "google.relief";
            Debug.Log("relief");
        }
        else
        {
            map.mapType = "google.satelite";
            Debug.Log("sate");
        }

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
#endregion
//this region should be on the UIManager on final build
    #region MenuButtonFunction
//is used on settings button to open the specific panel
void OpenSettingsScreen()
    {
        settingsScreen.SetActive(true);
        warningScreen.SetActive(false);
        btnBackFromSettingsScreen.gameObject.SetActive(true);
        btnPrefabToLoad.gameObject.SetActive(true);
        //LoadState();
    }

    //is called when x button is pressed
    void CloseScreenPanels()
    {
        if (warningScreen.activeSelf && !settingsScreen.activeSelf && routeScreen.activeSelf && !pnlNewRouteSave.activeSelf)
            warningScreen.SetActive(false);
        else if (settingsScreen.activeSelf && !warningScreen.activeSelf && !routeScreen.activeSelf && !pnlNewRouteSave.activeSelf)
            settingsScreen.SetActive(false);
        else if (routeScreen.activeSelf && !settingsScreen.activeSelf && !warningScreen.activeSelf && !pnlNewRouteSave.activeSelf)
            routeScreen.SetActive(false);
        else if (pnlNewRouteSave.activeSelf && !settingsScreen.activeSelf && !warningScreen.activeSelf && routeScreen.activeSelf)
        {
            pnlNewRouteSave.SetActive(false);
            btnNewRoute.GetComponent<Image>().sprite = sprAddRoute;
        }
    }

    //method to play the animation for main menu button, bottom right
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

    //to open the routesPanel and show the buttons with the saved routes.
    void OpenNewRoutesScreen()
    {
        routeScreen.SetActive(true);
        //settingsScreen.SetActive(false);
        
    }
    //when pressing plus icon, pnlNewRoute is activated and plus icon changes to save icon.
    void AddNewRoute()
    {
        btnNewRoute.GetComponent<Image>().sprite = sprSaveRoute;
        pnlNewRouteSave.SetActive(true);
        //open panel with info for new route. Save name, and selections for new route
    }
    //for now change the icon from plus to save
    void SaveUIButton()
    {
        pnlNewRouteSave.SetActive(false);
        btnNewRoute.GetComponent<Image>().sprite = sprSaveRoute;
        btnNewRoute.onClick.AddListener(() => SaveRoute());
        Debug.Log("Save UI method");
    }

    void SaveRoute()
    {
        pnlNewRouteSave.SetActive(false);
        btnNewRoute.GetComponent<Image>().sprite = sprAddRoute;
        btnNewRoute.onClick.AddListener(() => AddNewRoute());
        Debug.Log("Save route method");
    }
    //For Toggles on New Route panel isDrawingLine, isAutoSave, isGPSOn, isManualMode
    /*void SetToggle(Toggle toggle)
    {
        bool val = toggle.isOn;

        if (toggle == tglGPSOn)
        {
            isGPSOn = val;
            isManualMode = !val;
            tpgManualMode.isOn = !val;
        }
        else
        if (toggle == tpgManualMode)
        {
            isGPSOn = !val;
            isManualMode = val;
            tglGPSOn.isOn = !val;
        }
        else
        if (toggle == tglDrawLine)
        {
            isDrawingLine = val;
        }
        else
        if (toggle == tglAutoSavePoint)
        {
            isAutoSave = val;
        }

        Image tImg = toggle.transform.Find("Background").GetComponent<Image>();
        if (toggle.isOn) { tImg.color = Color.green; } else { tImg.color = Color.white; }
    }*/
    #endregion

    #region Markers
    //by pressing cancel button the marker is removed and the map remains unconstraint
    void RemoveMarker()
    {
        if (warningScreen.activeSelf && isMarkerCreated)
        {
            warningScreen.SetActive(false);
            OnlineMapsMarkerManager.RemoveAllItems(m => m != playerMarker);
            //OnlineMaps.instance.positionRange = new OnlineMapsPositionRange(-90, -180, 90, 180, OnlineMapsPositionRangeType.center);
            OnlineMaps.instance.Redraw();
        }
    }



    //create new marker and restrict location in map
    private void OnMapClick()
    {
        isNewAreaSet = true;
        isMarkerCreated = true;
        isMessiniPlace = false;// used this variable so when inside an area to not reconstraint the area. Messini is used as a prototype
        string label;
        if (isMarkerCreated && !isMessiniPlace)
        {
            // Get the coordinates under the cursor.
            double lng, lat;
            OnlineMapsControlBase.instance.GetCoords(out lng, out lat);
            //markerName.gameObject.SetActive(true);
            warningScreen.SetActive(true);

            label = "Marker " + +(OnlineMapsMarkerManager.CountItems + 1);
            // Create a new marker.
            OnlineMapsMarkerManager.CreateItem(lng, lat, label);

            //below are the constraints that were used when marker was created
            /*OnlineMaps.instance.zoomRange = new OnlineMapsRange(10, 20);
            OnlineMaps.instance.positionRange = new OnlineMapsPositionRange((float)lat, (float)lng, 
            (float)lat*locationService.desiredAccuracy, (float)lng, OnlineMapsPositionRangeType.center);
            OnlineMaps.instance.Redraw();*/


            txtBlack.text = "Do you want to save the location?";
            btnSave.gameObject.SetActive(true);
            btnCancelMarker.gameObject.SetActive(true);
            holderOnBlackScreen.SetActive(true);

        }

    }
    #endregion



    void TestEmma()
    {

        //this is a test method
	}

    public void TestAndrew()
    {
        // This is a test
        // This is a second test
        //commentEmma
    }
}
