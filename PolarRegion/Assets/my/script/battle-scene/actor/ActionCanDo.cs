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
{//�ܹ��������κ���Ϊ������Ϊ֮�����ԼӰ�죬�����ܲ���������Щ��Ϊ�Լ���Щ��Ϊ��Ӱ�죬������ģ�鸺�𣬳�����Ҫ��ģ�����

    //============================

    Disturb mDisturb;
    List<ExceptionProcess> mExcepStates;

    public void AriseException(IfoException exception)//�쳣���Ե��ӣ���������
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
        exceptionProc.readyRemain = 0.1f;//�������������쳣����Ȼ�����ߣ�����������
        exceptionProc.dir = exception.dir;
        exceptionProc.readyStart = true;
        mExcepStates.Add(exceptionProc);
    }

    void StartException()
    {
        mExcepStates = new List<ExceptionProcess>();
        mDisturb = new Disturb(false);//����update���ܱ��Լ���update��
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
                else//�쳣���µ�Ч���ǵ��ӵ�
                {
                    if (mDisturb.canAct)//���е��ۻ����
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
                return new Disturb(true);//�����������ɵĵ��ˣ��˴λ�����Ч
            }
        }
        else
        {
            ExceptionBounce bounceProc = proc as ExceptionBounce;
            if (bounceProc.haveLand)
            {
                SuRemoveListenJump(bounceProc.listenLand);
                return new Disturb(true);//Ч���Ѿ�����
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
        mMoveByWait = true;//�������ƶ�ʱ������ʱ�̸��µ���λ�ã���Ȼԭ����Ԥ����λ�û���ź����ƶ�
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
            if (!mMoveTo)//�ս����������ƶ�������
            {
                mAimPos = transform.position;//��������ϰ�ָ���ص�
                mMoveTo = true;
            }
        }
    }

    //==================================

    public void SuTakeJump(Action whenLand = null)
    {
        mMoveCtrl.meWhenLand += whenLand;//����ββ�������ʱֵ����Ȼ���ý��
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
        public Action listenLand;//�����ӷ���״̬����ص�˲��
        public bool haveLand;
    }

    class ExceptionSpring : ExceptionProcess
    {
        public float pushForce;//ʣ������
    }

    class ExceptionProcess
    {
        //�������ݡ�����=======================
        public EPR_Exception kind;
        public int degree;
        public Vector2 dir;
        public float readyRemain;//����������Ч

        //�쳣�Լ�����====================
        public float timeRemain;//Ч��ʣ��ʱ��
        public bool readyStart;
    }

    class Disturb
    {
        public bool canAct;
        public bool canLowAtk;
        public bool canSkill;
        public bool canMove;
        public bool end;

        public Disturb(bool end)//ע�⣬���������ù��캯��������������������캯���ǲ��ᱻ���õ�
        {
            canAct = true;
            canLowAtk = true;
            canSkill = true;
            canMove = true;
            this.end = end;
        }
    }
}
