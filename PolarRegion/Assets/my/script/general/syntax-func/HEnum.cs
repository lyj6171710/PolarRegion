using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IhrEnumMuster : IEnumerable<HEnum>
{
    //InheritableEnum

    //"��.��̬����"��ģ��"ö������.ö�ٵ�Ԫ"

    //�ʺϣ�ö����Ҫ�̳л�ѡ��ʱ

    //============================

    readonly HashSet<HEnum> ifoEnum;

    protected HEnum pickEnum;//��ǰѡ��

    protected IhrEnumMuster(HEnum id, HashSet<HEnum> ifoEnum)
    {
        //���ฺ���Ӧ��int�����Ҵ��ݽ�����Ϣ
        this.ifoEnum = ifoEnum;
        pickEnum = id;
    }

    //==============================

    public static bool operator ==(IhrEnumMuster e1, IhrEnumMuster e2)
    {
        return e1.pickEnum.Equals(e2.pickEnum);
    }

    public static bool operator !=(IhrEnumMuster e1, IhrEnumMuster e2)
    {
        return !(e1 == e2);
    }

    public static bool operator ==(IhrEnumMuster e1, HEnum e2)
    {
        return e1.pickEnum.Equals(e2);
    }

    public static bool operator !=(IhrEnumMuster e1, HEnum e2)
    {
        return !(e1 == e2);
    }

    public IEnumerator<HEnum> GetEnumerator()
    {
        return ifoEnum.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ifoEnum.GetEnumerator();
    }
}

public struct HEnum:IEquatable<HEnum>
{
    //���κ��Զ���Ŀɼ̳�ö������ʹ��

    public int index { get; private set; }
    //�����index�����ĳһ��ö�����͵�
    //index�������ڱ�����ĳЩ���㣬��ʱ��ס
    //index��һ��Ψһ�����ܻᱻ����ʹ�õ�
    public string name { get => value.NAME; }

    static int counter = -1;

    EValue value;

    public HEnum(string value, int index = -1)
    {
        this.value = EValue.Get(value);
        if (index < 0)
            this.index = ++counter;
        else
            this.index = counter = index;
    }

    //=========================

    public bool Equals(HEnum other)
    {
        return value == other.value;
    }

    //==========================

    internal class EValue
    {
        // ��Ϊ�ײ����������ֻ����ʽ��
        // ���ϲ�ֱ𰴸���������ʹ��(ʵ�ʻ�������)

        public string NAME { get; private set; }
        public int ID { get; private set; }
        //���id�Ͱ���ö�ٵ����ö�ٷ����id����һ������
        //��ĳһ��������ʽ��stringΨһ��Ӧ��������Խ�ʡ�ڴ�ռ�(����Ҫͬһ���ݶ�ݸ���)

        static int counter = -1;
        static Dictionary<string, EValue> dic = new Dictionary<string, EValue>();

        public static EValue Get(string name)
        {
            if (!dic.ContainsKey(name))
            {
                EValue one = new EValue();
                one.NAME = name;
                one.ID = ++counter;
                dic.Add(name, one);
            }
            return dic[name];
        }

        protected EValue() { }//Ϊ�˽���Ĭ�Ϲ��캯����ʹ��

        public static bool operator ==(EValue e1, EValue e2)
        {
            return e1.ID == e2.ID;
        }

        public static bool operator !=(EValue e1, EValue e2)
        {
            return e1.ID != e2.ID;
        }
    }

}

public class IhrEnumUseSample
{
    class EnumUse : IhrEnumMuster
    {
        public static readonly HashSet<HEnum> set = new HashSet<HEnum>() { my, his };
        //������Ҫ���������������ͬʱ��ʼ��
        //����ֻ��ʾ��

        public readonly static HEnum my = new HEnum("my");
        public readonly static HEnum his = new HEnum("his");

        public EnumUse(HEnum id) : base(id, set)
        {
        }
    }

    void UseProc()
    {
        EnumUse test = new EnumUse(EnumUse.my);
        if (test == EnumUse.my) ;
        foreach (HEnum elem in test) Debug.Log(elem.name);
    }
}

