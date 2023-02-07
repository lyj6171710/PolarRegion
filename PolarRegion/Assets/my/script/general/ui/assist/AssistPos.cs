using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssistPos : MonoBehaviour//该组件提供ugui位置方面上的功能支持
{//需要所布置ui都遵从一定的规则，具体规则不好说，但尽量保持默认即可
 //可以预置在物体上来使用，也可以动态添加

    //=======================================
    public RectTransform meCanvasLay { get { return mCanvasLay; } }
    public RectTransform meSelfLay { get { return mSelfLay; } }

    //======================================

    public void SuUpdatePosByWorld(Vector3 pos_need) //将自己的显示位置覆盖在指定场景位置前
    {
        if (!mHaveReady) return;
        Vector2 outVec = CoordFrame.SuCoordInRectFromWorld(pos_need, mCanvasLay, mCanvasIn.renderMode);

        meSelfLay.localPosition = outVec;//需要从自身父物体到祖父物体到直属Canvas的物体的锚点中心，在位置上都一样
    }

    public bool SuWhetherMouseInside(float pad = 0)
    {
        if (!mHaveReady) return false;
        return CoordUse.SuWhetherInside(UnifiedCursor.It.SuGetCursorLocate(mCanvasLay), meSelfLay, 0);
    }

    public RectMeter SuGetCornerAtInScreen(bool byPercent = false)
    {
        RectMeter rect = new RectMeter();
        rect.leftBottom = CoordFrame.SuGetCornerBL(meSelfLay);
        //rect.leftBottom.ff(0);
        rect.rightTop = CoordFrame.SuGetCornerTR(meSelfLay);
        if (byPercent)
        {
            rect.leftBottom = CoordFrame.SuCoordPercentInScreenFromCanvas(rect.leftBottom, mCanvasIn.renderMode);
            rect.rightTop = CoordFrame.SuCoordPercentInScreenFromCanvas(rect.rightTop, mCanvasIn.renderMode);
        }
        return rect;
    }

    //===========================================

    public void SuMakeMoveFromStartByPercent(Vector2 shift)
    {
        if (!mHaveReady) return;
        if (mHaveSetPosStart) MoveFromStartByPercent(shift);
    }

    public void SuSetStartRefer(Vector2 startNew)//刚挂载上去时候的那个位置，不一定有效
    {
        mPosStart = startNew;
        mHaveSetPosStart = true;
    }

    public void SuResetPosToStart()
    {
        if (!mHaveReady) return;
        mSelfLay.localPosition = mPosStart;
    }
    
    //===========================================

    bool mHaveReady;

    bool mHaveSetPosStart;
    Vector2 mPosStart;//开始时，所挂载物体的位置所在

    void MoveFromStartByPercent(Vector2 shift)
    {
        shift.x = Mathf.Clamp(shift.x, -1, 1);
        shift.y = Mathf.Clamp(shift.y, -1, 1);
        float x = mPosStart.x + shift.x * mSelfLay.rect.width;
        float y = mPosStart.y + shift.y * mSelfLay.rect.height;
        Vector2 need = new Vector2(x, y);
        mSelfLay.localPosition = need;
    }

    //===========================================

    protected Camera mCam { get { return Camera.main; } }//摄像机会随场景变化而变动

    RectTransform mCanvasLay;
    RectTransform mSelfLay;
    Canvas mCanvasIn;

    public void MakeReady()//外界确保状态准备好了，再主动调用该函数
    {
        if(mHaveReady) return;

        mSelfLay = GetComponent<RectTransform>();
        mCanvasIn = GetComponentInParent<Canvas>();
        mCanvasLay = mCanvasIn.GetComponent<RectTransform>();

        mHaveReady = true;
    }
}
