using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;
using System.Text.RegularExpressions;

public class UIManager : MonoBehaviour
{
    #region Variables
    public GameObject mapScreen;

    [Space]
    [Header("Top Screen")]
    //public Button btnBackToAreasScreen;
    public Button btnBack;
    public Button btnQuit;
    public Button btnOptions;
    public TextMeshProUGUI txtMainName;

    [Space]
    [Header("Options Screen")]
    // Options Screen
    public GameObject pnlOptionsScreen;
    public GameObject pnlOptions;
    public GameObject pnlLanguageScreen;
    public GameObject pnlAdminScreen;
    public GameObject pnlAreaSelectScreen;
    public GameObject pnlAboutScreen;
    public GameObject pnlSettingsScreen;
    public Button btnLanguageScreen;
    public Button btnAdminScreen;
    public Button btnAreaSelectScreen;
    public Button btnAboutScreen;
    public Button btnSettingsScreen;
    public GameObject pnlBtnLanguage;
    public GameObject pnlBtnAdmin;
    public GameObject pnlBtnAreaSelect;
    public GameObject pnlBtnAbout;
    public GameObject pnlBtnSettings;

    [Space]
    [Header("Language Screen")]
    public Button btnGreek;
    public Button btnEnglish;

    [Space]
    [Header("Admin Screen")]
    public GameObject pnlPassword;
    public TMP_InputField inptFldPasswordScreen;
    public Button btnPasswordLogin;
    public Button btnLogout;

    [Space]
    [Header("Areas Screen")]
    // Areas Screen
    public GameObject pnlAreasScreen;
    public GameObject pnlLoadedAreas;
    public GameObject selectAreaPrefab;
    public Button btnCreateArea;

    [Space]
    [Header("Settings Screen")]
    public Slider sldrDesiredAccuracy;
    public Slider sldrUpdateDistance;
    public TextMeshProUGUI tmpDesiredAccuracy;
    public TextMeshProUGUI tmpUpdateDistance;
    //public TMP_InputField inptFldDesiredAccuracy;
    //public TMP_InputField inptFldUpdateDistance;
    public Button btnSaveSettings;
    private readonly float DEFAULT_DESIRED_ACCURACY = 10;
    private readonly float DEFAULT_UPDATE_DISTANCE = 5;
    private readonly string DESIRED_ACCURACY_KEY = "desiredAccuracyKey";
    private readonly string UPDATE_DISTANCE_KEY = "updateDistanceKey";

    [Space]
    [Header("Create Area Screen")]
    // Create Area Screen
    public GameObject pnlCreateArea;
    public GameObject pnlSaveArea;
    public TMP_InputField inptFldCreateArea;
    public TMP_InputField inptFldCreateAreaEnglish;
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
    public TMP_InputField inptFldEditArea, inptFldEditAreaEnglish;
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
    [Header("GPS Signal panel")]
    //GPS Screen
    public GameObject pnlGPSSignal;

    [Space]
    [Header("Path Screen")]
    //PathScreen
    public GameObject pnlPathScreen, pnlRecordButton, pnlMainButtons;
    public Button btnAddNewPath, btnStopRecording, btnResumeRecording;
    public Image imgOnRecord, imgPauseRecording;
    public Animator imgRecord;
    public RectTransform imgRecordStopBar;

    [Space]
    [Header("Warning Area Screen")]
    //WarningScreen if user is near area
    public GameObject pnlWarningScreen;
    public TextMeshProUGUI txtWarning;
    public Button btnCancel;

    [Space]
    [Header("Warning Escape Screen")]
    public GameObject pnlWarningEscapeScreen;
    public Button btnEscapeYes;
    public Button btnEscapeNo;

    [Space]
    [Header("Warning ResetSurvey Screen")]
    public GameObject pnlWarningResetSurveyScreen;
    public Button btnResetSurveyYes;
    public Button btnResetSurveyNo;

    [Space]
    [Header("Warning Upload Info Screen")]
    public GameObject pnlWarningUploadInfoScreen;
    public TextMeshProUGUI txtUploadInfo;
    public Button btnUploadInfoOk;

    [Space]
    [Header("Warning Save Path Screen")]
    //WarningScreen when user is about to save the path
    public GameObject pnlWarningSavePathScreen;
    public Button btnSave, btnContinue, btnSaveDelete;

    [Space]
    [Header("Warning Delete")]
    //Warning screen when user is about to delete
    public GameObject pnlWarningDeleteScreen;
    public Button btnDeleteFinal, btnDeleteCancel;
    Transform pnlForDelete;

    [Space]
    [Header("Warning Delete Path")]
    public GameObject pnlWarningDeletePathScreen;
    public Button btnDeleteYes, btnDeleteNo;

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
    [Header("Warning Thank You Screen")]
    public GameObject pnlWarningThankYouScreen;
    public TextMeshProUGUI txtWarningThankYou;
    public Button btnThankYouExit;
    
    [Space]
    [Header("Warning Survey Intro Screen")]
    public GameObject pnlWarningSurveyIntroScreen;
    public Button btnSurveyIntroYes;
    public Button btnSurveyIntroNo;

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
    public TMP_Dropdown sexDropdown,ageDropdown;
    readonly List<string> sexValuesEN = new List<string> { "<Choose Below>", "Male", "Female","Other" };
    readonly List<string> sexValuesGR = new List<string> { "<Επιλέξτε από τα παρακάτω>", "Άρρεν", "Θήλυ", "Άλλο" };
    readonly List<string> optionValuesΕΝ = new List<string> { "<Choose Below>", "An Individual visit (member of a small group of friends or Family)", "An Organized group visit accompanied by a Tour Guide/ Group Leader.", "A School visit" };
    readonly List<string> optionValuesGR = new List<string> { "<Επιλέξτε από τα παρακάτω>", "Ατομικής επίσκεψης (ή μικρή ανεξάρτητη ομάδα) ", "Οργανωμένης επίσκεψης, με ξεναγό / αρχηγό ομάδας", "Σχολικής επίσκεψης" };
    readonly List<string> ageValuesEN = new List<string> {"<Choose Below>","0-12", "13-18", "19-25", "26-30", "31-40", "41-50", "51-60","61+" };
    readonly List<string> ageValuesGR = new List<string> { "<Επιλέξτε από τα παρακάτω>", "0-12", "13-18", "19-25", "26-30", "31-40", "41-50", "51-60", "61+" };

