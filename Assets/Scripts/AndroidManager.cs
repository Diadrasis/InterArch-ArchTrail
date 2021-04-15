//Class to check if gps is running on android and if not, prompt user to open grant permissions
using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif
using Stathis.Android;

/*
 Before Build, change all the UI elements with UIManager!!
 */
public class AndroidManager : MonoBehaviour
{
    private OnlineMapsLocationService locationService;
    public MainManager mm;

    // Start is called before the first frame update
    void Start()
    {
        mm.btnGPSPermission.onClick.AddListener(() => OpenNativeAndroidSettings());
        Init();
    }
    void Init()
    {
        locationService = OnlineMapsLocationService.instance;
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation) && locationService.TryStartLocationService())
        {

            Debug.Log("Please grant your gps location");
            Permission.RequestUserPermission(Permission.FineLocation);
            
            //infotext to inform user whty to give access on gps
        }
#endif
        if (CheckForLocationServices()) return;
    }
    private void Update()
    {

        //InAndroidBuild();// on final build uncomments.Is commented in order to work with gps emulator

    }
    //if on android prompt to get location permission on device
    void InAndroidBuild()
    {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {

            Permission.RequestUserPermission(Permission.FineLocation);
        }
#endif
        if (!locationService.TryStartLocationService())
        {
            mm.settingsScreen.SetActive(true);
            mm.txtSettings.text = "Press the gps button to grant the location permission of your mobile";
            mm.btnNewAreaOnMap.gameObject.SetActive(false);
            mm.btnMessiniMap.gameObject.SetActive(false);
            Debug.Log("Access the gps permissions");
            //locationService.StopLocationService();
        }
        else if (locationService.useGPSEmulator)
        {
            
            //locationService.StartLocationService();
            Debug.Log("Emulator is On");
        }
        else
        {
            mm.txtSettings.text = "We can start with our app";
            mm.btnGPSPermission.gameObject.SetActive(false);
            //also open mainScreen here when build like on useGpasEmulator
            //locationService.StartLocationService();
        }
    }

    //check if location services are active either with gpsemulator or on android
    private bool CheckForLocationServices()
    {
        if (locationService == null) return false;


        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            if (locationService.useGPSEmulator)
            {
                //mm.btnMessiniMap.onClick.AddListener(() => mm.MessiniLocation(locationService.position));
                mm.settingsScreen.SetActive(true);
                mm.txtSettings.text = "Επιλέξτε μια περιοχή";
                mm.btnNewAreaOnMap.gameObject.SetActive(true);
                mm.btnMessiniMap.gameObject.SetActive(true);
                mm.btnGPSPermission.gameObject.SetActive(false);
                Debug.Log(locationService);
                
                locationService.OnLocationChanged += mm.OnLocationChanged;
                //CheckMyLocation();
                return true;
            }
            else
            {
                InAndroidBuild();
                mm.settingsScreen.SetActive(true);
                mm.txtSettings.text = "Open gps";
                //btnClose.gameObject.SetActive(true); //on build we can remove it
                return true;
            }

        }
        return false;
    }
    //open the grant permissions on android devices
    void OpenNativeAndroidSettings()
    {
        AndroidBridge.OpenIntent(IntentNames.GPS_SETTINGS);
    }
    void OnApplicationQuit()
    {
        locationService.StopLocationService();
    }
}
