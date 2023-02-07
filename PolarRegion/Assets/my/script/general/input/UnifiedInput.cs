using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EKindInput { none, keyboard, mouse, touch }

public partial class UnifiedInput : MonoBehaviour,ISwitchScene
{
    //用来统一各种类型的玩家、外界输入
    //底层输入的改变牵动统一输入的改变，外界依据统一输入的状态做出行动反应

    //默认需要连续刺激，否则统一输入就会取消相应输入状态，除非有特殊说明

    //外界需要在update中运用输入，不然会与该组件机制冲突，造成不可知的结果

    public bool meInConfine => mAreaAsker != null;
    
    public void NormalizeArea(object asker)
    {
        if (asker != null && mAreaAsker == asker)
        {
            //有权解除限制
            RectMeter rect = new RectMeter();
            rect.leftBottom = Vector2.zero;
            rect.rightTop = SceneViewL.It.SuGetSizeViewCur();
            mAreaSure = rect;
            mAreaAsker = null;
        }
    }

    public void ConfineArea(RectMeter rect, object asker)
    {
        if (asker == null)
            return;//无权限制
        else if (mAreaAsker == null)
        {
            mAreaSure = rect;
            mAreaAsker = asker;
        }
        else if (mAreaAsker == asker)
            mAreaSure = rect;//可以重新调整
    }

    bool WhetherBanAnswerAction(object inquirer)
    {
        //可以不带询问者，会按最小宽限处理
        //被限区时，外界只能使用特殊接口才能对行为类操作进行感应了
        if (mAreaAsker == null) return false;
        else if (mAreaAsker == inquirer) return false;
        else return true;
    }//是否可以对外回应当前发生的输入行为
    //实现将当前输入专属于某位对象，其它事物将无法从该组件感应到输入行为的发生

    RectMeter mAreaSure;//视觉层面的限制
    //基于屏幕空间描述(百分比)，限制状态下才有效
    bool mInLock;//与视觉层面的限制区分开，待加入
    object mAreaAsker;//从限定区域到不限定区域为一个不可分阶段

    //======================================

    public EToward4 meGoOneStep(object inquirer = null){ 
        if (WhetherBanAnswerAction(inquirer)) return EToward4.middle;
        for (int i = 0; i < 4; i++)
        {
            if (mTowards[i])
                return MapGo(i);
        }
        return EToward4.middle;
    }

    public bool SuIfKeepGoing(EToward4 to, object inquirer = null)
    {
        if (WhetherBanAnswerAction(inquirer)) return false;
        return mShifts[MapGo(to)];
    }

    public EToward4 meInGo(object inquirer = null)
    {
        if (WhetherBanAnswerAction(inquirer)) return EToward4.middle;
        for (int i = 0; i < 4; i++)
        {
            if (mShifts[i])
                return MapGo(i);
        }
        return EToward4.middle;
    }

    bool[] mShifts;//行动去向，连续刺激时，一直为true
    bool[] mShiftWaits;//用来保持行动状态

    bool[] mTowards;//行动朝向，一次性，连续刺激时，不会一直为true
    float[] mGoGaps;//虽然不会一直为true，但是一定时间间隔后可以再次被激发
    const float cGoGap = 0.33f;
    const int cTowardNum = 4;

    public void ExciteGo(EToward4 go)
    {
        ProExciteGo(MapGo(go));
    }

    void ProExciteGo(int which)
    {
        mShiftWaits[which] = true;
        if (mGoGaps[which] < 0)
            mGoGaps[which] = cGoGap;
    }

    void AwakeToward()
    {
        mShifts = new bool[cTowardNum];//上下左右
        mShiftWaits = new bool[cTowardNum];

        mTowards = new bool[cTowardNum];
        mGoGaps = new float[cTowardNum];
    }

    void LateUpdateToward()
    {
        for (int i = 0; i < cTowardNum; i++)
        {
            if (OverAssist.OnThenOff(ref mShifts[i], ref mShiftWaits[i]))
            {
                if (mGoGaps[i] == cGoGap)//开始连续刺激或连续刺激一段时间后
                    mTowards[i] = true;
                else
                    mTowards[i] = false;
                mGoGaps[i] -= Time.deltaTime;
            }
            else//从且需从true变为false时
            {
                mTowards[i] = false;
                mGoGaps[i] = -1;//如果停止连续刺激，那么马上就可以再触发
            }
        }
    }

    int MapGo(EToward4 to)
    {
        switch (to)
        {
            case EToward4.up: return 0;
            case EToward4.down: return 1;
            case EToward4.left: return 2;
            case EToward4.right: return 3;
            default: return -1;
        }
    }

    EToward4 MapGo(int index)
    {
        switch (index)
        {
            case 0: return EToward4.up;
            case 1: return EToward4.down;
            case 2: return EToward4.left;
            case 3: return EToward4.right;
            default: return EToward4.middle;
        }
    }

    //======================================

