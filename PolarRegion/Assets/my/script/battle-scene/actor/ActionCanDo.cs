using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct IfoException
{
    public EPR_Exception kind;
    public float time;
    public int degree;
    public Vector2 dir;
}

public class ActionCanDo : MonoBehaviour,ITreeFocus,ITreeBranch
{//能够做出的任何行为，及行为之间的制约影响，至于能不能做出这些行为以及这些行为的影响，靠另外模块负责，除非需要该模块配合

    //============================

    Disturb mDisturb;
    List<ExceptionProcess> mExcepStates;

    public void AriseException(IfoException exception)//异常可以叠加，并不互斥
    {
        ExceptionProcess exceptionProc;
        if (exception.kind == EPR_Exception.bounce)
            exceptionProc = new ExceptionBounce();
        else if(exception.kind == EPR_Exception.spring)
            exceptionProc = new ExceptionSpring();
        else
            exceptionProc = new ExceptionProcess();
        exceptionProc.kind = exception.kind;
        exceptionProc.degree = exception.degree;
        exceptionProc.timeRemain = exception.time;
        exceptionProc.readyRemain = 0.1f;//不会立即触发异常，不然摸到边，就能跳起来
        exceptionProc.dir = exception.dir;
        exceptionProc.readyStart = true;
        mExcepStates.Add(exceptionProc);
    }

    void StartException()
    {
        mExcepStates = new List<ExceptionProcess>();
        mDisturb = new Disturb(false);//外界的update可能比自己的update快
    }

    void UpdateException()
    {
        mDisturb = new Disturb(false);
        for (int i = 0; i < mExcepStates.Count; i++)
        {
            ExceptionProcess exception = mExcepStates[i];
            if (exception.readyRemain > 0)
            {
                exception.readyRemain -= Time.deltaTime;
            }
            else
            {
                Disturb result;
                switch (exception.kind)
                {
                    case EPR_Exception.dizzy: result = UpdateDizzy(exception); break;
                    case EPR_Exception.disarm: result = UpdateDisarm(exception); break;
                    case EPR_Exception.silent: result = UpdateSilent(exception); break;
                    case EPR_Exception.stand: result = UpdateStand(exception); break;
                    case EPR_Exception.bounce: result = UpdateBounce(exception); break;
                    case EPR_Exception.spring: result = UpdateSpring(exception); break;
                    default: result = new Disturb(true); break;
                }
                if (result.end)
                {
                    mExcepStates.RemoveAt(i);
                    i--;
                }
                else//异常导致的效果是叠加的
                {
                    if (mDisturb.canAct)//已有的累积结果
                    {
                        if (result.canAct)
                        {
                            if (!result.canLowAtk) mDisturb.canLowAtk = false;
                            if (!result.canSkill) mDisturb.canSkill = false;
                            if (!result.canMove) mDisturb.canMove = false;
                        }
                        else
                            mDisturb.canAct = false;
                    }
                }
            }
        }
    }

    Disturb UpdateBounce(ExceptionProcess proc)
    {
        if (proc.readyStart)
        {
            proc.readyStart = false;

            ExceptionBounce bounceProc = proc as ExceptionBounce;
            bounceProc.listenLand = () => { bounceProc.haveLand = true; };

            if (!mMoveCtrl.meInJump)
            {
                SuTakeJump(bounceProc.listenLand);

                Disturb disturb = new Disturb(false);
                disturb.canAct = false;
                return disturb;
            }
            else
            {
                return new Disturb(true);//击飞正被击飞的敌人，此次击飞无效
            }
        }
        else
        {
            ExceptionBounce bounceProc = proc as ExceptionBounce;
            if (bounceProc.haveLand)
            {
                SuRemoveListenJump(bounceProc.listenLand);
                return new Disturb(true);//效果已经结束
            }
            else
            {
                Disturb disturb = new Disturb(false);
                disturb.canAct = false;
                return disturb;
            }
        } 
    }

