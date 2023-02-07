using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveStretchFromCenter : MoveImgByDir
{
    //从中央往某个角度匀速伸展，然后收缩回来

    //进程
    bool mWillBack;//有无返回过程
    bool mInRevert;
    public Action<int> SuWhenFurthest;

    public void MakeReady(MoveReady.IfoImg ifoImg, int id)
    {
        Ready(ifoImg, id);

        SuWhenFurthest += (idSelf) => { };
    }

    public void SuStartMove(MoveReady.Ifo ifo, bool willBack = true)
    {
        mWillBack = willBack;

        StartMove(ifo);
    }

    public void SuAdjustSpeed(float demand)
    {
        mReady.meIfo.speed = demand;
        mMoveSpeedRatio = mOffsetExe.SuSpanToRatio(mIfoMove.speed);
    }

    void Update()
    {
        if (mInMove)
        {
            if (!mInRevert)
            {
                if (mOffsetExe.SuWhetherCloseToEnd())
                {
                    if (mWillBack)
                    {
                        mInRevert = true;
                        SuWhenFurthest(mId);
                    }
                    else
                    {
                        mOffsetExe.SuVaryTo(0);
                        FinishMove();
                    }
                }
                else
                {
                    mReady.SuSetOpacity(mMoveProgress);
                    mMoveProgress += mMoveSpeedRatio * Time.deltaTime;
                    mOffsetExe.SuVaryTo(mMoveProgress);
                }
            }
            else
            {
                if (mOffsetExe.SuWhetherCloseToStart())
                {
                    mInMove = false;
                    FinishMove();
                }
                else
                {
                    mReady.SuSetOpacity(mMoveProgress);
                    mMoveProgress -= mMoveSpeedRatio * Time.deltaTime;
                    mOffsetExe.SuVaryTo(mMoveProgress);
                }
            }
        }
    }

    //================================

    protected override EKindFieldOffset WhatOffsetVaryTo()
    {
        return EKindFieldOffset.local_pos;
    }

    protected override void WhenStartReady1Progress()
    {
        mInRevert = false;
        mMotion.localPosition = Vector3.zero;
    }

    protected override void WhenStartReady2OffsetIfo()
    {
        mIfoOffset.start = Vector2.zero;
        mIfoOffset.form = EFormOffset.fix;
        mIfoOffset.toEnd = MathAngle.CoordTransfer(Vector2.zero, mIfoMove.dir, mIfoMove.span);
    }

    protected override void WhenStartReady4Other()
    {
        mMoveSpeedRatio = mOffsetExe.SuSpanToRatio(mIfoMove.speed);
    }

    protected override void WhenAdaptDir1UpdateOffsetIfo()
    {

    }

    protected override void WhenAdaptDir2UpdateDir()
    {

    }

    protected override void WhenAdaptDir3UpdateSpeedRatioOrOther()
    {
        
    }

}
