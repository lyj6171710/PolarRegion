using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraAim : MonoBehaviour
{
    //一次性赋值================================

    public float mOffsetXDp;//摄像机位置相对所追踪物位置的偏移，超过该指定值，会要求摄像机有移动而使不超过
    public float mOffsetYDp;

    [Range(0.05f, 2f)]
    public float mSmoothDp = 0.1f;//视野移动平滑度

    public bool mViewLimitDp = false;//视野限制在场景中的某个范围下
    public Vector2 mLimitUrDp;//摄像机移动边界
    public Vector2 mLimitDlDp;

    //私用变量======================================

    Transform mTransformAim;
    bool mFollowTarget;//指对所被赋值的Transform组件所在进行跟踪

    bool mFollowAsk;//指由外界来负责所自动跟踪的坐标位置
    float mAskContinue;//是否继续按外界要求的位置，去跟踪

    float mAimX;//任何形式跟踪，本质所参照跟踪的坐标点
    float mAimY;//最终参照目标

    void Awake()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -5);//让摄像机始终会处于游戏内容的前边
    }
    
    void LateUpdate()
    {
        ComputeNeed();//得出当前最终应追击的绝对坐标所在
        Follow();//follow时，一定会平滑往指定目标走
    }

    //内部机制================================================

    public void ComputeNeed()
    {
        if (mFollowAsk)
        {
            mAskContinue -= Time.deltaTime;
            if (mAskContinue < 0) mFollowAsk = false;
        }
        else if (mFollowTarget)
        {
            if (mTransformAim)
                SetFollowToPos(mTransformAim.position.x, mTransformAim.position.y);
            else
                mFollowTarget = false;//没有目标可追踪时，自行关闭追踪
        }
        else
        {
            SetFollowToPos(transform.position.x, transform.position.y);
            //外界此时再调用set_follow_to_pos_()，应该会是无效的
        }
    }

    public void Follow()
    {
        float tempX = 0;//当前帧将要移动到的位置的x坐标
        float tempY = 0;

        if (Mathf.Abs(mAimX - transform.position.x) > mOffsetXDp)//目标位置可能变化，绝对值大于上限时触发视野移动
            tempX = Mathf.Lerp(transform.position.x, mAimX, 0.05f * mSmoothDp);//在当前帧向目标位置前进一点
        else
            tempX = transform.position.x;//当处于静止移动范围时，追随当前所在即可
        if (Mathf.Abs(mAimY - transform.position.y) > mOffsetYDp)
            tempY = Mathf.Lerp(transform.position.y, mAimY, 0.05f * mSmoothDp);
        else
            tempY = transform.position.y;
        //lerp需要每一帧都使用，将其反馈的结果代入下一帧该次lerp函数调用时的参数
        //插值可以应对，每帧向目标前进一点，但每帧后目标位置又会有稍微改变的情况
        if (mViewLimitDp)
        {
            tempX = Mathf.Clamp(tempX, mLimitDlDp.x, mLimitUrDp.x);//clamp起强制到某值区间的作用
            tempY = Mathf.Clamp(tempY, mLimitDlDp.y, mLimitUrDp.y);
         }
        Vector2 mLastOffset = Vector2.zero;
        mLastOffset.x = tempX - transform.position.x;//位移量=目标坐标-当前坐标
        mLastOffset.y = tempY - transform.position.y;
        SceneViewL.It.SuTranslateView(mLastOffset);
    }

    //组合可用====================================

    public bool FollowAsk { get { return mFollowAsk; } }
    public bool FollowTarget { get { return !mFollowAsk && mFollowTarget; } }

    public void SetFollowToPos(float x, float y)//外界控制视野目标，需要不断调用
    {
        mFollowAsk = true;
        mAskContinue = 0.25f;//外界需要连续调用该函数，否则就会取消使用权
        mAimX = x;
        mAimY = y;
        //这里不同时让follow_target为false，一旦follow_ask变为false时，就转而继续原有的追踪
    }

    public void SetFollowTarget(Transform target)//视野自行追踪目标
    {
        mFollowTarget = true;
        mTransformAim = target;
    }

    public void StopFollow()
    {
        mFollowTarget = false;
        mFollowAsk = false;
    }

    //架构需要===================================

    static CameraAim it;
    public static CameraAim It
    { get { if (!it) it = GameObject.Find("Main Camera").GetComponent<CameraAim>(); return it; } }

}
