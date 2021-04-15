using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cArea
{
    #region Variables
    // We probably need to add an id to find areas with because the user might use the same name for two areas
    public string title;
    public Vector2 position; // longitude, latitude (x, y)
    public int zoom;
    public Vector4 constraints; // minLongitude, minLatitude, maxLongitude, maxLatitude (x, y, z, w)
    public List<cRouteAndrew> routes = new List<cRouteAndrew>();
    #endregion

    #region Methods
    public cArea(string _title, Vector2 _position, int _zoom, Vector4 _constraints)
    {
        title = _title;
        position = _position;
        zoom = _zoom;
        constraints = _constraints;
    }

    void Create()
    {

    }

    void Delete()
    {

    }

    void Show()
    {

    }

    void Save()
    {

    }
    #endregion
}
