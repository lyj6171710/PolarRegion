using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfoBuff
{
    public EPR_BuffVaryMode mode;
    public EPR_BuffVaryClass category;
    public EPR_BuffVaryForm form;
    public EPR_AttrBase attrBaseTo;
    public EPR_AttrAttach attrAttachTo;
    public bool addOrMinus;
    public float degree;//效果级别
    public float keepTime;//时长
    public Sprite diagram;//示意图
}

public class ActionBalance : MonoBehaviour, IDamageReceiver, IDamageMaker, ITreeFocus, ITreeBranch
{//负责属性与行为的基本关系

    ActionCanDo mCanDo;
    AttrBaseFigure mStateBasis;
    AttrAttachFigure mStateAttach;

    //================================

    float AffectInAttrByFix(FloatAttr attrNow, int which, float amount)
    {//返回值是实际变化量，包含正负信息
        if (attrNow.Belong == "base")
        {
            EPR_AttrBase attrSelect = AttrBaseFigure.Ifo.IntToEnum(which);
            if (attrSelect == EPR_AttrBase.hp || attrSelect == EPR_AttrBase.tp)
                return AffectInAttrByFix(attrNow, mStateBasis.mRefer, which, amount);
        }

        float affect = EstimateAffectInAttr(attrNow, which, amount, 1);
        //有下限1，下限为1而不是0，可以方便与加速程序处理
        attrNow.Offset(which, affect);
        return affect;
    }

    float AffectInAttrByFix(FloatAttr attrNow, FloatAttr attrRefer, int which, float amount)
    {
        float affect = EstimateAffectInAttr(attrNow, attrRefer, new AttrBaseFigure.Ifo(), which, amount);
        attrNow.Offset(which, affect);
        return affect;
    }

    float AffectInAttrByRatio(FloatAttr attrNow, int which, float ratio)
    {
        ratio = Mathf.Clamp(ratio, -0.99f, 1);
        float amount = attrNow.Get(which) * ratio;//变化量
        return AffectInAttrByFix(attrNow, which, amount);
    }

    float EstimateAffectInAttr(FloatAttr attrNow, FloatAttr attrCeiling, FloatAttr attrFloor, int which, float amount)
    {//refer参数，用来限制变化对象的变化范围
        float nowAt = attrNow.Get(which);
        float ceilAt = attrCeiling.Get(which);
        if (amount > 0 && nowAt >= ceilAt)
            return 0;
        else
        {
            if (nowAt + amount > ceilAt)//变化上限
                return ceilAt - nowAt;
            else
            {
                float floorAt = attrFloor.Get(which);
                return EstimateAffectInAttr(attrNow, which, amount, floorAt);//变化下限
            }
        }
    }

    float EstimateAffectInAttr(FloatAttr attrNow, int which, float amount, float floor)
    {//形式参数指示，对当前指定属性的变化量，正则加，负则减
     //返回应该造成的增减影响，包含正负信息
     //没有变化上的上限，但有下限
        float nowAt = attrNow.Get(which);
        if (amount < 0 && nowAt <= floor) return 0;
        else if (amount == 0) return 0;
        else
        {
            nowAt += amount;
            if (nowAt >= floor)//变化下限
                return amount;
            else
            {
                float overflow = nowAt - floor;
                return amount - overflow;
            }
        }
    }

    //接收伤害========================

    float ReceiveStraightHarm(float atkp, float atkm, IfoSetFeature pierce)
    {
        float amount = 0;
        amount += atkp / (1 + mStateBasis.mNow.Get(EPR_AttrBase.defp)/100);//随着防御力无限增长，攻击伤害呈反比变化
        amount += atkm / (1 + mStateBasis.mNow.Get(EPR_AttrBase.defm)/100);
        amount += mStateAttach.mNowF.GetImpact(pierce);
        float affect = AffectInAttrByFix(mStateBasis.mNow, AttrBaseFigure.Ifo.EnumToInt(EPR_AttrBase.hp), -amount);
        return affect;
    }

    public float TakeHarm(IfoHarm harm)//接受冲击，返回最终造成的hp偏移
    {
        //IfoException sample = new IfoException();
        //sample.kind = EPR_Exception.none;
        //sample.degree = 2;
        //sample.dir = Vector2.up;
        //harm.exceptions.Add(sample);
        foreach (IfoBuff buff in harm.buffs) AddBuff(buff);
        foreach (IfoException ex in harm.exceptions) mCanDo.AriseException(ex);
        return ReceiveStraightHarm(harm.atkpSum, harm.atkmSum, harm.featureSum);
    }

