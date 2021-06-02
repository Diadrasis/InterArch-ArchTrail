//Class to check if gps is running on android and if not, prompt user to open grant permissions
using UnityEngine;
using UnityEngine.Android;
using Stathis.Android;
using TMPro;
public class AndroidManager : MonoBehaviour
{
    private OnlineMapsLocationService locationService;

    // Start is called before the first frame update
    private void Start()
    {
        Init();
        //AppManager.Instance.serverManager.CheckInternet();
    }

    void Init()
    {
        locationService = OnlineMapsLocationService.instance;
        
        if (locationService == null)
        {
            Debug.LogError(
                "Location Service not found.\nAdd Location Service Component (Component / Infinity Code / Online Maps / Plugins / Location Service).");
            return;
        }
        
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {

            Debug.Log("Please grant your gps location");
            Permission.RequestUserPermission(Permission.FineLocation);
            
        }
#endif
        //locationService.OnLocationChanged += AppManager.Instance.mapManager.OnLocationChanged;
        if (CheckForLocationServices()) return;
    }

    //check if location services are active either with gpsemulator or on android
    public bool CheckForLocationServices()
    {
        if (locationService == null) return false;

        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            if (locationService.useGPSEmulator)
            {
                //AppManager.Instance.uIManager.infoText.text = "Location on emulator";//testing
                AppManager.Instance.uIManager.pnlGPSScreen.SetActive(false);
                //Debug.Log("Use the GPSEmulator");
                return true;
            }
            else
            {
                //AppManager.Instance.uIManager.infoText.text = "Location on else app on editor";//testing
                AppManager.Instance.uIManager.pnlGPSScreen.SetActive(true);
            }
            return false;
        }
        
        if (!locationService.IsLocationServiceRunning())
        {
            if (!locationService.TryStartLocationService())
            {
                AppManager.Instance.uIManager.pnlGPSScreen.SetActive(true);
                
                return true;
            }
            else
            {
                //AppManager.Instance.uIManager.infoText.text = "Location is running";//testing
                AppManager.Instance.uIManager.pnlGPSScreen.SetActive(false);
                return false;
            }
            
        }
        else
        {
            AppManager.Instance.uIManager.pnlGPSScreen.SetActive(false);
            //Debug.Log("Please grant your gps location");
            locationService.StopLocationService();// in case we want to chech if the user has closed the gps remove the comment, if not commnet this and uncomment UPDATE!!
            return false;
        }
        
    }
       
    //open the grant permissions on android devices
    public void OpenNativeAndroidSettings()
    {
        AndroidBridge.OpenIntent(IntentNames.GPS_SETTINGS);
    }
    public void OnApplicationQuit()
    {

        //System.Diagnostics.Process.GetCurrentProcess().Kill(); //this makes unity to close when stopping, beware!!
        locationService.StopLocationService();
    }

    private void Update()
    {
        //if (CheckForLocationServices()) return;//if we remove this line, we shouldn't leave the StopLocationService on the method ON, on the bool method!!!

    }
    


    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            //Debug.LogWarning("OnApplicationFocus = false");
            //popUpLocationOff.SetActive(false);
            //ShowErrorMessage(false, null);
            //locationService.StopLocationService();
        }
        else 
        {
            if (!AppManager.Instance.serverManager.isShownOnce)
            {
                AppManager.Instance.serverManager.CheckInternet();
            }
            //Debug.LogWarning("OnApplicationFocus = true");
            if (CheckForLocationServices()) { return; }
            
        }
    }
    #region NOT IN USE
    /*private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            if (CheckForLocationServices()) { return; }
            AppManager.Instance.serverManager.CheckInternet();
        }
        else
        {
            locationService.StopLocationService();
            
        }
    }*/

    /*public void OnCheckInternetCheckComplete(bool val)
    {
        if (val)
        {
            AppManager.Instance.uIManager.pnlWarningScreen.SetActive(false);
            Debug.Log("Internet check on Android Manager and val is: " + val);
            *//*btnInternet.image.color = Color.green;
            if (!isReadDatabase)
            {
                serverManager.GetPathsFromServer();
                isReadDatabase = true;
            }*//*
        }
        else
        {
            AppManager.Instance.uIManager.pnlWarningScreen.SetActive(true);
            AppManager.Instance.uIManager.txtWarning.text = "Please check your internet connection";
            Debug.Log("Internet check on Android Manager (else) and val is: " + val);

        }
    }*/
    #endregion

}
