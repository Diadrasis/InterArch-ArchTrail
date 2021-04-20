using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    #region Variables
    public GameObject mapScreen;

    [Space]
    [Header("Top Screen")]
    public Button btnBackToAreasScreen;
    public Animator imgRecord;

    [Space]
    [Header("Areas Screen")]
    // Areas Screen
    public GameObject pnlAreasScreen;
    public GameObject pnlLoadedAreas;
    public GameObject selectAreaPrefab;
    public Button btnCreateArea;

    [Space]
    [Header("Create Area Screen")]
    // Create Area Screen
    public GameObject pnlCreateArea;
    public TMP_InputField inptFldCreateArea;
    public Button btnCreateAreaSave;
    public Button btnCreateAreaCancel;

    private List<GameObject> selectAreaObjects;
    private float interval = 0.001f;

    private bool createArea = false;

    [Space]
    [Header("GPS Screen")]
    //GPS Screen
    public GameObject pnlGPSScreen;
    public Button btnGPSPermission;

    [Space]
    [Header("Route Screen")]
    //RouteScreen
    public GameObject pnlSelectedAreaScreen;
    public Button btnAddNewRoute;
    public Sprite sprAddNewRoute, sprSaveIcon;

    [Space]
    [Header("Warning Area Screen")]
    //WarningScreen if user is near area
    public GameObject pnlWarningScreen;
    public Button btnCancel;

    [Space]
    [Header("Warning Save Route Screen")]
    //WarningScreen when user is about to save the route
    public GameObject pnlWarningSaveRouteScreen;
    public Button btnSave, btnSaveCancel;
    #endregion

    #region Unity Functions
    public void Start()
    {
        selectAreaObjects = new List<GameObject>();

        SubscribeButtons();

        DisplayAreasScreen();

        pnlWarningScreen.SetActive(false);
        pnlWarningSaveRouteScreen.SetActive(false);

        //imgRecord = GetComponent<Animator>();
    }

    #endregion

    #region Methods
    private void SubscribeButtons()
    {
        // Map Screen
        btnBackToAreasScreen.onClick.AddListener(() => BackToAreasScreen());

        // Areas Screen
        btnCreateArea.onClick.AddListener(() => CreateNewAreaSelected());

        // CreateAreaScreen
        // inptFldCreateArea.onValidateInput.//AddListener(() => EnableScreen(pnlCreateArea, true));
        btnCreateAreaSave.onClick.AddListener(() => SaveArea());
        btnCreateAreaCancel.onClick.AddListener(() => EnableScreen(pnlCreateArea, false));

        //btn GPS
        btnGPSPermission.onClick.AddListener(() => AppManager.Instance.androidManager.OpenNativeAndroidSettings());

        //btn Route
        btnAddNewRoute.onClick.AddListener(() => AddNewRoute());

        //btn warning on area
        btnCancel.onClick.AddListener(() => CloseScreenPanels());

        //btn warning panel for save or cancel a route
        btnSave.onClick.AddListener(() =>SaveRoute());
        btnSaveCancel.onClick.AddListener(() => CancelSaveRoute());

    }

    public void DisplayAreasScreen()
    {
        pnlAreasScreen.SetActive(true);
        DestroySelectAreaObjects(selectAreaObjects);
        selectAreaObjects = InstantiateSelectAreaObjects();
        StartCoroutine(ReloadLayout(pnlLoadedAreas));
        createArea = false;
        EnableScreen(pnlSelectedAreaScreen, false);
        imgRecord.gameObject.SetActive(false);
    }

    private void EnableScreen(GameObject _screenToEnable, bool _valid) // CURRENTLY IN USE
    {
        if (_valid)
            _screenToEnable.SetActive(true);
        else
            _screenToEnable.SetActive(false);
    }

    private void Escape()
    {
        Application.Quit();
    }

    private List<GameObject> InstantiateSelectAreaObjects()
    {
        List<GameObject> newSelectAreaObjects = new List<GameObject>();
        List<cArea> areas = AppManager.Instance.mapManager.areas;

        foreach (cArea area in areas)
        {
            GameObject newSelectArea = Instantiate(selectAreaPrefab, Vector3.zero, Quaternion.identity, pnlLoadedAreas.GetComponent<RectTransform>());
            //newSelectArea.transform.SetAsFirstSibling();
            TMP_Text selectAreaText = newSelectArea.GetComponentInChildren<TMP_Text>();
            selectAreaText.text = area.title;
            Button button = newSelectArea.GetComponentInChildren<Button>();
            button.onClick.AddListener(OnAreaSelected);
            newSelectAreaObjects.Add(newSelectArea);
        }

        return newSelectAreaObjects;
    }

    private void OnAreaSelected()
    {
        GameObject selectAreaObject = EventSystem.current.currentSelectedGameObject;
        TMP_Text selectAreaText = selectAreaObject.GetComponentInChildren<TMP_Text>();

        cArea selectedArea = AppManager.Instance.mapManager.GetAreaByTitle(selectAreaText.text);
        AppManager.Instance.mapManager.currentArea = selectedArea;

        if (selectedArea != null)
        {
            pnlAreasScreen.SetActive(false);
            AppManager.Instance.mapManager.SetMapViewToArea(selectedArea);
        }
        EnableScreen(pnlSelectedAreaScreen, true);
        imgRecord.gameObject.SetActive(true);

        AppManager.Instance.mapManager.CheckUserPosition();
    }

    private void DestroySelectAreaObjects(List<GameObject> _selectAreaObjects)
    {
        if (_selectAreaObjects != null)
        {
            foreach (GameObject selectArea in _selectAreaObjects)
            {
                Destroy(selectArea);
            }

            _selectAreaObjects.Clear();
        }
    }

    IEnumerator ReloadLayout(GameObject _layoutGameObject)
    {
        yield return new WaitForSeconds(interval);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutGameObject.GetComponent<RectTransform>());
    }

    private void BackToAreasScreen()
    {
        DisplayAreasScreen();
        pnlWarningScreen.SetActive(false);
        //mapScreen.SetActive(false);
    }

    void IsRecording(bool val)
    {
        imgRecord.SetBool("isPlaying", val);
    }
    
    private void CreateNewAreaSelected()
    {
        pnlAreasScreen.SetActive(false);
        createArea = true;
        //mapScreen.SetActive(true);

        AppManager.Instance.mapManager.SetMapViewToLocation();
        // Resets the map view at my location, DONE
    }

    private void SaveArea() // MUST BE UPDATED
    {
        string newAreaTitle = inptFldCreateArea.text;
        AppManager.Instance.mapManager.SaveArea(new cArea(newAreaTitle, OnlineMaps.instance.position));

        pnlCreateArea.SetActive(false);
    }

    public void OnMapClick()
    {
        if (createArea)
        {
            pnlCreateArea.SetActive(true);
        }
        else
        {
            Debug.Log("Map Click!");
        }

        // EMMA
        /*
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

        /*
            txtBlack.text = "Do you want to save the location?";
            btnSave.gameObject.SetActive(true);
            btnCancelMarker.gameObject.SetActive(true);
            holderOnBlackScreen.SetActive(true);
        }*/

        // STATHIS
        /*if (isManualAddMarkerEnabled)
        {
            // Get the coordinates under the cursor.
            double lng, lat;
            OnlineMapsControlBase.instance.GetCoords(out lng, out lat);

            Vector2 position = new Vector2((float)lng, (float)lat);

            // Calculate the distance in km between locations.
            float distance = OnlineMapsUtils.DistanceBetweenPoints(position, previousPosition).magnitude;

            //Debug.LogWarning("dist = " + distance);
            // Debug.LogWarning("minDistanceToPutNewMarker = " + minDistanceToPutNewMarker);

            if (distance < minDistanceToPutNewMarker)
            {
                return;
            }
            else
            {
                previousPosition = position;
            }

            // Create a label for the marker.
            string label = "Marker " + (OnlineMaps.instance.markers.Length + 1);

            OnlineMapsMarker marker = new OnlineMapsMarker();

            marker.label = label;
            marker.SetPosition(lng, lat);

            // Create a new marker.
            OnlineMaps.instance.AddMarker(lng, lat, label);

            markerListCurrPath.Add(marker);

            if (isDrawLineOnEveryPoint)
            {
                OnlineMaps.instance.RemoveMarker(marker);
                MarkersManager.CreateLineFromList(markerListCurrPath, Color.red, 3f);
                OnlineMaps.instance.CheckRedrawType();//.Redraw();
            }

            if (isSavePathOnEveryPoint)
            {
                MarkersManager.SaveMarkers(currPathName, markerListCurrPath);
            }
        }*/
    }

    #region RoutePanel
    //changes icon from plus to save icon, listener changes to next method for saving route, here also have the drawing?
    void AddNewRoute()
    {
        btnAddNewRoute.GetComponent<Image>().sprite = sprSaveIcon;
        IsRecording(true);
        btnAddNewRoute.onClick.AddListener(() => SaveUIButton());
        Debug.Log("Add New Route");
    }

    //change the icon from plus to save, opens warning screen for saving or cancel route
    void SaveUIButton()
    {
        EnableScreen(pnlWarningSaveRouteScreen, true);
        IsRecording(false);
        btnAddNewRoute.GetComponent<Image>().sprite = sprSaveIcon;
        btnAddNewRoute.onClick.AddListener(() => SaveRoute());
        Debug.Log("Save UI method");
    }

    //when save button is pressed on warning screen, the save icon changes back to plus icon. Warning screen is deactivated and listener goes to original method
    void SaveRoute()
    {
        btnAddNewRoute.GetComponent<Image>().sprite = sprAddNewRoute;
        btnAddNewRoute.onClick.AddListener(() => AddNewRoute());
        EnableScreen(pnlWarningSaveRouteScreen, false);
        IsRecording(false);
        Debug.Log("Save route method");
    }
    #endregion


    #region Warnings
    //to close main warning screen for area check
    void CloseScreenPanels()
    {
        if (pnlWarningScreen.activeSelf)
            pnlWarningScreen.SetActive(false);
    }

    //to close route save plus remove everything from the map
    void CancelSaveRoute()
    {
        if (pnlWarningSaveRouteScreen.activeSelf)
        {
            SaveRoute();
            OnlineMapsDrawingElementManager.RemoveAllItems();
            OnlineMaps.instance.Redraw();
        }
    }
    #endregion


    #endregion


}
