using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Dug
{
    public static void ff(this bool f, int sign = 0)//��������õ�
    {
        Debug.Log(sign + ":::" + f);//ð���и����Ե�ͬʱ���������໥����
    }

    public static void ff(this float f, int sign = 0)//��������õ�
    {
        Debug.Log(sign + ":::" + f);//ð���и����Ե�ͬʱ���������໥����
    }

    public static void ff(this int i, int sign = 0)
    {
        Debug.Log(sign + ":::" + i);
    }

    public static void ff(this Vector2 v, int sign = 0)
    {
        Debug.Log(sign + ":::" + v.TrimToStr());
    }

    public static void ff(this Vector2Int v, int sign = 0)
    {
        Debug.Log(sign + ":::" + v.ToString());
    }

    public static void ff(this Vector3 v, int sign = 0)
    {
        Debug.Log(sign + ":::" + v);
    }

    public static void ff(this string s, int sign = 0)
    {
        Debug.Log(sign + ":::" + s);
    }
}

public class Du:MonoBehaviour,ISwitchScene
{//���������õ�

    public void WhenAwake()
    {
        if (dicd != null)
            dicd.Clear();
        else
            dicd = new Dictionary<int, bool>();
    }

    public void WhenSwitchScene()
    {

    }

    //���ܲ��Թ���=========================================================

    static DateTime nowTime;

    static void start_time_()
    {
        nowTime = DateTime.Now;
    }

    static int stop_time_()
    {
        return (int)(DateTime.Now - nowTime).TotalMilliseconds;
    }

    public static void check_cost_(Action act)
    {
        start_time_();
        for (int i = 0; i < 50000; i++)
            act();
        Debug.Log(stop_time_());
    }

    //bug���Թ���====================================================

    public static void ff()//��������
    {
        Debug.Log("exe!");
    }

    public static void ff(int sign)//��Ҫ������ⲻͬ�ص�����̣���ִ�еĴ������
    {
        Debug.Log(sign);
    }

    static Dictionary<int, bool> dicd;

    public static void ff(int id, bool onoff)//����״̬�仯ʱ���Ž���һ�δ�ӡ
    {//��ִͬ�еص㣬��Ҫid��һ�����������Ը�
        if (dicd.ContainsKey(id))
        {
            if (dicd[id] != onoff)
            {
                dicd[id] = onoff;
                Debug.Log(id);//idͬʱ�����������ֲ�ͬ�ص��������
            }
        }
        else
        {
            dicd.Add(id, onoff);
            Debug.Log(id);
        }
    }

    //==================================

    public static void ff(bool whenTrue, ref float printFloat, string tip = "")
    {
        if (whenTrue)
            Debug.Log(tip + "" + printFloat);
    }

    public static void ff(bool whenTrue, ref int printInt, string tip = "")
    {
        if (whenTrue)
            Debug.Log(tip + "" + printInt);
    }
    public static void ff(bool whenTrue, ref bool printBool, string tip = "")
    {
        if (whenTrue)
            Debug.Log(tip + "" + printBool);
    }

    public static void ff(bool whenTrue, ref Vector2 printVector2, string tip = "")
    {
        if (whenTrue)
            Debug.Log(tip + "" + printVector2);
    }

    public static void ff(bool whenTrue, ref Vector3 printVector3, string tip = "")
    {
        if (whenTrue)
            Debug.Log(tip + "" + printVector3);
    }

    //==================================

    public static void ff(bool whenTrue, int id = 0, string tip = "")
    {
        if (whenTrue)
            Debug.Log(tip + "" + id);
    }
}
