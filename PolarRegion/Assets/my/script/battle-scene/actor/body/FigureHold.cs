using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureHold : MonoBehaviour,ITreeBranch
{//直接管辖低级攻击，间接管辖技能施放

    public string mWeaponRefer;
    public Transform mTargetTmp;

    GameObject mHold;//普通攻击所用的武器被维持
    public WeaponInitial meHold { get; set; }

    bool inReady;

    void Ready()
    {
        mHold = DataLibrary.It.SuGetThing(mWeaponRefer,transform);
        mHold.SetActive(false);
        
        //--------------------------------

        meHold = mHold.GetComponent<WeaponInitial>();

        meHold.Ready1ForBase();

        string against = (branch.meSuper.Su<FigureInitial>().mCamp
            == ECamp.amity) ? GlobalConfig.sign_enemy : GlobalConfig.sign_amity;
        meHold.Ready2ForHit(false, against);

        meHold.Ready3ForMoveByDir();

        WeaponActionBase mAction = meHold.meWeapon.AddComponent<WeaponActionBase>();
        mAction.Ready(meHold.meAttr);

        FromOrBelong mFrom = meHold.meWeapon.AddComponent<FromOrBelong>();
        mFrom.fromGOjbect = branch.SuFind<ActionBalance>(EFgrNode.balance).gameObject;
        mFrom.fromTrans = transform;
        mFrom.fromMono = this;

        ReadyLowAtk();

        inReady = true;
    }

    //============================

    bool mInLowAtk;

    void ReadyLowAtk()
    {
        meHold.meMotion.SuWhenFinish += WhenLowAtkFinish;
    }

    public bool StartLowAtk(Vector2 to)
    {
        if (!inReady || mInLowAtk) return false;
        
        mHold.SetActive(true);

        MoveReady.Ifo ifo = new MoveReady.Ifo();
        ifo.dir = to;
        float rapid = branch.SuFind<AttrBaseFigure>(EFgrNode.attr_basis).mNow.Get(EPR_AttrBase.rapid);
        ifo.speed = rapid > 1 ? rapid : 1;
        ifo.span = meHold.meAttr.range;

        Vector3 localStart = transform.InverseTransformPoint(new Vector3(
            transform.position.x,
            branch.SuLeader<FigureProfile>().mExpress.meFootAt + 0.1f,
            transform.position.z)) ;
        meHold.meMotion.SuStartMove(ifo, localStart);

        mInLowAtk = true;

        meHold.meHit.meCanHit = true;
        meHold.gameObject.SetActive(true);

        return true;
    }

    void WhenLowAtkFinish(int id)
    {
        mInLowAtk = false;
        meHold.meHit.meCanHit = false;
        mHold.SetActive(false);
        branch.SuNotifyToUp(new InfoSeal(EFgrMsg.finish_low_atk));
    }

    //===================================

    void Start()
    {
        Ready();
    }

    TreeBranch branch;

    public void SelfReady(TreeBranch shelf)
    {
        branch = shelf;
    }

    public object RespondReadFromUp(InfoSeal seal, out int source)
    {
        source = 0;
        return null;
    }

    public void ReceiveNotifyFromUp(InfoSeal seal)
    {
        meHold.meWeapon.GetComponent<ImgFitToDir>().SuFitInstantly();
    }
}
