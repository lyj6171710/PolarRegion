using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class MoveImgToTarget : MoveImg
{//功能上，是一种必定会移动到目标的一种形式，各子类只是表现与速度的不同
 
    //属性
    protected Transform mTarget;

    protected void StartMove(MoveReady.Ifo ifo, Transform target)
    {
        mTarget = target;

        StartMove(ifo);
    }

    //===================================

    protected override void WhenStartReady3Dir()
    {
        mDirFit.meDirAim = mTarget.position - mMotion.position;
        mDirFit.SuFitInstantly();
        mDirFit.SuAutoFit(true);
    }
}
