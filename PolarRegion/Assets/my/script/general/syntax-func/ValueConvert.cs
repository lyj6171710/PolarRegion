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

    public static string TrimToStr(this float state)//不求精确的
    {
        return state.ToString("0.00"); //四舍五入
    }

    public static string TrimToStr(this Vector2 vec)
    {
        return "(" + vec.x.TrimToStr() + ", " + vec.y.TrimToStr() + ")";
        //直接转换Vector2到string，默认好像不仅不保留，还转换成整数
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
            Console.WriteLine("无法转换！");
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
            Console.WriteLine("无法转换！");
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
        //return System.Text.Encoding.UTF8.GetString(rawData);//这种方式出现了问题，暂时不知道原因
        //System.Text.Encoding.UTF8.GetString(XXX).TrimEnd('\0');给字符串加上结束标识
        return Convert.ToBase64String(rawData);
    }

    public static byte[] StrToBytes(string carrier)
    {
        //return System.Text.Encoding.UTF8.GetBytes(carrier);
        return Convert.FromBase64String(carrier);
    }

    //=================================

    //保留小数点后两位（方法总结）

    //void sample()
    //{
    //    //最简单使用：

    //    float i = 1.6667f;

    //    string show = i.ToString("0.00"); //结果1.67(四舍五入)



    //    //其他类似方法：

    //    string show = i.ToString("F");//"F2","f" 不区分大小写

    //    string show = String.Format("{0:F}", i);//也可以为F2,或者"{0:0.00}

    //    float j = Math.Round(i, 2);

    //    string show = j.ToString();  //结果为1.67

    //    decimal.Round(decimal.Parse(i), 2); //结果1.67

    //    System.Globalization.NumberFormatInfo nfi = new System.Globalization.NumberFormatInfo();

    //    nfi.NumberDecimalDigits = 2;
    //    string result = i.ToString("N", nfi);//结果1.67



    //    //以上方法都是四舍五入的方式，下边是直接截断：

    //    float i = 32.16667F;

    //    int j = (int)(i * 100);

    //    i = j * 0.01F;//结果32.16
    //}

}
