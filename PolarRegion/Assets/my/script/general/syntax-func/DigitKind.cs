using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Digit
{
    public bool[] binary = new bool[4];
    //true����1��false����0
    //����1�Ͷ�Ӧ��һ��������λ

    public int ToDecimal()
    {
        return Convert.ToInt32(AsString(), 2);//����ϵͳ�Դ�����
    }

    public void FromDecimal(int num)
    {
        FromString(Convert.ToString(num, 2));
        //ToString�����ĵڶ���������ʾĿ������ϵͳ�Ļ�����
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
        Array.Reverse(chars);//�ߵ�һ�£�ԭ�������ģ�Խ��λ��Խǰ����
        return new string(chars);
    }

    public void FromString(string refer)//��Ҫ����1��0��������ַ�����������д���Ľ��
    {
        char[] chars = refer.ToCharArray();//�ַ����еĵ�һ���ַ����ᱻ��Ϊ����ĵ�һ��Ԫ��
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
