using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class TouchInput : MonoBehaviour,ISwitchScene
{
    //外界可用===============================

    public bool meHaveTouch{get{ if (Input.touchCount > 0) return true; else return false; }}

    public Vector2 mePosIfOneTouch { get {
            if (mOneTouch)
                return mFingers[mOneWhere].touch.position;
            else 
                return Vector2.zero; 
        } 
    }

    public int meStretchState { get { return mStretchState; } }

    public EToward4 meSlideDir { get { 
            if (mSlideTo.binary[2]) 
                return EToward4.down; 
            else if (mSlideTo.binary[3])
                return EToward4.up; 
            else if (mSlideTo.binary[0])
                return EToward4.right; 
            else if (mSlideTo.binary[1])
                return EToward4.left; 
            else 
                return EToward4.middle; 
        } 
    }//if顺序按频率排序的

    public Vector2 meWipeState { get { return mWipeState; } }
    public bool meWiping { get { return mInWipe; } }
    
    //外界可用===============================

    public void ApplyForOneTouchLeave(Action action)
    {
        mWhenOneTouchLeave += action;
    }

    //一次性赋值=============================

    //私用变量===============================

    Action mWhenOneTouchLeave;

    bool mOneTouch;//是否处于单点触碰的状态
    int mOneIdLast;//之前单点触碰时所用的标识
    int mOnePosLast;//之前单点触碰时所占用的位置
    int mOneWhere;
    Digit mSlideTo;//滑动手势的滑动方向
    Vector2 mWipeState;//移动矢量
    bool mInWipe;
    const float cWipeRatio = 0.25f;

    int mStretchState;//是否在尝试拉伸
    bool mTwoTouch;//第二个触碰
    int mTouch1;//某一个触碰
    int mTouch2;
    Vector2 mPre1, mPre2;//某一个触碰在上一帧的位置
    Vector2 mPost1, mPost2;//记录两个手指的当前位置
    Vector2 mDeltaMove1, mDeltaMove2;//记录这两个手指的每帧移动距离

    static List<Finger> mFingers = new List<Finger>();//承载当前游戏逻辑可参考的触碰数据

    //内部机制=================================

    void Start()
    {
        Input.multiTouchEnabled = true;//开启多点触碰
        if (mFingers.Count == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                Finger f = new Finger();
                f.id = -1;
                mFingers.Add(f);
            }
        }

        mSlideTo = new Digit();
    }

    void Update()
    {
        UpdateFingersEarly();

        UpdateStates();

        UpdateFingersLately();
    }

    void UpdateFingersEarly()//通用的数据的处理
    {
        Touch[] touches = Input.touches;

        foreach (Finger f in mFingers)//掦除已经不存在的手指
        {
            if (f.id == -1)
                continue;
            else
            {
                bool still_exist = false;//触碰是否仍然存在的标记
                foreach (Touch t in touches) 
                {
                    if (f.id == t.fingerId)
                    {
                        still_exist = true;
                        break;
                    }
                }
                if (!still_exist)
                {
                    UnifiedCursor.It.ExciteOver(f.pos_last, EKindInput.touch);
                    UnifiedInput.It.ExciteReleaseSure(EKindInput.touch);

                    f.id = -1;//掦除，不过并不完全，仅标记为废弃
                }
            }
        }

        foreach (Touch t in touches)//遍历当前具有的触碰数据，检查它们在是否已经记录在手指列表中
        {
            bool hereIs = false;//是否记录有相应的触碰
            foreach (Finger f in mFingers)
            {
                if (t.fingerId == f.id)//是的话更新对应手指的状态
                {
                    f.touch = t;
                    f.time += Time.deltaTime;
                    hereIs = true;
                    break;
                }
            }
            if (hereIs)
                continue;
            else
            {//手指中不存在当前识别到的触碰时
                foreach (Finger f in mFingers)//不是则应加入到figure中，除非超过了可以同时承载的手指数量
                {//顺便会完全剔除之前有的数据
                    if (f.id == -1)
                    {
                        f.id = t.fingerId;
                        f.touch = t;
                        f.press_start = f.pos_last = t.position;
                        f.time = 0;//重置时间

                        UnifiedCursor.It.ExciteOver(f.press_start,EKindInput.touch);
                        UnifiedInput.It.ExciteStartSure(EKindInput.touch, true);

                        break;
                    }
                }
            }
        }

    }

    void UpdateStates()//只负责刚发生相应情况时的状态识别，无论在此前是怎么样的触屏状态
    {
        RespondOneTouch();
        RespondTwoTouch();
    }

    void RespondOneTouch()
    {
        if (Input.touchCount == 1)//当前只有一个触碰
        {
            mOneWhere = GetFinger(0);

            if (mOneWhere < 0)//失效时
            {
                ProEndOneTouch();
                return;
            }
            else if (mOneWhere != mOnePosLast)//可能突然换了手指，且没有占据被换前的手指，所对应位置
                mOneTouch = false;
            else if (mFingers[mOneWhere].id != mOneIdLast)//可能突然换了手指，但仍然使用的之前的位置记录
                mOneTouch = false;

            Finger f = mFingers[mOneWhere];
            if (!mOneTouch)//一般情况下，对应刚发生单点触碰时
            {
                f.new_one = true;
                f.move_time = f.move_stop = 0;
                f.move_begin = f.pos_last = f.touch.position;
                mOnePosLast = mOneWhere;
                mOneIdLast = f.id;
                mOneTouch = true;
            }
            else//识别到单点触碰时的下一帧会执行
            {
                mWipeState = f.touch.position - f.pos_last;//外界需求复杂灵活时，直接使用这个数据
                if (mWipeState.magnitude > Mathf.Epsilon)//手指在屏幕上发生移动
                {
                    //手势识别主要靠滑动速度而不是滑动时间
                    //与上一帧比，相对上一帧的状态而非开始移动的时候
                    if (mWipeState.y < -10 * cWipeRatio)//上
                    {
                        if (Mathf.Abs(mWipeState.x) < - mWipeState.y)  //往上的比例需要更高
                            ProSetSilde(EToward4.up);
                        else
                            ProSlideLeftRight();
                    }
                    else if (mWipeState.y > 10 * cWipeRatio)//下，与上逻辑互斥
                    {
                        if (Mathf.Abs(mWipeState.x) < mWipeState.y)
                            ProSetSilde(EToward4.down);
                        else
                            ProSlideLeftRight();
                    }
                    else
                        ProSlideLeftRight();

                    f.move_time += Time.deltaTime;
                    f.move_stop = 0;
                    mInWipe = true;
                }
                else
                {
                    f.move_stop += Time.deltaTime;
                    if (f.move_stop > 1) //不立即归零，可能只是小停顿，因此保留当前情况
                    {
                        f.move_time = 0;
                        f.move_begin = f.touch.position;
                        mSlideTo.AllNone();
                    }
                    mInWipe = false;
                }

                if (f.new_one) f.new_one = false;
            }
        }
        else
            ProEndOneTouch();
    }
    void ProEndOneTouch()
    {
        if (mOneTouch)
        {
            mOneTouch = false;
            mWipeState = Vector2.zero;
            mInWipe = false;
            mSlideTo.AllNone();
            if (mWhenOneTouchLeave != null)
            {
                Action WhenOneTouchLeave = mWhenOneTouchLeave;
                mWhenOneTouchLeave = null;//每一次触发后，都会撤除所有反应
                WhenOneTouchLeave();
            }
        }
    }
    void ProSlideLeftRight()
    {
        if (mWipeState.x < -5 * cWipeRatio)
            ProSetSilde(EToward4.right);
        else if (mWipeState.x > 5 * cWipeRatio)
            ProSetSilde(EToward4.left);
        else
            ProSetSilde(EToward4.middle);
    }
    void ProSetSilde(EToward4 to)//数据内容互斥化
    {
        switch (to)
        {
            case EToward4.up:ProSetSilde(true, false, false, false);break;
            case EToward4.down:ProSetSilde(false, true, false, false);break;
            case EToward4.left:ProSetSilde(false, false, true, false);break;
            case EToward4.right:ProSetSilde(false, false, false, true); break;
            default: ProSetSilde(false, false, false, false); break;
        }
    }
    void ProSetSilde(bool up,bool down,bool left,bool right)//设置
    {
        mSlideTo.binary[3] = up;
        mSlideTo.binary[2] = down;
        mSlideTo.binary[1] = left;
        mSlideTo.binary[0] = right;
    }

    void RespondTwoTouch()
    {

        if (Input.touchCount == 2)//多点触碰
        {
            if (mTouch1 < 0 || mTouch2 < 0)
                mTwoTouch = false;
            else if (mFingers[mTouch1].id < 0 || mFingers[mTouch2].id < 0)
                mTwoTouch = false;

            if (!mTwoTouch)
            {
                mTouch1 = GetFinger(0);
                mTouch2 = GetFinger(mTouch1 + 1);
                mPre1 = mFingers[mTouch1].touch.position;
                mPre2 = mFingers[mTouch2].touch.position;
                mTwoTouch = true;
            }
            else
            {
                mPost1 = mFingers[mTouch1].touch.position;
                mDeltaMove1 = mFingers[mTouch1].touch.deltaPosition;
                mPost2 = mFingers[mTouch2].touch.position;
                mDeltaMove2 = mFingers[mTouch2].touch.deltaPosition;

                mStretchState = WhetherStretch(mPre1, mPre2, mPost1, mPost2);

                mPre1 = mPost1;
                mPre2 = mPost2;
            }
        }
        else
        {
            mTwoTouch = false;
            mStretchState = 0;
        }
    }

    void UpdateFingersLately()
    {
        foreach (Finger f in mFingers)
        {
            if (f.id > -1)
            {
                f.pos_last = f.touch.position;//滞后记录

                UnifiedCursor.It.ExciteOver(f.pos_last, EKindInput.touch);
            }
        }
    }

    //内部工具=====================================

    int WhetherStretch(Vector2 pre_1, Vector2 pre_2, Vector2 post_1, Vector2 post_2)//判断是否被拉伸
    {//还只是简单的判断
        float gap_pre = Mathf.Pow(pre_1.x - pre_2.x, 2) + Mathf.Pow(pre_1.y - pre_2.y, 2);//原来位置之间的距离
        float gap_post = Mathf.Pow(post_1.x - post_2.x, 2) + Mathf.Pow(post_1.y - post_2.y, 2);//后来位置之间的距离
        if (gap_pre < gap_post)//放大手势
            return 1;
        else if (gap_pre > gap_post) //缩小手势
            return -1;
        else
            return 0;
    }

    int GetFinger(int start)//获取到一个当前有效的手指
    {//参数是指从哪个序数开始，该方法会从这个序数到后续序数所对应有的手指中寻找
        for (int i = start; i < mFingers.Count; i++)
        {
            if (mFingers[i].id != -1)
                return i;
        }
        return -1;
    }

    //架构需要==================================================

    public static TouchInput It;

    public void WhenAwake()
    {
        It = this;
    }

    public void WhenSwitchScene()
    {
        
    }

    class Finger//中间层，通过id维持对应关系
    {
        //通用-------------------------------------
        public int id = -1;//-1表示无效
        //从有效到无效，只有相应触碰不再触碰时才会发生
        public Touch touch;//相应的操作状态数据
        public float time;//保持按住的时间，从按下到松开
        public Vector3 worldPos { get { return Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane + 10)); } }

        //专用------------------------------------
        //暂时只在单点触碰时，会有这些数据的记录

        public Vector2 press_start = Vector2.zero;//按下时的位置
        public float move_stop = 0;//此次划动结束，到下一次划动的停留时间
        public float move_time = 0;//这一次划动过程的时间
        public Vector2 move_begin = Vector2.zero;//这一次划动过程的位置
        public Vector2 pos_last = Vector2.zero;//上一刻的位置
        public bool new_one = true;//是否是新来的
    }
    
}

