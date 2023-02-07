using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Digit
{
    public bool[] binary = new bool[4];
    //true代表1，false代表0
    //序数1就对应第一个正数数位

    public int ToDecimal()
    {
        return Convert.ToInt32(AsString(), 2);//利用系统自带函数
    }

    public void FromDecimal(int num)
    {
        FromString(Convert.ToString(num, 2));
        //ToString函数的第二个参数表示目标数字系统的基数。
    }

    public string AsString()
    {
        string convert = "";
        for (int i = 0; i < binary.Length; i++)
        {
            if (binary[i])
                convert += "1";
            else
                convert += "0";
        }
        char[] chars = convert.ToCharArray();
        Array.Reverse(chars);//颠倒一下，原来所做的，越低位在越前边了
        return new string(chars);
    }

    public void FromString(string refer)//不要带有1、0外的其它字符，否则可能有错误的结果
    {
        char[] chars = refer.ToCharArray();//字符串中的第一个字符，会被作为数组的第一个元素
        Array.Reverse(chars);
        for (int i = 0; i < binary.Length; i++)
        {
            if (i < chars.Length)
            {
                if (chars[i] == '0')
                    binary[i] = false;
                else
                    binary[i] = true;
            }
            else
                binary[i] = false;
        }
    }

    public void AllNone()
    {
        for (int i = 0; i < binary.Length; i++)
            binary[i] = false;
    }
}
