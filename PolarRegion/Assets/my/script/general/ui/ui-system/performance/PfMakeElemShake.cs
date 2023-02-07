using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PfMakeElemShake : PerformChoose
{
    public bool meShaking { get { return mInShake; } }

    protected override void MakeReadyWhenAsPreset()
    {
        throw new System.NotImplementedException();
    }

    public void MakeShake(int last, float speed)//ǰһ��������ֵ����ζ�Ŷ����Σ�һ������Ϊ����
    {
        mNeedShake = true;

        last = last > 0 ? last : 1;
        mLast = last;//��������
        speed = speed > 0.5f ? speed : 0.5f;
        mSpeed = speed;//����ǿ��

        mPawn.SuSetStartRefer(mTarget.localPosition);
        mDir = new Vector2(1, 1).normalized;
        mRate = 0;
        mPeriod = 0;
        mInDown = false;
    }

    public void MakeReady(RectTransform target, Action whenEnd = null)
    {
        if (!mHaveReady)
        {
            mTarget = target;
            mWhenEnd = whenEnd;

            mPawn = mTarget.GetComponent<AssistPos>();
            if (mPawn == null) mPawn = mTarget.gameObject.AddComponent<AssistPos>();
            mPawn.MakeReady();
            mHaveReady = true;
        }
    }

    //=======================

    RectTransform mTarget;
    Action mWhenEnd;

    bool mNeedShake;
    int mLast;
    float mSpeed;

    bool mInShake;
    AssistPos mPawn;

    float mRate;
    Vector2 mDir;
    int mPeriod;//�׶�
    bool mInDown;//��ǰҪ���¶�����������

    void Update()
    {
        if (!mHaveReady) return;

        if (mNeedShake)
        {
            if (!mInShake) mInShake = true;

            if (mPeriod == 0)//��ʼ�׶�
            {
                if (mRate < 0.1f)
                    mRate += mSpeed * Time.deltaTime;
                else
                {
                    mPeriod = 1;
                    mInDown = true;
                }
            }
            else if (mPeriod == 1)//�м�׶�
            {
                if (MakeShiftLoop())
                    mLast--;
                if (mLast == 0)
                    mPeriod = 2;
            }
            else if (mPeriod == 2)//�����׶�
            {
                if (mInDown)
                {
                    mRate -= mSpeed * Time.deltaTime;
                    if (mRate < 0)
                    {
                        EndShake();
                        return;
                    }
                }
                else
                {
                    mRate +=  mSpeed * Time.deltaTime;
                    if (mRate > 0)
                    {
                        EndShake();
                        return;
                    }
                }
            }

            mPawn.SuMakeMoveFromStartByPercent(mDir * mRate);
        }
    }

    bool MakeShiftLoop()
    {
        if (mInDown)
        {
            if (mRate > -0.1f)
                mRate -= mSpeed * Time.deltaTime;
            else
            {
                mInDown = false;
                return true;
            }
        }
        else
        {
            if (mRate < 0.1f)
                mRate += mSpeed * Time.deltaTime;
            else
            {
                mInDown = true;
                return true;
            }
        }
        return false;//�Ƿ񵽴�˵�
    }

    void EndShake()
    {
        mPawn.SuResetPosToStart();
        mNeedShake = false;
        mInShake = false;
        if (mWhenEnd != null) mWhenEnd();
    }

}
