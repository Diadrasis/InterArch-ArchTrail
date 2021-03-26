using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stathis.Android;
using UnityEngine.Android;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    OnlineMaps instance;
    private OnlineMapsMarker playerMarker;
    public Button btnLayer, btnCurrentLoc, btnGPS;
    public Text infoText;
    public float time = 3;
    private float angle;

    private bool isMovement;
    private Vector2 fromPosition;
    private Vector2 toPosition;
    private double fromTileX, fromTileY, toTileX, toTileY;
    private int moveZoom;
    OnlineMapsLocationService locationService;
    //public GameObject blackScreen;

    private void Awake()
    {

        Application.runInBackground = true;

    }
    void Start()
    {
        
        btnGPS.onClick.AddListener(() => OpenNativeAndroidSettings());
        InitLocation();
        //StartCoroutine(GetStarting());
    }
    void InitLocation()
    {
        locationService = OnlineMapsLocationService.instance;
        playerMarker = OnlineMapsMarkerManager.CreateItem(new Vector2(0, 0), null, "Player");

        if (locationService == null)
        {
            Debug.LogError(
                "Location Service not found.\nAdd Location Service Component (Component / Infinity Code / Online Maps / Plugins / Location Service).");
            return;
        }

        // Subscribe to the change location event.
        locationService.OnLocationChanged += OnLocationChanged;

        if (CheckForLocationServices()) return;
    }

    private bool CheckForLocationServices()
    {
        if (locationService == null) return false;

#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            if (Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                locationService.StartLocationService();
                locationService.IsLocationServiceRunning();
            }
        }
#endif
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            if (locationService.useGPSEmulator)

            {
                Debug.Log(locationService);
                infoText.text = "Checking location";
                StartCoroutine(CheckAppLocation());
                return true;
            }
            else
            {
                infoText.text = "Press the gps button";
                StartCoroutine(CheckAppLocation());
                return true;
            }
            
        }
        return false;
    }

    /* IEnumerator GetStarting()
{

#if UNITY_EDITOR
    int editorMaxWait = 15;
    while (Input.location.status == LocationServiceStatus.Stopped && editorMaxWait > 0)
    {
        yield return new WaitForSecondsRealtime(1);
        editorMaxWait--;
    }
#endif
    // Start service before querying location
    Input.location.Start();
    if (!Input.location.isEnabledByUser)
        yield break;

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
        print("Timed out");
        yield break;
    }

    // Connection has failed
    if (Input.location.status == LocationServiceStatus.Failed)
    {
        infoText.text = "Can't get location";
        print("Unable to determine device location");
        yield break;
    }
    else
    {
        print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
    }

    Input.location.Stop();
}*/
    void Update()
    {
        if (!isMovement) return;

        // update relative position
        angle += Time.deltaTime / time;

        if (angle > 1)
        {
            // stop movement
            isMovement = false;
            angle = 1;
        }

        // Set new position
        double px = (toTileX - fromTileX) * angle + fromTileX;
        double py = (toTileY - fromTileY) * angle + fromTileY;
        OnlineMaps.instance.projection.TileToCoordinates(px, py, moveZoom, out px, out py);
        OnlineMaps.instance.SetPosition(px, py);
        infoText.text = "Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp;
    }
   /* IEnumerator OpenSettings()
    {
        yield return new WaitForSeconds(1f);
#if PLATFORM_ANDROID
        if (Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            if (Permission.HasUserAuthorizedPermission(Permission.FineLocation) && !locationService.IsLocationServiceRunning())
            {
                try
                {
                    using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                    using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        *//*string packageName = currentActivityObject.Call<string>("getPackageName");

                        using (var uriClass = new AndroidJavaClass("android.net.Uri"))
                        using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))*//*
                        using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.ACTION_LOCATION_SOURCE_SETTINGS"*//*, uriObject*//*))
                        {
                            *//*intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                            intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);*//*
                            currentActivityObject.Call("startActivity", intentObject);
                        }
                    }
#endif
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }

            }
        }
        yield break;
    }*/
    void CheckMyLocation()
    {
        fromPosition = OnlineMaps.instance.position;

        // to GPS position;
        toPosition = /*new Vector2(Input.location.lastData.longitude, Input.location.lastData.latitude)*/ OnlineMapsLocationService.instance.position;

        // calculates tile positions
        moveZoom = OnlineMaps.instance.zoom;
        OnlineMaps.instance.projection.CoordinatesToTile(fromPosition.x, fromPosition.y, moveZoom, out fromTileX, out fromTileY);
        OnlineMaps.instance.projection.CoordinatesToTile(toPosition.x, toPosition.y, moveZoom, out toTileX, out toTileY);

        // if tile offset < 4, then start smooth movement
        if (OnlineMapsUtils.Magnitude(fromTileX, fromTileY, toTileX, toTileY) < 4)
        {
            // set relative position 0
            angle = 0;

            // start movement
            isMovement = true;
        }
        else // too far
        {
            OnlineMaps.instance.position = toPosition;
        }
    }
    void CheckLayer()
    {
        OnlineMaps map = OnlineMaps.instance;
        if (map.mapType == "google.relief")
        {
            map.mapType = "google.terrain";

        }
        else
        if (map.mapType == "google.terrain")
        {
            map.mapType = "google.satelite";
        }
        else
        {
            map.mapType = "google.relief";
        }
    }

    private void OnLocationChanged(Vector2 position)
    {
        // Change the position of the marker.
        playerMarker.position = position;

        // Redraw map.
        OnlineMaps.instance.Redraw();
    }

    IEnumerator CheckAppLocation()
    {
        //Vector2 currentPos = new Vector2(37.98f, 23.72413f);
        float distance = OnlineMapsUtils.DistanceBetweenPoints(OnlineMaps.instance.position,new Vector2(38f, 23.72413f)).magnitude;
        if (distance <= 100)
        {
            infoText.text = "You are in the correct area";
            btnLayer.onClick.AddListener(() => CheckLayer());
            btnCurrentLoc.onClick.AddListener(() => CheckMyLocation());
            Debug.Log("Accuracy"+Input.location.lastData.horizontalAccuracy);
            Debug.Log("Long " + Input.location.lastData.longitude + " Lat: " + Input.location.lastData.latitude);
            Debug.Log(distance);
            Debug.Log(locationService.desiredAccuracy);
        }
        else
        {
            infoText.text = "Please go near the area of GPS Location";
            locationService.StopLocationService();
            btnLayer.onClick.RemoveListener(() => CheckLayer());
            btnCurrentLoc.onClick.RemoveListener(() => CheckMyLocation());
        }
        yield break;
    }

    public void OpenNativeAndroidSettings()
    {
        AndroidBridge.OpenIntent(IntentNames.GPS_SETTINGS);
    }

    private void OnApplicationQuit()
    {
        locationService.StopLocationService();
    }
}
