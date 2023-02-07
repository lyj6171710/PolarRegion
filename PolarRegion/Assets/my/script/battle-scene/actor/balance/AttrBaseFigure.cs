using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class AttrBaseFigure : MonoBehaviour,ITreeBranch
{//只负责值及变化，不负责效应

    public bool meWhetherShowHp = true;
    ShowHp mHpShow;
    AssistPos mHpPos;

    //================================

    public IfoSet mStartDp;
    public IfoSet mMaxDp;

    Ifo mStart;//基础属性的初始值
    Ifo mMax;//基础属性的最大值

    public Ifo mRefer;//当前理论值
    public Ifo mNow;//当前实际值

    void InitialAttr()
    {
        mStart = new Ifo();
        mMax = new Ifo();
        mRefer = new Ifo();
        mNow = new Ifo();
        mStartDp.InputToAttr(mStart);
        mMaxDp.InputToAttr(mMax);
    }

    //==================================

    [Range(1,99)] public int mLevelStartDp;
    [Range(1, 99)] public int mLevelMaxDp;//最大级时到达最大属性值
    int mLevelNow;//最低1级
    int[] mExpNeed;//从等级i到等级i+1所需经验值
    int mExpNow;//相对当前等级的经验增量

    public int meLevelNow { get { return mLevelNow; } }

    void InitialLevel()
    {
        mExpNow = 0;
        mLevelNow = 0;

        mExpNeed = new int[mLevelMaxDp];
        for (int i = 0; i < mLevelMaxDp; i++)//ComputeToNext
        {
            int level = i + 1;//从等级i到等级i+1所需经验值
            float pow = Mathf.Pow(level, 2);
            mExpNeed[i] = Convert.ToInt32(pow);
        }

        for (int i = 0; i < mLevelStartDp; i++) RaiseLevel();
    }

    void RaiseLevel()
    {
        if (mLevelNow < mLevelMaxDp)
        {
            mLevelNow++;
            float progress = (float)mLevelNow / mLevelMaxDp;

            for (int i = 0; i < mRefer.Num; i++)
            {//i指第几个成员
                float last = mRefer.Get(i);
                mRefer.Set(i, progress * mMax.Get(i));
                mNow.Set(i, mNow.Get(i)+ mRefer.Get(i) - last);
            }
        }
    }

    public void GainExp(int amount)
    {
        if (amount <= 0)
            return;
        else if (mLevelNow >= mLevelMaxDp)
            return;
        else
        {
            if (amount < ExpToNextLevel())
                mExpNow += amount;
            else
            {
                int remainderExp = amount - ExpToNextLevel();
                mExpNow = 0;
                RaiseLevel();
                if (remainderExp > 0)
                    GainExp(remainderExp);
            }
        }
    }

    int ExpToNextLevel()
    {
        return mExpNeed[mLevelNow] - mExpNow;//前者是当前级别到下一级别所需总经验值
    }
    //===============================

    void HpShowStart()
    {
        if (meWhetherShowHp)
        {
            mHpShow = UiMager.It.meAlone.SuUse(EChartAlone.hp).GetComponent<ShowHp>();
            mHpShow.SuSetSize(0.5f);
            mHpPos = mHpShow.gameObject.AddComponent<AssistPos>();
            mHpPos.MakeReady();
            mHpShow.SuOpen();
            mHpShow.SuRefresh(mNow.Get(EPR_AttrBase.hp), mRefer.Get(EPR_AttrBase.hp));
        }
    }

    void HpShowUpdate()
    {
        if (meWhetherShowHp)
        {
            mHpShow.SuRefresh(mNow.Get(EPR_AttrBase.hp), mRefer.Get(EPR_AttrBase.hp));
            mHpPos.SuUpdatePosByWorld(transform.position + new Vector3(0, 1, 0));
        }
    }

    //=================================

    void Start()
    {
        HpShowStart();
    }

    void Update()
    {
        HpShowUpdate();
    }


    public void SelfReady(TreeBranch shelf)
    {
        InitialAttr();
        InitialLevel();
    }

    public object RespondReadFromUp(InfoSeal seal, out int source)
    {
        source = 0;
        return null;
    }

    public void ReceiveNotifyFromUp(InfoSeal seal)
    {

    }

    //====================================

    public class Ifo:FloatAttr
    {
        public Ifo():base() { }

        public override int Num { get { return 9; } }

        public override string Belong { get { return "base"; } }

        public static int EnumToInt(EPR_AttrBase match)
        {
            switch (match)
            {
                case EPR_AttrBase.hp: return 0;
                case EPR_AttrBase.tp: return 1;
                case EPR_AttrBase.restore: return 2;
                case EPR_AttrBase.atkp: return 3;
                case EPR_AttrBase.defp: return 4;
                case EPR_AttrBase.atkm: return 5;
                case EPR_AttrBase.defm: return 6;
                case EPR_AttrBase.rapid: return 7;
                case EPR_AttrBase.move: return 8;
                default: return -1;
            }
        }

        public static EPR_AttrBase IntToEnum(int which)
        {
            switch (which)
            {
                case 0: return EPR_AttrBase.hp;
                case 1: return EPR_AttrBase.tp;
                case 2: return EPR_AttrBase.restore;
                case 3: return EPR_AttrBase.atkp;
                case 4: return EPR_AttrBase.defp;
                case 5: return EPR_AttrBase.atkm;
                case 6: return EPR_AttrBase.defm;
                case 7: return EPR_AttrBase.rapid;
                case 8: return EPR_AttrBase.move;
                default: Debug.Log("没有这种属性");return EPR_AttrBase.hp;
            }
        }

        public void Set(EPR_AttrBase match,float value)
        {
            Set(EnumToInt(match), value);
        }

        public float Get(EPR_AttrBase match)
        {
            return Get(EnumToInt(match));
        }
    }

    [System.Serializable]
    public class IfoSet:IfoSetFloatAttr//与Attr结构同步
    {
        [Min(1)] public float hp = 1;//血量
        [Min(1)] public float tp = 1;//气力
        [Min(1)] public float restore = 1;//气力回复
        [Min(1)] public float atkp = 1;//物理攻击力
        [Min(1)] public float defp = 1;//物理防御力
        [Min(1)] public float atkm = 1;//魔法攻击力
        [Min(1)] public float defm = 1;//魔法防御力
        [Min(1)] public float rapid = 1;//攻速
        [Min(1)] public float move = 1;//移速

        public override FieldInfo[] FieldInfos { get { return typeof(IfoSet).GetFields(); } }
    }
}

