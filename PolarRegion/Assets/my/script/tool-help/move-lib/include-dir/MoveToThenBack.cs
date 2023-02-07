using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveToThenBack : MoveImgToTarget
{
    //近程攻击，会有来回，指定秒数内必定贴至敌人

    bool mInRevert;
    public Action<int> SuWhenFurthest;
    Transform mFrom;

    public void MakeReady(MoveReady.IfoImg ifo, int id, Transform from)
    {
        Ready(ifo, id);

        SuWhenFurthest += (idSelf) => { };
        mFrom = from;
    }

    public void SuStartMove(MoveReady.Ifo ifo, Transform target)
    {
        StartMove(ifo, target);
    }

    void Update()
    {
        if (mInMove)
        {
            if (!mInRevert)
            {
                if (mOffsetExe.SuWhetherCloseToEnd())
                {
                    mInRevert = true;
                    SuWhenFurthest(mId);
                }
                else
                {
                    mMoveProgress += mMoveSpeedRatio * Time.deltaTime;
                    mOffsetExe.SuVaryTo(mMoveProgress);
                    mReady.SuSetOpacity(mMoveProgress);
                    AdaptDir();
                }
            }
            else
            {
                if (mOffsetExe.SuWhetherCloseToStart())
                {
                    FinishMove();
                }
                else
                {
                    mMoveProgress -= mMoveSpeedRatio * Time.deltaTime;
                    mOffsetExe.SuVaryTo(mMoveProgress);
                    mReady.SuSetOpacity(mMoveProgress);
                    AdaptDir();
                }
            }
        }
    }

    //============================

    protected override EKindFieldOffset WhatOffsetVaryTo()
    {
        return EKindFieldOffset.local_pos;
    }

    protected override void WhenStartReady1Progress()
    {
        mInRevert = false;
    }

    protected override void WhenStartReady2OffsetIfo()
    {
        mMotion.localPosition = Vector3.zero;
        mIfoOffset.form = EFormOffset.to;
        mIfoOffset.start = Vector2.zero;
        WhenAdaptDir1UpdateOffsetIfo();
    }

    protected override void WhenStartReady4Other()
    {
        mMoveSpeedRatio = mIfoMove.speed;
    }

    protected override void WhenAdaptDir1UpdateOffsetIfo()
    {
        mIfoOffset.toEnd = mFrom.InverseTransformPoint(mTarget.position);//目标位置可能随时在变
    }

    protected override void WhenAdaptDir2UpdateDir()
    {
        mDirFit.meDirAim = mTarget.position - mMotion.position;
    }

    protected override void WhenAdaptDir3UpdateSpeedRatioOrOther()
    {
        
    }

}
