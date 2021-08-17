//Class to check if gps is running on android and if not, prompt user to open grant permissions
using UnityEngine;
using UnityEngine.Android;
using Stathis.Android;
using TMPro;
using System.Collections;

public class AndroidManager : MonoBehaviour
{
    #region Variables
    private OnlineMapsLocationService locationService;
    #endregion

    #region Unity Functions
    private void Start()
    {
        SetupLocationService();
    }

    private void Update()
    {
        // Check if location service is enabled by user
        if (!IsLSEnabledByUser())
            return;

        //locationService.TryStartLocationService();

        /*AppManager.Instance.uIManager.pnlGPSSignal.SetActive(true);
        string status = Input.location.lastData.horizontalAccuracy.ToString();//Input.location.status.ToString();
        
        if (HasGPS())
        {
            *//*if (AppManager.Instance.uIManager.pnlGPSSignal.activeSelf)
                AppManager.Instance.uIManager.pnlGPSSignal.SetActive(false);*//*
        }
        else
        {
            *//*if (!AppManager.Instance.uIManager.pnlGPSSignal.activeSelf)
            {
                //Debug.Log(Input.location.status);
                AppManager.Instance.uIManager.pnlGPSSignal.SetActive(true);
            }*//*
            status += " No signal";
        }

        AppManager.Instance.uIManager.pnlGPSSignal.GetComponent<TextMeshProUGUI>().text = status;*/
    }

    /*private void OnApplicationFocus(bool focus)
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
            if (CheckForLocationServices())
                return;
        }
    }*/

    private void OnApplicationQuit()
    {
        if (locationService != null)
            locationService.StopLocationService();
    }
    #endregion

    #region Methods
    private void SetupLocationService()
    {
        locationService = OnlineMapsLocationService.instance;

        if (locationService == null)
        {
            Debug.LogError("Location Service not found.\nAdd Location Service Component (Component / Infinity Code / Online Maps / Plugins / Location Service).");
            return;
        }

        //locationService.TryStartLocationService();

        /*#if PLATFORM_ANDROID
                if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                {
                    //Debug.Log("Please grant your gps location");
                    Permission.RequestUserPermission(Permission.FineLocation);
                }
        #endif
                if (CheckForLocationServices())
                    return;*/
    }

    private bool IsLSEnabledByUser()
    {
        if (!(Application.platform == RuntimePlatform.WindowsEditor) && !(Application.platform == RuntimePlatform.OSXEditor))
        {
            if (!Input.location.isEnabledByUser)
            {
                if (!AppManager.Instance.uIManager.pnlGPSScreen.activeSelf)
                    AppManager.Instance.uIManager.pnlGPSScreen.SetActive(true);
                return false;
            }
            else
            {
                if (AppManager.Instance.uIManager.pnlGPSScreen.activeSelf)
                    AppManager.Instance.uIManager.pnlGPSScreen.SetActive(false);
                return true;
            }
        }
        else
            return true;
    }

    /*IEnumerator StartLocationService()
    {
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
            yield break;

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            //print("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            //print("Unable to determine device location");
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            //print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }

        // Stop service if there is no need to query location updates continuously
        //Input.location.Stop();
    }*/

    //check if location services are active either with gpsemulator or on android
    /*public bool CheckForLocationServices()
    {
        if (locationService == null)
            return false;

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
    }*/

    // if true then location servise is activated by user and is receiving GPS data.
    public bool HasGPS()
    {
        if (locationService == null)
            return false;

        // Check if platform is editor
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            // Reurn true if gps emulator is enabled and false if it is not.
            return locationService.useGPSEmulator;
        }
        else
        {
            // If location service is running then we are receiving gps data normally.
            /*if (locationService.IsLocationServiceRunning())
                return true;
            else
                return locationService.TryStartLocationService();*/
            return (locationService.IsLocationServiceRunning() && (Input.location.lastData.horizontalAccuracy <= locationService.desiredAccuracy));
        }
    }

    //open the grant permissions on android devices
    public void OpenNativeAndroidSettings()
    {
#if PLATFORM_ANDROID
        AndroidBridge.OpenIntent(IntentNames.GPS_SETTINGS);
#endif
    }
    #endregion

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
