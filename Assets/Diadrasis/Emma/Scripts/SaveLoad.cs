using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            
            
            List<OnlineMapsMarker> markers = new List<OnlineMapsMarker>();

            Debug.Log("Try Load State: " + position);
            // Create marker
            OnlineMapsMarkerManager.SetItems(markers);
            OnlineMapsMarkerManager.CreateItem(position, label);
            OnlineMapsControlBase.instance.GetPosition(position);
            

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
            //FindObjectOfType<MainManager>().SetActive(false);
            OnlineMapsMarkerManager.CreateItem(position, label);
        }
    }

    /// <summary>
    /// Draw line 
    /// </summary>
    /// <param name="markerList"></param>
    public static void DrawingOnWalking(List<OnlineMapsMarker> markerList, Color color, float width)
    {
        //FindObjectOfType<MainManager>().OnLocationChanged(OnlineMapsLocationService.instance.position);
        //FindObjectOfType<MainManager>().CheckMyLocation();
        //double distance = OnlineMapsUtils.DistanceBetweenPoints(locationService.position, locationService.);
        if (markerList.Count > 0)
        {
            List<Vector2> line = new List<Vector2>(); ;

            for (int i = 0; i < markerList.Count; i++)
            {
                line.Add(markerList[i].position);
            }
            if (OnlineMapsLocationService.instance.allowUpdatePosition)
            {
                OnlineMapsMarkerManager.CreateItem(OnlineMapsLocationService.instance.position.y, OnlineMapsLocationService.instance.position.x);
                OnlineMapsDrawingLine newLine = new OnlineMapsDrawingLine(line, color, width)
                {
                    followRelief = true
                };
                OnlineMapsDrawingElementManager.AddItem(newLine);
            }
        }

    }
}
