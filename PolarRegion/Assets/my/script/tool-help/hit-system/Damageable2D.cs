using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EDamageOpportunity {
    //注意是"可以"，不是"就会"
    only_first,//只在第一次接触目标时，可以给目标造成伤害
    until_gap,//接触目标后，如果还在目标身上，一定时间后就可以造成伤害
    until_leave//每次从相离到相交于目标时，就可以造成伤害
}

public struct DamageEvent//具体运算交给外界，这里提供数据
{
    public int kind;//外界要转换成它需要的枚举来用
    public int atkForm;//同一来源可能裹藏多种伤害
    public GameObject harmFrom;//攻击者(极中心负责人)
    public MonoBehaviour harmBy;//攻击者的武器(极分支中，与外界交流的负责人)
    public Vector3 prePos;//造成伤害前的位置
    public Vector3 postPos;//造成伤害后的位置
    public EDamageOpportunity excite;//爆发出伤害的时机
}

public interface IDamageForm
{
    public MonoBehaviour Mount { get; }
    public int AtkForm { get; }
}

public class Damageable2D : MonoBehaviour
{
    //可被伤害者，接收并过滤伤害
    //同一事物对自己的伤害，可以由该组件管理，被动控制是否接收此次伤害
    
    //只负责可以造成伤害的期间，确保物理层面上已经可以受到伤害
    //不负责识别数据，只管按外界给来的数据，计算与判断可否造成伤害

    [Range(0, 360.0f)]
    public float mHitAngleDp = 360.0f;//有些事物或有些时候，只能某一面能被伤害
    
    public void MakeReady(Action<DamageEvent> receiver, Func<MonoBehaviour, int, bool> inTouch, Func<MonoBehaviour, int, bool> haveEnd)
    {
        if (haveReady) return;

        mCoolings = new Dictionary<MonoBehaviour, DamageCooling>();
        mCoolingsEnd = new List<MonoBehaviour>();

        mReceiveHarm = receiver;
        mIfHarmTouch = inTouch;
        mIfHarmEnd = haveEnd;

        haveReady = true;
    }

    //外界可用=================================================

    public bool SuApplyDamage(DamageEvent damage)
    {
        //只要外界调用了该函数，就会准备造成伤害

        //外界可以用不同组件monobehaviour区分，不同攻击，也可以用不同int区分

        if (!haveReady) return false;

        //先看是否可被伤害
        if (WhetherIgnore(damage.harmBy, damage.atkForm))//判断是否是同一攻击
            return false;
        else
            ConsiderDamage(damage.harmBy, damage.atkForm, damage.excite);
        
        if (SuWhetherDamageInAngle(damage.prePos, mHitAngleDp))//能否受到伤害的夹角范围
        {
            mReceiveHarm(damage);
            return true;
        }

        return false;
    }

    public bool SuWhetherDamageInAngle(Vector3 damageWill,float angleLimit)//此伤害是否在某个夹角内
    {
        return MathAngle.WhetherInAngle(damageWill, transform.position, transform.forward, angleLimit);
    }

    public bool SuWhetherCanDamage(MonoBehaviour damager, int atkForm = 0)
    {
        return !WhetherIgnore(damager, atkForm);
    }

    //=======================================

    Action<DamageEvent> mReceiveHarm;
    Func<MonoBehaviour, int, bool> mIfHarmTouch;//告知伤害源是否仍然接触着自己
    Func<MonoBehaviour, int, bool> mIfHarmEnd;//告知这一次攻击是否还存留
    
    bool haveReady;

    Dictionary<MonoBehaviour,DamageCooling> mCoolings;//在这个列表中标记的，都不会再触发伤害
    List<MonoBehaviour> mCoolingsEnd;//收集当前应结束屏蔽的伤害，将能够再次造成伤害

    void Update()
    {
        mCoolingsEnd.Clear();
        foreach (MonoBehaviour damager in mCoolings.Keys)
        {
            if (damager == null)
                mCoolingsEnd.Add(damager);//这里认为就算为null，变量本身还是有之前所引用对象的相关信息的
            else
            {
                if (mCoolings[damager].DecSelf(Time.deltaTime))//被告知需要消除
                    mCoolingsEnd.Add(damager);
            }
        }
        foreach (MonoBehaviour damager in mCoolingsEnd)
        {
            mCoolings.Remove(damager);//或许这里已经能直接清除掉所有为null的元素
        }
    }

    //内部接口===================================================

    bool WhetherIgnore(MonoBehaviour damager, int atkForm)
    {//同一次攻击，暂时不应该对同一物体多次造成同样的伤害
        if (mCoolings.ContainsKey(damager))
        {
            if (mCoolings[damager].WhetherHave(atkForm))
                return true;
            else
                return false;
        }
        else
            return false;
    }

    void ConsiderDamage(MonoBehaviour damager, int atkForm, EDamageOpportunity atkCan)
    {//调用前提是，属于一次新攻击，之前没有受到过相关伤害的
        if (mCoolings.ContainsKey(damager))
            mCoolings[damager].AddForm(atkForm, atkCan);
        else//不存在该攻击者时
            mCoolings.Add(damager, new DamageCooling(damager, atkForm, atkCan, this));
    }
    
    //=========================================

    class DamageCooling//配合该组件用的，凡是当前被这个类对象记录的，都暂时不会触发伤害了
    {
        public MonoBehaviour damager;//伤害体
        public List<DamageOne> forms;
        public Damageable2D mager;

        public DamageCooling(MonoBehaviour damager, int atkForm, EDamageOpportunity atkCan, Damageable2D mager)
        {
            forms = new List<DamageOne>();

            this.damager = damager;
            this.mager = mager;

            AddForm(atkForm, atkCan);
        }

        //内外机制--------------------------

        public bool DecSelf(float gap)//返回值表示自己是否应该被消除
        {//自己可以负责消除攻击形式，上级可以负责消除自己
            for (int i = 0; i < forms.Count; i++)
            {
                DamageOne thisForm = forms[i];
                if (mager.mIfHarmEnd(damager, thisForm.id))
                {
                    forms.RemoveAt(i);
                    i--;
                    continue;
                }
                switch (thisForm.can)
                {
                    case EDamageOpportunity.only_first:break;
                    case EDamageOpportunity.until_gap:
                        thisForm.wait -= gap;
                        if (thisForm.wait <= 0)
                        {
                            forms.RemoveAt(i);
                            i--;
                        }
                        //间隔短，不需要继续判断伤害体还在接触没有
                        break;
                    case EDamageOpportunity.until_leave:
                        if (!mager.mIfHarmTouch(damager, thisForm.id))
                        {
                            forms.RemoveAt(i);
                            i--;
                        }
                        break;
                    default:
                        break;
                }
            }

            if (forms.Count == 0)
                return true;
            else
                return false;
        }

        //外界可用-----------------------------------------------

        public bool WhetherHave(int formId)
        {
            foreach (DamageOne form in forms)
                if (form.id == formId)
                    return true;
            return false;
        }

        public void AddForm(int formId, EDamageOpportunity can)
        {
            if (!WhetherHave(formId))//自动过滤
            {
                DamageOne form = new DamageOne();
                form.id = formId;
                form.can = can;
                form.wait = 0.5f;
                forms.Add(form);
            }
        }

        //-----------------------------------

        public class DamageOne
        {
            public int id;//攻击形式，不一定必需代指形式，看外界自己安排
            public float wait;//该攻击者，该攻击形式，下一次能伤害到自己的时隔，一种倒计时
            public EDamageOpportunity can;//激发伤害的能力
        }
    }

}
