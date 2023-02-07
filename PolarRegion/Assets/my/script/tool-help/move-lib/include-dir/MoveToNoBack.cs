using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToNoBack : MoveImgToTarget
{
    //ֻ������û�лأ�ÿ���ƶ�����̶�

    public void MakeReady(MoveReady.IfoImg ifoImg, int id)
    {
        Ready(ifoImg, id);
    }

    public void SuStartMove(MoveReady.Ifo ifo, Transform target)
    {
        StartMove(ifo, target);
    }

    void Update()
    {
        if (mInMove)
        {
            if (mOffsetExe.SuWhetherCloseToEnd())
            {
                mInMove = false;
                FinishMove();
            }
            else
            {
                AdaptDir();
                mMoveProgress += mMoveSpeedRatio * Time.deltaTime;
                mOffsetExe.SuVaryTo(mMoveProgress);
            }
        }
    }

    float UpdateSpeedRatio()
    {
        return mOffsetExe.SuSpanToRatio(mIfoMove.speed);
    }

    //==========================

    protected override EKindFieldOffset WhatOffsetVaryTo()
    {
        return EKindFieldOffset.world_pos;//����������Ӱ��ʱ��Ӧ�ò�����������
    }

    protected override void WhenStartReady1Progress()
    {
    }

    protected override void WhenStartReady2OffsetIfo()
    {
        mMotion.localPosition = Vector3.zero;
        WhenAdaptDir1UpdateOffsetIfo();
    }

    protected override void WhenStartReady4Other()
    {
        UpdateSpeedRatio();
    }

    protected override void WhenAdaptDir1UpdateOffsetIfo()
    {
        mIfoOffset.form = EFormOffset.to;
        mIfoOffset.start = mMotion.position;
        mIfoOffset.toEnd = mTarget.position;
    }

    protected override void WhenAdaptDir2UpdateDir()
    {
        mDirFit.meDirAim = mTarget.position - mMotion.position;
    }

    protected override void WhenAdaptDir3UpdateSpeedRatioOrOther()
    {
        mMoveSpeedRatio = UpdateSpeedRatio();
    }

}
