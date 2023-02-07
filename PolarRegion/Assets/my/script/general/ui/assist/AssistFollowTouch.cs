using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssistFollowTouch : AssistFollow
{//追随触摸位置的效果

    [Range(-1, 1)] public float mOffsetX = 0f;//0值时，会处在触点的右下角
    [Range(-1, 1)] public float mOffsetY = 0f;//x变大为往右，y变大为往上

    bool mJustFollow;//是否刚开始进行跟踪
    Vector2 mStartPos;
    Vector2 mOffsetAcc;

    protected override void StartNer()
    {
        mStartPos = meSelf.localPosition;
    }

    protected override void UpdateNer()
    {
        if (mFollow)
        {
            if (mMode == Mode.point)
            {
                mNeed = CoordFrame.SuCoordInCanvasFromScreenByMeter(mPos.meCanvasLay, TouchInput.It.mePosIfOneTouch);
                mNeed += new Vector2(meSelf.sizeDelta.x / 2 + meSelf.sizeDelta.x * mOffsetX, -meSelf.sizeDelta.y / 2 + meSelf.sizeDelta.y * mOffsetY);
                if (mJustFollow)
                {
                    mJustFollow = false;
                }
            }
            else
            {
                mNeed += UnifiedCursor.It.meSlideInCanvas;
                if (mJustFollow)
                { 
                    mJustFollow = false;
                }
                mOffsetAcc += UnifiedCursor.It.meSlideInCanvas;
            }
        }
        else
        {
            mJustFollow = true;
        }
    }

    public override void BackToStart()
    {
        if (mMode == Mode.point)
        {
            mForce = true;
            mNeed = mStartPos;//离开拖拽状态时，直接回到原来位置
        }
        else if(mMode == Mode.offset)
        {
            mForce = true;
            mNeed -= mOffsetAcc;
            mOffsetAcc = Vector2.zero;
        }
    }

}
