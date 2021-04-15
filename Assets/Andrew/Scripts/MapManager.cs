using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    #region Variables
    [HideInInspector]
    public List<cArea> areas = new List<cArea>();
    [HideInInspector]
    public cArea currentArea;

    #endregion

    #region Unity Functions
    private void Start()
    {
        areas = LoadAreas();
        //DisplayAreaDebug(0);
    }
    #endregion

    #region Methods
    private List<cArea> LoadAreas()
    {
        List<cArea> areasFromDatabase = new List<cArea>()
        {
            new cArea("Μεσσήνη", new Vector2(21.9202085525009f, 37.17642261183837f), 17, new Vector4(21.9160667457503f, 37.1700252387224f , 21.9227518498302f, 37.178659594564f)),
            new cArea("Κνωσός", new Vector2(25.16310005634713f, 35.29800050616538f), 19, new Vector4(25.1616718900387f, 35.2958874528396f , 25.1645352578472f, 35.3000733065711f))
        };

        return areasFromDatabase;
    }

    private void DisplayAreaDebug(int _index)
    {
        // Title
        Debug.Log("Title = " + areas[_index].title);

        // Position
        Debug.Log("Longitude = " + areas[_index].position.x);
        Debug.Log("Latitude = " + areas[_index].position.y);

        // Zoom
        Debug.Log("Zoom = " + areas[_index].zoom);

        // Constraints
        Debug.Log("minLongitude = " + areas[_index].constraints.x);
        Debug.Log("minLatitude = " + areas[_index].constraints.y);
        Debug.Log("maxLongitude = " + areas[_index].constraints.z);
        Debug.Log("maxLatitude = " + areas[_index].constraints.w);
    }

    public void SetMapView(cArea _areaToView)
    {
        //if (_areaToView.constraints != null)
        OnlineMaps.instance.positionRange = new OnlineMapsPositionRange(_areaToView.constraints.y, _areaToView.constraints.x, _areaToView.constraints.w, _areaToView.constraints.z);
        OnlineMaps.instance.zoomRange = new OnlineMapsRange(_areaToView.zoom, OnlineMaps.MAXZOOM);
        OnlineMaps.instance.SetPositionAndZoom(_areaToView.position.x, _areaToView.position.y, _areaToView.zoom);
    }

    public cArea GetAreaByTitle(string _areaTitle)
    {
        foreach (cArea area in areas)
        {
            if (area.title.Equals(_areaTitle))
                return area;
        }

        return null;
    }
    #endregion
}
