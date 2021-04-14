//Class to check if gps is running on android and if not, prompt user to open grant permissions
using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif
using Stathis.Android;

public class AppManager : MonoBehaviour
{
    private OnlineMapsLocationService locationService;
    public MainManager mm;

    // Start is called before the first frame update
    void Start()
    {
        locationService = OnlineMapsLocationService.instance;
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {

            Debug.Log("Please grant your gps location");
            Permission.RequestUserPermission(Permission.FineLocation);

            //infotext to inform user whty to give access on gps
        }
#endif
        if (CheckForLocationServices()) return;
    }

    //if on android prompt to get location permission on device
    private void IsAndroidBuild()
    {
        Debug.Log("OnGUI before Android");
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            
            Debug.Log("Please grant your gps location");
            Permission.RequestUserPermission(Permission.FineLocation);
            mm.settingsScreen.SetActive(true);
            mm.settingsText.text = "Press the gps button to grant the access permission";
            mm.btnGPS.onClick.AddListener(() => OpenNativeAndroidSettings());
            mm.btnMessiniMap.gameObject.SetActive(false);
            mm.btnNewMap.gameObject.SetActive(false);
            //infotext to inform user whty to give access on gps
        }
#endif
        if (!locationService.TryStartLocationService())
        {
            locationService.StartLocationService();
            //locationService.StopLocationService();
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
                Debug.Log("Use the GPSEmulator");
                Debug.Log(locationService);
                return true;
            }
            else
            {
                //these lines can uncomment on build or make suer to use the gpsEmulator from Map gameObject when UI manager is finished change the objects(texts,btns etc)
                IsAndroidBuild();
                mm.settingsScreen.SetActive(true);
                mm.settingsText.text = "Press the gps button to grant the access permission";
                mm.btnGPS.onClick.AddListener(() => OpenNativeAndroidSettings());
                mm.btnMessiniMap.gameObject.SetActive(false);
                mm.btnNewMap.gameObject.SetActive(false);
                Debug.Log("Please grant your gps location");
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