    [HideInInspector]
    public bool downloadTiles = false;
    [HideInInspector]
    public bool isAdmin;
    private string passwordAdmin = "2791";
    private string ancientMesseneTitle = "Αρχαία Μεσσήνη";

    private bool userAnsweredSurvey = false;
    //readonly string ENGLISH = "English (en)";
    //readonly string GREEK = "Greek (el)";

    private bool stopRecording = false;
    readonly private float stopRecordingPressDuration = 3f; // seconds
    private float stopRecordingCounter = 0;

    bool quit = false;
    #endregion

    #region Unity Functions
    private void Awake()
    {
        if (!LocalizationSettings.InitializationOperation.IsDone)
            StartCoroutine(InitializeLocalizationSettings());
    }

    public void Start()
    {
        // Initialize variables
        selectAreaObjects = new List<GameObject>();
        selectPathObjects = new List<GameObject>();
        isAdmin = false;
        userAnsweredSurvey = false;
        stopRecording = false;

        // Subscribe Buttons
        SubscribeButtons();

        // Display Areas Screen
        //DisplayAreaSelectScreen();

        // Display Admin
        //pnlWarningsAdminScreen.SetActive(true);

        // Initialize panels
        pnlWarningScreen.SetActive(false);
        pnlWarningSavePathScreen.SetActive(false);
        pnlQuestionnaireScreen.SetActive(false);
        txtMainName.text = DEFAULT_TEXT_NAME;
        imgPauseRecording.gameObject.SetActive(false);
        //btnBackToAreasScreen.interactable = true;
        //imgRecord = GetComponent<Animator>();
        //sexDropdown = GetComponent<TMP_Dropdown>();

        // Display Options Screen / Language Selection
        pnlOptionsScreen.SetActive(true);
        pnlLanguageScreen.SetActive(true);
        btnBack.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (pnlPathScreen.activeSelf && !pnlOptionsScreen.activeSelf && !pnlSavedPaths.activeSelf)
        {
            if (AppManager.Instance.androidManager.HasGPS())
            {
                // Disable GPS signal panel
                if (pnlGPSSignal.activeSelf)
                    pnlGPSSignal.SetActive(false);

                // Enable new path button
                if (!btnAddNewPath.interactable && AppManager.Instance.mapManager.IsWithinConstraints())
                    btnAddNewPath.interactable = true;
            }
            else
            {
                // Enable GPS signal panel
                if (!pnlGPSSignal.activeSelf && !quit)
                    pnlGPSSignal.SetActive(true);

                // Disable new path button
                if (btnAddNewPath.interactable)
                    btnAddNewPath.interactable = false;
            }
        }
        else
        {
            if (pnlGPSSignal.activeSelf)
                pnlGPSSignal.SetActive(false);
        }

        // Disable Options button if is recording path
        if (AppManager.Instance.mapManager.isRecordingPath)
        {
            // Disable options button
            if (btnOptions.interactable)
                btnOptions.interactable = false;

            // Disable quit button
            if (btnQuit.interactable)
                btnQuit.interactable = false;
        }
        else
        {
            // Enable options button
            if (!btnOptions.interactable)
                btnOptions.interactable = true;

            // Enable quit button
            if (!btnQuit.interactable)
                btnQuit.interactable = true;
        }

        // Stop recording check
        if (stopRecording)
        {
            // Change graphic over time
            imgRecordStopBar.localScale = new Vector3(imgRecordStopBar.localScale.x + ((Time.deltaTime) / stopRecordingPressDuration), 1f, 1f);

            // Update counter
            stopRecordingCounter += Time.deltaTime;

            // Check counter
            if (stopRecordingCounter >= stopRecordingPressDuration)
            {
                SaveUIButton();
                stopRecording = false;
                stopRecordingCounter = 0;
                imgRecordStopBar.localScale = new Vector3(0f, 1f, 1f);
            }
        }
    }
    #endregion

