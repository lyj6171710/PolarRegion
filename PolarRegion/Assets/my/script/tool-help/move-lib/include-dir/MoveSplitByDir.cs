using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveSplitByDir : MoveImgByDir
{
    //使物体从某地按某个方向移动，非相对运动，会使脱离自己

    public void MakeReady(MoveReady.IfoImg ifo, int id)
    {
        Ready(ifo, id);
    }

    public void SuStartMove(MoveReady.Ifo ifo, Vector3 posLocalStart)
    {
        mPosLocalStart = posLocalStart;
        StartMove(ifo);
    }

    Vector3 mPosLocalStart;

    void Update()
    {
        if (mInMove)
        {
            if (mOffsetExe.SuWhetherCloseToEnd())
            {
                FinishMove();
            }
            else
            {
                mMoveProgress += mMoveSpeedRatio * Time.deltaTime;
                mOffsetExe.SuVaryTo(mMoveProgress);
                mDirFit.SuFitInstantly();
            }
        }
    }

    protected override EKindFieldOffset WhatOffsetVaryTo()
    {
        return EKindFieldOffset.world_pos;
    }

    protected override void WhenStartReady1Progress()
    {
        mMotion.localPosition = mPosLocalStart;
    }

    protected override void WhenStartReady2OffsetIfo()
    {
        mIfoOffset.start = transform.position;
        mIfoOffset.form = EFormOffset.fix;
        mIfoOffset.toEnd = MathAngle.CoordTransfer(Vector2.zero, mIfoMove.dir, mIfoMove.span).ToVector3();
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
