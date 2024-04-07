using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

public class ObjectToSave<T>
{
    public T @object;
}

public enum ParamType
{
    LevelData,
    ModeGame,
    LevelIndex,
    PreviousLevel,
    CoinTxt,
    GemTxt,
}

public class Parameter : MonoBehaviour
{
    private readonly Dictionary<PopupKey, UnityAction> actionDic = new();
    private readonly Dictionary<string, string> storage = new();

    public void AddAction(PopupKey type, UnityAction action, string text = null)
    {
        SaveObject(type.ToString(), text);
        if (!actionDic.ContainsKey(type)) actionDic.Add(type, action);
        else actionDic[type] = action;
    }

    public UnityAction GetAction(PopupKey type)
    {
        var action = actionDic.ContainsKey(type) ? actionDic[type] : null;
        actionDic.Remove(type);
        return action;
    }

    public void SaveObject<T>(ParamType key, T obj)
    {
        var saveObject = new ObjectToSave<T>();
        saveObject.@object = obj;
        var jsonString = JsonUtility.ToJson(saveObject);
        if (!storage.ContainsKey(key.ToString())) storage.Add(key.ToString(), jsonString);
        else storage[key.ToString()] = jsonString;
    }

    public T GetObject<T>(ParamType key)
    {
        if (!storage.ContainsKey(key.ToString())) return default(T);
        var jsonString = storage[key.ToString()];
        var saveObject = JsonUtility.FromJson<ObjectToSave<T>>(jsonString);
        return saveObject.@object;
    }

    public void SaveObject<T>(string key, T obj)
    {
        var saveObject = new ObjectToSave<T>();
        saveObject.@object = obj;
        var jsonString = JsonUtility.ToJson(saveObject);
        if (!storage.ContainsKey(key)) storage.Add(key, jsonString);
        else storage[key] = jsonString;
    }

    public T GetObject<T>(String key)
    {
        if (!storage.ContainsKey(key)) return default(T);
        var jsonString = storage[key];
        var saveObject = JsonUtility.FromJson<ObjectToSave<T>>(jsonString);
        return saveObject.@object;
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}