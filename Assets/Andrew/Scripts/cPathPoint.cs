using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cPathPoint
{
    Vector2 position;
    float time;

    public cPathPoint(Vector2 _position, float _time)
    {
        position = _position;
        time = _time;
    }

    //create path points when position or time is updated
    /*void Create()
    {

    }*/

    //save each point in a path
    /*void Save()
    {

    }*/

    //delete a path point
    void Delete()
    {

    }

    //show/load path points in a path. Probable will be associated with cPath
    void Show()
    {

    }
}
