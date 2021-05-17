using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class MethodHelper
{
    // ============ CONVERSION ============ //
    public static Vector2 ToVector2(string _stringToVector2)
    {
        string cleanString = _stringToVector2.Replace("(", "").Replace(")", "");
        string[] splitString = cleanString.Split(',');
        float x = float.Parse(splitString[0], CultureInfo.InvariantCulture.NumberFormat);
        float y = float.Parse(splitString[1], CultureInfo.InvariantCulture.NumberFormat);
        Vector2 stringConvertedToVector2 = new Vector2(x, y);
        return stringConvertedToVector2;
    }

    // ============ JSON ============ //
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    //If this is a Json array from the server
    //string jsonString = FixJson(yourJsonFromServer);
    //Player[] player = JsonHelper.FromJson<Player>(jsonString);
    public static string SetupJson(string value)
    {
        value = "{\"Items\":" + value + "}";
        return value;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