    public void ReceiveHarm(DamageEvent damage)
    {
        IDamageMaker damageMaker = damage.harmFrom.GetComponent<IDamageMaker>();//来源数据
        IDamageBring medium = damage.harmBy.GetComponent<IDamageBring>();//结果数据
        IfoHarm power = damageMaker.GatherHarm(medium);//来源处理中间而得到结果
        float result = TakeHarm(power);
        float nowHp = mStateBasis.mNow.Get(EPR_AttrBase.hp);
        int nowLevel = mStateBasis.meLevelNow;
        if (result < 0 && nowHp <= 0) damageMaker.TellInDefeat(nowLevel * 2);//对方获得经验值
    }

    public void TellInDefeat(int get)
    {
        mStateBasis.GainExp(get);
    }

    //造成伤害=========================

    void OverlieDamage(IfoHarm acc, AttrBaseEquip equip)//叠加伤害效果
    {
        acc.atkpSum += equip.mBasis.atkp;
        acc.atkmSum += equip.mBasis.atkm;
        acc.featureSum += equip.mFeature;
    }

    void OverlieDamage(IfoHarm acc, AttrSkillBody skill)
    {
        acc.atkpSum += skill.mBasis.atkp;
        acc.atkpSum += skill.mBasis.atkm;
        acc.featureSum += skill.mFeature;
    }

    void OverlieDamage(IfoHarm acc)
    {
        acc.atkpSum += mStateBasis.mNow.Get(EPR_AttrBase.atkp);
        acc.atkmSum += mStateBasis.mNow.Get(EPR_AttrBase.atkm);
        acc.featureSum += mStateAttach.mNowF.ToIfoSet();
    }

    public IfoHarm GatherHarm(IDamageBring medium)//自己作为受害者，但也可能作为攻击者
    {
        IfoHarm harm = new IfoHarm();
        foreach (AttrBaseEquip equip in medium.EquipTake)
        {
            OverlieDamage(harm, equip);
        }
        if (medium.Skill != null)
            OverlieDamage(harm, medium.Skill);
        OverlieDamage(harm);
        return harm;
    }

    //增减益处理===============================

    List<TimeBuff> mBuffHold;

    void ReadyBuff()
    {
        mBuffHold = new List<TimeBuff>();
    }

    void AddBuff(IfoBuff buff)
    { }

