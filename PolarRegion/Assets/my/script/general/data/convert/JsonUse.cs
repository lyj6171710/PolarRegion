using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class JsonUse
{
    //�������json��ת��=====================================

    public static string SuToJson(object need)
    {
        string content = JsonUtility.ToJson(need, true);
        return content;
    }

    public static bool SuFromJson<T>(string content, out T result)
    {////���������ʽ����������Ϣ(����ֻ������Ļ�������)����Ȼ������ȷ��ȡ����
        result = JsonUtility.FromJson<T>(content);//Ҫע�⣬�б�Ϊ�պ��б�Ԫ������Ϊ0������ͬһ��ֵ
        if (result == null)
            return false;
        else
            return true;
    }

}

[Serializable]
public class JsonDictionary<TKey, TValue> : ISerializationCallbackReceiver //����ӿ������ܹ�����json��ʽ����Ĺؼ�
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

    public void OnBeforeSerialize() //ϵͳ����
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