    Disturb UpdateDizzy(ExceptionProcess proc)
    {
        Action<Disturb> effect = (result) => { result.canAct = false; };
        return ProUpdateNormalExc(proc, effect, effect);
    }

    Disturb UpdateDisarm(ExceptionProcess proc)
    {
        Action<Disturb> effect = (result) => { result.canLowAtk = false; };
        return ProUpdateNormalExc(proc, effect, effect);
    }

    Disturb UpdateSilent(ExceptionProcess proc)
    {
        Action<Disturb> effect = (result) => { result.canSkill = false; };
        return ProUpdateNormalExc(proc, effect, effect);
    }

    Disturb UpdateStand(ExceptionProcess proc)
    {
        Action<Disturb> effect = (result) => { result.canMove = false; };
        return ProUpdateNormalExc(proc, effect, effect);
    }

    Disturb UpdateSpring(ExceptionProcess proc)
    {
        if (proc.readyStart)
        {
            proc.readyStart = false;

            ExceptionSpring springProc = proc as ExceptionSpring;
            springProc.pushForce = proc.degree;

            return new Disturb(false);
        }
        else
        {
            ExceptionSpring springProc = proc as ExceptionSpring;
            if (MathNum.Abs(springProc.pushForce) > 0.3f)
            {
                float offset = springProc.pushForce * Time.deltaTime;
                branch.meSuper.transform.Translate(offset * proc.dir);
                springProc.pushForce -= offset;

                Disturb disturb = new Disturb(false);
                disturb.canAct = false;
                return disturb;
            }
            else
                return new Disturb(true);
        }
    }

    Disturb ProUpdateNormalExc(ExceptionProcess proc, Action<Disturb> WhenReady, Action<Disturb> WhenRemain)
    {
        if (proc.readyStart)
        {
            proc.readyStart = false;
            Disturb disturb = new Disturb(false);
            WhenReady(disturb);
            return disturb;
        }
        else
        {
            proc.timeRemain -= Time.deltaTime;
            if (proc.timeRemain < 0)
            {
                return new Disturb(true);
            }
            else
            {
                Disturb disturb = new Disturb(false);
                WhenRemain(disturb);
                return disturb;
            }
        }
    }

    //=========================================

    Vector3 mAimPos;
    bool mReach;
    bool mFaceFollowMove;
    bool mMoveByWait;
    bool mMoveBy;
    bool mMoveTo;

    void RespondLowAtkStartToMove()
    {
        mReach = true;
        mMoveCtrl.meCanMove = false;
    }

    public void SuStopMove()
    {
        mReach = true;
    }

    public void SuTakeMoveBy(Vector2 vec, bool faceFollow = true)
    {
        SuTakeMoveTo(transform.position + vec.ToVector3(), faceFollow);
        mMoveByWait = true;//按方向移动时，必需时刻更新到新位置，不然原来所预定的位置会干扰后续移动
    }

    public void SuTakeMoveTo(Vector2 pos, bool faceFollow = true)
    {
        if (WhetherCanMove())
        {
            mAimPos = new Vector3(pos.x, pos.y, transform.position.z);
            mReach = false;
            mFaceFollowMove = faceFollow;
        }
        else
            SuStopMove();
    }

    bool WhetherCanMove()
    {
        if (!mDisturb.canAct) return false;
        if (!mDisturb.canMove) return false; 
        return true;
    }

    void StartMove()
    {
        mAimPos = transform.position;
    }

    void UpdateMove()
    {
        if (mMoveBy)
        {
            Vector3 offsetToAim = mAimPos - transform.position;
            mMoveCtrl.SuTakeMove(offsetToAim);
            if (mFaceFollowMove) mProfile.SuSwitchFaceTo(mMoveCtrl.meDirCurAimX);
        }
        else if (mMoveTo)
        {
            if (!mReach)
            {
                Vector3 offsetToAim = mAimPos - transform.position;
                if (offsetToAim.magnitude < 0.1f)
                    mReach = true;
                else
                {
                    mMoveCtrl.SuTakeMove(offsetToAim);
                    if (mFaceFollowMove) mProfile.SuSwitchFaceTo(mMoveCtrl.meDirCurAimX);
                }
            }
        }
    }

