using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveOnPlane : MonoBehaviour, ITreeBranch
{
    //该组件用来承包所挂载物体的行走功能(不是单纯的移动，但可以帮助移动)

    //有一个父物体，角色是子物体，但本组件挂载到这个父物体的另一个子物体上
    //自己负责了阴影以及行走碰撞，自己会控制角色来实现跳跃

    //===================================

    Transform mFoot;//人偶的父物体，它脚底投影到地面的位置
    Transform mPawn;//被操控移动的人偶，外界赋值后，该组件才会生效
    
    //内部机制===============================

    bool mHaveReady;

    [ContextMenu("BuildShadow")]
    void BuildShadow()//需要预置时调用，只需手动调整好localscale，其它属性会被组件执行时自动赋值好
    {
        SpriteRenderer render = gameObject.AddComponent<SpriteRenderer>();
        Sprite shadow = Resources.Load<Sprite>(AddrResource.path_shadow);
        render.sprite = shadow;
    }

    public MoveOnPlane MakeReady(Transform pawn, string layer)//由外界手动调用，这样方便控制流程
    {//pawn要是某一个子物体
     //该组件所挂载物体需与pawn是同父同级的不同物体
        
        mPawn = pawn;
        mLayer = layer;

        mRenderShadow = GbjAssist.AddCompSafe<SpriteRenderer>(gameObject);
        if (mRenderShadow.sprite == null)
            mRenderShadow.sprite = Resources.Load<Sprite>(AddrResource.path_shadow);
        mRenderShadow.sortingOrder = GlobalConfig.order_shadow;
        mRenderShadow.flipX = false;

        mFoot = GbjAssist.GetSuper(mPawn);
        //脚底位置，不是阴影物体，也不一定是pawn的父物体，一般应是带动所有物体移动的物体

        SpriteAssist.MakeSibAlignToButtom(mPawn, transform);
        //要求sib一定已经挂载好SpriteRender
        mShowPawn = mPawn.GetComponent<ImgSideExpress>();//同理

        mCollid = gameObject.AddComponent<BoxCollider2D>();
        mCollid.isTrigger = true;
        
        gameObject.layer = LayerMask.NameToLayer(mLayer);

        meWhenJump += () => { };
        meWhenLand += () => { };

        mRigid = gameObject.AddComponent<Rigidbody2D>();//用来支持碰撞时机函数能够被触发
        mRigid.gravityScale = 0;
        mHits = new HashSet<Transform>();

        ReadyMove();
        ReadyLift();

        mShowShadow = gameObject.AddComponent<ImgSideExpress>();
        mShowShadow.MakeReady(mRenderShadow);//这个组件用来算数据
        mShowShadow.ReadyCountSize();
        
        if (mPawn != null) mHaveReady = true;

        return this;
    }

    public void ReadyBuildShapeSelf(SpriteAssist.Ifo ifo)//依赖外界情况的初始化，外界初始化好sib后再调用
    {
        SpriteAssist.VaryScaleFitToSibLook(mPawn, transform, ifo);
        mShowShadow.ReadyCountSize();//重新算
        SpriteAssist.VaryCollidToFitSibLook(mPawn, transform, ifo);
    }

    public void ReadyConsiderDir()//外界应在该物体上安装上控制方向的组件后再调用该方法
    {//如果该组件所挂载物体不会旋转，就不需要调用，因为不影响本组件对显示层级的控制
        mShowShadow.ReadyConsiderDir();
    }

    //=======================================

    public bool meCanMove = true;
    [Min(0)] public float meMoveSpeedMaxDp = 2;//移动速度的标准
    [Min(0)] public float meMoveAccelerateDp = 10.0f;
    [Min(0)] public float meMoveDecelerateDp = 30.0f;

    public void SuTakeMove(Vector2 to)//这要求外界需要每帧调用(非fixedupdate)，否则会认为是跑停跑停
    {
        if (!meCanMove) return;

        if (to.magnitude > 0.05f)//如果没有速度了，就需要保留原有方向(外界可以自行不保留)
            mDirMoveWant = to.normalized;//注意这个normalize函数，能让明明正向的数据，产生微弱的方向偏移，有坑
        //else 这里else是难有用的，因为该函数靠外界调用才能执行，除非外界始终在调用该函数

        mSpeedMoveWant = Mathf.Clamp(to.magnitude, 0, 1);//以比例形式描述

        mMoveMakeWait = true;//只下一帧生效
    }

    //-------------------------------------

    BoxCollider2D mCollid;//自身的碰撞体，检测周围碰撞

    float mSpeedMoveWant;//以相对最大可有速度的比例来描述，无符号
    float mSpeedMoveCan;//当前可以达到的移动速度，更实际，无符号
    float mSpeedMoveCur;//当前的移动速度，无符号
    float mVelCurX;//当前在X方向上的移动速度，有符号
    float mVelCurY;//当前在X方向上的移动速度，有符号

    Vector2 mDirMoveWant;//移动欲使方向
    Vector2 mDirMoveCur;//当前正处在的移动方向
    bool mMoveMakeWait;//用来停止移动输入
    bool mMoveMake;//可以且应该进行移动
    Vector2 mVelMoveNew;//记录当前的速度（方向+实际速度）
    Vector2 mOffsetMoveCur;//当前的移动偏移，最实际的移动结果
    Vector2 mPosMoveStart;
    Vector2 mPosOffsetSinceStart;

    public Vector2 meDirCurAim { get {  return mDirMoveWant; } }//单位向量
    public Vector2 meOffsetCurByMove { get { return mOffsetMoveCur; } }//有距离信息
    public float meDirCurAimX { get { if (mDirMoveWant.x > 0.05f) return 1; else if (mDirMoveWant.x > -0.05f) return 0; else return -1; } }

    void ReadyMove()
    {
        mPosMoveStart = mFoot.position;
    }

    void UpdateMove()
    {
        StopMove();//移动需求识别
        ComputeMove(Time.deltaTime);
        MakeMove(Time.deltaTime);
    }

    void LateUpdateMove()
    {
        OverAssist.OnThenOff(ref mMoveMake, ref mMoveMakeWait);
    }

    void StopMove()//移动包括方向和速度
    {
        if (!mMoveMake)
            mSpeedMoveWant = 0;
        else if (!meCanMove)
            mSpeedMoveWant = 0;
    }

    void ComputeMove(float gap)
    {
        mSpeedMoveCan = mSpeedMoveWant * meMoveSpeedMaxDp;//want会归零，所以x、y不会在0周围摇摆
        float curVelCanX = mSpeedMoveCan * Vector2.Dot(mDirMoveWant, Vector2.right);//有正负
        float curVelCanY = mSpeedMoveCan * Vector2.Dot(mDirMoveWant, Vector2.up);
        
        float GetAccReal(ref float cur, ref float can)//在同一个方向上考虑
        {
            float GetAccReal(float cur,float can,float acc)//需要能得到最终每帧会加速多少
            {
                float accRatio = gap;
                float dirAffect = MathNum.Abs(cur) / meMoveSpeedMaxDp;
                accRatio *= dirAffect > 0.25f ? dirAffect : 0.25f;
                //速度越大，综合速度方向往该速度所在方向偏，能获得的加速度自然越大
                if (mInJump) accRatio /= 4;
                return acc * accRatio;
            }

            float acc = 0;//实际意义就是控制这一帧的位移量及方向，有正负
            bool accOver = false;//是否溢出加速度的前提范围
            float signMulti = MathNum.Sign10(cur) * MathNum.Sign10(can);
            if (signMulti < 0)
            {
                if (cur > 0)
                {
                    acc = -meMoveDecelerateDp;//往反方向，先主动启用减速度(一般大于加速度)，能更快
                    acc = GetAccReal(cur, can, acc);
                    accOver = cur + acc < 0;//减速度只应生效于负方向移动时
                }
                else // cur < 0
                {
                    acc = meMoveDecelerateDp;//朝速度0时，就是用减速度
                    acc = GetAccReal(cur, can, acc);
                    accOver = cur + acc > 0;
                }
                if (accOver)
                    acc = MathNum.Sign1(can) * 0.02f - cur;//保证先归到同一方向
            }
            else 
            {
                if (signMulti > 0) //同一移动方向后
                {
                    if (cur > 0)
                    {
                        if (cur < can)
                        {
                            acc = meMoveAccelerateDp;
                            acc = GetAccReal(cur, can, acc);
                            accOver = cur + acc > can;//满加速度发生在，就算满加速度也不能立即到达目标速度时
                                                      //不这么做，会发生移动抖动，超过目标，随后又达不到目标
                        }
                        else if (cur > can)
                        {
                            acc = -meMoveDecelerateDp;
                            acc = GetAccReal(cur, can, acc);
                            accOver = cur + acc < can;
                        }
                        else
                            acc = 0;
                    }
                    else// cur < 0
                    {
                        if (cur > can)
                        {
                            acc = -meMoveAccelerateDp;
                            acc = GetAccReal(cur, can, acc);
                            accOver = cur + acc < can;
                        }
                        else if (cur < can)
                        {
                            acc = meMoveDecelerateDp;
                            acc = GetAccReal(cur, can, acc);
                            accOver = cur + acc > can;
                        }
                        else
                            acc = 0;
                    }
                }
                else // multi == 0
                {
                    if (MathNum.IsNear0(cur))
                    {
                        if (MathNum.IsNear0(can))
                        {
                            accOver = true;
                        }
                        else if (can > 0)
                        {
                            acc = meMoveAccelerateDp;
                            acc = GetAccReal(cur, can, acc);
                            accOver = cur + acc > can;
                        }
                        else //can < 0
                        {
                            acc = -meMoveAccelerateDp;
                            acc = GetAccReal(cur, can, acc);
                            accOver = cur + acc < can;
                        }
                    }
                    else // MathNum.FloatNear0(can) == true
                    {
                        if (cur > 0)
                        {
                            acc = -meMoveDecelerateDp;
                            acc = GetAccReal(cur, can, acc);
                            accOver = cur + acc < can;
                        }
                        else //cur < 0
                        {
                            acc = meMoveDecelerateDp;
                            acc = GetAccReal(cur, can, acc);
                            accOver = cur + acc > can;
                        }
                    }
                }
                if (accOver) acc = can - cur;//加速度过剩，这里使得直接达标
            }
            
            return acc;
        }

        mVelCurX += GetAccReal(ref mVelCurX, ref curVelCanX);
        mVelCurY += GetAccReal(ref mVelCurY, ref curVelCanY);

        Vector2 curVel = new Vector2(mVelCurX, mVelCurY);

        mSpeedMoveCur = curVel.magnitude;
        if (mSpeedMoveCur > 0.05f)
            mDirMoveCur = curVel.normalized;

        if (!mInJump)
            mVelMoveNew = curVel;//当需要移动时
        else
            mVelMoveNew = mDirJump * mSpeedMoveCur;//跳跃朝向取决于发起跳跃时的移动方向
    }

    void MakeMove(float gap)
    {
        //不使用刚体的velocity属性来移动，因为可自定义性太弱

        float moveResist = 0 ;
        foreach (Transform one in mHits)//遇到其它物体，会被暂时减速
        {
            Vector3 dir = (one.position - mFoot.position).normalized;
            float confront = Vector3.Dot(dir, mDirMoveCur);
            if (confront > moveResist) moveResist = confront;
        }

        mOffsetMoveCur = mVelMoveNew * gap * (1 - moveResist);
        mFoot.Translate(mOffsetMoveCur);
        mPosOffsetSinceStart = mFoot.position.ToVector2() - mPosMoveStart;//顺便记录
    }

    //========================================
    //第三维效果

    public void SuTakeLiftInstant(float height)
    {
        mHeightLiftAt = mLift.meHeightStart + height;
        ApplyLiftHeight();
    }

    public void SuTakeLift(float ratio)//控制升降
    {
        mRatioKeep = true;
        mRatioLiftUse = ratio;
    }

    public void SuStopLift()
    {
        mRatioKeep = false;
    }

    KeepOnAbove mLift;//升降控制器，无视本地空间对竖向位置的旋转影响
    float mHeightLiftAt;//当前相对初始值的升降高度，当前升降在的位置

    float mRatioLiftUse;//相对地面移动距离的比例，由此可以维持上升角度的正确及固定
    bool mRatioKeep;//是否处于升降状态
    
    void ReadyLift()
    {
        mLift = mPawn.gameObject.AddComponent<KeepOnAbove>();//先已经调整好pawn的位置
        mHeightLiftAt = mLift.meHeightStart;
    }

    void UpdateLift()
    {
        if (mRatioKeep)
        {
            mHeightLiftAt = mLift.meHeightStart + mPosOffsetSinceStart.magnitude * mRatioLiftUse;
            ApplyLiftHeight();
        }
    }

    void ApplyLiftHeight()
    {
        if (!mInJump)
            mLift.SuSetHeightTo(mHeightLiftAt);
    }

    //========================================

    //跳跃

    public bool meInJump { get { return mInJump; } }
    public Action meWhenJump;
    public Action meWhenLand;
    public float meJumpVelDp = 0.1f;//跳跃时初始向up的速度v
    public float meGravityAccDp = 0.01f;//重力加速度a

    public void SuTakeJump()//只负责了子物体相对位置，其它事项由外界自己负责
    {
        mJumpWant = true;
        mDirJump = mDirMoveCur;
    }

    public void SuStopJump()
    {
        if (mInJump) MakeLand();
    }

    //--------------------------------------
    
    bool mInJump;
    bool mJumpWant;
    bool mNeedJump;
    Vector2 mDirJump;
    float mHeightJumpNow;//当前跳跃高度，此值作为二维平面up的离开地面的逻辑真实高度
    Coroutine mJumping;

    void InspectJump()
    {
        if (mJumpWant && !mInJump)
            mNeedJump = true;
        mJumpWant = false;
    }
    
    void MakeJump()
    {
        if (mNeedJump)
        {
            mJumping = StartCoroutine(Jump(meJumpVelDp));//将单体向up抬升
            meWhenJump();
            mInJump = true;
            mNeedJump = false;
        }
    }

    IEnumerator Jump(float vStart)//往上弹跳，然后启用重力，直到向上位移为0时停止
    {
        float vDecline = vStart;//初始向up的速度
        mHeightJumpNow = mHeightLiftAt;//起跳时的高度所在
        while (true)
        {
            while (mInPause) yield return null;
            mLift.SuDeviateHeight(vDecline);//v在时间上的积分就为总高度
            mHeightJumpNow += vDecline;//相对变化的记录
            vDecline -= meGravityAccDp;//速度随重力加速度递减
            if (mHeightJumpNow <= mHeightLiftAt) MakeLand();//说明此时到达"地面"
            yield return null;//这个等待需要放在着地检测后，不然出现卡入地面的情况
        }
    }

    void MakeLand()
    {
        mHeightJumpNow = 0;//矫正为0
        mLift.SuSetHeightTo(mLift.meHeightStart);
        mInJump = false;
        StopCoroutine(mJumping);
        meWhenLand();
    }

    void UpdateJump()
    {
        InspectJump();//跳跃需求识别
        MakeJump();
    }

    //=====================================

    SpriteRenderer mRenderShadow;//影子渲染
    ImgSideExpress mShowShadow;
    ImgSideExpress mShowPawn;
    int mShowLayerNow;
    float mPosLowestNow;

    void UpdateShowLayer()
    {
        mPosLowestNow = transform.position.y - mShowShadow.meOffsetToFoot;
        //if (transform.parent.gameObject.name == "cando1") mShowShadow.meOffsetToFoot.ff(0);
        //if (transform.parent.gameObject.name == "skillO") mShowShadow.mSize.x.ff(1);//.meOffsetToFoot.ff(0);
        mShowLayerNow = (int)(MathNum.MapClamp(
            mPosLowestNow, 20, 0, 0, 1) * GlobalConfig.order_actor);
        //本来是基于人偶图像谁底部更下，谁优先显示，但是现在新增了高度的影响
        //影子底部的位置控制显示层级，最下边的影子所属，肯定位于更前方
        mShowPawn.SuSetShowLayer(mShowLayerNow);
    }

    public bool IsOverlapMutual(MoveOnPlane other)
    {//该移动组件承包了移动和相互位置关系，所以如果涉及到相互接触，该组件应该有资本给个说法
     //该移动组件实现了在二维平面上有三维效果的移动，光是人偶形体的相互接触，不能说法它们相互重叠

        //需要已经识别到相互接触，再调用该方法，所返回结果才是有效的
        //如果是因为人偶图像相互接触了，再调用的该方法，高度上也能自动适配的

        //这里判断双方处于平面上的地点是否位于同一处(平面上的两水平线内)
        //直观来说，就是对方阴影贴在自己阴影上时就认为重叠
        if (other.transform.position.y + other.mShowShadow.meOffsetToFoot > mPosLowestNow//对方阴影顶部高于自己阴影底部时
            && other.mPosLowestNow < transform.position.y + mShowShadow.meOffsetToFoot)//对方阴影底部低于自己阴影顶部时
            return true;
        else
            return false;
        
    }

    //=====================================

    bool mInPause;
    bool mNeedPause;

    public void SuSuspendMove(bool need)
    {
        mNeedPause = need;
    }

    void UpdatePause()
    {
        if (!mNeedPause)
        {
            if (mInPause)
            {
                mInPause = false;
            }
        }
        else
            mInPause = true;
    }

    //===================================================

    void Update()
    {
        if (!mHaveReady) return;

        UpdatePause();

        if (!mInPause)
        {
            UpdateMove();

            UpdateJump();

            UpdateLift();

            UpdateShowLayer();
        }
    }

    void LateUpdate()
    {
        LateUpdateMove();
    }

    Rigidbody2D mRigid;
    HashSet<Transform> mHits;
    string mLayer;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(mLayer))
            mHits.Add(collision.transform);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        mHits.Remove(collision.transform);
    }

    TreeBranch branch;

    public void SelfReady(TreeBranch shelf)
    {
        branch = shelf;
    }

    public object RespondReadFromUp(InfoSeal seal, out int source)
    {
        throw new NotImplementedException();
    }

    public void ReceiveNotifyFromUp(InfoSeal seal)
    {

    }
}