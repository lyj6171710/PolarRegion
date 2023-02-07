using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListAssist
{
    public static List<T> NewRemoveLast<T>(this List<T> list)
    {
        List<T> newList = list.CopyValueToNew();
        if (newList.Count > 0)
            newList.RemoveAt(newList.Count - 1);
        return newList;
    }

    //==================================

    public static List<T> SelfAddLast<T>(this List<T> list,T elem)
    {
        list.Add(elem);
        return list;
    }

    public static List<T> SelfRemoveLast<T>(this List<T> list)
    {
        if (list.Count > 0)
            list.RemoveAt(list.Count - 1);
        return list;
    }


    public static void SelfRemoveFirst<T>(this List<T> list, T one)
    {//���removeֻ��ɾ����˳���һ���ҵ���
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Equals(one))
            {
                list.RemoveAt(i);
                return;
            }
        }
    }

    public static List<T> SelfTrimSameFromLeft<T>(this List<T> list, List<T> refer)
    {
        for (int i = 0; i < refer.Count; i++)
        {
            if (list[i].Equals(refer[i]))
                list.RemoveAt(i);
            else
            {
                Debug.Log("����ȫƥ��");
                break;
            }
        }
        return list;
    }

    //===============================================

    public static bool IfContainAndDo<T>(this List<T> list, T one, System.Action<int> action)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Equals(one))
            {
                action(i);
                return true;
            }
        }
        return false;
    }

    public static T GetFirstDifferFromLeft<T>(this List<T> list, List<T> refer)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (i < refer.Count)
            {
                if (list[i].Equals(refer[i]))
                    continue;
                else
                    return list[i];
            }
            else
                return list[i];
        }
        Debug.Log("��ȫһ��");
        return default(T);
    }

    public static T GetLast<T>(this List<T> list) 
    {
        if (list.Count == 0)
            return default(T);
        else
            return list[list.Count - 1];
    }

    //=========================================

    public static List<T> BreakRefToNew<T>(this List<T> list) where T : ICopySelf<T>
    {//T���������ͣ����������õ���ʽ���ݸ���磬����ʹ���γ�һ������Ʒ���ж����ù�ϵ
        List<T> copy = new List<T>();
        for (int i = 0; i < list.Count; i++)
        {
            copy.Add(list[i].GetCopy());
        }
        return copy;
    }

    public static List<T> CopyValueToNew<T>(this List<T> list)
    {//T��ֵ���ͣ����������Ը��Ƶ���ʽ���ݸ����
        List<T> copy = new List<T>();
        for (int i = 0; i < list.Count; i++)
        {
            copy.Add(list[i]);
        }
        return copy;
    }

    public static Dictionary<T, F> CopyValueToNew<T, F>(this Dictionary<T, F> dic)
    {
        Dictionary<T, F> copy = new Dictionary<T, F>();
        foreach (T key in dic.Keys)
        {
            copy.Add(key, dic[key]);
        }
        return copy;
    }

    //=========================================
}

public static class ArrayAssist
{
    public static T[] GetSelfExceptFirst<T>(this T[] array)
    {
        //Array.ConstrainedCopy����ĳ�������һ���ָ��Ƶ���һ�������� ��
        //Array.ConstrainedCopy �Ը���Ҫ���ϸ�ֻ����ͬ���ͻ���Դ����������Ŀ�����͵�����Ԫ�����ͣ���ִ��װ�䣬���䣬����ת��
        T[] picks = new T[array.Length - 1];
        Array.ConstrainedCopy(array, 1, picks, 0, picks.Length);
        return picks;
    }
}

public static class DicAssist
{
    public static void ForEachCanModify<K, V>(this Dictionary<K, V> dic, Action<K> eachDo)
    {//���Ƽ���ÿ֡����ʱִ�У�
     //�����Ҫ�����������һ��list��Ϊ������Ȼ��ο����������ֱ�Ӱ�����д��������
        List<K> keys = new List<K>(dic.Keys);
        foreach (K key in keys) eachDo(key);
    }

    public static void ForEachUntilModify<K, V>(this Dictionary<K, V> dic, Func<K, bool> meetDo)
    {//���Ƽ���ÿ֡����ʱִ�У�
     //�����Ҫ�����������һ��list��Ϊ������Ȼ��ο����������ֱ�Ӱ�����д��������
        List<K> keys = new List<K>(dic.Keys);
        foreach (K key in keys) if (meetDo(key)) return;
    }
}