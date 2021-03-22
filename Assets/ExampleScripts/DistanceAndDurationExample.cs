using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/DistanceAndDurationExample")]
public class DistanceAndDurationExample : MonoBehaviour
{
    public Vector2 userCoordinares;

    /// <summary>
    /// The coordinates of the destination.
    /// </summary>
    public Vector2 markerCoordinates;

    /// <summary>
    /// The direction of the compass.
    /// </summary>
    public float compassTrueHeading = 0;
    private void Start()
    {
        OnlineMaps.instance.AddMarker(userCoordinares);
        OnlineMaps.instance.AddMarker(markerCoordinates);
    }
    private void OnGUI()
    {
        if (GUI.Button(new Rect(50, 50, 100, 180), "Calc"))
        {
            // Calculate the distance in km between locations.
            float distance = OnlineMapsUtils.DistanceBetweenPoints(userCoordinares, markerCoordinates).magnitude;
            FindObjectOfType<MainManager>().infoText.text = "Distance: " + distance;
            Debug.Log("Distance: " + distance);

            int zoom = 5;
            int maxX = 1 << (zoom - 1);

            // Calculate the tile position of locations.
            double userTileX, userTileY, markerTileX, markerTileY;
            OnlineMaps.instance.projection.CoordinatesToTile(userCoordinares.x, userCoordinares.y, zoom, out userTileX, out userTileY);
            OnlineMaps.instance.projection.CoordinatesToTile(markerCoordinates.x, markerCoordinates.y, zoom, out markerTileX, out markerTileY);

            // Calculate the angle between locations.
            double angle = OnlineMapsUtils.Angle2D(userTileX, userTileY, markerTileX, markerTileY);
            if (Math.Abs(userTileX - markerTileX) > maxX) angle = 360 - angle;
            
            Debug.Log("Angle: " + angle);

            // Calculate relative angle between locations.
            double relativeAngle = angle - compassTrueHeading;
            Debug.Log("Relative angle: " + relativeAngle);
        }
    }
}
