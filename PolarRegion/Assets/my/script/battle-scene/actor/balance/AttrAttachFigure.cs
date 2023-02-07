using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public class AttrAttachFigure : MonoBehaviour,ITreeBranch
{
    public Ifo mRefer;//当前理论值
    public Ifo mNow;//当前实际值

    public IfoFeature mReferF;//元素性质理论值
    public IfoFeature mNowF;

    //===================================

    public void ReceiveNotifyFromUp(InfoSeal seal)
    {

    }

    public object RespondReadFromUp(InfoSeal seal, out int source)
    {
        source = 0;
        return null;
    }

    public void SelfReady(TreeBranch shelf)
    {
        mRefer = new Ifo();
        mNow = new Ifo();
        mReferF = new IfoFeature();
        mNowF = new IfoFeature();
    }


    public class Ifo:FloatAttr
    {
        public Ifo() : base() { }

        public override int Num { get { return 5; } }

        public override string Belong { get { return "extra"; } }

        public static int EnumToInt(EPR_AttrAttach match)
        {
            switch (match)
            {
                case EPR_AttrAttach.cooling:return 0;
                case EPR_AttrAttach.defpPierceFix:return 1;
                case EPR_AttrAttach.defmPierceFix:return 2;
                case EPR_AttrAttach.defpPiercePer:return 3;
                case EPR_AttrAttach.defmPiercePer: return 4;
                default:return -1;
            }
        }

        public static EPR_AttrAttach IntToEnum(int match)
        {
            switch (match)
            {
                case 0:return EPR_AttrAttach.cooling;
                case 1:return EPR_AttrAttach.defpPierceFix;
                case 2:return EPR_AttrAttach.defmPierceFix;
                case 3:return EPR_AttrAttach.defpPiercePer;
                case 4:return EPR_AttrAttach.defmPiercePer;
                default: Debug.Log("没有这种属性"); return EPR_AttrAttach.cooling;
            }
        }


        public void Set(EPR_AttrAttach match, float value)
        {
            Set(EnumToInt(match), value);
        }

        public float Get(EPR_AttrAttach match)
        {
            return Get(EnumToInt(match));
        }
    }

    [System.Serializable]
    public class IfoSet : IfoSetFloatAttr
    {
        [Min(1)] public int cooling = 1;//冷却缩减

        [Min(1)] public int defpPierceFix = 1;//护甲固定穿透
        [Min(1)] public int defmPierceFix = 1;//魔抗固定穿透

        [Min(1)] public int defpPiercePer = 1;//护甲百分比穿透
        [Min(1)] public int defmPiercePer = 1;//魔抗百分比穿透

        public override FieldInfo[] FieldInfos { get { return typeof(IfoSet).GetFields(); } }
    }

}

public class IfoFeature : FloatAttr
{
    public override int Num { get { return 8; } }

    public override string Belong { get { return "feature"; } }

    public static int EnumToInt(EPR_Feature match)
    {
        switch (match)
        {
            case EPR_Feature.wind: return 0;
            case EPR_Feature.fire: return 1;
            case EPR_Feature.water: return 2;
            case EPR_Feature.metal: return 3;
            case EPR_Feature.wood: return 4;
            case EPR_Feature.soil: return 5;
            case EPR_Feature.light: return 6;
            case EPR_Feature.dark: return 7;
            default: return -1;
        }
    }


    public static EPR_Feature IntToEnum(int match)
    {
        switch (match)
        {
            case 0: return EPR_Feature.wind;
            case 1: return EPR_Feature.fire;
            case 2: return EPR_Feature.water;
            case 3: return EPR_Feature.metal;
            case 4: return EPR_Feature.wood;
            case 5: return EPR_Feature.soil;
            case 6: return EPR_Feature.light;
            case 7: return EPR_Feature.dark;
            default: Debug.Log("不存在"); return EPR_Feature.wind;
        }
    }

    public void Set(EPR_Feature match, float value)
    {
        Set(EnumToInt(match), value);
    }

    public float Get(EPR_Feature match)
    {
        return Get(EnumToInt(match));
    }

    public IfoSetFeature ToIfoSet()
    {
        IfoSetFeature result = new IfoSetFeature();
        result.wind = Get(0);
        result.fire = Get(1);
        result.water = Get(2);
        result.metal = Get(3);
        result.wood = Get(4);
        result.soil = Get(5);
        result.light = Get(6);
        result.dark = Get(7);
        return result;
    }

    //扩展----------------------------------------

    public float GetImpact(IfoFeature from)//返回负面影响的程度
    {
        return GetImpact(from.ToIfoSet());
    }

    public float GetImpact(IfoSetFeature from)
    {
        float acc = 0;
        acc += from.wind;
        acc += MathNum.AtLeast0(from.fire - Get(EPR_Feature.water));
        acc += MathNum.AtLeast0(from.water - Get(EPR_Feature.soil));
        acc += MathNum.AtLeast0(from.metal - Get(EPR_Feature.fire));
        acc += MathNum.AtLeast0(from.wood - Get(EPR_Feature.metal));
        acc += MathNum.AtLeast0(from.soil - Get(EPR_Feature.wood));
        acc += MathNum.AtLeast0(from.light - Get(EPR_Feature.dark));
        acc += MathNum.AtLeast0(from.dark - Get(EPR_Feature.light));
        return acc;
    }
}

[System.Serializable]
public class IfoSetFeature : IfoSetFloatAttr
{

    [Min(1)] public float wind = 1;//不受克制
    [Min(1)] public float fire = 1;//克制金
    [Min(1)] public float water = 1;//克制火
    [Min(1)] public float metal = 1;//克制木
    [Min(1)] public float wood = 1;//克制土
    [Min(1)] public float soil = 1;//克制水
    [Min(1)] public float light = 1;//克制暗
    [Min(1)] public float dark = 1;//克制光

    public override FieldInfo[] FieldInfos { get { return typeof(IfoSetFeature).GetFields(); } }

    public static IfoSetFeature operator +(IfoSetFeature one,IfoSetFeature other)
    {
        IfoSetFeature result = new IfoSetFeature();
        result.wind = one.wind + other.wind;
        result.fire = one.fire + other.fire;
        result.water = one.water + other.water;
        result.metal = one.metal + other.metal;
        result.wood = one.wood + other.wood;
        result.soil = one.soil + other.soil;
        result.light = one.light + other.light;
        result.dark = one.dark + other.dark;
        return result;
    }
}