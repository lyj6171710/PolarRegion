using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ValueConvert
{

    //=============================

    public static bool ToBool(this int num)
    {
        return num > 0 ? true : false;
    }

    public static Vector2Int ToInt(this Vector2 vec)
    {
        return new Vector2Int(MathNum.FloatToClosestInt(vec.x), MathNum.FloatToClosestInt(vec.y));
    }

    public static string TrimToStr(this float state)//����ȷ��
    {
        return state.ToString("0.00"); //��������
    }

    public static string TrimToStr(this Vector2 vec)
    {
        return "(" + vec.x.TrimToStr() + ", " + vec.y.TrimToStr() + ")";
        //ֱ��ת��Vector2��string��Ĭ�Ϻ��񲻽�����������ת��������
    }

    public static EToward4 ToDir(this Vector2Int offset)
    {
        if (offset.y == 0)
        {
            if (offset.x == 0)
                return EToward4.middle;
            else if (offset.x < 0)
                return EToward4.left;
            else//offset.x > 0
                return EToward4.right;
        }
        else if (offset.x == 0)
        {
            if (offset.y > 0)
                return EToward4.up;
            else // offset.y < 0
                return EToward4.down;
        }
        else
            return EToward4.middle;
    }

    public static EToward4 ToDir(this Vector2 offset)
    {
        if (MathNum.Abs(offset.x) >= MathNum.Abs(offset.y))
        {
            if (offset.x > 0) return EToward4.right;
            else if (offset.x < 0) return EToward4.left;
            else return EToward4.middle;
        }
        else
        {
            if (offset.y > 0) return EToward4.up;
            else if (offset.y < 0) return EToward4.down;
            else return EToward4.middle;
        }
    }

    //================================================

    public static bool StrToInt(string source,out int result)
    {
        if (int.TryParse(source, out result))
        {
            return true;
        }
        else
        {
            Console.WriteLine("�޷�ת����");
            return false;
        }
    }
    public static bool StrToFloat(string source,out float result)
    {
        if (float.TryParse(source, out result))
        {
            return true;
        }
        else
        {
            Console.WriteLine("�޷�ת����");
            return false;
        }
    }

    public static bool StrToBool(string source,out bool result)
    {
        switch (source)
        {
            case "false":result = false; return true;
            case "true":result = true; return true;
            default:result = false;return false;
        }
    }

    //===============================

    public static string BytesToStr(byte[] rawData)
    {
        //return System.Text.Encoding.UTF8.GetString(rawData);//���ַ�ʽ���������⣬��ʱ��֪��ԭ��
        //System.Text.Encoding.UTF8.GetString(XXX).TrimEnd('\0');���ַ������Ͻ�����ʶ
        return Convert.ToBase64String(rawData);
    }

    public static byte[] StrToBytes(string carrier)
    {
        //return System.Text.Encoding.UTF8.GetBytes(carrier);
        return Convert.FromBase64String(carrier);
    }

    //=================================

    //����С�������λ�������ܽᣩ

    //void sample()
    //{
    //    //���ʹ�ã�

    //    float i = 1.6667f;

    //    string show = i.ToString("0.00"); //���1.67(��������)



    //    //�������Ʒ�����

    //    string show = i.ToString("F");//"F2","f" �����ִ�Сд

    //    string show = String.Format("{0:F}", i);//Ҳ����ΪF2,����"{0:0.00}

    //    float j = Math.Round(i, 2);

    //    string show = j.ToString();  //���Ϊ1.67

    //    decimal.Round(decimal.Parse(i), 2); //���1.67

    //    System.Globalization.NumberFormatInfo nfi = new System.Globalization.NumberFormatInfo();

    //    nfi.NumberDecimalDigits = 2;
    //    string result = i.ToString("N", nfi);//���1.67



    //    //���Ϸ���������������ķ�ʽ���±���ֱ�ӽضϣ�

    //    float i = 32.16667F;

    //    int j = (int)(i * 100);

    //    i = j * 0.01F;//���32.16
    //}

}
