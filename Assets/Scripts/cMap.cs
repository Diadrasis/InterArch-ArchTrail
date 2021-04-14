using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cMap 
{

    public string title;
    public Vector2 position;
    public int zoom;
    public Vector4 constraints;
    public List<cRoute> cRoutes = new List<cRoute>();

    public cMap(string _title,Vector2 _position, int _zoom, Vector4 _constraints )
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
}