    #region Methods
    private void SubscribeButtons()
    {
        // Map Screen
        //btnBackToAreasScreen.onClick.AddListener(() => BackToAreasScreen());
        btnBack.onClick.AddListener(() => BackToOptions());
        btnQuit.onClick.AddListener(() => EnableScreen(pnlWarningEscapeScreen, true));
        btnOptions.onClick.AddListener(() => DisplayOptionsScreen());

        // Options Screen
        btnLanguageScreen.onClick.AddListener(() => DisplayLanguageScreen());
        btnAdminScreen.onClick.AddListener(() => DisplayAdminScreen());
        btnAreaSelectScreen.onClick.AddListener(() => DisplayAreas());
        btnAboutScreen.onClick.AddListener(() => DisplayAboutScreen());
        btnSettingsScreen.onClick.AddListener(() => DisplaySettingsScreen());

        // Language Screen
        btnGreek.onClick.AddListener(() => ChangeLanguage(true));
        btnEnglish.onClick.AddListener(() => ChangeLanguage(false));

        // Areas Screen
        btnCreateArea.onClick.AddListener(() => CreateNewAreaSelected());

        // Settings Screen
        sldrDesiredAccuracy.onValueChanged.AddListener(ChangeSliderTxtDesiredAccuracy);
        sldrUpdateDistance.onValueChanged.AddListener(ChangeSliderTxtUpdateDistance);
        btnSaveSettings.onClick.AddListener(() => CheckSettings());
        //inptFldDesiredAccuracy.characterLimit = 6;
        //inptFldUpdateDistance.characterLimit = 6;
        SetLocationServiceSettings(); // TODO: Map Manager method
        SetSettingsTexts();
        //inptFldDesiredAccuracy.text = OnlineMapsLocationService.instance.desiredAccuracy.ToString();
        //inptFldUpdateDistance.text = OnlineMapsLocationService.instance.updateDistance.ToString();

        // CreateAreaScreen
        btnSaveArea.onClick.AddListener(() => EnableScreen(pnlSaveArea, true));
        btnCreateAreaSave.onClick.AddListener(() => SaveArea());
        btnCreateAreaCancel.onClick.AddListener(() => RemoveNewArea());

        //btn GPS
        btnGPSPermission.onClick.AddListener(() => AppManager.Instance.androidManager.OpenNativeMobileSettings());

        // Admin (OLD)
        //btnAdminYes.onClick.AddListener(() => DisplayPasswordScreen());
        //btnAdminNo.onClick.AddListener(() => SetAdmin(false));

        // Admin (NEW) / Password
        inptFldPasswordScreen.characterLimit = 4;
        btnPasswordLogin.onClick.AddListener(() => CheckPassword(inptFldPasswordScreen.text));
        btnLogout.onClick.AddListener(() => SetAdmin(false));
        //btnPasswordScreenClose.onClick.AddListener(() => SetAdmin(false));

        //btn Path and stop record
        btnAddNewPath.onClick.AddListener(() => AddNewPath());
        btnResumeRecording.onClick.AddListener(() => ResumePath());

        // Stop Recording
        btnStopRecording.onClick.AddListener(() => CancelStopRecord());
        // OnPointerDown
        EventTrigger triggerDown = btnStopRecording.gameObject.AddComponent<EventTrigger>();
        var pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((s) => StartStopRecord());
        triggerDown.triggers.Add(pointerDown);
        // OnPointerExit
        EventTrigger triggerExit = btnStopRecording.gameObject.AddComponent<EventTrigger>();
        var pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((c) => CancelStopRecord());
        triggerExit.triggers.Add(pointerExit);

        //btn warning on area
        btnCancel.onClick.AddListener(() => CloseScreenPanels());

        // escape
        btnEscapeYes.onClick.AddListener(() => Escape());
        btnEscapeNo.onClick.AddListener(() => EnableScreen(pnlWarningEscapeScreen, false));

        // reset survey
        btnResetSurveyYes.onClick.AddListener(() => ResetSurvey());
        btnResetSurveyNo.onClick.AddListener(() => EnableScreen(pnlWarningResetSurveyScreen, false));

        // upload info
        btnUploadInfoOk.onClick.AddListener(() => EnableScreen(pnlWarningUploadInfoScreen, false));

        //btn on delete warning cancel or final delete
        btnDeleteCancel.onClick.AddListener(() => CloseScreenPanels());
        btnDeleteFinal.onClick.AddListener(() => DeleteFinal());

        //btn warning panel for save or cancel a path
        btnSave.onClick.AddListener(() => SavePath());
        btnContinue.onClick.AddListener(() => ResumePath());
        btnSaveDelete.onClick.AddListener(() => EnableScreen(pnlWarningDeletePathScreen, true));

        // delete path warning
        btnDeleteYes.onClick.AddListener(() => CancelInGeneral());
        btnDeleteNo.onClick.AddListener(() => EnableScreen(pnlWarningDeletePathScreen, false));

        // for testing the saving of paths is happening smoothly
        btnPaths.onClick.AddListener(() => DisplaySavedPathsScreen());
        btnCancelShow.onClick.AddListener(() => CancelInGeneral());

        // warning DownloadTiles
        btnDownloadTilesYes.onClick.AddListener(() => SetDownloadTiles(true));
        btnDownloadTilesNo.onClick.AddListener(() => SetDownloadTiles(false));

        // warning ThankYou
        btnThankYouExit.onClick.AddListener(() => Escape()); 

        // warning Survey Intro
        btnSurveyIntroYes.onClick.AddListener(() => OpenQuestionnaire());
        btnSurveyIntroNo.onClick.AddListener(() => DisplayThankYouScreenNoSurvey()); // EnableScreen(pnlWarningSurveyIntroScreen, false)

        //btn for edit Area
        btnEditAreaCancel.onClick.AddListener(() => CancelInGeneral());
        //btnEditAreaSave.onClick.AddListener(()=> )

        //btn for profiler screen
        btnProfiler.onClick.AddListener(() => DisplayQuestionnaire());
        btnResetQuestionnaire.onClick.AddListener(() => EnableScreen(pnlWarningResetSurveyScreen, true));

        //btn for close Internet screen
        btnCloseInternetScreen.onClick.AddListener(() => EnableScreen(pnlWarningInternetScreen, false)); // CancelInGeneral()
    }

    private void StartStopRecord()
    {
        stopRecordingCounter = 0;
        stopRecording = true;
    }

    private void CancelStopRecord()
    {
        stopRecording = false;
        imgRecordStopBar.localScale = new Vector3(0f, 1f, 1f);
    }

    public void ResetSurvey()
    {
        AppManager.Instance.questionnaireManager.ResetValues();
        pnlWarningResetSurveyScreen.SetActive(false);
    }

    private void DisplayThankYouScreenNoSurvey()
    {
        pnlWarningSurveyIntroScreen.SetActive(false);

        EnableThankYouScreen();
    }
    
    public void DisplayUploadInfo(string _info)
    {
        txtUploadInfo.text = _info;
        pnlWarningUploadInfoScreen.SetActive(true);
    }

    private void SetDownloadTiles(bool _value)
    {
        downloadTiles = _value;
        pnlWarningDownloadTilesScreen.SetActive(false);
    }

    void ActivateButtons(bool valBack, bool valQuit, bool valReset, bool valProfiler)
    {
        //btnBackToAreasScreen.gameObject.SetActive(valBack);
        //btnQuit.gameObject.SetActive(valQuit);
        btnProfiler.gameObject.SetActive(valProfiler);
        btnResetQuestionnaire.gameObject.SetActive(valReset);
    }

    IEnumerator InitializeLocalizationSettings()
    {
        // Wait for the localization system to initialize, loading Locales, preloading etc.
        yield return LocalizationSettings.InitializationOperation;

        // Initialize language as English
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
    }

    private void DisplayAreas()
    {
        AppManager.Instance.serverManager.DownloadAreas();
        DisplayAreaSelectScreen();
        Debug.Log("Display Areas");
    }

