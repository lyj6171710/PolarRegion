using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureProfile : MonoBehaviour, ITreeBranch, ITreeFocus
{
    FigureHold mHolder;
    FigureWear mWear;
    public CollidBody mCollidDp;
    [HideInInspector]public ImgSideExpress mExpress;
    FigureSkill mSkill;
    [HideInInspector]public HitRespond mHitRespond;
    
    void Ready()
    {
        mExpress = gameObject.AddComponent<ImgSideExpress>();
        mExpress.MakeReady(GetComponent<SpriteRenderer>());
        mExpress.ReadyCountSize();
        branch.SuFind<MoveOnPlane>(EFgrNode.move).MakeReady(transform, GlobalConfig.layer_foot);
        branch.SuNotifyToUp(new InfoSeal(EFgrMsg.kill_render));

        mHitRespond = gameObject.AddComponent<HitRespond>();
        string belong = (branch.meSuper.Su<FigureInitial>().mCamp 
            == ECamp.amity) ? GlobalConfig.sign_amity : GlobalConfig.sign_enemy;
        mHitRespond.MakeReady(GlobalConfig.layer_atk).ReadyBelong(true, belong);
        mHitRespond.meWhenAgainst += WhenEnemyTouchSelf;

        mCollidDp.MakeReady(mHitRespond);

        mMoveCtrl = branch.SuFind<MoveOnPlane>(EFgrNode.move);
        mEnemies = new Dictionary<GameObject, EnemyTouch>();
        mHits = new HashSet<GameObject>();
        mNonTouch = new List<GameObject>();

        ReadyHurt();
    }

    //从接收触碰到接收伤害=============================

    Damageable2D mDamage;//伤害触发时机

    public bool MakeHurt(MonoBehaviour which)
    {//外界调用该函数来造成一次伤害
     //伤害本身性质允许多次伤害时，多次调用就会造成多次伤害
     //虽然也可以让该组件自己负责触发对方对自己的伤害，但是无法应对复杂的伤害情况，毕竟自己对对方具体状况并不知情
        if (CanTakeHurt(which))
        {
            WhenEnemyHurtSelf(mEnemies[which.gameObject]);
            return true;
        }
        else
            return false;
    }

    bool CanTakeHurt(MonoBehaviour which)
    {
        if (WhetherInHit(which, 0))
        {
            if (mDamage.SuWhetherCanDamage(which))
                return true;
        }
        return false;
    }

    void ReadyHurt()
    {
        mDamage = gameObject.AddComponent<Damageable2D>();
        mDamage.MakeReady(
            branch.SuFind<ActionBalance>(EFgrNode.balance).ReceiveHarm,
            WhetherInHit,
            (hit, form) => {
                return !mHitRespond.SuHaveHitAgainThreaten(hit.GetComponent<HitTryMake>());
            });
    }

    void WhenEnemyHurtSelf(EnemyTouch enemy)
    {
        DamageEvent hit = new DamageEvent();
        FromOrBelong cite = enemy.resist.GetComponent<FromOrBelong>();
        hit.harmFrom = cite.fromGOjbect;
        hit.prePos = enemy.resist.transform.position;
        hit.postPos = mHitRespond.transform.position;
        hit.excite = enemy.damage.Excite;

        IDamageForm act = enemy.resist.GetComponent<IDamageForm>();
        hit.harmBy = act.Mount;
        hit.atkForm = act.AtkForm;
        mDamage.SuApplyDamage(hit);//负责受伤害的组件，会进一步确认是否会受到伤害
    }

    //----------------------------

    MoveOnPlane mMoveCtrl;
    Dictionary<GameObject, EnemyTouch> mEnemies;//对自己有威胁的敌方形体，使用字典可以加速
    HashSet<GameObject> mHits;//加速用的，并且收集当前发生逻辑上真实碰撞的敌方形体
    List<GameObject> mNonTouch;//加速用，临时用

    void WhenEnemyTouchSelf(GameObject resist)
    {//只是图面上交互接触了，逻辑上可能还不能认为相互碰撞
     //本游戏在二维平面基础上，近似实现了三维效果，二维形体相接触还不能认为是发生了碰撞

        IDamageBring damage = resist.GetComponentInChildren<IDamageBring>(true);
        if (damage != null)//敌方事物且具有伤害
        {
            EnemyTouch enemy = new EnemyTouch();
            enemy.damage = damage;
            enemy.inHit = false;
            enemy.resist = resist;//至少已经能确认，这个物体对自己有害
            enemy.move = resist.transform.parent.GetComponentInChildren<MoveOnPlane>();
            //这里利用起了共通约定，凡是平面上相互互动的事物，都应有该移动控制组件
            mEnemies.Add(resist, enemy);
        }
    }

    void UpdateHitState()
    {
        //任何与自己交互交叉的形体，都不断被检测其是否与自己发生碰撞
        foreach (EnemyTouch touch in mEnemies.Values)
        {
            if (mHitRespond.SuWhtherInHit(touch.resist))
            {
                bool nowHit = mMoveCtrl.IsOverlapMutual(touch.move);
                if (touch.inHit)
                {
                    if (!nowHit)
                    {
                        touch.inHit = false;
                        mHits.Remove(touch.resist);
                    }
                }
                else
                {
                    if (nowHit)
                    {
                        touch.inHit = true;
                        mHits.Add(touch.resist);
                        //现在还有遗留一个问题，如果伤害体一直在自己身上，不会再次触发伤害的，如果伤害体是持续伤害就实现不了
                        //以后需要考虑继续分拆，碰撞响应者可以提供碰撞者当前是否可以造成伤害，碰撞者就能适时主动激发伤害
                        //也就是说，伤害激发者在碰撞者自己身上，不再由接收碰撞者自己取得伤害，接收碰撞者控制伤害接收
                    }
                }
            }
            else
            {
                mHits.Remove(touch.resist);
                mNonTouch.Add(touch.resist);
            }
        }

        for (int i = 0; i < mNonTouch.Count; i++)
        {
            mEnemies.Remove(mNonTouch[i]);
        }
        mNonTouch.Clear();
    }

    bool WhetherInHit(MonoBehaviour hit, int form)
    {
        if (mHits.Contains(hit.gameObject))
            return true;
        else
            return false;
    }

    //================================

    public void SuSwitchFaceTo(float x)
    {
        mExpress.SuSwitchFace(x);
    }

    //==================================

    public List<AttrBaseEquip> RequestEquips()
    {
        List<AttrBaseEquip> equips = new List<AttrBaseEquip>();
        equips.Add(mHolder.meHold.meAttr);
        foreach (AttrBaseEquip wear in mWear.GetPropertyEachWear()) equips.Add(wear);
        return equips;
    }

    //================================

    void Update()
    {
        UpdateHitState();
    }

    TreeFocus focus;
    TreeBranch branch;

    public List<TreePart> GetParts(TreeFocus shelf)
    {
        focus = shelf;
        List<TreePart> parts = new List<TreePart>();
        parts.Add(new TreePart(EFgrNode.atk_low, mHolder = transform.GetChild(0).GetComponent<FigureHold>()));
        parts.Add(new TreePart(EFgrNode.wear_body, mWear = transform.GetChild(1).GetComponent<FigureWear>()));
        parts.Add(new TreePart(EFgrNode.skill, mSkill = transform.GetChild(2).GetComponent<FigureSkill>()));
        return parts;
    }

    public bool HearOfDown(InfoSeal seal)
    {
        return false;
    }

    public object RespondReadFromUp(InfoSeal seal, out int source)
    {
        throw new System.NotImplementedException();
    }

    public object RespondRequestFromDown(InfoSeal seal)
    {
        throw new System.NotImplementedException();
    }

    public void ReceiveNotifyFromUp(InfoSeal seal)
    {
        
    }

    public void SelfReady(TreeBranch shelf)
    {
        branch = shelf;
        Ready();
    }

    class EnemyTouch
    {
        public bool inHit;
        public GameObject resist;
        public MoveOnPlane move;
        public IDamageBring damage;
    }
}
