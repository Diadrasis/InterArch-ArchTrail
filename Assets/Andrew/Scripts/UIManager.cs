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
    Transform pnlForEdit;
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
    public Button btnAddNewPath, btnStopRecording, btnResumeRecording;
    public Image imgOnRecord, imgPauseRecording;
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
    public Button btnSave, btnContinue, btnSaveCancel;

    [Space]
    [Header("Warning Delete")]
    //Warning screen when user is about to delete
    public GameObject pnlWarningDeleteScreen;
    public Button btnDeleteFinal, btnDeleteCancel;
    Transform pnlForDelete;

    [Space]
    [Header("Warning Server Screen")]
    public GameObject pnlWarningServerScreen;
    public TextMeshProUGUI txtWarningServer;

    [Space]
    [Header("Warning Internet Screen")]
    public GameObject pnlWarningInternetScreen;
    public Button btnCloseInternetScreen;

    [Space]
    [Header("Warning Tiles Screen")]
    public GameObject pnlWarningDownloadTilesScreen;
    public TextMeshProUGUI txtWarningDownloadTiles;
    public Button btnDownloadTilesYes;
    public Button btnDownloadTilesNo;

    [Space]
    [Header("Testing Purposes")]
    public Texture2D userMarker;
    public TextMeshProUGUI infoText;
    public Button btnPaths, btnCancelShow, btnUploadServer, btnDownloadServer;
    public GameObject pnlSavedPaths, btnShowPath, pnlScrollViewPaths;
    private List<GameObject> selectPathObjects;

    private readonly string DEFAULT_TEXT_NAME = "";

    [Space]
    [Header("Loading Screen")]
    public Image imgLoading;

    [Space]
    [Header("Profiler Screen")]
    public GameObject pnlQuestionnaireScreen;
    public GameObject pnlMainQuestions;
    public GameObject pnlOptionA;
    public GameObject pnlOptionB;
    public GameObject pnlOptionB1;
    public GameObject pnlOptionB2;
    public GameObject pnlOptionC;
    public GameObject pnlOptionC1;
    public GameObject pnlOptionC2;
    public Button btnProfiler;
	public Button btnResetQuestionnaire;
	
    [HideInInspector]
    public bool downloadTiles = false;
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
        pnlQuestionnaireScreen.SetActive(false);
        txtMainName.text = DEFAULT_TEXT_NAME;
        imgPauseRecording.gameObject.SetActive(false);
        btnBackToAreasScreen.interactable = true;
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
        btnResumeRecording.onClick.AddListener(()=> ResumePath());

        //btn warning on area
        btnCancel.onClick.AddListener(() => CloseScreenPanels());

        //btn on delete warning cancel or final delete
        btnDeleteCancel.onClick.AddListener(() => CloseScreenPanels());
        btnDeleteFinal.onClick.AddListener(() => DeleteFinal());

        //btn warning panel for save or cancel a path
        btnSave.onClick.AddListener(() =>SavePath());
        btnContinue.onClick.AddListener(() => ResumePath());
        btnSaveCancel.onClick.AddListener(() => CancelInGeneral());

        // for testing the saving of paths is happening smoothly
        btnPaths.onClick.AddListener(() => DisplaySavedPathsScreen());
        btnCancelShow.onClick.AddListener(() => CancelInGeneral());

        // warning DownloadTiles
        btnDownloadTilesYes.onClick.AddListener(() => SetDownloadTiles(true));
        btnDownloadTilesNo.onClick.AddListener(() => SetDownloadTiles(false));

        //btn for edit Area
        btnEditAreaCancel.onClick.AddListener(() => CancelInGeneral());
        //btnEditAreaSave.onClick.AddListener(()=> )

        //btn for profiler screen
        btnProfiler.onClick.AddListener(() => DisplayQuestionnaire());
        btnResetQuestionnaire.onClick.AddListener(() => AppManager.Instance.questionnaireManager.ResetValues());


        //btn for close Internet screen
        btnCloseInternetScreen.onClick.AddListener(() => CancelInGeneral());


    }
	
    private void SetDownloadTiles(bool _value)
    {
        downloadTiles = _value;
        pnlWarningDownloadTilesScreen.SetActive(false);
    }
	
    void ActivateButtons(bool valBack, bool valQuit, bool valReset, bool valProfiler)
    {
        btnBackToAreasScreen.gameObject.SetActive(valBack);
        btnQuit.gameObject.SetActive(valQuit);
        btnProfiler.gameObject.SetActive(valProfiler);
        btnResetQuestionnaire.gameObject.SetActive(valReset);
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
        ActivateButtons(false,true, false, true);
        txtMainName.text = DEFAULT_TEXT_NAME;
        pnlWarningDeleteScreen.SetActive(false);
        AppManager.Instance.questionnaireManager.ResetValues();
    }

    public void EnableScreen(GameObject _screenToEnable, bool _valid) // CURRENTLY IN USE
    {
        if (_valid)
            _screenToEnable.SetActive(true);
        else
            _screenToEnable.SetActive(false);
    }

    private void Escape()
    {
        Application.Quit();
        OnlineMapsLocationService.instance.StopLocationService();
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
                        btnEditArea.onClick.AddListener(OnEditSelect);
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
        GameObject selectAreaObject = EventSystem.current.currentSelectedGameObject.transform.parent.transform.parent.gameObject;
        //TMP_Text selectAreaText = selectAreaObject.GetComponentInChildren<TMP_Text>();

        //cArea selectedArea = AppManager.Instance.mapManager.GetAreaByTitle(selectAreaText.text);
        cArea selectedArea = AppManager.Instance.mapManager.GetAreaByIndex(selectAreaObjects.IndexOf(selectAreaObject.gameObject));

        if (selectedArea != null)
        {
            AppManager.Instance.mapManager.currentArea = selectedArea;

            // Download Paths of Area
            if (selectedArea.server_area_id != -1)
            {
                AppManager.Instance.serverManager.DownloadPaths(selectedArea.server_area_id);
            }

            txtMainName.text = selectedArea.title; // selectAreaText.text;
            pnlAreasScreen.SetActive(false);
            AppManager.Instance.mapManager.SetMapViewToArea(selectedArea);
        }

        EnableScreen(pnlPathScreen, true);
        imgRecord.gameObject.SetActive(true);
        EnableScreen(pnlSavedPaths, false); //the panel for saved paths can be removed afterwards, for testing purposes
        ActivateButtons(true,false, false, false);
        AppManager.Instance.mapManager.CheckUserPosition();
    }

    private void OnAreaDeletePressed()
    {
        pnlWarningDeleteScreen.SetActive(true);
        GameObject btnDeleteArea = EventSystem.current.currentSelectedGameObject;
        //string areaTitle = string.Empty;
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

    //to edit an area
    private void OnEditSelect()
    {
        // Get selected area
        GameObject btnEditArea = EventSystem.current.currentSelectedGameObject;
        pnlForEdit = btnEditArea.transform.parent;
        cArea selectedArea = AppManager.Instance.mapManager.areas[selectAreaObjects.IndexOf(pnlForEdit.gameObject)];

        // Change UI
        pnlEditArea.SetActive(true);
        pnlAreasScreen.SetActive(false);
        txtMainName.text = selectedArea.title;
        btnEditAreaSave.onClick.AddListener(EditSaveArea);
        btnSaveEditArea.interactable = true;
        btnSaveEditArea.onClick.AddListener(() => EnableScreen(pnlSaveEditArea, true));
        inptFldEditArea.text = selectedArea.title;
        ActivateButtons(true, true, false, false);

        // Start edit selected area
        AppManager.Instance.mapManager.StartEditArea(selectedArea);
    }

    private void OnPathSelectPressed()
    {
        GameObject selectPathObject = EventSystem.current.currentSelectedGameObject.transform.parent.transform.parent.gameObject;
        //TMP_Text selectPathText = selectPathObject.GetComponentInChildren<TMP_Text>();
        // Get index
        //Debug.Log("OnPathSelectPressed");
        cPath selectedPath = AppManager.Instance.mapManager.GetPathByIndex(selectPathObjects.IndexOf(selectPathObject.gameObject));  //AppManager.Instance.mapManager.GetPathByTitle(selectPathText.text); // AppManager.Instance.mapManager.currentArea[selectAreaObjects.IndexOf(selectPathObject.gameObject)]; 
        //AppManager.Instance.mapManager.currentPath = selectedPath;
        if (selectedPath != null)
        {
            // Download Points of Path
            if (selectedPath.server_path_id != -1)
            {
                AppManager.Instance.serverManager.DownloadPoints(selectedPath.server_path_id); // TODO: Wait until download is finished to display path
            }

            pnlScrollViewPaths.SetActive(false);
            pnlSavedPaths.SetActive(false);
            //if (selectedPath.pathPoints.Count > 0)
            AppManager.Instance.mapManager.DisplayPath(selectedPath);
        }
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
        if (pnlSavedPaths.activeSelf && !pnlAreasScreen.activeSelf && pnlPathScreen.activeSelf &&
            !pnlSaveArea.activeSelf && !pnlEditArea.activeSelf && !pnlCreateArea.activeSelf && !AppManager.Instance.mapManager.isRecordingPath && !AppManager.Instance.mapManager.isPausePath)
        {
            pnlSavedPaths.SetActive(false);
            AppManager.Instance.mapManager.RemoveMarkersAndLine();
        }

        else if (!pnlSavedPaths.activeSelf && !pnlAreasScreen.activeSelf && pnlPathScreen.activeSelf &&
            !pnlSaveArea.activeSelf && !pnlEditArea.activeSelf && !pnlCreateArea.activeSelf && !AppManager.Instance.mapManager.isRecordingPath && !AppManager.Instance.mapManager.isPausePath)
        {
            DisplayAreasScreen();
            AppManager.Instance.serverManager.DownloadAreas();
            pnlPathScreen.SetActive(false);
            AppManager.Instance.mapManager.RemoveMarkersAndLine();
        }
        //pnlAreasScreen.SetActive(false);
        else if (!pnlSavedPaths.activeSelf && !pnlAreasScreen.activeSelf && !pnlPathScreen.activeSelf &&
            pnlSaveArea.activeSelf && !pnlEditArea.activeSelf && pnlCreateArea.activeSelf && 
            !AppManager.Instance.mapManager.isRecordingPath && !AppManager.Instance.mapManager.isPausePath &&
          !pnlQuestionnaireScreen.activeSelf && !pnlOptionA.activeSelf && !pnlOptionB.activeSelf && !pnlOptionC.activeSelf)
        {
            pnlSaveArea.SetActive(false);
            pnlCreateArea.SetActive(false);
            DisplayAreasScreen();
            AppManager.Instance.serverManager.DownloadAreas();
            //Debug.Log("pnlCreateArea false");
            AppManager.Instance.mapManager.RemoveMarkersAndLine();
        }
        else if (!pnlSavedPaths.activeSelf && !pnlAreasScreen.activeSelf && pnlPathScreen.activeSelf &&
            !pnlSaveArea.activeSelf && !pnlEditArea.activeSelf && !pnlCreateArea.activeSelf && 
            (AppManager.Instance.mapManager.isRecordingPath || AppManager.Instance.mapManager.isPausePath) &&
          !pnlQuestionnaireScreen.activeSelf && !pnlOptionA.activeSelf && !pnlOptionB.activeSelf && !pnlOptionC.activeSelf)
        {
            SaveUIButton();
        }
        else if (!pnlSavedPaths.activeSelf && !pnlAreasScreen.activeSelf && pnlPathScreen.activeSelf &&
            !pnlSaveArea.activeSelf && !pnlEditArea.activeSelf && !pnlCreateArea.activeSelf && 
            !AppManager.Instance.mapManager.isRecordingPath && !AppManager.Instance.mapManager.isPausePath &&
          !pnlQuestionnaireScreen.activeSelf && !pnlOptionA.activeSelf && !pnlOptionB.activeSelf && !pnlOptionC.activeSelf)
        {
            DisplaySavedPathsScreen();
        }
        else if (!pnlSavedPaths.activeSelf && !pnlAreasScreen.activeSelf && !pnlPathScreen.activeSelf &&
            !pnlSaveArea.activeSelf && pnlEditArea.activeSelf && !pnlCreateArea.activeSelf &&
            !AppManager.Instance.mapManager.isRecordingPath && !AppManager.Instance.mapManager.isPausePath &&
          !pnlQuestionnaireScreen.activeSelf && !pnlOptionA.activeSelf && !pnlOptionB.activeSelf && !pnlOptionC.activeSelf)
        {
            //to open edit panel
            pnlEditArea.SetActive(false);
            AppManager.Instance.mapManager.RemoveMarkersAndLine();
            DisplayAreasScreen();
            AppManager.Instance.serverManager.DownloadAreas();
        }
        else if (!pnlSavedPaths.activeSelf && !pnlAreasScreen.activeSelf && !pnlPathScreen.activeSelf &&
            !pnlSaveArea.activeSelf && !pnlEditArea.activeSelf && !pnlCreateArea.activeSelf &&
            !AppManager.Instance.mapManager.isRecordingPath && !AppManager.Instance.mapManager.isPausePath && pnlQuestionnaireScreen.activeSelf &&
            !pnlOptionA.activeSelf && !pnlOptionB.activeSelf && !pnlOptionC.activeSelf)
        {
            pnlQuestionnaireScreen.SetActive(false);
            DisplayAreasScreen();
            AppManager.Instance.questionnaireManager.ResetValues();
            AppManager.Instance.serverManager.DownloadAreas();
        }
        else if (!pnlSavedPaths.activeSelf && !pnlAreasScreen.activeSelf && !pnlPathScreen.activeSelf &&
           !pnlSaveArea.activeSelf && !pnlEditArea.activeSelf && !pnlCreateArea.activeSelf &&
           !AppManager.Instance.mapManager.isRecordingPath && !AppManager.Instance.mapManager.isPausePath &&
           pnlQuestionnaireScreen.activeSelf && pnlOptionA.activeSelf && !pnlOptionB.activeSelf && !pnlOptionC.activeSelf)
        {
            pnlQuestionnaireScreen.SetActive(false);
            DisplayAreasScreen();
            pnlOptionA.SetActive(false);
            AppManager.Instance.questionnaireManager.ResetValues();
            AppManager.Instance.serverManager.DownloadAreas();
        }
        else if (!pnlSavedPaths.activeSelf && !pnlAreasScreen.activeSelf && !pnlPathScreen.activeSelf &&
           !pnlSaveArea.activeSelf && !pnlEditArea.activeSelf && !pnlCreateArea.activeSelf &&
           !AppManager.Instance.mapManager.isRecordingPath && !AppManager.Instance.mapManager.isPausePath &&
           pnlQuestionnaireScreen.activeSelf && !pnlOptionA.activeSelf && pnlOptionB.activeSelf && !pnlOptionC.activeSelf)
        {
            pnlQuestionnaireScreen.SetActive(false);
            DisplayAreasScreen();
            pnlOptionB.SetActive(false);
            AppManager.Instance.questionnaireManager.ResetValues();
            AppManager.Instance.serverManager.DownloadAreas();

        }
        else if (!pnlSavedPaths.activeSelf && !pnlAreasScreen.activeSelf && !pnlPathScreen.activeSelf &&
          !pnlSaveArea.activeSelf && !pnlEditArea.activeSelf && !pnlCreateArea.activeSelf &&
          !AppManager.Instance.mapManager.isRecordingPath && !AppManager.Instance.mapManager.isPausePath &&
          pnlQuestionnaireScreen.activeSelf && !pnlOptionA.activeSelf && !pnlOptionB.activeSelf && pnlOptionC.activeSelf)
        {
            pnlQuestionnaireScreen.SetActive(false);
            DisplayAreasScreen();
            pnlOptionC.SetActive(false);
            AppManager.Instance.questionnaireManager.ResetValues();
            AppManager.Instance.serverManager.DownloadAreas();
        }
        else if (!pnlSavedPaths.activeSelf && !pnlAreasScreen.activeSelf && !pnlPathScreen.activeSelf &&
         !pnlSaveArea.activeSelf && !pnlEditArea.activeSelf && !pnlCreateArea.activeSelf &&
         !AppManager.Instance.mapManager.isRecordingPath && !AppManager.Instance.mapManager.isPausePath &&
         pnlQuestionnaireScreen.activeSelf && pnlMainQuestions.activeSelf&& !pnlOptionA.activeSelf && !pnlOptionB.activeSelf && !pnlOptionC.activeSelf)
        {
            pnlQuestionnaireScreen.SetActive(false);
            DisplayAreasScreen();
            pnlMainQuestions.SetActive(false);
            AppManager.Instance.questionnaireManager.ResetValues();
            AppManager.Instance.serverManager.DownloadAreas();
        }
        else
        {
            DisplayAreasScreen();
            EnableScreen(pnlQuestionnaireScreen, false);
            AppManager.Instance.questionnaireManager.ResetValues();
            AppManager.Instance.serverManager.DownloadAreas();
        }

        //mapScreen.SetActive(false);
    }

    public void IsInRecordingPath(bool val)
    {
        imgRecord.SetBool("isPlaying", val);
    }

    private void CreateNewAreaSelected()
    {
        pnlAreasScreen.SetActive(false);
        pnlCreateArea.SetActive(true);
        btnSaveArea.interactable = false;

        inptFldCreateArea.text = "";

        ActivateButtons(true,false, false, false);
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
            DisplayAreasScreen();
        }
        else
        {
            pnlWarningScreen.SetActive(true);
            txtWarning.text = "Please enter a valid name";
        }
    }

    public void EditSaveArea()
    {
        string newAreaTitle = inptFldEditArea.text;

        if (!string.IsNullOrEmpty(newAreaTitle))
        {
            AppManager.Instance.mapManager.EditArea(AppManager.Instance.mapManager.currentArea, newAreaTitle);
            pnlSaveEditArea.SetActive(false);
            pnlEditArea.SetActive(false);
            txtMainName.text = newAreaTitle;
            DisplayAreasScreen();
        }
        else
        {
            pnlWarningScreen.SetActive(true);
            txtWarning.text = "Please enter a valid name";
        }
    }

    #region PathPanel
    //changes icon from plus to save icon, listener changes to next method for saving path, here also have the drawing?
    private void AddNewPath()
    {
        //check if user or app for some reason location services are off, enable appropriate panel(when build remove comments)
       /* if (AppManager.Instance.androidManager.CheckForLocationServices())
        {
            EnableScreen(pnlGPSScreen, true);
            return;
        }*/

        AppManager.Instance.mapManager.RemoveMarkersAndLine();
        AppManager.Instance.mapManager.StartRecordingPath();
        //btnPaths.interactable = false;
        pnlRecordButton.SetActive(true);
        imgOnRecord.gameObject.SetActive(true);
        imgPauseRecording.gameObject.SetActive(false);
        IsInRecordingPath(true);
        pnlMainButtons.SetActive(false);
    }

    //stops the anim of recording, opens warning screen for saving, continue or cancel path
    private void SaveUIButton()
    {
        EnableScreen(pnlWarningSavePathScreen, true);
        pnlRecordButton.SetActive(true);
        imgOnRecord.gameObject.SetActive(false);
        imgPauseRecording.gameObject.SetActive(true);
        IsInRecordingPath(false);
        AppManager.Instance.mapManager.PauseRecordingPath();
        
    }

    //when save button is pressed on warning screen and recording, pausing goes to false. Same with anim recording
    private void SavePath()
    {
        AppManager.Instance.mapManager.SavePath();
               
        EnableScreen(pnlWarningSavePathScreen, false);
        pnlRecordButton.SetActive(false);
        pnlMainButtons.SetActive(true);
        IsInRecordingPath(false);
        AppManager.Instance.mapManager.StopRecordingPath();
        //pnlProfilerScreen.SetActive(true);
        OpenQuestionnaire();
    }

    //opens the saved paths screen (on click event)
    void DisplaySavedPathsScreen()
    {
        /*// Download Paths of Area
        if (AppManager.Instance.mapManager.currentArea.server_area_id != -1)
        {
            AppManager.Instance.serverManager.DownloadPaths(AppManager.Instance.mapManager.currentArea.server_area_id);
        }*/
        pnlSavedPaths.SetActive(true);
        DestroySelectAreaObjects(selectPathObjects);
        selectPathObjects = InstantiateSelectPathObjects();
        StartCoroutine(ReloadLayout(pnlSavedPaths));
        pnlScrollViewPaths.SetActive(true);
        ActivateButtons(true,false, false, false);
        AppManager.Instance.mapManager.RemoveMarkersAndLine();
        pnlWarningDeleteScreen.SetActive(false);
    }

    //when you pause the path and you resume the recording of the path
    void ResumePath()
    {
        //Debug.Log("Resume Path bool: "+AppManager.Instance.mapManager.isRecordingPath);
        AppManager.Instance.mapManager.ResumeRecordingPath();
        pnlRecordButton.SetActive(true);
        imgOnRecord.gameObject.SetActive(true);
        imgPauseRecording.gameObject.SetActive(false);
        IsInRecordingPath(true);
        pnlMainButtons.SetActive(false);
        if (pnlWarningSavePathScreen.activeSelf) pnlWarningSavePathScreen.SetActive(false);
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
            AppManager.Instance.mapManager.RemoveMarkersAndLine();
            EnableScreen(pnlWarningSavePathScreen, false);
            EnableScreen(pnlMainButtons, true);
            AppManager.Instance.mapManager.StopRecordingPath();
        }

        //the panel for saved paths
        if (pnlSavedPaths.activeInHierarchy)
        {
            pnlSavedPaths.SetActive(false);
        }

        if (pnlSaveEditArea.activeInHierarchy)
        {
            pnlSaveEditArea.SetActive(false);
        }

        if (pnlWarningInternetScreen.activeInHierarchy)
        {
            pnlWarningInternetScreen.SetActive(false);
        }
    }
    //final warning on delete
    public void DeleteFinal()
    {
        if (pnlPathScreen.activeSelf)
        {
            AppManager.Instance.mapManager.DeletePath(selectPathObjects.IndexOf(pnlForDelete.gameObject));
            DisplaySavedPathsScreen();
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
            Button btnEditPath;
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
                if (child.name.Equals("btnEditArea"))
                {
                    btnEditPath = child.GetComponent<Button>();
                    btnEditPath.gameObject.SetActive(false);
                }
            }

            newPathPrefab.Add(newSelectPath);
        }

        return newPathPrefab;

    }

    //when uploading/loading or updating from and to server, to show info (needs fixing)
    /*public void LoadingScreen(string txtMessage, float timeAmount, float speedAmount)
    {
        txtLoading.text = txtMessage;
        speedAmount += Time.deltaTime;
        if (speedAmount > 0)
        {
            timeAmount = Mathf.Lerp(0, 1, speedAmount);
            imgLoading.fillAmount = timeAmount;
        }
        else
        {
            imgLoading.fillAmount = 0;
        }
        
    }*/
    #endregion

    #region QuestionnairePanel
    void DisplayQuestionnaire()
    {
        pnlQuestionnaireScreen.SetActive(true);
        pnlMainQuestions.SetActive(true);
        pnlAreasScreen.SetActive(false);
        ActivateButtons(true, true, true, true);
    }

    void OpenQuestionnaire()
    {
        pnlQuestionnaireScreen.SetActive(true);
        pnlMainQuestions.SetActive(true);
        pnlOptionA.SetActive(false);
        pnlOptionB.SetActive(false);
        pnlOptionC.SetActive(false);
        pnlPathScreen.SetActive(false);
        ActivateButtons(true, true, true, false);
    }
    #endregion

    #region NotInUse
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

    /*void EditAreaMain(cArea _areaForEdit)
    {
        AppManager.Instance.mapManager.EditSelectedArea(_areaForEdit);
        
        ActivateButtons(false, true, true);
        txtMainName.text = _areaForEdit.title;
        
        Debug.Log("Edit Area Main"+ _areaForEdit.title);
    }*/

    //for testing purposes
    void DebugButton()
    {
        Debug.Log("button pressed");
    }
    #endregion


    #endregion
}
