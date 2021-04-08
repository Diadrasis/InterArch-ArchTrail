using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoad : MonoBehaviour
{
    
    public static void SaveNewMarkersAndArea(string prefsKey)
    {
        OnlineMapsXML xml = new OnlineMapsXML("Markers");
        
        foreach (OnlineMapsMarker marker in OnlineMapsMarkerManager.instance)
        {
            // Create marker node
            OnlineMapsXML markerNode = xml.Create("Marker");
            markerNode.Create("Position", marker.position);
            markerNode.Create("Label", marker.label);
            Debug.Log("Save New "+marker.label);
        }

        // Save xml string
        PlayerPrefs.SetString(prefsKey, xml.outerXml);
        PlayerPrefs.Save();
    }
    public void SaveMarkers(string prefsKey, List<OnlineMapsMarker> listMarkers)
    {
        // Create XMLDocument and first child
        OnlineMapsXML xml = new OnlineMapsXML("Markers");

        // Save markers data
        foreach (OnlineMapsMarker marker in listMarkers)
        {
            // Create marker node
            OnlineMapsXML markerNode = xml.Create("Marker");
            markerNode.Create("Position", marker.position);
            markerNode.Create("Label", prefsKey + "_" + marker.label);
        }

        // Save xml string
        PlayerPrefs.SetString(prefsKey, xml.outerXml);
        PlayerPrefs.Save();
    }
    //load markers in a menu so user can find easily places that have already visit
    public static void TryLoadMarkers(string prefsKey)
    {

        // If the key does not exist, returns.
        if (!PlayerPrefs.HasKey(prefsKey)) return;

        // Load xml string from PlayerPrefs
        string xmlData = PlayerPrefs.GetString(prefsKey);

        // Load xml document
        OnlineMapsXML xml = OnlineMapsXML.Load(xmlData);

        // Load markers
        foreach (OnlineMapsXML node in xml)
        {
            // Gets coordinates and label
            Vector2 position = node.Get<Vector2>("Position");
            string label = node.Get<string>("Label");

            // Create marker
            OnlineMapsMarkerManager.CreateItem(position, label);
        }
    }

    public static void TryLoadMarkers(string prefsKey, out List<OnlineMapsMarker> listMarkers)
    {
        listMarkers = new List<OnlineMapsMarker>();

        // If the key does not exist, returns.
        if (!PlayerPrefs.HasKey(prefsKey))
        {
            return;
        }

        // Load xml string from PlayerPrefs
        string xmlData = PlayerPrefs.GetString(prefsKey);

        // Load xml document
        OnlineMapsXML xml = OnlineMapsXML.Load(xmlData);

        // Load markers
        foreach (OnlineMapsXML node in xml)
        {
            OnlineMapsMarker marker = new OnlineMapsMarker();

            // Gets coordinates and label
            Vector2 position = node.Get<Vector2>("Position");
            string label = node.Get<string>("Label");

            marker.position = position;
            marker.label = label;

            listMarkers.Add(marker);

            // Create marker
            FindObjectOfType<MainManager>().settingsScreen.SetActive(false);
            OnlineMapsMarkerManager.CreateItem(position, label);
        }
    }
}
