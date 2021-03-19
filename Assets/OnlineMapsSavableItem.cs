using System;

public class OnlineMapsSavableItem
{
    public bool enabled = true;
    public Action invokeCallback;
    public Func<OnlineMapsJSONItem> jsonCallback;
    public Action<OnlineMapsJSONObject> loadCallback;
    public string label;
    public string name;
    public int priority;

    public OnlineMapsSavableItem(string name, string label, Func<OnlineMapsJSONItem> jsonCallback)
    {
        this.name = name;
        this.label = label;
        this.jsonCallback = jsonCallback;
    }

    public OnlineMapsSavableItem(string label, Action invokeCallback)
    {
        this.label = label;
        this.invokeCallback = invokeCallback;
    }
}