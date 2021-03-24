using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/DistanceAndDurationExample")]
public class DistanceAndDurationExample : MonoBehaviour
{
    public float radiusKM = 0.1f;
    //public GameObject circle;
    /// <summary>
    /// Number of segments
    /// </summary>
    public int segments = 32;
    //private OnlineMapsMarker marker;
    double lng, lat;
    /// <summary>
    /// This method is called when a user clicks on a map
    /// </summary>
    private IEnumerator CreateMarker()
    {
        Debug.Log("CreateMarker");
        // Get the coordinates under cursor
        yield return new WaitForSeconds(1f);
        lng = OnlineMaps.instance.position.x;
        lat = OnlineMaps.instance.position.y;
        OnlineMapsControlBase.instance.GetCoords(out lng, out lat);

        // Create a new marker under cursor
        OnlineMapsMarkerManager.CreateItem(lng, lat, "Marker5 " + OnlineMapsMarkerManager.CountItems);

        OnlineMaps map = OnlineMaps.instance;

        // Get the coordinate at the desired distance
        double nlng, nlat;
        OnlineMapsUtils.GetCoordinateInDistance(lng, lat, radiusKM, 90, out nlng, out nlat);

        double tx1, ty1, tx2, ty2;

        // Convert the coordinate under cursor to tile position
        map.projection.CoordinatesToTile(lng, lat, 20, out tx1, out ty1);

        // Convert remote coordinate to tile position
        map.projection.CoordinatesToTile(nlng, nlat, 20, out tx2, out ty2);

        // Calculate radius in tiles
        double r = tx2 - tx1;

        // Create a new array for points
        OnlineMapsVector2d[] points = new OnlineMapsVector2d[segments];

        // Calculate a step
        double step = 360d / segments;

        // Calculate each point of circle
        for (int i = 0; i < segments; i++)
        {
            double px = tx1 + Math.Cos(step * i * OnlineMapsUtils.Deg2Rad) * r;
            double py = ty1 + Math.Sin(step * i * OnlineMapsUtils.Deg2Rad) * r;
            map.projection.TileToCoordinates(px, py, 20, out lng, out lat);
            points[i] = new OnlineMapsVector2d(lng, lat);
        }

        // Create a new polygon to draw a circle
        if(OnlineMaps.instance.position.x <= lng)
        OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingPoly(points, Color.red, 13));
        OnlineMaps.instance.AddMarker(OnlineMaps.instance.position,"AAA");
        List<OnlineMapsMarker> markers = OnlineMapsMarkerManager.instance.items;

        float distance = OnlineMapsUtils.DistanceBetweenPoints(markers[0].position,markers[1].position).magnitude;
        Debug.Log(distance);

        yield break;
    }

    /// <summary>
    /// This method is called when the script starts
    /// </summary>
    private void Start()
    {
        //marker = OnlineMaps.instance.AddMarker(OnlineMaps.instance.position, "Marker");
        // Subscribe to click on map event
        //OnlineMapsControlBase.instance.OnMapClick += OnMapClick;
        StartCoroutine(CreateMarker());
    }

    /*private void Update()
    {
        circle.transform.position = OnlineMapsTileSetControl.instance.GetWorldPosition(marker.position);

        OnlineMaps api = OnlineMaps.instance;

        Vector2 distance = OnlineMapsUtils.DistanceBetweenPoints(api.topLeftPosition, api.bottomRightPosition);

        float scaleX = radiusKM / distance.x * api.tilesetSize.x;
        float scaleY = radiusKM / distance.y * api.tilesetSize.y;
        float scale = (scaleX + scaleY) / 2;

        circle.transform.localScale = new Vector3(scale, segments, scale);
    }*/

}