    public bool meIsMoving(object inquirer = null) => mInMove && !WhetherBanAnswerAction(inquirer);
    public Vector2 meMoveDir => mMoveDir;

    Vector2 mMoveDir;
    bool mInMove;
    bool mMoveWait;

    public void ExciteMove(Vector2 to)
    {
        if (!MathNum.IsNear0(to.magnitude))//不建议用系统的，系统内部机制自己不清楚
        {
            mMoveDir = to.normalized;
            mMoveWait = true;
        }
    }

    void LateUpdateMove()
    {
        if (!OverAssist.OnThenOff(ref mInMove, ref mMoveWait))
            mMoveDir = Vector2.zero;
    }

    //==================================

    public bool SuTapSureUdCfn(object inquirer)
    {
        //需要验证身份，因为可以同时存在多个部门有限区需求
        if (inquirer == mAreaAsker)
            return mTapSure;
        else
            return false;
    }

    public bool meInConfirm(object inquirer = null) 
        => mInSure && !WhetherBanAnswerAction(inquirer);
    public bool meInConfirmJust(object inquirer = null) 
        => mInSureJust && !WhetherBanAnswerAction(inquirer);//非有持续感应需求时，少用
    public bool meTapConfirm(object inquirer = null) 
        => mTapSure && !WhetherBanAnswerAction(inquirer);
    public bool meLongConfirm(object inquirer = null) 
        => ((mInSure && mInSureTime >= cTapLimit) ? true : false) && !WhetherBanAnswerAction(inquirer);
    public bool meDoubleConfirm(object inquirer = null) 
        => mDoubleSure && !WhetherBanAnswerAction(inquirer);

    //public bool meReleaseConfirm => mReleaseSure;//低价值接口

    bool mInSure;//按下，直到release
    bool mInSureJust;//按下后的后一帧为true，直到下一次按下才会再次为true
    bool mReleaseSure;
    bool mTapSure;//按下又短时间内松开
    bool mDoubleSure;

    const float cTapLimit = 0.3f;
    float mInSureTime;//按下的时间总量

    bool mReleaseSureWait;//延迟一帧后关掉状态
    bool mTapSureWait;//同理
    bool mInSureJustWait;
    bool mDoubleSureWait;

    EKindInput mSureBind;
    Vector2 mSurePosStart;
    IClick mSureRespond;//事件，外界可以加入进来

    public void ExciteStartSure(EKindInput eKind, bool isCursor)
    {
        if (!mInSure)
        {
            if (meInConfine && isCursor)
            {
                Vector2 percentAt = CoordFrame.
                    SuCoordPercentInScreenFromMeter(UnifiedCursor.It.meMeterAt);
                if (!MathRect.SuWhetherInside(percentAt, mAreaSure, 0))
                    return;//由光标确认，但不在限区内，则不能激发确认输入
            }

            if (IsAvailable(ref mSureBind, eKind)) 
            {
                mInSureTime = 0;
                mInSureJustWait = true;//执行后的下一帧生效
            }
        }
    }
    //不需要连续刺激，通过刺激release来表示结束
    //默认至少在release的后一帧，外界才可能会执行该函数，不然没意义

    public void ExciteReleaseSure(EKindInput eKind)
    {
        if (!mReleaseSure)
        {
            if (mSureBind != EKindInput.none && eKind == mSureBind) 
            {
                mReleaseSureWait = true;
            }
        }
    }
    //不需要连续刺激
    //默认至少在开始sure的后一帧，外界才可能会执行该函数，不然没意义
    //执行后的下一帧生效

    void UpdateSure()
    {
        if (mInSure)
        {
            mInSureTime += Time.deltaTime;
        }
    }

    void LateUpdateSure()
    {
        if (OverAssist.OnThenOff(ref mInSureJust, ref mInSureJustWait))
        {
            mInSure = true;
            mSurePosStart = UnifiedCursor.It.meMeterAt;
            mSureRespond = CoordUse.SuCheckHit(UnifiedCursor.It.meMeterAt);
            if (mSureRespond != null) mSureRespond.WhenDown();

            if (TimeCount.It.SuMakeClickIfTwo("UnInSure", cTapLimit))
                mDoubleSureWait = true;//双击时和第二次单击，同一帧下应同步其开启状态
        }

        OverAssist.OnThenOff(ref mDoubleSure, ref mDoubleSureWait);

        if (OverAssist.OnThenOff(ref mReleaseSure, ref mReleaseSureWait))
        {
            if (mInSureTime < cTapLimit)//长按时，不认为做了一次点击
            {
                mTapSureWait = true;
            }
            mInSure = false;
            mSureBind = EKindInput.none;

            if (mSureRespond != null)
            {
                mSureRespond.WhenUp();
            }
        }

        if (OverAssist.OnThenOff(ref mTapSure, ref mTapSureWait))
        {
            if (mSureRespond != null)
            {
                if ((mSurePosStart - UnifiedCursor.It.meMeterAt).magnitude < 10) //一旦转移位置了，也不再认为是点击行为
                    mSureRespond.WhenHit();
            }
        }
    }

