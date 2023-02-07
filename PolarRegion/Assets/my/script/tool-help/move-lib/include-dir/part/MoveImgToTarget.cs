using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class MoveImgToTarget : MoveImg
{//�����ϣ���һ�ֱض����ƶ���Ŀ���һ����ʽ��������ֻ�Ǳ������ٶȵĲ�ͬ
 
    //����
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
