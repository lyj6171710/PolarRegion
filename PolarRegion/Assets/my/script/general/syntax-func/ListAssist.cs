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
    {//这个remove只会删除按顺序第一次找到的
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
                Debug.Log("不完全匹配");
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
        Debug.Log("完全一致");
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
    {//T是引用类型，自身以引用的形式传递给外界，这里使得形成一个复制品，切断引用关系
        List<T> copy = new List<T>();
        for (int i = 0; i < list.Count; i++)
        {
            copy.Add(list[i].GetCopy());
        }
        return copy;
    }

    public static List<T> CopyValueToNew<T>(this List<T> list)
    {//T是值类型，自身本来就以复制的形式传递给外界
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
        //Array.ConstrainedCopy（把某个数组的一部分复制到另一个数组中 ）
        //Array.ConstrainedCopy 对复制要求严格，只能是同类型或者源数组类型是目标类型的派生元素类型，不执行装箱，拆箱，向下转换
        T[] picks = new T[array.Length - 1];
        Array.ConstrainedCopy(array, 1, picks, 0, picks.Length);
        return picks;
    }
}

public static class DicAssist
{
    public static void ForEachCanModify<K, V>(this Dictionary<K, V> dic, Action<K> eachDo)
    {//不推荐在每帧调用时执行，
     //如果需要，请外界新增一个list作为变量，然后参考这个函数，直接把流程写明在外面
        List<K> keys = new List<K>(dic.Keys);
        foreach (K key in keys) eachDo(key);
    }

    public static void ForEachUntilModify<K, V>(this Dictionary<K, V> dic, Func<K, bool> meetDo)
    {//不推荐在每帧调用时执行，
     //如果需要，请外界新增一个list作为变量，然后参考这个函数，直接把流程写明在外面
        List<K> keys = new List<K>(dic.Keys);
        foreach (K key in keys) if (meetDo(key)) return;
    }
}