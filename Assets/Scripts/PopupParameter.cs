using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
public class ObjectToSave<T>
{
    public T @object;
}
public class PopupParameter : MonoBehaviour
{
    private readonly Dictionary<ActionType, Action> actionDic = new();
    private readonly Dictionary<string, string> storage = new();
    
    public PopupParameter AddAction(ActionType type, Action action, string text = null)
    {
        SaveObject<string>(type.ToString(), text);
        if (!actionDic.ContainsKey(type)) actionDic.Add(type, action);
        else actionDic[type] = action;
        return this;
    }

    public Action GetAction(ActionType type)
    {
        return actionDic.ContainsKey(type) ? actionDic[type] : null;
    }

    private PopupParameter SaveObject<T>(String key, T obj)
    {
        ObjectToSave<T> saveObject = new ObjectToSave<T>();
        saveObject.@object = obj;
        string jsonString = JsonUtility.ToJson(saveObject);
        if (!storage.ContainsKey(key)) storage.Add(key, jsonString);
        else storage[key] = jsonString;
        return this;
    }

    public T GetObject<T>(String key)
    {
        if (!storage.ContainsKey(key)) return default(T);
        string jsonString = storage[key];
        ObjectToSave<T> saveObject = JsonUtility.FromJson<ObjectToSave<T>>(jsonString);
        return saveObject.@object;
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}