using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IDirGet
{
    public float meAngleNow { get; }
}

public class ImgSideExpress : MonoBehaviour
{//注重图像本身的表达

    public Action meWhenFlip;

    public Sprite meProfile { get { return mRenderSelf.sprite; } }

    public bool meIsFaceToRight { get { return mStartFlip ? (-mCurSignFlip).ToBool() : mCurSignFlip.ToBool(); } }

    public float meOffsetToFoot { get 
        {
            if (!mConsiderDir)//默认认为，按底边与场右平行，一直放置着
                return mOffsetToFoot;
            else
                return MathRect.CalcHighestPosY(mInclineHalf, mRadianToUR, mDir.meAngleNow);
        } 
    }

    //----------------------------------
    
    bool mStartFlip;//开始时的翻转状态，统一应预设为图像内容朝右时的翻转状态
    int mCurSignFlip;

    SpriteRenderer mRenderSelf;
    float mOffsetToFoot;
    public Vector2 mSize;

    bool mConsiderDir;//考虑旋转影响
    public float mInclineHalf;
    float mRadianToUR;
    IDirGet mDir;

    bool mHaveReady;

    public float meFootAt { get { return transform.position.y - mOffsetToFoot; } }

    public void MakeReady(SpriteRenderer diagramRefer)
    {
        mRenderSelf = GbjAssist.AddCompSafe<SpriteRenderer>(gameObject);
        //能同时适应参考者就是自己的情况
        if (diagramRefer != null)
        {//不给参照，说明不需要该组件来重新设定了
            mRenderSelf.sprite = diagramRefer.sprite;
            mStartFlip = diagramRefer.flipX;
        }
        else
        {
            mStartFlip = mRenderSelf.flipX;
        }
        meWhenFlip = () => { };
        mRenderSelf.flipX = false;
        //渲染组件上预置的flip状态只是用来标记正反，最终得用scale来实现正反
        SuSwitchFace(mStartFlip ? -1 : 1);

        mHaveReady = true;
    }

    public void ReadyCountSize()
    {//该组件所挂载物体的伸缩可能发生在该组件准备活动之后
     //外界确保不会发生伸缩后，再主动调用该方法，来进一步初始化该组件
        Vector2 scaleAcc = GbjAssist.GetSumScaleWhenSelf(transform);
        mSize = SpriteAssist.GetSizeInScene(mRenderSelf.sprite) * scaleAcc;
        mOffsetToFoot = mSize.y / 2;//计算脚底相对中心的场景偏移量
    }

    public void ReadyConsiderDir()
    {
        //调用前需已经挂载有控制方向的组件
        mConsiderDir = true;

        //计算矩形在场景y轴方向上的最大偏移点
        mDir = gameObject.GetComponent<IDirGet>();
        mInclineHalf = MathRect.CalcRectInclineHalf(mSize);
        mRadianToUR = MathRect.CalcRadianToUR(mSize);
    }

    public void SuSwitchFace(float faceTo = 0)
    {//如果传来值大于0，则保证图像的内容是朝右的
        if (faceTo != 0)
        {
            Vector3 origin = transform.localScale;
            origin.x = MathNum.Abs(origin.x);
            int lastSign = mCurSignFlip;
            if (faceTo > 0)
                mCurSignFlip = mStartFlip ? -1 : 1;
            //如果一开始就是朝右的，那么一开始需要的翻转就是false
            else //faceTo < 0
                mCurSignFlip = mStartFlip ? 1 : -1;
            if (lastSign != mCurSignFlip)
            {
                transform.localScale = new Vector3(mCurSignFlip * origin.x, origin.y, origin.z);//转向的改变，不只是图像本身，还伴随它的碰撞体
                meWhenFlip();
            }
        }
    }

    public void SuSetShowLayer(int need)
    {
        mRenderSelf.sortingOrder = need;
    }

}
