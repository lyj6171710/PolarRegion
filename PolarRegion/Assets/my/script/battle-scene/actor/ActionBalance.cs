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
    public float degree;//Ч������
    public float keepTime;//ʱ��
    public Sprite diagram;//ʾ��ͼ
}

public class ActionBalance : MonoBehaviour, IDamageReceiver, IDamageMaker, ITreeFocus, ITreeBranch
{//������������Ϊ�Ļ�����ϵ

    ActionCanDo mCanDo;
    AttrBaseFigure mStateBasis;
    AttrAttachFigure mStateAttach;

    //================================

    float AffectInAttrByFix(FloatAttr attrNow, int which, float amount)
    {//����ֵ��ʵ�ʱ仯��������������Ϣ
        if (attrNow.Belong == "base")
        {
            EPR_AttrBase attrSelect = AttrBaseFigure.Ifo.IntToEnum(which);
            if (attrSelect == EPR_AttrBase.hp || attrSelect == EPR_AttrBase.tp)
                return AffectInAttrByFix(attrNow, mStateBasis.mRefer, which, amount);
        }

        float affect = EstimateAffectInAttr(attrNow, which, amount, 1);
        //������1������Ϊ1������0�����Է�������ٳ�����
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
        float amount = attrNow.Get(which) * ratio;//�仯��
        return AffectInAttrByFix(attrNow, which, amount);
    }

    float EstimateAffectInAttr(FloatAttr attrNow, FloatAttr attrCeiling, FloatAttr attrFloor, int which, float amount)
    {//refer�������������Ʊ仯����ı仯��Χ
        float nowAt = attrNow.Get(which);
        float ceilAt = attrCeiling.Get(which);
        if (amount > 0 && nowAt >= ceilAt)
            return 0;
        else
        {
            if (nowAt + amount > ceilAt)//�仯����
                return ceilAt - nowAt;
            else
            {
                float floorAt = attrFloor.Get(which);
                return EstimateAffectInAttr(attrNow, which, amount, floorAt);//�仯����
            }
        }
    }

    float EstimateAffectInAttr(FloatAttr attrNow, int which, float amount, float floor)
    {//��ʽ����ָʾ���Ե�ǰָ�����Եı仯��������ӣ������
     //����Ӧ����ɵ�����Ӱ�죬����������Ϣ
     //û�б仯�ϵ����ޣ���������
        float nowAt = attrNow.Get(which);
        if (amount < 0 && nowAt <= floor) return 0;
        else if (amount == 0) return 0;
        else
        {
            nowAt += amount;
            if (nowAt >= floor)//�仯����
                return amount;
            else
            {
                float overflow = nowAt - floor;
                return amount - overflow;
            }
        }
    }

    //�����˺�========================

    float ReceiveStraightHarm(float atkp, float atkm, IfoSetFeature pierce)
    {
        float amount = 0;
        amount += atkp / (1 + mStateBasis.mNow.Get(EPR_AttrBase.defp)/100);//���ŷ��������������������˺��ʷ��ȱ仯
        amount += atkm / (1 + mStateBasis.mNow.Get(EPR_AttrBase.defm)/100);
        amount += mStateAttach.mNowF.GetImpact(pierce);
        float affect = AffectInAttrByFix(mStateBasis.mNow, AttrBaseFigure.Ifo.EnumToInt(EPR_AttrBase.hp), -amount);
        return affect;
    }

