using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class OnlineMapsInteractiveElementManager<T, U> : MonoBehaviour, IEnumerable<U>
    where T : OnlineMapsInteractiveElementManager<T, U>
    where U : IOnlineMapsInteractiveElement
{
    protected static T _instance;

    [SerializeField]
    protected List<U> _items;

    public static T instance
    {
        get
        {
            if (_instance == null && Application.isPlaying) Init();
            return _instance;
        }
    }

    public List<U> items
    {
        get
        {
            if (_items == null) _items = new List<U>();
            return _items;
        }
        set
        {
            _items = new List<U>(value);
        }
    }

    public static int CountItems
    {
        get { return instance.Count; }
    }

    public int Count
    {
        get { return items.Count; }
    }

    public U this[int index]
    {
        get { return items[index]; }
        set { items[index] = value; }
    }

    public static U AddItem(U item)
    {
        return instance.Add(item);
    }

    public static void AddItems(IEnumerable<U> collection)
    {
        instance.AddRange(collection);
    }

    public static void Init()
    {
        _instance = FindObjectOfType<T>() ?? FindObjectOfType<OnlineMaps>().gameObject.AddComponent<T>();
    }

    public static void RemoveAllItems()
    {
        instance.items.Clear();
    }

    public static void RemoveAllItems(Predicate<U> match)
    {
        instance.items.RemoveAll(match);
    }

    public static bool RemoveItem(U item, bool dispose = true)
    {
        return instance.Remove(item, dispose);
    }

    public static U RemoveItemAt(int index)
    {
        return instance.RemoveAt(index);
    }

    protected static void Redraw()
    {
        if (OnlineMaps.instance != null) OnlineMaps.instance.Redraw();
    }

    public static void SetItems(IEnumerable<U> collection)
    {
        instance.items = new List<U>(collection);
    }

    public U Add(U item)
    {
        items.Add(item);
        Redraw();
        return item;
    }

    public void AddRange(IEnumerable<U> collection)
    {
        items.AddRange(collection);
        Redraw();
    }

    public IEnumerator GetEnumerator()
    {
        return items.GetEnumerator();
    }

    IEnumerator<U> IEnumerable<U>.GetEnumerator()
    {
        return items.GetEnumerator();
    }

    protected virtual void OnEnable()
    {

    }

    public bool Remove(U item, bool dispose = true)
    {
        if (dispose) item.Dispose();
        Redraw();
        return items.Remove(item);
    }

    public void RemoveAll(bool dispose = true)
    {
        if (dispose) foreach (U item in items) item.Dispose();
        items.Clear();
        Redraw();
    }

    public void RemoveAll(Predicate<U> match, bool dispose = true)
    {
        if (dispose)
        {
            foreach (U item in items.FindAll(match)) item.Dispose();
        }

        items.RemoveAll(match);
        Redraw();
    }

    public U RemoveAt(int index, bool dispose = true)
    {
        if (index < 0 || index >= items.Count) return default(U);
        U item = items[index];
        if (dispose) item.Dispose();
        items.RemoveAt(index);
        Redraw();
        return item;
    }
}