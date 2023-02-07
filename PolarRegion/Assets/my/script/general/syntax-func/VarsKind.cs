using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using Sirenix.OdinInspector;
using System;

//==============================

public abstract class FloatAttr//һ�����Լ���ÿ��Ԫ������һ��ʱʹ��
{//ʹ��ʱ��������ö�ٺ�int��Ӧ���������ʹ��ö����ʹ��������Լ�
 //�����ĺô��ǣ���������������޸ġ�������Ա
    protected Dictionary<int, Float> values;

    public FloatAttr()
    {
        values = new Dictionary<int, Float>();
        for (int i = 0; i < Num; i++)
        {
            values.Add(i, new Float());//ע���Ǵ�0��ʼ��0���ڳ������
        }
    }

    public abstract int Num { get; }//�������������Ե�����

    public abstract string Belong { get; }//�������������Եı�ʶ

    public void Set(int i, float value)
    {//����鷶Χ����Ϊ��Ӧ�ô��ڣ�����ֱ���״�
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

    //����Ӧ��ʵ�����º���������ֻ��ʾ������粻Ӧ��ʹ��================

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
            default: Debug.Log("û����������"); return ESample.one;
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

public abstract class IfoSetFloatAttr//����Ԥ�ã���������ʱ��ʹ�ø���
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
    //��̬���������ԣ���Բ�ͬ���壬�в�ͬ����

    //�ʺ϶���ֻ�и�������Ż��е����ԣ������Ͳ�������Ԫ�ض�ռ��һ��λ�����洢������

    internal Dictionary<string, string> set;
    //��̬������ϡ����������Ծ���string��¼����ֵ���У���ͨ��
    //��粻Ҫֱ��ʹ���������

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
    {//����֧�ִ洢��Ԥ�ã��м����
     
        //û�ҵ���ʶ��Ԫ�����ݸı�ʱ���ܴ��������ԣ��������������취��ʵ�����Ч��
        //OnValueChanged����Ӧ����Ԫ��ֵ�ĸı�,OnCollectionChangedֻ��Ӧ����Ԫ�����ĸı�
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
        { //ȷ������ֱ�۱༭�ĸ���������������ڲ�����������ģ���Ȼ���ܳ�����
            CheckReady();
            WhenNumChanged = () => { }; 
            WhenElemChanged = () => { };
        }

        internal void CheckReady() //��羭�������л����ɵĸ��������û�б���ʼ�������Ի���Ҫȷ���ֶ�
        {
            if (!haveInitial)
            {
                storage = new List<Str>(); haveInitial = true;
            }
        }

        //����֧�ֶ����ݱ仯�ĸ���=======================
        //����Լ��������������Ч������������������

        public Action WhenNumChanged;
        public Action WhenElemChanged;

        [Button]
        void IfAddPleaseHere(string value = null) { AddAttr(value); WhenNumChanged(); }
        //ֻ����ڱ༭����������Ԫ��ʱʹ��

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