    public float TakeHarm(IfoHarm harm)//���ܳ��������������ɵ�hpƫ��
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
        IDamageMaker damageMaker = damage.harmFrom.GetComponent<IDamageMaker>();//��Դ����
        IDamageBring medium = damage.harmBy.GetComponent<IDamageBring>();//�������
        IfoHarm power = damageMaker.GatherHarm(medium);//��Դ�����м���õ����
        float result = TakeHarm(power);
        float nowHp = mStateBasis.mNow.Get(EPR_AttrBase.hp);
        int nowLevel = mStateBasis.meLevelNow;
        if (result < 0 && nowHp <= 0) damageMaker.TellInDefeat(nowLevel * 2);//�Է���þ���ֵ
    }

    public void TellInDefeat(int get)
    {
        mStateBasis.GainExp(get);
    }

    //����˺�=========================

    void OverlieDamage(IfoHarm acc, AttrBaseEquip equip)//�����˺�Ч��
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

    public IfoHarm GatherHarm(IDamageBring medium)//�Լ���Ϊ�ܺ��ߣ���Ҳ������Ϊ������
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

    //�����洦��===============================

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
                ProAffectStartByForm(ProBuffTo(it.varyClass), it);//��������
                it.cost = true;
            }
            else
            {
                it.surplus -= Time.fixedDeltaTime;//ʣ������ʱ�����
                if (it.surplus >= 0)//��������
                    ProAffectDurationByForm(ProBuffTo(it.varyClass), it);
                else//��������
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
        switch (belong)//��������
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
                float frameNum = MathNum.AtLeast1(buff.keepTime / Time.fixedDeltaTime);//��Ԥ�ƻ��������֡
                buff.resultEach = buff.resultAll / frameNum;//ÿһ֡Ӧ���еı仯
            }
        }
        else if(buff.varyForm == EPR_BuffVaryForm.loss_gradual)
        {
            float frameNum = MathNum.AtLeast1(buff.keepTime / Time.fixedDeltaTime);//��Ԥѡ���������֡
            buff.resultEach = buff.degree / frameNum;//ÿһ֡Ӧ���еı仯
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
    class TimeBuff//��ʱ�޵ĸ������Ա仯Ч����ֻ������ֵ���ԣ��Ӽ�����
    {
        //��������-----------------------------
        public EPR_BuffVaryMode varyMode;
        public EPR_BuffVaryClass varyClass;
        public EPR_BuffVaryForm varyForm;
        public int varyTo;
        public float degree;//Ч������
        public float keepTime;//ʱ��
        public Sprite diagram;

        //��������-----------------------------
        public float resultAll;//ʵ����ɵ�Ч�����ļ�¼
        public float resultEach;//ÿ֡��Ҫ��ɵı仯

        public bool cost;//�Ƿ���Ч�����ɷ���ĳЩ����Ч���Ĺ���
        public float surplus;//ʣ��ʱ��
        public int identity;//�ɰ�������������ֲ�ͬ״̬
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
    public float atkpSum;//�������˺�
    public float atkmSum;//ħ�������˺�
    public IfoSetFeature featureSum;//�������ʵ�˺�
    public int numBuff;//��ֵ״̬
    public List<IfoBuff> buffs;
    public int numException;//�쳣״̬
    public List<IfoException> exceptions;

    public IfoHarm() { featureSum = new IfoSetFeature(); buffs = new List<IfoBuff>();exceptions = new List<IfoException>(); }
}

public interface IDamageBring:IDamageForm //����˺��ߵ����ճ����߼̳У�������֪���Լ�����Դ������
{
    List<AttrBaseEquip> EquipTake { get; }//�˺�����һ���������������������֧�֣�ʹ�÷���Ч����ĳЩ���͵�װ�����

    AttrSkillBody Skill { get; }

    EPR_Damage Nature{ get; }

    EDamageOpportunity Excite { get; }
}

public interface IDamageMaker//����˺��ĸ�����Դ�߼̳У���������ݸ�����������˺�
{
    IfoHarm GatherHarm(IDamageBring medium);

    void TellInDefeat(int get);
}

public interface IDamageReceiver//����˺��ĸ�����Դ�߼̳У������㴦���ܵ��˺��ĺ�����Ӧ
{
    void ReceiveHarm(DamageEvent damage);

    float TakeHarm(IfoHarm harm);
}

