using System;
using UnityEngine;

[Serializable]
[DisallowMultipleComponent]
public class OnlineMapsMarkerManager : OnlineMapsMarkerManagerBase<OnlineMapsMarkerManager, OnlineMapsMarker>
{
    public Texture2D defaultTexture;
    public OnlineMapsAlign defaultAlign = OnlineMapsAlign.Bottom;
    public bool allowAddMarkerByM = true;

    public static OnlineMapsMarker CreateItem(Vector2 location, string label)
    {
        return instance.Create(location.x, location.y, null, label);
    }

    public static OnlineMapsMarker CreateItem(Vector2 location, Texture2D texture = null, string label = "")
    {
        return instance.Create(location.x, location.y, texture, label);
    }

    public static OnlineMapsMarker CreateItem(double longitude, double latitude, string label)
    {
        return instance.Create(longitude, latitude, null, label);
    }

    public static OnlineMapsMarker CreateItem(double longitude, double latitude, Texture2D texture = null, string label = "")
    {
        return instance.Create(longitude, latitude, texture, label);
    }

    public OnlineMapsMarker Create(Vector2 location, Texture2D texture = null, string label = "")
    {
        return Create(location.x, location.y, texture, label);
    }

    public OnlineMapsMarker Create(double longitude, double latitude, Texture2D texture = null, string label = "")
    {
        if (texture == null) texture = defaultTexture;
        OnlineMapsMarker marker = _CreateItem(longitude, latitude);
        marker.defaultTexture = texture;
        marker.label = label;
        marker.align = defaultAlign;
        marker.Init();
        Redraw();
        return marker;
    }

    public override OnlineMapsSavableItem[] GetSavableItems()
    {
        if (savableItems != null) return savableItems;

        savableItems = new[]
        {
            new OnlineMapsSavableItem("markers", "2D Markers", SaveSettings)
            {
                priority = 90,
                loadCallback = LoadSettings
            }
        };

        return savableItems;
    }

    public void LoadSettings(OnlineMapsJSONItem json)
    {
        OnlineMapsJSONItem jitems = json["items"];
        RemoveAll();
        foreach (OnlineMapsJSONItem jitem in jitems)
        {
            OnlineMapsMarker marker = new OnlineMapsMarker();

            double mx = jitem.ChildValue<double>("longitude");
            double my = jitem.ChildValue<double>("latitude");

            marker.SetPosition(mx, my);

            marker.range = jitem.ChildValue<OnlineMapsRange>("range");
            marker.label = jitem.ChildValue<string>("label");
            marker.defaultTexture = OnlineMapsUtils.GetObject(jitem.ChildValue<int>("texture")) as Texture2D;
            marker.align = (OnlineMapsAlign)jitem.ChildValue<int>("align");
            marker.rotation = jitem.ChildValue<float>("rotation");
            marker.enabled = jitem.ChildValue<bool>("enabled");
            Add(marker);
        }

        OnlineMapsJSONItem jsettings = json["settings"];
        defaultTexture = OnlineMapsUtils.GetObject(jsettings.ChildValue<int>("defaultTexture")) as Texture2D;
        defaultAlign = (OnlineMapsAlign)jsettings.ChildValue<int>("defaultAlign");
        allowAddMarkerByM = jsettings.ChildValue<bool>("allowAddMarkerByM");
    }

    protected override OnlineMapsJSONItem SaveSettings()
    {
        OnlineMapsJSONItem jitem = base.SaveSettings();
        jitem["settings"].AppendObject(new
        {
            defaultTexture = defaultTexture != null ? defaultTexture.GetInstanceID() : -1,
            defaultAlign = (int)defaultAlign,
            allowAddMarkerByM
        });
        return jitem;
    }

    protected override void Start()
    {
        base.Start();

        foreach (OnlineMapsMarker marker in items) marker.Init();
    }

    protected override void Update()
    {
        base.Update();

       if (allowAddMarkerByM && Input.GetKeyUp(KeyCode.M)) CreateItem(OnlineMapsControlBase.instance.GetCoords());
    }
}