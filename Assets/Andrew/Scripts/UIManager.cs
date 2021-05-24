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
    public Button btnBackToAreasScreen, btnQuit;
    public TextMeshProUGUI txtMainName;

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
    public GameObject pnlSaveArea;
    public TMP_InputField inptFldCreateArea;
    public Button btnCreateAreaSave;
    public Button btnCreateAreaCancel;
    public Button btnSaveArea;

    private List<GameObject> selectAreaObjects;
    private float interval = 0.001f;

    //private bool createArea = false;
    [Space]
    [Header("Edit Area Screen")]
    public GameObject pnlEditArea;
    public GameObject pnlSaveEditArea;
    public TMP_InputField inptFldEditArea;
    public Button btnEditAreaSave;
    public Button btnEditAreaCancel;
    public Button btnSaveEditArea;
    //public Button btnEditArea;

    [Space]
    [Header("GPS Screen")]
    //GPS Screen
    public GameObject pnlGPSScreen;
    public Button btnGPSPermission;

    [Space]
    [Header("Path Screen")]
    //PathScreen
    public GameObject pnlPathScreen, pnlRecordButton, pnlMainButtons;
    public Button btnAddNewPath, btnStopRecording;
    public Sprite sprAddNewPath, sprSaveIcon;
    public Animator imgRecord;

    [Space]
    [Header("Warning Area Screen")]
    //WarningScreen if user is near area
    public GameObject pnlWarningScreen;
    public TextMeshProUGUI txtWarning;
    public Button btnCancel;

    [Space]
    [Header("Warning Save Path Screen")]
    //WarningScreen when user is about to save the path
    public GameObject pnlWarningSavePathScreen;
    public Button btnSave, btnSaveCancel;
    [Space]
    [Header("Warning Delete")]
    //Warning screen when user is about to delete
    public GameObject pnlWarningDeleteScreen;
    public Button btnDeleteFinal, btnDeleteCancel;
    Transform pnlForDelete;

    [Space]
    [Header("Testing Purposes")]
    public Texture2D userMarker;
    public TextMeshProUGUI infoText;
    public Button btnPaths, btnCancelShow, btnUploadServer, btnDownloadServer;
    public GameObject pnlSavedPaths, btnShowPath, pnlScrollViewPaths;
    private List<GameObject> selectPathObjects;

    private readonly string DEFAULT_TEXT_NAME = "";
    #endregion

    #region Unity Functions
    public void Start()
    {
        selectAreaObjects = new List<GameObject>();

        selectPathObjects = new List<GameObject>();
        SubscribeButtons();

        //DisplayAreasScreen();

        pnlWarningScreen.SetActive(false);
        pnlWarningSavePathScreen.SetActive(false);
        txtMainName.text = DEFAULT_TEXT_NAME;
        //imgRecord = GetComponent<Animator>();
    }

    #endregion

    #region Methods
    private void SubscribeButtons()
    {
        // Map Screen
        btnBackToAreasScreen.onClick.AddListener(() => BackToAreasScreen());
        btnQuit.onClick.AddListener(() => Escape());

        // Areas Screen
        btnCreateArea.onClick.AddListener(() => CreateNewAreaSelected());

        // CreateAreaScreen
        btnSaveArea.onClick.AddListener(() => EnableScreen(pnlSaveArea, true));
        btnCreateAreaSave.onClick.AddListener(() => SaveArea());
        btnCreateAreaCancel.onClick.AddListener(() => RemoveNewArea());

        //btn GPS
        btnGPSPermission.onClick.AddListener(() => AppManager.Instance.androidManager.OpenNativeAndroidSettings());

        //btn Path and stop record
        btnAddNewPath.onClick.AddListener(() => AddNewPath());
        btnStopRecording.onClick.AddListener(() => SaveUIButton());

        //btn warning on area
        btnCancel.onClick.AddListener(() => CloseScreenPanels());

        //btn on delete warning cancel or final delete
        btnDeleteCancel.onClick.AddListener(() => CloseScreenPanels());
        btnDeleteFinal.onClick.AddListener(() => DeleteFinal());

        //btn warning panel for save or cancel a path
        btnSave.onClick.AddListener(() =>SavePath());
        btnSaveCancel.onClick.AddListener(() => CancelInGeneral());

        //for testing the saving of paths is happening smoothly
        btnPaths.onClick.AddListener(() => DisplayPathsScreen());
        btnCancelShow.onClick.AddListener(() => CancelInGeneral());

       /* //btn for edit Area
        btnEditArea.onClick.AddListener(() => EditArea());*/
    }
    void ActivateButtons(bool valPath, bool valBack, bool valQuit)
    {   
        btnPaths.gameObject.SetActive(valPath);
        btnBackToAreasScreen.gameObject.SetActive(valBack);
        btnQuit.gameObject.SetActive(valQuit);

    }
    public void DisplayAreasScreen()
    {
        pnlAreasScreen.SetActive(true);
        pnlCreateArea.SetActive(false);
        DestroySelectAreaObjects(selectAreaObjects);
        selectAreaObjects = InstantiateSelectAreaObjects();
        StartCoroutine(ReloadLayout(pnlLoadedAreas));
        AppManager.Instance.mapManager.CreateNewAreaFinalize();
        EnableScreen(pnlPathScreen, false);
        imgRecord.gameObject.SetActive(false);
        EnableScreen(pnlSavedPaths, false); //the panel for saved paths
        ActivateButtons(false, false,true);
        txtMainName.text = DEFAULT_TEXT_NAME;
        pnlWarningDeleteScreen.SetActive(false);
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
                Button btnEditArea;
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

                    if (child.name.Equals("btnEditArea"))
                    {
                        btnEditArea = child.GetComponent<Button>();
                        btnEditArea.onClick.AddListener(()=>EditAreaMain(area));
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
        txtMainName.text = selectAreaText.text;

        if (selectedArea != null)
        {
            pnlAreasScreen.SetActive(false);
            AppManager.Instance.mapManager.SetMapViewToArea(selectedArea);
        }
        EnableScreen(pnlPathScreen, true);
        imgRecord.gameObject.SetActive(true);
        EnableScreen(pnlSavedPaths, false); //the panel for saved paths can be removed afterwards, for testing purposes
        ActivateButtons(true,true,false);
        AppManager.Instance.mapManager.CheckUserPosition();
    }

    private void OnAreaDeletePressed()
    {
        pnlWarningDeleteScreen.SetActive(true);
        GameObject btnDeleteArea = EventSystem.current.currentSelectedGameObject;
        string areaTitle = string.Empty;
        pnlForDelete = btnDeleteArea.transform.parent;
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
        //AppManager.Instance.mapManager.DeleteArea(selectAreaObjects.IndexOf(pnlSelectArea.gameObject)); // areaTitle
        //DisplayAreasScreen();
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
        pnlWarningDeleteScreen.SetActive(true);
        GameObject btnDeleteArea = EventSystem.current.currentSelectedGameObject;
        pnlForDelete = btnDeleteArea.transform.parent;
        //AppManager.Instance.mapManager.DeletePath(selectPathObjects.IndexOf(pnlSelectArea.gameObject));
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

    //back to all the panel accordingly and even if we press back whilst recording path
    private void BackToAreasScreen()
    {
        if (pnlSavedPaths.activeSelf && !pnlAreasScreen.activeSelf && pnlPathScreen.activeSelf && !pnlSaveArea.activeSelf && !pnlEditArea.activeSelf && !pnlCreateArea.activeSelf && !AppManager.Instance.mapManager.isRecordingPath )
        {
            pnlSavedPaths.SetActive(false);
            AppManager.Instance.mapManager.RemoveMarkersAndLine();
        }

        else if (!pnlSavedPaths.activeSelf && !pnlAreasScreen.activeSelf && pnlPathScreen.activeSelf && !pnlSaveArea.activeSelf && !pnlEditArea.activeSelf && !pnlCreateArea.activeSelf && !AppManager.Instance.mapManager.isRecordingPath)
        {
            DisplayAreasScreen();
            pnlPathScreen.SetActive(false);
            AppManager.Instance.mapManager.RemoveMarkersAndLine();
        }
        //pnlAreasScreen.SetActive(false);
        else if (!pnlSavedPaths.activeSelf && !pnlAreasScreen.activeSelf && !pnlPathScreen.activeSelf && pnlSaveArea.activeSelf && !pnlEditArea.activeSelf && !pnlCreateArea.activeSelf && !AppManager.Instance.mapManager.isRecordingPath)
        {
            pnlSaveArea.SetActive(false);
            Debug.Log("pnlCreateArea false");
            AppManager.Instance.mapManager.RemoveMarkersAndLine();
        } 
        else if(!pnlSavedPaths.activeSelf && !pnlAreasScreen.activeSelf && pnlPathScreen.activeSelf && !pnlSaveArea.activeSelf && !pnlEditArea.activeSelf && !pnlCreateArea.activeSelf && AppManager.Instance.mapManager.isRecordingPath)
        { 
            SaveUIButton();
        }
        else if (!pnlSavedPaths.activeSelf && !pnlAreasScreen.activeSelf && pnlPathScreen.activeSelf && !pnlSaveArea.activeSelf && !pnlEditArea.activeSelf && !pnlCreateArea.activeSelf && !AppManager.Instance.mapManager.isRecordingPath)
        {
            DisplayPathsScreen();
        }
        else if (!pnlSavedPaths.activeSelf && !pnlAreasScreen.activeSelf && !pnlPathScreen.activeSelf && !pnlSaveArea.activeSelf && pnlEditArea.activeSelf && !pnlCreateArea.activeSelf && !AppManager.Instance.mapManager.isRecordingPath)
        {
            //to open edit panel
            pnlEditArea.SetActive(false);
            DisplayAreasScreen();
        }
        else 
        {
            DisplayAreasScreen();
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
        pnlCreateArea.SetActive(true);
        btnSaveArea.interactable = false;
        //mapScreen.SetActive(true);
        ActivateButtons(false,true,false);
        AppManager.Instance.mapManager.CreateNewAreaInitialize();
    }

    private void RemoveNewArea()
    {
        AppManager.Instance.mapManager.RemoveNewArea();
        btnSaveArea.interactable = false;
        EnableScreen(pnlSaveArea, false);
    }

    private void SaveArea()
    {
        string newAreaTitle = inptFldCreateArea.text;

        if (!string.IsNullOrEmpty(newAreaTitle))
        {
            AppManager.Instance.mapManager.SaveArea(newAreaTitle);

            pnlSaveArea.SetActive(false);
            pnlCreateArea.SetActive(false);
        }
    }

    private void EditSaveArea(cArea _areaToEdit)
    {
        string newAreaTitle = inptFldEditArea.text;

        if (!string.IsNullOrEmpty(newAreaTitle))
        {
            AppManager.Instance.mapManager.EditArea(_areaToEdit, newAreaTitle);

            pnlSaveEditArea.SetActive(false);
            pnlEditArea.SetActive(false);
        }
    }

    void EditAreaMain(cArea _areaForEdit)
    {
        AppManager.Instance.mapManager.EditSelectedArea(_areaForEdit);
        pnlEditArea.SetActive(true);
        pnlAreasScreen.SetActive(false);
        ActivateButtons(false, true, true);
        txtMainName.text = _areaForEdit.title;
        Debug.Log("Edit Area Main");
    }
    public void EnableSaveAreaScreen()
    {
        pnlSaveArea.SetActive(true);
    }

    /*public void OnMapClick()
    {
        if (createArea)
        {
            pnlCreateArea.SetActive(true);
        }
        else
        {
            //Debug.Log("Map Click!");

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
            }
        }
    }*/

    /*private void OnMapRelease()
    {

    }*/

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
        AppManager.Instance.mapManager.StartRecordingPath();
        btnPaths.interactable = false;
        pnlRecordButton.SetActive(true);
        IsInRecordingPath(true);
        pnlMainButtons.SetActive(false);
    }

    //change the icon from plus to save, opens warning screen for saving or cancel path
    private void SaveUIButton()
    {
        EnableScreen(pnlWarningSavePathScreen, true);
        pnlRecordButton.SetActive(false);
        pnlMainButtons.SetActive(true);
        AppManager.Instance.mapManager.StopRecordingPath();
        btnPaths.interactable = true;
        //btnAddNewPath.GetComponentInChildren<Text>().text = "Save";// sprSaveIcon;
        //btnAddNewPath.onClick.AddListener(() => SavePath());
    }

    //when save button is pressed on warning screen, the save icon changes back to plus icon. Warning screen is deactivated and listener goes to original method
    private void SavePath()
    {
        AppManager.Instance.mapManager.SavePath();
        /*btnAddNewPath.GetComponentInChildren<Text>().text = "Add"; // sprAddNewPath;

        btnAddNewPath.onClick.RemoveAllListeners();
        btnAddNewPath.onClick.AddListener(() => AddNewPath());*/
        
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
        ActivateButtons(true, true,false);
        AppManager.Instance.mapManager.RemoveMarkersAndLine();
        pnlWarningDeleteScreen.SetActive(false);
    }
    #endregion


    #region Warnings
    //to close main warning screen for area check
    private void CloseScreenPanels()
    {
        if (pnlWarningScreen.activeSelf && !pnlWarningDeleteScreen.activeSelf)
            pnlWarningScreen.SetActive(false);
        else
        {
            pnlWarningDeleteScreen.SetActive(false);
            pnlForDelete = null;
        }
            
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

    public void DeleteFinal()
    {
        if (pnlPathScreen.activeSelf)
        {
            AppManager.Instance.mapManager.DeletePath(selectPathObjects.IndexOf(pnlForDelete.gameObject));
            DisplayPathsScreen();
        }
        else if (pnlAreasScreen.activeSelf)
        {
            AppManager.Instance.mapManager.DeleteArea(selectAreaObjects.IndexOf(pnlForDelete.gameObject));
            DisplayAreasScreen();
        }
        
    }
    //instantiating paths
    private List<GameObject> InstantiateSelectPathObjects()
    {
        List<GameObject> newPathPrefab = new List<GameObject>();
        List<cPath> paths = AppManager.Instance.mapManager.GetPaths();
        
        foreach (cPath path in paths)
        {
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
    //for testing purposes
    void DebugButton()
    {
        Debug.Log("button pressed");
    }
    #endregion


    #endregion
}
