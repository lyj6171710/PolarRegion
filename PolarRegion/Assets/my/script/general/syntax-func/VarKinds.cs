using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Int
{ //让单个整型变量也可引用，所用引用了的地方指向同一个值
    public int at = 0;

    public static Int operator ++(Int self)
    {
        self.at++;
        return self;
    }

    public static explicit operator int(Int self)
    {//显式强制向int转换
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
    string name;//推迟命名，临时命名，外界随不同情况，给不同名字

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
            Debug.Log("不匹配");//不应该出现这种情况，告诉外界有差错
            return default(T);
        }
    }

    public void Set(string key,T value)
    {
        if (key == name) this.value = value;
        else Debug.Log("不匹配");
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