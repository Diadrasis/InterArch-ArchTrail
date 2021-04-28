using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;

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
    [Header("Path Screen")]
    //PathScreen
    public GameObject pnlPathScreen;
    public Button btnAddNewPath;
    public Sprite sprAddNewPath, sprSaveIcon;

    [Space]
    [Header("Warning Area Screen")]
    //WarningScreen if user is near area
    public GameObject pnlWarningScreen;
    public Button btnCancel;

    [Space]
    [Header("Warning Save Path Screen")]
    //WarningScreen when user is about to save the path
    public GameObject pnlWarningSavePathScreen;
    public Button btnSave, btnSaveCancel;

    [Space]
    [Header("Testing Purposes")]
    public Texture2D userMarker;
    public TextMeshProUGUI infoText;
    public Button btnPaths, btnCancelShow;
    public GameObject pnlSavedPaths, btnShowPath, pnlScrollViewPaths;
    private List<GameObject> selectPathObjects;
    #endregion

    #region Unity Functions
    public void Start()
    {
        selectAreaObjects = new List<GameObject>();

        selectPathObjects = new List<GameObject>();
        SubscribeButtons();

        DisplayAreasScreen();

        pnlWarningScreen.SetActive(false);
        pnlWarningSavePathScreen.SetActive(false);
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

        //btn Path
        btnAddNewPath.onClick.AddListener(() => AddNewPath());

        //btn warning on area
        btnCancel.onClick.AddListener(() => CloseScreenPanels());

        //btn warning panel for save or cancel a path
        btnSave.onClick.AddListener(() =>SavePath());
        btnSaveCancel.onClick.AddListener(() => CancelInGeneral());

        //for testing the saving of paths is happening smoothly
        btnPaths.onClick.AddListener(() => DisplayPathsScreen());
        btnCancelShow.onClick.AddListener(() => CancelInGeneral());

        
    }

    public void DisplayAreasScreen()
    {
        pnlAreasScreen.SetActive(true);
        DestroySelectAreaObjects(selectAreaObjects);
        selectAreaObjects = InstantiateSelectAreaObjects();
        StartCoroutine(ReloadLayout(pnlLoadedAreas));
        createArea = false;
        EnableScreen(pnlPathScreen, false);
        imgRecord.gameObject.SetActive(false);
        EnableScreen(pnlSavedPaths, false);//the panel for saved paths can be removed afterwards, for testing purposes
        
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

        if (areas != null && areas.Count > 0)
        {
            foreach (cArea area in areas)
            {
                GameObject newSelectArea = Instantiate(selectAreaPrefab, Vector3.zero, Quaternion.identity, pnlLoadedAreas.GetComponent<RectTransform>());
                //newSelectArea.transform.SetAsFirstSibling();
                TMP_Text selectAreaText = newSelectArea.GetComponentInChildren<TMP_Text>();
                selectAreaText.text = area.title;

                Button btnSelectArea;
                Button btnDeleteArea;
                foreach (Transform child in newSelectArea.transform)
                {
                    if (child.name.Equals("pnlSelectArea"))
                    {
                        btnSelectArea = child.GetComponentInChildren<Button>();
                        btnSelectArea.onClick.AddListener(OnAreaSelectPressed);
                    }

                    if (child.name.Equals("btnDeleteArea"))
                    {
                        btnDeleteArea = child.GetComponent<Button>();
                        btnDeleteArea.onClick.AddListener(OnAreaDeletePressed);
                    }
                }

                //Button btnSelectArea = newSelectArea.GetComponentInChildren<Button>();
                //btnSelectArea.onClick.AddListener(OnAreaSelectPressed);
                newSelectAreaObjects.Add(newSelectArea);
            }
        }

        return newSelectAreaObjects;
    }

    private void OnAreaSelectPressed()
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
        EnableScreen(pnlPathScreen, true);
        imgRecord.gameObject.SetActive(true);
        EnableScreen(pnlSavedPaths,false);//the panel for saved paths can be removed afterwards, for testing purposes
        AppManager.Instance.mapManager.CheckUserPosition();
    }

    private void OnAreaDeletePressed()
    {
        GameObject btnDeleteArea = EventSystem.current.currentSelectedGameObject;
        string areaTitle = string.Empty;
        Transform pnlSelectArea = btnDeleteArea.transform.parent;
        //Debug.Log(pnlSelectArea.name);
        /*foreach (Transform child in pnlSelectArea)
        {
            if (child.name.Equals("pnlSelectArea"))
            {
                TMP_Text btnSelectAreaText = child.GetComponentInChildren<TMP_Text>();
                areaTitle = btnSelectAreaText.text;
            }
        }*/
        
        //Debug.Log(areaTitle);
        AppManager.Instance.mapManager.DeleteArea(selectAreaObjects.IndexOf(pnlSelectArea.gameObject)); // areaTitle
        DisplayAreasScreen();
    }

    private void OnPathSelectPressed()
    {
        Debug.Log("Path selected");
        GameObject selectPathObject = EventSystem.current.currentSelectedGameObject;
        TMP_Text selectPathText = selectPathObject.GetComponentInChildren<TMP_Text>();
        Debug.Log("OnPathSelectPressed");
        cPath selectedPath = AppManager.Instance.mapManager.GetPathByTitle(selectPathText.text);
        //AppManager.Instance.mapManager.currentPath = selectedPath;
        
        if (selectedPath != null)
        {
            pnlScrollViewPaths.SetActive(false);
            pnlSavedPaths.SetActive(false);
            AppManager.Instance.mapManager.DisplayPath(selectedPath);
        }
        //EnableScreen(pnlSelectedAreaScreen, true);
        //imgRecord.gameObject.SetActive(true);
        //EnableScreen(pnlSavedPaths, false);//the panel for saved paths can be removed afterwards, for testing purposes

        //AppManager.Instance.mapManager.CheckUserPosition();
    }

    private void OnPathDeletePressed()
    {
        GameObject btnDeleteArea = EventSystem.current.currentSelectedGameObject;

        Transform pnlSelectArea = btnDeleteArea.transform.parent;

        AppManager.Instance.mapManager.DeletePath(selectPathObjects.IndexOf(pnlSelectArea.gameObject));
        DisplayPathsScreen();
    }

    private void DestroySelectAreaObjects(List<GameObject> _selectObjects)
    {
        if (_selectObjects != null)
        {
            foreach (GameObject selectArea in _selectObjects)
            {
                Destroy(selectArea);
            }

            _selectObjects.Clear();
        }
    }

    IEnumerator ReloadLayout(GameObject _layoutGameObject)
    {
        yield return new WaitForSeconds(interval);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutGameObject.GetComponent<RectTransform>());
    }

    private void BackToAreasScreen()
    {
        if (!AppManager.Instance.mapManager.isRecordingPath)
        {
            DisplayAreasScreen();
            pnlWarningSavePathScreen.SetActive(false);
            pnlWarningScreen.SetActive(false);
            AppManager.Instance.mapManager.RemoveMarkersAndLine();
        }
        else
        {
            //if we press back button whilst "recording" path, to get pnlWarningSave enabled.
            SaveUIButton();
        }
        
        //mapScreen.SetActive(false);
    }

    void IsInRecordingPath(bool val)
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
            //manual path creation after pressing the plus button!(for testing purposes mostly)
            /*if (AppManager.Instance.mapManager.isRecordingPath)
            {
                // Get the coordinates under the cursor.
                double lng, lat;
                OnlineMapsControlBase.instance.GetCoords(out lng, out lat);

                Vector2 position = new Vector2((float)lng, (float)lat);

                // Calculate the distance in km between locations.
                float distance = OnlineMapsUtils.DistanceBetweenPoints(position, AppManager.Instance.mapManager.previousPosition).magnitude;

                //Debug.LogWarning("dist = " + distance);
                // Debug.LogWarning("minDistanceToPutNewMarker = " + minDistanceToPutNewMarker);

                if (distance < AppManager.Instance.mapManager.minDistanceToPutNewMarker)
                {
                    return;
                }
                else
                {
                    AppManager.Instance.mapManager.previousPosition = position;
                }

                // Create a label for the marker.
                string label = "New Path " + (OnlineMaps.instance.markers.Length + 1);

                OnlineMapsMarker marker = new OnlineMapsMarker();

                marker.label = label;
                marker.SetPosition(lng, lat);

                // Create a new marker.
                OnlineMapsMarkerManager.CreateItem(lng, lat, label);

                AppManager.Instance.mapManager.markerListCurrPath.Add(marker);


                OnlineMapsMarkerManager.instance.Remove(marker);
                OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingLine(OnlineMapsMarkerManager.instance.Select(m => m.position).ToArray(), Color.red, 3));
                //OnlineMapsDrawingElementManager.AddItem(path);
                OnlineMaps.instance.Redraw();
            }*/
        }

    }

    #region PathPanel
    //changes icon from plus to save icon, listener changes to next method for saving path, here also have the drawing?
    private void AddNewPath()
    {
        //check if user or app for some reason location services are off, enable appropriate panel(when build remove comments)
        /*if (AppManager.Instance.androidManager.CheckForLocationServices())
        {
            EnableScreen(pnlGPSScreen, true);
            //infoText.text = "Add New Path on location Services";//testing
            return;
        }*/
        
        AppManager.Instance.mapManager.RemoveMarkersAndLine();

        btnAddNewPath.GetComponentInChildren<Text>().text = "Save"; // sprSaveIcon;
        IsInRecordingPath(true);

        btnAddNewPath.onClick.RemoveAllListeners();
        btnAddNewPath.onClick.AddListener(() => SaveUIButton());

        AppManager.Instance.mapManager.StartRecordingPath();
        btnPaths.interactable = false;
    }

    //change the icon from plus to save, opens warning screen for saving or cancel path
    private void SaveUIButton()
    {
        EnableScreen(pnlWarningSavePathScreen, true);
        AppManager.Instance.mapManager.StopRecordingPath();
        btnPaths.interactable = true;
        //btnAddNewPath.GetComponentInChildren<Text>().text = "Save";// sprSaveIcon;
        //btnAddNewPath.onClick.AddListener(() => SavePath());
    }

    //when save button is pressed on warning screen, the save icon changes back to plus icon. Warning screen is deactivated and listener goes to original method
    private void SavePath()
    {
        AppManager.Instance.mapManager.SavePath();
        btnAddNewPath.GetComponentInChildren<Text>().text = "Add"; // sprAddNewPath;

        btnAddNewPath.onClick.RemoveAllListeners();
        btnAddNewPath.onClick.AddListener(() => AddNewPath());

        EnableScreen(pnlWarningSavePathScreen, false);
        IsInRecordingPath(false);
        btnPaths.interactable = true;
        //AppManager.Instance.mapManager.isRecordingPath = false;
    }
    //the panel for saved paths can be removed afterwards, for testing purposes
    void DisplayPathsScreen()
    {
        pnlSavedPaths.SetActive(true);
        DestroySelectAreaObjects(selectPathObjects);
        selectPathObjects = InstantiateSelectPathObjects();
        StartCoroutine(ReloadLayout(pnlSavedPaths));
        pnlScrollViewPaths.SetActive(true);
        AppManager.Instance.mapManager.RemoveMarkersAndLine();
    }
    #endregion


    #region Warnings
    //to close main warning screen for area check
    private void CloseScreenPanels()
    {
        if (pnlWarningScreen.activeSelf)
            pnlWarningScreen.SetActive(false);
    }

    //to close path save plus remove everything from the map
    private void CancelInGeneral()
    {
        if (pnlWarningSavePathScreen.activeSelf)
        {
            btnAddNewPath.GetComponentInChildren<Text>().text = "Add";// sprAddNewPath;

            btnAddNewPath.onClick.RemoveAllListeners();
            btnAddNewPath.onClick.AddListener(() => AddNewPath());

            AppManager.Instance.mapManager.RemoveMarkersAndLine();

            EnableScreen(pnlWarningSavePathScreen, false);
            IsInRecordingPath(false);
        }

        //the panel for saved paths can be removed afterwards, for testing purposes
        if (pnlSavedPaths.activeInHierarchy)
        {
            pnlSavedPaths.SetActive(false);
        }
    }

    //instantiating paths
    private List<GameObject> InstantiateSelectPathObjects()
    {
        //Debug.Log("Instantiate Paths");
        List<GameObject> newPathPrefab = new List<GameObject>();
        //Debug.Log("on Instantiate Paths, prefab: " + newPathPrefab.ToString());
        List<cPath> paths = AppManager.Instance.mapManager.GetPaths();
        //Debug.Log("on Instantiate Paths, paths" + paths.ToString());

        foreach (cPath path in paths)
        {
            //Debug.Log("insta 1st foreach");
            GameObject newSelectPath = Instantiate(btnShowPath, Vector3.zero, Quaternion.identity, pnlScrollViewPaths.GetComponent<RectTransform>());
            //newSelectPath.transform.SetAsFirstSibling();
            TMP_Text selectPathText = newSelectPath.GetComponentInChildren<TMP_Text>();
            selectPathText.text = path.title;

            Button btnSelectPath;
            Button btnDeletePath;
            foreach (Transform child in newSelectPath.transform) // Fix Menu
            {
                if (child.name.Equals("pnlSelectArea"))
                {
                    btnSelectPath = child.GetComponentInChildren<Button>();
                    //Debug.Log("insta button");
                    btnSelectPath.onClick.AddListener(OnPathSelectPressed);
                }

                if (child.name.Equals("btnDeleteArea"))
                {
                    btnDeletePath = child.GetComponent<Button>();
                    btnDeletePath.onClick.AddListener(OnPathDeletePressed);
                }
            }

            newPathPrefab.Add(newSelectPath);
        }

        return newPathPrefab;

    }

    void DebugButton()
    {
        Debug.Log("button pressed");
    }
    #endregion


    #endregion


}
