using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveImgByDir : MoveImg
{
    //===================================

    protected override void WhenStartReady3Dir()
    {
        mDirFit.meDirAim = mIfoMove.dir;
        mDirFit.SuFitInstantly();
    }
}
