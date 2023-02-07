using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using Sirenix.OdinInspector;
using System;

//==============================

public abstract class FloatAttr//一个属性集，每个元素类型一样时使用
{//使用时，建议用枚举和int对应起来，外界使用枚举来使用这个属性集
 //这个类的好处是，可以批量随机地修改、所含成员
    protected Dictionary<int, Float> values;

    public FloatAttr()
    {
        values = new Dictionary<int, Float>();
        for (int i = 0; i < Num; i++)
        {
            values.Add(i, new Float());//注意是从0开始，0利于程序设计
        }
    }

    public abstract int Num { get; }//子类中所含属性的数量

    public abstract string Belong { get; }//子类自身特殊性的标识

    public void Set(int i, float value)
    {//不检查范围，因为不应该存在，所以直接抛错
        values[i].at = value;
    }

    public void Offset(int i, float degree)
    {
        values[i].at += degree;
    }

    public float Get(int i)
    {
        return values[i].at;
    }

    //子类应该实现以下函数，这里只是示例，外界不应该使用================

    public static int SampleEnumToInt(ESample match)
    {
        switch (match)
        {
            case ESample.one: return 0;
            case ESample.two: return 1;
            default: return -1;
        }
    }

    public static ESample SampleIntToEnum(int match)
    {
        switch (match)
        {
            case 0: return ESample.one;
            case 1: return ESample.two;
            default: Debug.Log("没有这种属性"); return ESample.one;
        }
    }

    public void SampleSet(ESample match, float value)
    {
        Set(SampleEnumToInt(match), value);
    }

    public float SampleGet(ESample match)
    {
        return Get(SampleEnumToInt(match));
    }

    public IfoSetSample ToIfoSetSample()
    {
        IfoSetSample result = new IfoSetSample();
        result.one = Get(0);
        result.two = Get(1);
        return result;
    }
}

public abstract class IfoSetFloatAttr//用来预置，程序运行时不使用该类
{
    public void InputToAttr(FloatAttr attr)
    {
        FieldInfo[] fieldInfos = FieldInfos;
        for (int i = 0; i < fieldInfos.Length; i++)
        {
            attr.Set(i, (float)fieldInfos[i].GetValue(this));
        }
    }

    public abstract FieldInfo[] FieldInfos { get; }
}

public enum ESample { one, two }

public class IfoSetSample : IfoSetFloatAttr
{
    public float one;
    public float two;

    public override FieldInfo[] FieldInfos { get { return typeof(IfoSetSample).GetFields(); } }

    public static IfoSetSample operator +(IfoSetSample any,IfoSetSample other)
    {
        IfoSetSample result = new IfoSetSample();
        result.one=any.one+ other.one;
        result.two=any.two+ other.two;
        return result;
    }
}

//==============================

[Serializable]
public class StrAttrs: ICopySelf<StrAttrs>
{
    //动态增减的属性，相对不同个体，有不同内容

    //适合对于只有个别事物才会有的属性，这样就不用所有元素都占用一个位置来存储该属性

    internal Dictionary<string, string> set;
    //动态属性是稀少情况，所以就用string记录名与值就行，高通用
    //外界不要直接使用这个变量

    public StrAttrs()
    {
        set = new Dictionary<string, string>();
    }

    public void Set(string name, float value, bool safe = false)
    {
        if (set.ContainsKey(name))
            set[name] = value.TrimToStr();
        else if (!safe)
            set.Add(name, value.TrimToStr());
    }

    public void Set(string name, int value, bool safe = false)
    {
        if (set.ContainsKey(name))
            set[name] = value.ToString();
        else if (!safe)
            set.Add(name, value.ToString());
    }

    public void Set(string name, bool state, bool safe = false)
    {
        if (set.ContainsKey(name))
            set[name] = state.ToString();
        else if (!safe)
            set.Add(name, state.ToString());
    }

    public void Set(string name, string mean, bool safe = false)
    {
        if (set.ContainsKey(name))
            set[name] = mean;
        else if (!safe)
            set.Add(name, mean);
    }

    //------------------------------------

    public bool Get(string name, out float value)
    {
        if (set.ContainsKey(name))
        {
            return ValueConvert.StrToFloat(set[name], out value);
        }
        else
        {
            value = 0;
            return false;
        }
    }

    public bool Get(string name, out int value)
    {
        if (set.ContainsKey(name))
        {
            return ValueConvert.StrToInt(set[name], out value);
        }
        else
        {
            value = 0;
            return false;
        }
    }

    public bool Get(string name, out bool value)
    {
        if (set.ContainsKey(name))
        {
            return ValueConvert.StrToBool(set[name], out value);
        }
        else
        {
            value = false;
            return false;
        }
    }

    public bool Get(string name, out string value)
    {
        if (set.ContainsKey(name))
        {
            value = set[name];
            return true;
        }
        else
        {
            value = null;
            return false;
        }
    }

    public StrAttrs GetCopy()
    {
        StrAttrs copy = new StrAttrs();
        copy.set = set.CopyValueToNew();
        return copy;
    }

    [Serializable]
    public class StoSave
    {//用来支持存储或预置，中间介质
     
        //没找到可识别元素内容改变时就能触发的特性，这里采用了特殊办法来实现这个效果
        //OnValueChanged不响应集合元素值的改变,OnCollectionChanged只响应集合元素量的改变
        public List<Str> storage;

        internal int meNum { get { return storage.Count; } }

        internal string GetAttr(int index) 
        {
            if (storage[index].value != null)
                return storage[index].value;
            else
                return "";
        }

        internal void AddAttr(string value) 
        {
            storage.Add(new Str(value, this));
        }

        public void ResetNum() { CheckReady(); storage.Clear(); }

        //============================

        [NonSerialized]
        bool haveInitial;

        public StoSave()
        { //确保外界可直观编辑的该类变量，都是以内部程序构造出来的，不然可能出问题
            CheckReady();
            WhenNumChanged = () => { }; 
            WhenElemChanged = () => { };
        }

        internal void CheckReady() //外界经过反序列化生成的该类变量，没有被初始化，所以还需要确保手段
        {
            if (!haveInitial)
            {
                storage = new List<Str>(); haveInitial = true;
            }
        }

        //用来支持对内容变化的跟踪=======================
        //外界自己可以利用起跟踪效果，随需做立即处理

        public Action WhenNumChanged;
        public Action WhenElemChanged;

        [Button]
        void IfAddPleaseHere(string value = null) { AddAttr(value); WhenNumChanged(); }
        //只针对在编辑器界面增加元素时使用

        [Serializable]
        public struct Str
        {
            [OnValueChanged("OnValueChanged")]
            public string value;

            [NonSerialized]
            StoSave attach;

            public Str(string value, StoSave attach) { this.value = value; this.attach = attach; }

            void OnValueChanged() { attach.WhenElemChanged(); }
        }
    }
}

public static class StrAttrStatic
{
    public static void ReadIn(this StrAttrs.StoSave save, StrAttrs from)
    {
        save.CheckReady();
        foreach (string varName in from.set.Keys)
        {
            string pack = varName + "-" + from.set[varName];
            save.AddAttr(pack);
        }
    }

    public static StrAttrs WriteOut(this StrAttrs.StoSave save)
    {
        save.CheckReady();
        StrAttrs attrs = new StrAttrs();
        for (int i = 0; i < save.meNum; i++)
        {
            string one = save.GetAttr(i);
            if (one != "")
            {
                string[] var = StrAnalyse.GetBothSides(one, '-');
                attrs.set.Add(var[0], var[1]);
            }
        }
        return attrs;
    }
}