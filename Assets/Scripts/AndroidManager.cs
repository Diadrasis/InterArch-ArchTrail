//Class to check if gps is running on android and if not, prompt user to open grant permissions
using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif
using Stathis.Android;

public class AndroidManager : MonoBehaviour
{
    private OnlineMapsLocationService locationService;
    //public MainManager mm;

    // Start is called before the first frame update
    void Start()
    {
        locationService = OnlineMapsLocationService.instance;
        AppManager.Instance.uIManager.btnGPSPermission.onClick.AddListener(() => OpenNativeAndroidSettings());
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {

            Debug.Log("Please grant your gps location");
            Permission.RequestUserPermission(Permission.FineLocation);
            
        }
#endif
        if (CheckForLocationServices()) return;
    }

    //if on android prompt to get location permission on device
    private void IsAndroidBuild()
    {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Debug.Log("Please grant your gps location");
            Permission.RequestUserPermission(Permission.FineLocation);
        }
        
#endif
        if (!locationService.TryStartLocationService())
        {
            AppManager.Instance.uIManager.pnlGPSScreen.SetActive(true);
            //locationService.StopLocationService();
        }
        else
        {
            locationService.StartLocationService();
            AppManager.Instance.uIManager.pnlGPSScreen.SetActive(false);
            AppManager.Instance.uIManager.DisplayAreasScreen();
        }

    }

    //check if location services are active either with gpsemulator or on android
    public bool CheckForLocationServices()
    {
        if (locationService == null) return false;
        
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            if (locationService.useGPSEmulator)
            {
                AppManager.Instance.uIManager.pnlGPSScreen.SetActive(false);
                AppManager.Instance.uIManager.DisplayAreasScreen();
                Debug.Log("Use the GPSEmulator");
                Debug.Log(locationService);
                return true;
            }
            else
            {
                //these lines can uncomment on build or make sure to use the gpsEmulator from Map gameObject when UI manager is finished change the objects(texts,btns etc)
                AppManager.Instance.uIManager.DisplayAreasScreen();
                
                /*AppManager.Instance.uIManager.pnlGPSScreen.SetActive(true);
                AppManager.Instance.uIManager.btnGPSPermission.onClick.AddListener(() => OpenNativeAndroidSettings());*/
                /*mm.txtSettings.text = "Press the gps button to grant the access permission";
                mm.btnGPSPermission.onClick.AddListener(() => OpenNativeAndroidSettings());
                mm.btnMessiniMap.gameObject.SetActive(false);
                mm.btnNewAreaOnMap.gameObject.SetActive(false);
                Debug.Log("Please grant your gps location");*/
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

    private void Update()
    {
        IsAndroidBuild();
    }

}
