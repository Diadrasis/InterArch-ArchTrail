using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    OnlineMaps instance;
    private OnlineMapsMarker playerMarker;
    public Button btnLayer,btnCurrentLoc;
    public Text infoText;
    public float time = 3;
    private float angle;

    private bool isMovement;
    private Vector2 fromPosition;
    private Vector2 toPosition;
    private double fromTileX, fromTileY, toTileX, toTileY;
    private int moveZoom;

   
    void Start()
    {
       
        //StartCoroutine(GetStarting());
        btnLayer.onClick.AddListener(()=>CheckLayer());
        btnCurrentLoc.onClick.AddListener(() => CheckMyLocation());
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            StartCoroutine(GetStarting());
        }
        playerMarker = OnlineMapsMarkerManager.CreateItem(new Vector2(0, 0), null, "Player");

        // Get instance of LocationService.
        OnlineMapsLocationService locationService = OnlineMapsLocationService.instance;

        if (locationService == null)
        {
            Debug.LogError(
                "Location Service not found.\nAdd Location Service Component (Component / Infinity Code / Online Maps / Plugins / Location Service).");
            return;
        }

        // Subscribe to the change location event.
        locationService.OnLocationChanged += OnLocationChanged;

#endif
    }

    IEnumerator GetStarting()
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
    }
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
        if(map.mapType == "google.relief")
        {
            map.mapType = "google.terrain";

        }else 
        if(map.mapType == "google.terrain")
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
}