    public void DisplayOptionsScreen()
    {
        // Enable Options
        pnlOptions.gameObject.SetActive(true);

        // Close other option screens
        if (pnlQuestionnaireScreen.activeSelf)
        {
            /*btnLanguageScreen.gameObject.SetActive(true);
            btnAdminScreen.gameObject.SetActive(false);
            btnAreaSelectScreen.gameObject.SetActive(false);
            btnAboutScreen.gameObject.SetActive(false);*/

            // Button Panels
            pnlBtnLanguage.SetActive(true);
            pnlBtnAdmin.SetActive(false);
            pnlBtnAreaSelect.SetActive(false);
            pnlBtnAbout.SetActive(false);
            pnlBtnSettings.SetActive(false);
        }
        else
        {
            //btnLanguageScreen.gameObject.SetActive(true);
            pnlBtnLanguage.SetActive(true);
            //btnAdminScreen.gameObject.SetActive(true);
            pnlBtnAdmin.SetActive(true);
            //btnAreaSelectScreen.gameObject.SetActive(true);
            pnlBtnAreaSelect.SetActive(true);
            //btnAboutScreen.gameObject.SetActive(true);
            pnlBtnAbout.SetActive(true);
            if (isAdmin)
                pnlBtnSettings.SetActive(true);
            else
                pnlBtnSettings.SetActive(false);
        }

        // Close other option screens
        pnlLanguageScreen.gameObject.SetActive(false);
        pnlAdminScreen.gameObject.SetActive(false);
        pnlAreaSelectScreen.gameObject.SetActive(false);
        pnlAboutScreen.gameObject.SetActive(false);
        pnlSettingsScreen.gameObject.SetActive(false);

        // Close Saved Paths
        if (pnlSavedPaths.activeSelf)
            pnlSavedPaths.SetActive(false);

        // Enable/Disable Options Screen
        pnlOptionsScreen.gameObject.SetActive(!pnlOptionsScreen.activeSelf);

        // Disable Back Button
        btnBack.gameObject.SetActive(false);
    }

    private void DefaultMap()
    {
        ResetPanels();

        // Reset Map Graphics
        AppManager.Instance.mapManager.CreateNewAreaFinalize();

        // Reset Title
        txtMainName.text = DEFAULT_TEXT_NAME;
    }

    private void BackToOptions()
    {
        // Enable Options
        pnlOptions.gameObject.SetActive(true);

        // Close other screens
        if (pnlQuestionnaireScreen.activeSelf)
        {
            /*btnLanguageScreen.gameObject.SetActive(true);
            btnAdminScreen.gameObject.SetActive(false);
            btnAreaSelectScreen.gameObject.SetActive(false);
            btnAboutScreen.gameObject.SetActive(false);*/

            // Button Panels
            pnlBtnLanguage.SetActive(true);
            pnlBtnAdmin.SetActive(false);
            pnlBtnAreaSelect.SetActive(false);
            pnlBtnAbout.SetActive(false);
            pnlBtnSettings.SetActive(false);
        }
        else
        {
            //btnLanguageScreen.gameObject.SetActive(true);
            pnlBtnLanguage.SetActive(true);
            //btnAdminScreen.gameObject.SetActive(true);
            pnlBtnAdmin.SetActive(true);
            //btnAreaSelectScreen.gameObject.SetActive(true);
            pnlBtnAreaSelect.SetActive(true);
            //btnAboutScreen.gameObject.SetActive(true);
            pnlBtnAbout.SetActive(true);
            if (isAdmin)
                pnlBtnSettings.SetActive(true);
            else
                pnlBtnSettings.SetActive(false);
        }

        // Close other screens
        pnlLanguageScreen.gameObject.SetActive(false);
        pnlAdminScreen.gameObject.SetActive(false);
        pnlAreaSelectScreen.gameObject.SetActive(false);
        pnlAboutScreen.gameObject.SetActive(false);
        pnlSettingsScreen.gameObject.SetActive(false);

        // Disable Back Button
        btnBack.gameObject.SetActive(false);
    }

    public void DisplayLanguageScreen()
    {
        // Disable Options
        pnlOptions.gameObject.SetActive(false);

        // Enable Language Screen
        pnlLanguageScreen.gameObject.SetActive(true);

        // Enable Back Button
        btnBack.gameObject.SetActive(true);
    }

    public void DisplayAdminScreen()
    {
        // Disable Options
        pnlOptions.gameObject.SetActive(false);

        // Enable Admin Screen
        pnlAdminScreen.gameObject.SetActive(true);

        // Enable Back Button
        btnBack.gameObject.SetActive(true);
    }
    
    public void DisplayAreaSelectScreen()
    {
        pnlOptions.SetActive(false);
        pnlAreaSelectScreen.SetActive(true);
        //pnlAreasScreen.SetActive(true);

        //pnlCreateArea.SetActive(false);

        if (isAdmin)
            btnCreateArea.gameObject.SetActive(true);
        else
            btnCreateArea.gameObject.SetActive(false);

        DestroySelectObjects(selectAreaObjects);
        selectAreaObjects = InstantiateSelectAreaObjects();
        StartCoroutine(ReloadLayout(pnlLoadedAreas));
        //AppManager.Instance.mapManager.CreateNewAreaFinalize();
        //EnableScreen(pnlPathScreen, false);
        //imgRecord.gameObject.SetActive(false);
        //EnableScreen(pnlSavedPaths, false); //the panel for saved paths
        ActivateButtons(false, true, false, false);
        //txtMainName.text = DEFAULT_TEXT_NAME;
        pnlWarningDeleteScreen.SetActive(false);
        AppManager.Instance.questionnaireManager.ResetValues();

        // Enable Back Button
        btnBack.gameObject.SetActive(true);
    }

    private void ResetPanels()
    {
        // Create Area
        //AppManager.Instance.mapManager.CreateNewAreaFinalize();
        pnlCreateArea.SetActive(false);
        pnlSaveArea.SetActive(false);

        // Edit Area
        pnlEditArea.SetActive(false);
        pnlSaveEditArea.SetActive(false);

        // Path Screen
        imgRecord.gameObject.SetActive(false);
        pnlPathScreen.SetActive(false);
        pnlWarningDownloadTilesScreen.SetActive(false);

        // Extras
        pnlSavedPaths.SetActive(false);
        AppManager.Instance.mapManager.RemoveMarkersAndLine();
        // Disable Back Button
        btnBack.gameObject.SetActive(false);
    }

    public void DisplayAboutScreen()
    {
        // Disable Options
        pnlOptions.gameObject.SetActive(false);

        // Enable About Screen
        pnlAboutScreen.gameObject.SetActive(true);

        // Enable Back Button
        btnBack.gameObject.SetActive(true);
    }

    public void DisplaySettingsScreen()
    {
        // Disable Options
        pnlOptions.gameObject.SetActive(false);

        // Reset Settings
        SetSettingsTexts();

        // Enable About Screen
        pnlSettingsScreen.gameObject.SetActive(true);

        // Enable Back Button
        btnBack.gameObject.SetActive(true);
    }

