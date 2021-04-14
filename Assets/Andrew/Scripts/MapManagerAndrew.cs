using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManagerAndrew : MonoBehaviour
{
    #region Variables
    public List<cArea> areas = new List<cArea>();
    #endregion

    #region Unity Functions
    private void Start()
    {
        areas = LoadAreas();
        DisplayAreaDebug(0);
    }
    #endregion

    #region Methods
    private List<cArea> LoadAreas()
    {
        List<cArea> areasFromDatabase = new List<cArea>()
        {
            new cArea("Μεσσήνη", new Vector2(21.9202085525009f, 37.17642261183837f), 17, new Vector4(21.9160667457503f, 37.1700252387224f , 21.9227518498302f, 37.178659594564f)),
            new cArea("", new Vector2(), 4, new Vector4())
        };

        return areasFromDatabase;
    }

    private void DisplayAreaDebug(int _index)
    {
        OnlineMaps.instance.SetPositionAndZoom(areas[_index].position.x, areas[_index].position.y, areas[_index].zoom);
        OnlineMaps.instance.positionRange = new OnlineMapsPositionRange(areas[_index].constraints.y, areas[_index].constraints.x, areas[_index].constraints.w, areas[_index].constraints.z);
        //OnlineMaps.instance.zoomRange = 

        Debug.Log("Title = " + areas[_index].title);

        Debug.Log("Longitude = " + areas[_index].position.x);
        Debug.Log("Latitude = " + areas[_index].position.y);

        Debug.Log("Zoom = " + areas[_index].zoom);

        Debug.Log("minLongitude = " + areas[_index].constraints.x);
        Debug.Log("minLatitude = " + areas[_index].constraints.y);
        Debug.Log("maxLongitude = " + areas[_index].constraints.z);
        Debug.Log("maxLatitude = " + areas[_index].constraints.w);
    }

    //public 
    #endregion
}