    //====================================

    public bool meWhenBack(object inquirer = null)
        => mBack && !WhetherBanAnswerAction(inquirer);

    bool mBack;
    bool mBackWait;

    bool mBackWant;
    float mBackTime;//刺激暂停的时间叠加，过久时视为取消
    int mBackDown;//倒计时
    const float cBackLimit = 0.3f;

    public void ExciteBack()//可以连续刺激，但会无效
    {
        if (!mBack)
        {
            mBackWant = true;
            mBackDown = 2;//应该马上要松开
        }
    }

    void UpdateBack()
    {
        if (mBackWant)
        {
            mBackDown -= 1;
            mBackTime += Time.deltaTime;
            if (mBackDown == 0)
            {
                if (mBackTime <= cBackLimit) 
                {
                    mBackWait = true;
                }
                mBackTime = 0;
                mBackWant = false;
            }
        }
    }

    void LateUpdateBack()
    {
        OverAssist.OnThenOff(ref mBack,ref mBackWait);
    }

    //==================================

    public int meNumInput => mNumInput;

    int mNumInput;

    public void ExciteNum(int num)
    {
        mNumInput = num;
    }

    //==================================

    Dictionary<string, InfoKeyPress> mKeys;

    public bool SuWhenTap(string key, object inquirer = null)
    {
        if (WhetherBanAnswerAction(inquirer)) return false;
        if (mKeys.ContainsKey(key))
            return mKeys[key].pressTap;
        else
            return false;
    }
    
    public bool SuWhenInPressJust(string key, object inquirer = null)//不管短按还是长按，都只执行一次
    {
        if (WhetherBanAnswerAction(inquirer)) return false;
        if (mKeys.ContainsKey(key))
            return mKeys[key].pressJust;
        else
            return false;
    }

    public bool SuWhenInPress(string key, object inquirer = null)//只要按着，就一直执行
    {
        if (WhetherBanAnswerAction(inquirer)) return false;
        if (mKeys.ContainsKey(key))
            return mKeys[key].inPress;
        else
            return false;
    }

    public void ExciteKey(string key)
    {
        if (mKeys.ContainsKey(key))
        {
            mKeys[key].pressWait = true;
        }
        else
        {
            InfoKeyPress keyPress = new InfoKeyPress();
            keyPress.pressWait = true;
            mKeys.Add(key, keyPress);
        }
    }

    void AwakeKey()
    {
        mKeys = new Dictionary<string, InfoKeyPress>();
    }

    void LateUpdateKey()
    {
        foreach (string key in mKeys.Keys)
        {
            InfoKeyPress state = mKeys[key];
            
            if (OverAssist.OnThenOff(ref state.pressJust, ref state.pressWait))
            {
                if (state.releaseJust)//松开帧的后一帧，都应该关掉了松开状态了
                {
                    state.pressTap = false;
                    state.releaseJust = false;
                }

                if (state.inPress)//判断刚按状态，第一次到这时会跳过
                    state.pressJust = false;//下一LateUpdate前是false状态
                else
                    state.inPress = true;//进入按住状态

                state.pressTime += Time.deltaTime;
            }
            else//长按状态结束时，这一帧下确实松开了
            {
                if (state.inPress)
                {
                    if (state.pressTime < 0.33f)//判断是否属于短按
                        state.pressTap = true;

                    state.pressTime = 0;
                    state.inPress = false;
                    state.releaseJust = true;
                }
                else//松开那一帧的下一帧结束时
                {
                    if (state.releaseJust)
                    {
                        state.releaseJust = false;
                        state.pressTap = false;
                    }
                }
            }
        }
    }

    //====================================

    public static bool IsAvailable(ref EKindInput bind, EKindInput apply)
    {
        if (bind == EKindInput.none)
        {
            bind = apply;
            return true;
        }
        else if (bind == apply)
        {
            return true;
        }
        else return false;
    }//可用来暂时限制输入方式的种类

    //=================================

    void Awake()
    {
        AwakeToward();
        AwakeKey();
    }

    void Update()
    {
        UpdateSure();
        UpdateBack();
    }

    void LateUpdate()
    {
        LateUpdateBack();
        LateUpdateSure();
        LateUpdateToward();
        LateUpdateMove();
        LateUpdateKey();
    }

    //=================================

    public static UnifiedInput It;

    public void WhenAwake()
    {
        It = this;

        mNumInput = 1;
    }

    public void WhenSwitchScene()
    {

    }
}

public class InfoKeyPress
{
    public bool pressWait;//当前是否激活了按键
    public bool pressJust;//是否刚才才按
    public bool pressTap;//非长按的按
    public bool inPress;//是否正按住
    public float pressTime;//按住的时间
    public bool releaseJust;//刚才松开
}

public interface IClick
{
    void WhenDown();
    void WhenUp();
    void WhenHit();
}