    void BuffAffect()
    {
        TimeBuff it;
        for (int i = 0; i < mBuffHold.Count; i++)
        {
            it = mBuffHold[i];
            if (!it.cost)
            {
                ProAffectStartByForm(ProBuffTo(it.varyClass), it);//启动工作
                it.cost = true;
            }
            else
            {
                it.surplus -= Time.fixedDeltaTime;//剩余作用时间减少
                if (it.surplus >= 0)//持续工作
                    ProAffectDurationByForm(ProBuffTo(it.varyClass), it);
                else//结束工作
                {
                    ProAffectEndByForm(ProBuffTo(it.varyClass), it); 
                    mBuffHold.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    FloatAttr ProBuffTo(EPR_BuffVaryClass belong)
    {
        switch (belong)//启动工作
        {
            case EPR_BuffVaryClass.basis: return mStateBasis.mNow; 
            case EPR_BuffVaryClass.attach: return mStateAttach.mNow;
            case EPR_BuffVaryClass.feature: return mStateAttach.mNowF;
            default:return null;
        }
    }

    void ProAffectStartByForm(FloatAttr attr, TimeBuff buff)
    {
        if (buff.varyForm.toInt() < 3)
        {
            if (buff.varyMode == EPR_BuffVaryMode.fix) buff.resultAll = AffectInAttrByFix(attr, buff.varyTo, buff.degree);
            else if (buff.varyMode == EPR_BuffVaryMode.percent) buff.resultAll = AffectInAttrByRatio(attr, buff.varyTo, buff.degree);

            if (buff.varyForm == EPR_BuffVaryForm.tmp_gradual)
            {
                float frameNum = MathNum.AtLeast1(buff.keepTime / Time.fixedDeltaTime);//先预计会持续多少帧
                buff.resultEach = buff.resultAll / frameNum;//每一帧应该有的变化
            }
        }
        else if(buff.varyForm == EPR_BuffVaryForm.loss_gradual)
        {
            float frameNum = MathNum.AtLeast1(buff.keepTime / Time.fixedDeltaTime);//先预选会持续多少帧
            buff.resultEach = buff.degree / frameNum;//每一帧应该有的变化
        }
    }

    void ProAffectDurationByForm(FloatAttr attr, TimeBuff buff)
    {
        switch (buff.varyForm)
        {
            case EPR_BuffVaryForm.tmp_instant:break;
            case EPR_BuffVaryForm.tmp_gradual:AffectInAttrByFix(attr, buff.varyTo, -buff.resultEach); break;
            case EPR_BuffVaryForm.loss_instant:break;
            case EPR_BuffVaryForm.loss_gradual:
                if (buff.varyMode == EPR_BuffVaryMode.fix)
                    AffectInAttrByFix(attr, buff.varyTo, buff.resultEach);
                else if (buff.varyMode == EPR_BuffVaryMode.percent)
                    AffectInAttrByRatio(attr, buff.varyTo, buff.resultEach);
                break;
        }
    }

    void ProAffectEndByForm(FloatAttr attr, TimeBuff buff)
    {
        if (buff.varyForm == EPR_BuffVaryForm.tmp_instant)
            AffectInAttrByFix(attr, buff.varyTo, -buff.resultAll);
    }

    //==================================

    void FixedUpdate()
    {
        BuffAffect();
    }

    //===================================

    TreeFocus focus;
    TreeBranch branch;

    public List<TreePart> GetParts(TreeFocus shelf)
    {
        focus = shelf;
        List<TreePart> parts = new List<TreePart>();
        parts.Add(new TreePart(EFgrNode.attr_basis, mStateBasis = transform.GetChild(0).GetComponent<AttrBaseFigure>()));
        parts.Add(new TreePart(EFgrNode.attr_attach, mStateAttach = transform.GetChild(1).GetComponent<AttrAttachFigure>()));
        return parts;
    }

    public bool HearOfDown(InfoSeal seal)
    {
        throw new System.NotImplementedException();
    }

    public object RespondRequestFromDown(InfoSeal seal)
    {
        throw new System.NotImplementedException();
    }

    public void SelfReady(TreeBranch shelf)
    {
        branch = shelf;
        mCanDo = branch.SuFind<ActionCanDo>(EFgrNode.can_do);
        ReadyBuff();
    }

    public object RespondReadFromUp(InfoSeal seal, out int source)
    {
        throw new System.NotImplementedException();
    }

    public void ReceiveNotifyFromUp(InfoSeal seal)
    {

    }
    class TimeBuff//有时限的各种属性变化效果，只面向有值属性，加减不定
    {
        //基本属性-----------------------------
        public EPR_BuffVaryMode varyMode;
        public EPR_BuffVaryClass varyClass;
        public EPR_BuffVaryForm varyForm;
        public int varyTo;
        public float degree;//效果级别
        public float keepTime;//时长
        public Sprite diagram;

        //机制属性-----------------------------
        public float resultAll;//实际造成的效果量的记录
        public float resultEach;//每帧需要造成的变化

        public bool cost;//是否生效过，可方便某些负面效果的构建
        public float surplus;//剩余时间
        public int identity;//可帮助外界用来区分不同状态
        static int acc;

        public TimeBuff(IfoBuff ifoBuff)
        {
            varyMode = ifoBuff.mode;
            varyClass = ifoBuff.category;
            varyForm = ifoBuff.form;
            switch (varyClass)
            {
                case EPR_BuffVaryClass.basis: varyTo = AttrBaseFigure.Ifo.EnumToInt(ifoBuff.attrBaseTo); break;
                case EPR_BuffVaryClass.attach: varyTo = AttrAttachFigure.Ifo.EnumToInt(ifoBuff.attrAttachTo); break;
            }
            float degree = Mathf.Abs(ifoBuff.degree);
            this.degree = ifoBuff.addOrMinus ? degree : -degree;
            keepTime = ifoBuff.keepTime;
            diagram = ifoBuff.diagram;
            //-------------------------------
            cost = false;
            surplus = keepTime;
            acc += 1;
            identity = acc;
        }
    }

}

public class IfoHarm
{
    public float atkpSum;//物理攻击伤害
    public float atkmSum;//魔法攻击伤害
    public IfoSetFeature featureSum;//将造成真实伤害
    public int numBuff;//变值状态
    public List<IfoBuff> buffs;
    public int numException;//异常状态
    public List<IfoException> exceptions;

    public IfoHarm() { featureSum = new IfoSetFeature(); buffs = new List<IfoBuff>();exceptions = new List<IfoException>(); }
}

public interface IDamageBring:IDamageForm //造成伤害者的最终承载者继承，它方便知道自己的来源及性质
{
    List<AttrBaseEquip> EquipTake { get; }//伤害经由一个技能与这个技能所可以支持，使得发挥效果的某些类型的装备打出

    AttrSkillBody Skill { get; }

    EPR_Damage Nature{ get; }

    EDamageOpportunity Excite { get; }
}

public interface IDamageMaker//造成伤害的根本来源者继承，它方便根据各种情况计算伤害
{
    IfoHarm GatherHarm(IDamageBring medium);

    void TellInDefeat(int get);
}

public interface IDamageReceiver//造成伤害的根本来源者继承，它方便处理受到伤害的后续反应
{
    void ReceiveHarm(DamageEvent damage);

    float TakeHarm(IfoHarm harm);
}

