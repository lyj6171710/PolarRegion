using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssistFollowTouch : AssistFollow
{//׷�津��λ�õ�Ч��

    [Range(-1, 1)] public float mOffsetX = 0f;//0ֵʱ���ᴦ�ڴ�������½�
    [Range(-1, 1)] public float mOffsetY = 0f;//x���Ϊ���ң�y���Ϊ����

    bool mJustFollow;//�Ƿ�տ�ʼ���и���
    Vector2 mStartPos;
    Vector2 mOffsetAcc;

    protected override void StartNer()
    {
        mStartPos = meSelf.localPosition;
    }

    protected override void UpdateNer()
    {
        if (mFollow)
        {
            if (mMode == Mode.point)
            {
                mNeed = CoordFrame.SuCoordInCanvasFromScreenByMeter(mPos.meCanvasLay, TouchInput.It.mePosIfOneTouch);
                mNeed += new Vector2(meSelf.sizeDelta.x / 2 + meSelf.sizeDelta.x * mOffsetX, -meSelf.sizeDelta.y / 2 + meSelf.sizeDelta.y * mOffsetY);
                if (mJustFollow)
                {
                    mJustFollow = false;
                }
            }
            else
            {
                mNeed += UnifiedCursor.It.meSlideInCanvas;
                if (mJustFollow)
                { 
                    mJustFollow = false;
                }
                mOffsetAcc += UnifiedCursor.It.meSlideInCanvas;
            }
        }
        else
        {
            mJustFollow = true;
        }
    }

    public override void BackToStart()
    {
        if (mMode == Mode.point)
        {
            mForce = true;
            mNeed = mStartPos;//�뿪��ק״̬ʱ��ֱ�ӻص�ԭ��λ��
        }
        else if(mMode == Mode.offset)
        {
            mForce = true;
            mNeed -= mOffsetAcc;
            mOffsetAcc = Vector2.zero;
        }
    }

}