    public void DisplayPathsScreen()
    {
        //pnlPathScreen.SetActive(true);

        //pnlSavedPaths.SetActive(!pnlSavedPaths.activeSelf);

        DestroySelectObjects(selectPathObjects);
        selectPathObjects = InstantiateSelectPathObjects();
        StartCoroutine(ReloadLayout(pnlSavedPaths));
        pnlScrollViewPaths.SetActive(true);
        ActivateButtons(true, false, false, false);
        AppManager.Instance.mapManager.RemoveMarkersAndLine();
        pnlWarningDeleteScreen.SetActive(false);
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
        //pnlOptionsScreen.SetActive(false);
        //EnableScreen(pnlWarningThankYouScreen, false);
        Application.Quit();
        OnlineMapsLocationService.instance.StopLocationService();
        pnlGPSSignal.SetActive(false);
        quit = true;
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

                if (LocalizationSettings.InitializationOperation.IsDone && LocalizationSettings.SelectedLocale.Equals(LocalizationSettings.AvailableLocales.Locales[1])) // English
                    selectAreaText.text = area.titleEnglish;
                else
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
                        if (isAdmin)
                        {
                            btnDeleteArea = child.GetComponent<Button>();
                            btnDeleteArea.onClick.AddListener(OnAreaDeletePressed);
                        }
                        else
                            child.gameObject.SetActive(false);
                    }

