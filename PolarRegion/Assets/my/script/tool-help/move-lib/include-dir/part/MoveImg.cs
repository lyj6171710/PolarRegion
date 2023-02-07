using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class MoveImg : MonoBehaviour
{//�ƶ������ص��ĳ���������

    //��Ҫ���̳�ʹ��
    //�������ƶ��������޸�ͼ����ײ���������

    //֧��
    protected int mId;
    public Action<int> SuWhenFinish;
    public Action SuWhenStartMove;//���ܻ���Ҫж������Ч��

    //����
    protected bool mInMove;
    protected float mMoveProgress;

    //�仯
    protected float mMoveSpeedRatio;
    protected IfoOffset mIfoOffset;

    //����
    protected Transform mMotion;//��Ȼһ������Լ�������������������ڿ�����Ҫ����չ
    protected MoveReady mReady;
    protected ToolOffsetFieldLinear mOffsetExe;
    protected ImgFitToDir mDirFit;
    protected MoveReady.Ifo mIfoMove { get { return mReady.meIfo; } }
    protected MoveReady.IfoImg mIfoImg { get { return mReady.meIfoImg; } }

    protected void Ready(MoveReady.IfoImg ifoImg, int id)//��������
    {
        mId = id;
        mMotion = transform;

        mReady = new MoveReady();
        mReady.MakeReady(mMotion.gameObject, ifoImg);

        mDirFit = ifoImg.mount.gameObject.AddComponent<ImgFitToDir>();
        mDirFit.MakeReady(ifoImg.angleSelf, EAngle.vector);

        SuWhenFinish += (idSelf) => { };
        SuWhenStartMove += () => { };

        mIfoOffset = new IfoOffset();
        //�����׼��������δȷ�����ƶ�����˭
    }

    protected void StartMove(MoveReady.Ifo ifoMove)//��������
    {
        ifoMove.dir = ifoMove.dir.normalized;
        mReady.Go(ifoMove);

        //����
        mInMove = true;
        mMoveProgress = 0;
        WhenStartReady1Progress();

        //�ƶ�
        WhenStartReady2OffsetIfo();
        mOffsetExe = new ToolOffsetFieldLinear();
        mOffsetExe.MakeReady(new ToolOffsetFieldLinear.IfoTarget(mMotion), mIfoOffset, WhatOffsetVaryTo());

        //����
        WhenStartReady3Dir();
        mReady.SuSetOpacity(1);

        //����
        WhenStartReady4Other();
        SuWhenStartMove();
    }

    protected abstract EKindFieldOffset WhatOffsetVaryTo();

    protected abstract void WhenStartReady1Progress();

    protected abstract void WhenStartReady2OffsetIfo();

    protected abstract void WhenStartReady3Dir();

    protected abstract void WhenStartReady4Other();


    //============================================

    protected abstract void WhenAdaptDir3UpdateSpeedRatioOrOther();

    protected abstract void WhenAdaptDir1UpdateOffsetIfo();

    protected abstract void WhenAdaptDir2UpdateDir();

    //�������------------------------------------

    protected void AdaptDir()
    {
        //·��
        WhenAdaptDir1UpdateOffsetIfo();
        mOffsetExe.SuResetBy(mIfoOffset);

        WhenAdaptDir2UpdateDir();//�����·���������

        WhenAdaptDir3UpdateSpeedRatioOrOther();
    }

    protected void FinishMove()
    {
        mInMove = false;
        mReady.SuSetOpacity(0);
        mDirFit.SuAutoFit(false);
        SuWhenFinish(mId);
    }
}