    void LateUpdateMove()
    {
        if (OverAssist.OnThenOff(ref mMoveBy, ref mMoveByWait))
        {
             mMoveTo = false;
        }
        else
        {
            if (!mMoveTo)//刚结束按方向移动的命令
            {
                mAimPos = transform.position;//按方向会打断按指定地点
                mMoveTo = true;
            }
        }
    }

    //==================================

    public void SuTakeJump(Action whenLand = null)
    {
        mMoveCtrl.meWhenLand += whenLand;//这个形参不能是临时值，不然不好解绑
        mMoveCtrl.SuTakeJump();
    }

    public void SuRemoveListenJump(Action whenLand)
    {
        mMoveCtrl.meWhenLand -= whenLand;
    }

    //=================================

    void WhenFinishLowAtk()
    {
        mMoveCtrl.meCanMove = true;
    }

    public void SuTakeLowAtk(Vector2 to)
    {
        if (WhetherCanLowAtk())
        {
            if (branch.SuFind<FigureHold>(EFgrNode.atk_low).StartLowAtk(to))
            {
                RespondLowAtkStartToMove();
            }
        }
    }

    bool WhetherCanLowAtk()
    {
        if (!mDisturb.canAct) return false;
        if (!mDisturb.canLowAtk) return false;
        return true;
    }

    //==================================

    void Start()
    {
        StartException();
        StartMove();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateException();
        UpdateMove();
    }

    void LateUpdate()
    {
        LateUpdateMove();
    }

    TreeFocus focus;
    TreeBranch branch;

    MoveOnPlane mMoveCtrl;
    FigureProfile mProfile;

    public List<TreePart> GetParts(TreeFocus shelf)
    {
        focus = shelf;
        List<TreePart> parts = new List<TreePart>();
        parts.Add(new TreePart(EFgrNode.profile, mProfile = transform.GetChild(0).GetComponent<FigureProfile>()));
        parts.Add(new TreePart(EFgrNode.move, mMoveCtrl = transform.GetChild(1).GetComponent<MoveOnPlane>()));
        return parts;
    }

    public bool HearOfDown(InfoSeal seal)
    {
        if (seal == EFgrMsg.finish_low_atk)
        {
            WhenFinishLowAtk();
            return true;
        }
        return false;
    }

    public object RespondRequestFromDown(InfoSeal seal)
    {
        return null;
    }

    public void SelfReady(TreeBranch shelf)
    {
        branch = shelf;
    }

    public void ReceiveNotifyFromUp(InfoSeal seal)
    {

    }

    public object RespondReadFromUp(InfoSeal seal, out int source)
    {
        source = 0;
        return null;
    }

    //====================

    class ExceptionBounce : ExceptionProcess
    {
        public Action listenLand;//监听从飞起状态到落地的瞬间
        public bool haveLand;
    }

    class ExceptionSpring : ExceptionProcess
    {
        public float pushForce;//剩余推力
    }

    class ExceptionProcess
    {
        //基础数据、机制=======================
        public EPR_Exception kind;
        public int degree;
        public Vector2 dir;
        public float readyRemain;//不会立即生效

        //异常自己处理====================
        public float timeRemain;//效果剩余时间
        public bool readyStart;
    }

    class Disturb
    {
        public bool canAct;
        public bool canLowAtk;
        public bool canSkill;
        public bool canMove;
        public bool end;

        public Disturb(bool end)//注意，外界如果调用构造函数，不带参数，这个构造函数是不会被调用的
        {
            canAct = true;
            canLowAtk = true;
            canSkill = true;
            canMove = true;
            this.end = end;
        }
    }
}
