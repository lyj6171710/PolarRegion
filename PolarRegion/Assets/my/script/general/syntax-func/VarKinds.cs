using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Int
{ //�õ������ͱ���Ҳ�����ã����������˵ĵط�ָ��ͬһ��ֵ
    public int at = 0;

    public static Int operator ++(Int self)
    {
        self.at++;
        return self;
    }

    public static explicit operator int(Int self)
    {//��ʽǿ����intת��
        return self.at;
    }
}

public class SVector2
{
    public Vector2 v;
    public SVector2(Vector2 v) => this.v = v;

    public static implicit operator SVector2(Vector2 v)
    {
        return new SVector2(v);
    }
}

public class SVector3
{
    public Vector3 v;
    public SVector3(Vector3 v) => this.v = v;
}

public class Float
{
    public float at = 0;

    public static Float operator ++(Float self)
    {
        self.at++;
        return self;
    }
}

public class Str
{
    public string s = "";
    public static Str operator +(Str self, Str other)
    {
        Str str = new Str();
        str.s = self.s + other.s;
        return str;
    }
}

public class NameVar<T>
{
    T value;
    string name;//�Ƴ���������ʱ����������治ͬ���������ͬ����

    public NameVar(string name, T initial = default(T)) 
    {
        this.name = name;
        value = initial;
    }

    public T Value(string key)
    {
        if (key == name)
            return value;
        else
        {
            Debug.Log("��ƥ��");//��Ӧ�ó��������������������в��
            return default(T);
        }
    }

    public void Set(string key,T value)
    {
        if (key == name) this.value = value;
        else Debug.Log("��ƥ��");
    }
}

public struct RectMeter
{
    public Vector2 leftBottom;
    public Vector2 rightTop;

    public static RectMeter Normal()
    {
        RectMeter rect = new RectMeter();
        rect.leftBottom = Vector2.zero;
        rect.rightTop = Vector2.one;
        return rect;
    }
}