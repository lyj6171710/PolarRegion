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

    //�ӽ��մ����������˺�=============================

    Damageable2D mDamage;//�˺�����ʱ��

    public bool MakeHurt(MonoBehaviour which)
    {//�����øú��������һ���˺�
     //�˺����������������˺�ʱ����ε��þͻ���ɶ���˺�
     //��ȻҲ�����ø�����Լ����𴥷��Է����Լ����˺��������޷�Ӧ�Ը��ӵ��˺�������Ͼ��Լ��ԶԷ�����״������֪��
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
        mDamage.SuApplyDamage(hit);//�������˺�����������һ��ȷ���Ƿ���ܵ��˺�
    }

    //----------------------------

    MoveOnPlane mMoveCtrl;
    Dictionary<GameObject, EnemyTouch> mEnemies;//���Լ�����в�ĵз����壬ʹ���ֵ���Լ���
    HashSet<GameObject> mHits;//�����õģ������ռ���ǰ�����߼�����ʵ��ײ�ĵз�����
    List<GameObject> mNonTouch;//�����ã���ʱ��

    void WhenEnemyTouchSelf(GameObject resist)
    {//ֻ��ͼ���Ͻ����Ӵ��ˣ��߼��Ͽ��ܻ�������Ϊ�໥��ײ
     //����Ϸ�ڶ�άƽ������ϣ�����ʵ������άЧ������ά������Ӵ���������Ϊ�Ƿ�������ײ

        IDamageBring damage = resist.GetComponentInChildren<IDamageBring>(true);
        if (damage != null)//�з������Ҿ����˺�
        {
            EnemyTouch enemy = new EnemyTouch();
            enemy.damage = damage;
            enemy.inHit = false;
            enemy.resist = resist;//�����Ѿ���ȷ�ϣ����������Լ��к�
            enemy.move = resist.transform.parent.GetComponentInChildren<MoveOnPlane>();
            //�����������˹�ͨԼ��������ƽ�����໥�����������Ӧ�и��ƶ��������
            mEnemies.Add(resist, enemy);
        }
    }

    void UpdateHitState()
    {
        //�κ����Լ�������������壬�����ϱ�������Ƿ����Լ�������ײ
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
                        //���ڻ�������һ�����⣬����˺���һֱ���Լ����ϣ������ٴδ����˺��ģ�����˺����ǳ����˺���ʵ�ֲ���
                        //�Ժ���Ҫ���Ǽ����ֲ���ײ��Ӧ�߿����ṩ��ײ�ߵ�ǰ�Ƿ��������˺�����ײ�߾�����ʱ���������˺�
                        //Ҳ����˵���˺�����������ײ���Լ����ϣ������ɽ�����ײ���Լ�ȡ���˺���������ײ�߿����˺�����
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