                    if (child.name.Equals("btnEditArea"))
                    {
                        if (isAdmin)
                        {
                            btnEditArea = child.GetComponent<Button>();
                            btnEditArea.onClick.AddListener(OnEditSelect);
                        }
                        else
                            child.gameObject.SetActive(false);
                    }
                }

                //Button btnSelectArea = newSelectArea.GetComponentInChildren<Button>();
                //btnSelectArea.onClick.AddListener(OnAreaSelectPressed);
                newSelectAreaObjects.Add(newSelectArea);
            }
        }

        return newSelectAreaObjects;
    }

    public void DisplayAncientMessene()
    {
        // Load Αρχαία Μεσσήνη
        cArea loadedArea = AppManager.Instance.mapManager.GetAreaByTitle(ancientMesseneTitle);
        
        if (loadedArea != null)
        {
            AppManager.Instance.mapManager.currentArea = loadedArea;

            // Download Tiles
            AppManager.Instance.serverManager.DownloadTiles();

            // Set main title
            txtMainName.text = loadedArea.titleEnglish;

            /*if (LanguageIsEnglish())
                txtMainName.text = loadedArea.titleEnglish;
            else
                txtMainName.text = loadedArea.title;*/

            //pnlAreasScreen.SetActive(false);
            AppManager.Instance.mapManager.SetMapViewToArea(loadedArea);

            EnableScreen(pnlPathScreen, true);
            imgRecord.gameObject.SetActive(true);
            EnableScreen(pnlSavedPaths, false);
            ActivateButtons(true, false, false, false);
            AppManager.Instance.mapManager.isShown = false;
            AppManager.Instance.mapManager.CheckUserPosition();
        }
        else
        {
            DefaultMap();
        }
    }

    private void OnAreaSelectPressed()
    {
        // Reset Panels
        ResetPanels();
        AppManager.Instance.mapManager.CreateNewAreaFinalize();

        GameObject selectAreaObject = EventSystem.current.currentSelectedGameObject.transform.parent.transform.parent.gameObject;
        //TMP_Text selectAreaText = selectAreaObject.GetComponentInChildren<TMP_Text>();

        //cArea selectedArea = AppManager.Instance.mapManager.GetAreaByTitle(selectAreaText.text);
        cArea selectedArea = AppManager.Instance.mapManager.GetAreaByIndex(selectAreaObjects.IndexOf(selectAreaObject.gameObject));

        if (selectedArea != null)
        {
            AppManager.Instance.mapManager.currentArea = selectedArea;

            // Download Paths of Area
            /*if (selectedArea.server_area_id != -1)
            {
                AppManager.Instance.serverManager.DownloadPaths(selectedArea.server_area_id);
            }*/

            if (LanguageIsEnglish())
                txtMainName.text = selectedArea.titleEnglish;
            else
                txtMainName.text = selectedArea.title;

            // Download Tiles
            AppManager.Instance.serverManager.DownloadTiles();
            
            //pnlAreasScreen.SetActive(false);
            pnlOptionsScreen.SetActive(false);
            AppManager.Instance.mapManager.SetMapViewToArea(selectedArea);
        }

        EnableScreen(pnlPathScreen, true);
        imgRecord.gameObject.SetActive(true);
        EnableScreen(pnlSavedPaths, false); //the panel for saved paths can be removed afterwards, for testing purposes
        ActivateButtons(true, false, false, false);
        AppManager.Instance.mapManager.isShown = false;
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
        // Reset Panels
        ResetPanels();

        // Get selected area
        GameObject btnEditArea = EventSystem.current.currentSelectedGameObject;
        pnlForEdit = btnEditArea.transform.parent;
        cArea selectedArea = AppManager.Instance.mapManager.areas[selectAreaObjects.IndexOf(pnlForEdit.gameObject)];

        // Change UI
        pnlEditArea.SetActive(true);
        pnlOptionsScreen.SetActive(false);
        //pnlAreasScreen.SetActive(false);

        if (LanguageIsEnglish())
            txtMainName.text = selectedArea.titleEnglish;
        else
            txtMainName.text = selectedArea.title;
        btnEditAreaSave.onClick.AddListener(EditSaveArea);
        btnSaveEditArea.interactable = true;
        btnSaveEditArea.onClick.AddListener(() => EnableScreen(pnlSaveEditArea, true));
        inptFldEditArea.text = selectedArea.title;
        inptFldEditAreaEnglish.text = selectedArea.titleEnglish;
        ActivateButtons(true, true, false, false);

        // Start edit selected area
        AppManager.Instance.mapManager.StartEditArea(selectedArea);
    }

    private void OnPathSelectPressed()
    {
        // Reset Panels
        //ResetPanels();
        //pnlPathScreen.SetActive(true);

        GameObject selectPathObject = EventSystem.current.currentSelectedGameObject.transform.parent.transform.parent.gameObject;
        //TMP_Text selectPathText = selectPathObject.GetComponentInChildren<TMP_Text>();
        // Get index
        //Debug.Log("OnPathSelectPressed");
        cPath selectedPath = AppManager.Instance.mapManager.GetPathByIndex(selectPathObjects.IndexOf(selectPathObject.gameObject));  //AppManager.Instance.mapManager.GetPathByTitle(selectPathText.text); // AppManager.Instance.mapManager.currentArea[selectAreaObjects.IndexOf(selectPathObject.gameObject)]; 
        //AppManager.Instance.mapManager.currentPath = selectedPath;
        if (selectedPath != null)
        {
            // Try to load points of path
            List<cPathPoint> loadedPoints = cPathPoint.LoadPathPointsOfPath(selectedPath.local_path_id);

            // Check if points are loaded, else download points
            if (loadedPoints != null && loadedPoints.Count > 0)
            {
                selectedPath.pathPoints = loadedPoints;

                // Display Path
                AppManager.Instance.mapManager.DisplayPath(selectedPath);
            }
            else
            {
                // Download Points of Path
                if (selectedPath.server_path_id != -1)
                {
                    AppManager.Instance.mapManager.pathToDisplay = selectedPath;
                    AppManager.Instance.serverManager.DownloadPoints(selectedPath.server_path_id); // TODO: Wait until download is finished to display path
                }
            }

            /*pnlScrollViewPaths.SetActive(false);
            pnlSavedPaths.SetActive(false);
            AppManager.Instance.mapManager.DisplayPath(selectedPath);*/
        }
    }

    private void OnPathDeletePressed()
    {
        pnlWarningDeleteScreen.SetActive(true);
        GameObject btnDeleteArea = EventSystem.current.currentSelectedGameObject;
        pnlForDelete = btnDeleteArea.transform.parent;
        //AppManager.Instance.mapManager.DeletePath(selectPathObjects.IndexOf(pnlSelectArea.gameObject));
    }

    private void DestroySelectObjects(List<GameObject> _selectObjects)
    {
        if (_selectObjects != null)
        {
            foreach (GameObject selectObject in _selectObjects)
            {
                Destroy(selectObject);
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
    /*private void BackToAreasScreen()
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
            DisplayAreaSelectScreen();
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
            DisplayAreaSelectScreen();
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
            DisplayAreaSelectScreen();
            AppManager.Instance.serverManager.DownloadAreas();
        }
        else if (!pnlSavedPaths.activeSelf && !pnlAreasScreen.activeSelf && !pnlPathScreen.activeSelf &&
            !pnlSaveArea.activeSelf && !pnlEditArea.activeSelf && !pnlCreateArea.activeSelf &&
            !AppManager.Instance.mapManager.isRecordingPath && !AppManager.Instance.mapManager.isPausePath && pnlQuestionnaireScreen.activeSelf &&
            !pnlOptionA.activeSelf && !pnlOptionB.activeSelf && !pnlOptionC.activeSelf)
        {
            pnlQuestionnaireScreen.SetActive(false);
            DisplayAreaSelectScreen();
            AppManager.Instance.questionnaireManager.ResetValues();
            AppManager.Instance.serverManager.DownloadAreas();
        }
        else if (!pnlSavedPaths.activeSelf && !pnlAreasScreen.activeSelf && !pnlPathScreen.activeSelf &&
           !pnlSaveArea.activeSelf && !pnlEditArea.activeSelf && !pnlCreateArea.activeSelf &&
           !AppManager.Instance.mapManager.isRecordingPath && !AppManager.Instance.mapManager.isPausePath &&
           pnlQuestionnaireScreen.activeSelf && pnlOptionA.activeSelf && !pnlOptionB.activeSelf && !pnlOptionC.activeSelf)
        {
            pnlQuestionnaireScreen.SetActive(false);
            DisplayAreaSelectScreen();
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
            DisplayAreaSelectScreen();
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
            DisplayAreaSelectScreen();
            pnlOptionC.SetActive(false);
            AppManager.Instance.questionnaireManager.ResetValues();
            AppManager.Instance.serverManager.DownloadAreas();
        }
        else if (!pnlSavedPaths.activeSelf && !pnlAreasScreen.activeSelf && !pnlPathScreen.activeSelf &&
         !pnlSaveArea.activeSelf && !pnlEditArea.activeSelf && !pnlCreateArea.activeSelf &&
         !AppManager.Instance.mapManager.isRecordingPath && !AppManager.Instance.mapManager.isPausePath &&
         pnlQuestionnaireScreen.activeSelf && pnlMainQuestions.activeSelf && !pnlOptionA.activeSelf && !pnlOptionB.activeSelf && !pnlOptionC.activeSelf)
        {
            pnlQuestionnaireScreen.SetActive(false);
            DisplayAreaSelectScreen();
            pnlMainQuestions.SetActive(false);
            AppManager.Instance.questionnaireManager.ResetValues();
            AppManager.Instance.serverManager.DownloadAreas();
        }
        else
        {
            DisplayAreaSelectScreen();
            EnableScreen(pnlQuestionnaireScreen, false);
            AppManager.Instance.questionnaireManager.ResetValues();
            AppManager.Instance.serverManager.DownloadAreas();
        }

        //mapScreen.SetActive(false);
    }*/

    public void IsInRecordingPath(bool val)
    {
        imgRecord.SetBool("isPlaying", val);
    }

    private void CreateNewAreaSelected()
    {
        // Reset Panels
        ResetPanels();
        AppManager.Instance.mapManager.CreateNewAreaFinalize();

        // Disable Options Screen
        pnlOptionsScreen.SetActive(false);

        pnlAreaSelectScreen.SetActive(false);
        //pnlAreasScreen.SetActive(false);
        pnlCreateArea.SetActive(true);
        btnSaveArea.interactable = false;

        inptFldCreateArea.text = "";
        inptFldCreateAreaEnglish.text = "";

        ActivateButtons(true, false, false, false);
        AppManager.Instance.mapManager.CreateNewAreaInitialize();
    }

    private void RemoveNewArea()
    {
        //AppManager.Instance.mapManager.RemoveNewArea();
        //btnSaveArea.interactable = false;
        EnableScreen(pnlSaveArea, false);
    }

    private void SaveArea()
    {
        string newAreaTitleGreek = inptFldCreateArea.text;
        string newAreaTitleEnglish = inptFldCreateAreaEnglish.text;

        if (!string.IsNullOrEmpty(newAreaTitleGreek) && !Regex.IsMatch(newAreaTitleGreek, "^[0-9A-Za-z ]+$")
            && !string.IsNullOrEmpty(newAreaTitleEnglish) && Regex.IsMatch(newAreaTitleEnglish, "^[0-9A-Za-z ]+$"))
        {
            AppManager.Instance.mapManager.SaveArea(newAreaTitleGreek, newAreaTitleEnglish);
            pnlSaveArea.SetActive(false);
            pnlCreateArea.SetActive(false);

            // Enable Options Screen
            DisplayOptionsScreen();
            DisplayAreaSelectScreen();
        }
        else
        {
            pnlWarningScreen.SetActive(true);

            if (LanguageIsEnglish())
                txtWarning.text = "Please enter a valid name";
            else
                txtWarning.text = "Παρακαλώ εισάγετε ένα έγκυρο όνομα";
        }
    }

    public void EditSaveArea()
    {
        string newAreaTitleGreek = inptFldEditArea.text;
        string newAreaTitleEnglish = inptFldEditAreaEnglish.text;

        if (!string.IsNullOrEmpty(newAreaTitleGreek) && !Regex.IsMatch(newAreaTitleGreek, "^[0-9A-Za-z ]+$")
            && !string.IsNullOrEmpty(newAreaTitleEnglish) && Regex.IsMatch(newAreaTitleEnglish, "^[0-9A-Za-z ]+$"))
        {
            AppManager.Instance.mapManager.EditArea(AppManager.Instance.mapManager.currentArea, newAreaTitleGreek, newAreaTitleEnglish);
            pnlSaveEditArea.SetActive(false);
            pnlEditArea.SetActive(false);
            if (LanguageIsEnglish())
                txtMainName.text = newAreaTitleEnglish;
            else
                txtMainName.text = newAreaTitleGreek;

            // Enable Options Screen
            DisplayOptionsScreen();
            DisplayAreaSelectScreen();
        }
        else
        {
            pnlWarningScreen.SetActive(true);

            if (LanguageIsEnglish())
                txtWarning.text = "Please enter a valid name";
            else
                txtWarning.text = "Παρακαλώ εισάγετε ένα έγκυρο όνομα";
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
        pnlSavedPaths.SetActive(false);
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
        //OpenQuestionnaire();
        pnlWarningSurveyIntroScreen.SetActive(true);
    }

    //opens the saved paths screen (on click event)
    public void DisplaySavedPathsScreen()
    {
        pnlSavedPaths.SetActive(!pnlSavedPaths.activeSelf);

        if (pnlSavedPaths.activeSelf)
        {
            // Download Paths of Area
            if (AppManager.Instance.mapManager.currentArea.server_area_id != -1)
            {
                AppManager.Instance.serverManager.DownloadPaths(AppManager.Instance.mapManager.currentArea.server_area_id);
            }

            DisplayPathsScreen();
        }
        
        /*pnlSavedPaths.SetActive(true);
        DestroySelectObjects(selectPathObjects);
        selectPathObjects = InstantiateSelectPathObjects();
        StartCoroutine(ReloadLayout(pnlSavedPaths));
        pnlScrollViewPaths.SetActive(true);
        ActivateButtons(true, false, false, false);
        AppManager.Instance.mapManager.RemoveMarkersAndLine();
        pnlWarningDeleteScreen.SetActive(false);*/
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
            EnableScreen(pnlWarningDeletePathScreen, false);
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
        if (pnlSavedPaths.activeSelf) //pnlPathScreen.activeSelf
        {
            AppManager.Instance.mapManager.DeletePath(selectPathObjects.IndexOf(pnlForDelete.gameObject));
            // DisplaySavedPathsScreen();
            DisplayPathsScreen();
            //Debug.Log("Delete Path");
        }
        else if (pnlAreasScreen.activeSelf)
        {
            AppManager.Instance.mapManager.DeleteArea(selectAreaObjects.IndexOf(pnlForDelete.gameObject));
            DisplayAreaSelectScreen();
            DisplayAncientMessene();
            //Debug.Log("Delete Area");
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
    /*public void DisplayPasswordScreen()
    {
        pnlWarningsAdminScreen.SetActive(false);
        pnlWarningsPasswordScreen.SetActive(true);
    }*/

    /*public void ClosePasswordScreen()
    {
        pnlWarningsPasswordScreen.SetActive(false);
    }*/

    public void CheckPassword(string _input)
    {
        if (_input.Equals(passwordAdmin))
        {
            SetAdmin(true);
        }
        /*else
        {
            inptFldPasswordScreen.text = "";
            //pnlWarningsPasswordScreen.SetActive(false);
            //pnlWarningsAdminScreen.SetActive(true);
        }*/

        inptFldPasswordScreen.text = "";
    } 

    public void SetAdmin(bool _value)
    {
        isAdmin = _value;
        
        if (isAdmin)
        {
            // Enable Saved Paths button
            btnPaths.gameObject.SetActive(true);

            // Deactivate login button and password panel
            pnlPassword.gameObject.SetActive(false);
            btnPasswordLogin.gameObject.SetActive(false);

            // Activate logout button
            btnLogout.gameObject.SetActive(true);
        }
        else
        {
            // Disable Saved Paths button
            btnPaths.gameObject.SetActive(false);

            // Deactivate logout button
            btnLogout.gameObject.SetActive(false);

            // Activate login button and password panel
            pnlPassword.gameObject.SetActive(true);
            btnPasswordLogin.gameObject.SetActive(true);

            // Reload Ancient Messene
            DisplayAncientMessene();
        }

        // Disable Options Screen
        pnlOptionsScreen.gameObject.SetActive(false);

        // Disable Back Button
        btnBack.gameObject.SetActive(false);

        /*pnlWarningsPasswordScreen.SetActive(false);
        pnlWarningsAdminScreen.SetActive(false);
        DisplayAreaSelectScreen();*/
    }

    void CheckSettings()
    {
        // Check if all settings are valid
        if (IsValidPathSetting(tmpDesiredAccuracy.text) && IsValidPathSetting(tmpUpdateDistance.text)) // inptFldDesiredAccuracy.text inptFldUpdateDistance.text
        {
            // Save settings locally
            SaveSettings(float.Parse(tmpDesiredAccuracy.text), float.Parse(tmpUpdateDistance.text));

            // Set settings in location service
            SetLocationServiceSettings();

            // Close Settings Screen
            DisplayOptionsScreen();
        }
        else
        {
            //inptFldDesiredAccuracy.text = DEFAULT_DESIRED_ACCURACY.ToString();
            //inptFldUpdateDistance.text = DEFAULT_UPDATE_DISTANCE.ToString();

            sldrDesiredAccuracy.value = DEFAULT_DESIRED_ACCURACY;
            sldrUpdateDistance.value = DEFAULT_UPDATE_DISTANCE;
        }
    }

    void SetSettingsTexts()
    {
        // Set input field texts
        //inptFldDesiredAccuracy.text = OnlineMapsLocationService.instance.desiredAccuracy.ToString();
        //inptFldUpdateDistance.text = OnlineMapsLocationService.instance.updateDistance.ToString();
        sldrDesiredAccuracy.value = OnlineMapsLocationService.instance.desiredAccuracy;
        sldrUpdateDistance.value = OnlineMapsLocationService.instance.updateDistance;
    }

    void SetLocationServiceSettings()
    {
        // Load settings
        float? desiredAccuracy = LoadSetting(DESIRED_ACCURACY_KEY);
        float? updateDistance = LoadSetting(UPDATE_DISTANCE_KEY);

        // Set Location Service
        OnlineMapsLocationService.instance.desiredAccuracy = desiredAccuracy != null ? (float)desiredAccuracy : DEFAULT_DESIRED_ACCURACY;
        OnlineMapsLocationService.instance.updateDistance = updateDistance != null ? (float)updateDistance : DEFAULT_UPDATE_DISTANCE;
    }

    void SaveSettings(float _desiredAccuracy, float _updateDistance)
    {
        PlayerPrefs.SetFloat(DESIRED_ACCURACY_KEY, _desiredAccuracy);
        PlayerPrefs.SetFloat(UPDATE_DISTANCE_KEY, _updateDistance);
    }

    float? LoadSetting(string _key)
    {
        if (PlayerPrefs.HasKey(_key))
            return PlayerPrefs.GetFloat(_key);
        else
            return null;
    }

    bool IsValidPathSetting(string _text)
    {
        float f;

        return float.TryParse(_text, out f) && f >= 0;
    }

    public void ChangeSliderTxtDesiredAccuracy(float _number)
    {
        tmpDesiredAccuracy.text = _number.ToString();
    }

    public void ChangeSliderTxtUpdateDistance(float _number)
    {
        tmpUpdateDistance.text = _number.ToString();
    }
    #endregion

    #region QuestionnairePanel
    void DisplayQuestionnaire() // TODO: Remove
    {
        pnlWarningSurveyIntroScreen.SetActive(true);
        pnlQuestionnaireScreen.SetActive(true);
        pnlMainQuestions.SetActive(true);
        //pnlAreaSelectScreen.SetActive(false);
        pnlOptionsScreen.SetActive(false);
        //pnlAreasScreen.SetActive(false);
        ActivateButtons(true, true, true, false);
    }

    void OpenQuestionnaire()
    {
        userAnsweredSurvey = true;
        pnlWarningSurveyIntroScreen.SetActive(false);
        pnlQuestionnaireScreen.SetActive(true);
        pnlMainQuestions.SetActive(true);
        pnlOptionA.SetActive(false);
        pnlOptionB.SetActive(false);
        pnlOptionC.SetActive(false);
        pnlPathScreen.SetActive(false);
        ActivateButtons(true, true, true, false);
    }

    public void EnableThankYouScreen()
    {
        if (userAnsweredSurvey)
        {
            if (LanguageIsEnglish())
                txtWarningThankYou.text = "Thank you for answering our survey!";
            else
                txtWarningThankYou.text = "Ευχαριστούμε που απαντήσατε στο ερωτηματολόγιο μας!";
        }
        else
        {
            if (LanguageIsEnglish())
                txtWarningThankYou.text = "Thank you for your participation!";
            else
                txtWarningThankYou.text = "Ευχαριστούμε για τη συμμετοχή σας!";
        }

        pnlWarningThankYouScreen.SetActive(true);
    }

    /*public void CheckLanguage()
    {
        if (LocalizationSettings.SelectedLocale.Identifier == "en")
        {
            sexDropdown.ClearOptions();
            ageDropdown.ClearOptions();
            ageDropdown.AddOptions(ageValuesEN);
            sexDropdown.AddOptions(sexValuesEN);
            AppManager.Instance.questionnaireManager.optionDropdown.ClearOptions();
            AppManager.Instance.questionnaireManager.optionDropdown.AddOptions(optionValuesΕΝ);
            Debug.Log("English");
        }
        if (LocalizationSettings.SelectedLocale.Identifier == "el")
        {
            sexDropdown.ClearOptions();
            ageDropdown.ClearOptions();
            ageDropdown.AddOptions(ageValuesGR);
            AppManager.Instance.questionnaireManager.optionDropdown.ClearOptions();
            AppManager.Instance.questionnaireManager.optionDropdown.AddOptions(optionValuesGR);
            sexDropdown.AddOptions(sexValuesGR);
            Debug.Log("Greek");
        }
    }*/

    public void ChangeLanguage(bool _isGreek)
    {
        // Change language
        if (_isGreek)
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
        else
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];

        // Disable Options Screen
        pnlOptionsScreen.gameObject.SetActive(false);

        // Set main title
        if (AppManager.Instance.mapManager.currentArea != null)
        {
            if (LanguageIsEnglish())
                txtMainName.text = AppManager.Instance.mapManager.currentArea.titleEnglish;
            else
                txtMainName.text = AppManager.Instance.mapManager.currentArea.title;
        }

        // Disable Back Button
        btnBack.gameObject.SetActive(false);
    }

    public bool LanguageIsEnglish()
    {
        if (LocalizationSettings.InitializationOperation.IsDone)
        {
            return (LocalizationSettings.SelectedLocale.Identifier == "en");//.name == ENGLISH);
        }

        return true;
    }

    public void SetWarningTxtOnCheckUserPosition()
    {
        if (LanguageIsEnglish())
            txtWarning.text = "You are out of the Active Area";
        else
            txtWarning.text = "Βρίσκεστε εκτός ενεργής περιοχής";
    }

    public void SetWarningTxtOnCheckValue()
    {
        if (LanguageIsEnglish())
            txtWarning.text = "Please enter a valid option";
        else
            txtWarning.text = "Παρακαλώ εισάγετε μία έγκυρη επιλογή";
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
