using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class MoveImg : MonoBehaviour
{//移动所挂载到的场景物体上

    //需要被继承使用
    //负责了移动，可能修改图像、碰撞组件的属性

    //支持
    protected int mId;
    public Action<int> SuWhenFinish;
    public Action SuWhenStartMove;//可能会需要卸载其它效果

    //进程
    protected bool mInMove;
    protected float mMoveProgress;

    //变化
    protected float mMoveSpeedRatio;
    protected IfoOffset mIfoOffset;

    //基础
    protected Transform mMotion;//虽然一般就是自己，但是设置这个有利于可能需要的扩展
    protected MoveReady mReady;
    protected ToolOffsetFieldLinear mOffsetExe;
    protected ImgFitToDir mDirFit;
    protected MoveReady.Ifo mIfoMove { get { return mReady.meIfo; } }
    protected MoveReady.IfoImg mIfoImg { get { return mReady.meIfoImg; } }

    protected void Ready(MoveReady.IfoImg ifoImg, int id)//子类重载
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
        //这里的准备，并还未确定被移动者是谁
    }

    protected void StartMove(MoveReady.Ifo ifoMove)//子类重载
    {
        ifoMove.dir = ifoMove.dir.normalized;
        mReady.Go(ifoMove);

        //进程
        mInMove = true;
        mMoveProgress = 0;
        WhenStartReady1Progress();

        //移动
        WhenStartReady2OffsetIfo();
        mOffsetExe = new ToolOffsetFieldLinear();
        mOffsetExe.MakeReady(new ToolOffsetFieldLinear.IfoTarget(mMotion), mIfoOffset, WhatOffsetVaryTo());

        //表现
        WhenStartReady3Dir();
        mReady.SuSetOpacity(1);

        //其它
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

    //子类可用------------------------------------

    protected void AdaptDir()
    {
        //路程
        WhenAdaptDir1UpdateOffsetIfo();
        mOffsetExe.SuResetBy(mIfoOffset);

        WhenAdaptDir2UpdateDir();//方向和路程有相关性

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
