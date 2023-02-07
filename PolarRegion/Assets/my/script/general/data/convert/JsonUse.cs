using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class JsonUse
{
    //类对象与json的转换=====================================

    public static string SuToJson(object need)
    {
        string content = JsonUtility.ToJson(need, true);
        return content;
    }

    public static bool SuFromJson<T>(string content, out T result)
    {////这里必需显式给出类型信息(不能只给对象的基类类型)，不然不能正确提取内容
        result = JsonUtility.FromJson<T>(content);//要注意，列表为空和列表元素数量为0可能是同一个值
        if (result == null)
            return false;
        else
            return true;
    }

}

[Serializable]
public class JsonDictionary<TKey, TValue> : ISerializationCallbackReceiver //这个接口是让能够被以json形式存读的关键
{
    Dictionary<TKey, TValue> target;

    public JsonDictionary() => target = new Dictionary<TKey, TValue>();

    public TValue this[TKey key] => target[key];
    public bool ContainsKey(TKey key) => target.ContainsKey(key);
    public void Add(TKey key, TValue value) => target.Add(key, value);
    public void Remove(TKey key) => target.Remove(key);
    public void Clear() => target.Clear();
    public Dictionary<TKey,TValue>.KeyCollection Keys => target.Keys;

    //================================

    [SerializeField]
    List<TKey> keys;
    [SerializeField]
    List<TValue> values;

    public void OnBeforeSerialize() //系统调用
    {
        keys = new List<TKey>(target.Keys);
        values = new List<TValue>(target.Values);
    }

    public void OnAfterDeserialize()
    {
        target = new Dictionary<TKey, TValue>(keys.Count);
        for (var i = 0; i < keys.Count; ++i)
        {
            target.Add(keys[i], values[i]);
        }
    }
}