using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignOtherOneRot : MonoBehaviour,IDirGet
{//������ķ�������һ������ķ���һ��

    ImgFitToDir mDir;
    ImgFitToDir mDirOther;
    float mAngleSinceRight;

    public float meAngleNow { get { return mDir.meAngleAim; } }

    public void MakeReady(float angleSinceRight ,ImgFitToDir other)
    {
        mAngleSinceRight = angleSinceRight;
        mDirOther = other;

        mDir = gameObject.AddComponent<ImgFitToDir>();
        mDir.MakeReady(mAngleSinceRight, EAngle.degree);
    }

    public void SuAlignInstantly()
    {
        mDir.meAngleAim = mDirOther.meAngleNow;
        mDir.SuFitInstantly();
    }

    void Update()
    {
        SuAlignInstantly();
    }
}
