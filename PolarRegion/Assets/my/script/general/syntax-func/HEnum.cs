using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IhrEnumMuster : IEnumerable<HEnum>
{
    //InheritableEnum

    //"类.静态变量"可模拟"枚举类型.枚举单元"

    //适合：枚举需要继承或选用时

    //============================

    readonly HashSet<HEnum> ifoEnum;

    protected HEnum pickEnum;//当前选择

    protected IhrEnumMuster(HEnum id, HashSet<HEnum> ifoEnum)
    {
        //子类负责对应到int，并且传递进来信息
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
    //被任何自定义的可继承枚举类型使用

    public int index { get; private set; }
    //这里的index是相对某一个枚举类型的
    //index能有利于遍历与某些计算，暂时留住
    //index不一定唯一，可能会被特殊使用的
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
        // 作为底层零件，但又只是形式上
        // 被上层分别按各自需求复制使用(实际还是引用)

        public string NAME { get; private set; }
        public int ID { get; private set; }
        //这个id和包含枚举的类给枚举分配的id不是一个东西
        //与某一种内容形式的string唯一对应，或许可以节省内存空间(不需要同一内容多份复制)

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

        protected EValue() { }//为了禁用默认构造函数的使用

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
        //子类里要单独定义出来并且同时初始化
        //这里只是示范

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

