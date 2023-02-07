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

    public void MakeShake(int last, float speed)//前一个参数的值，意味着抖几次，一个来回为两次
    {
        mNeedShake = true;

        last = last > 0 ? last : 1;
        mLast = last;//持续长度
        speed = speed > 0.5f ? speed : 0.5f;
        mSpeed = speed;//抖动强度

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
    int mPeriod;//阶段
    bool mInDown;//当前要往下抖，还是往上

    void Update()
    {
        if (!mHaveReady) return;

        if (mNeedShake)
        {
            if (!mInShake) mInShake = true;

            if (mPeriod == 0)//开始阶段
            {
                if (mRate < 0.1f)
                    mRate += mSpeed * Time.deltaTime;
                else
                {
                    mPeriod = 1;
                    mInDown = true;
                }
            }
            else if (mPeriod == 1)//中间阶段
            {
                if (MakeShiftLoop())
                    mLast--;
                if (mLast == 0)
                    mPeriod = 2;
            }
            else if (mPeriod == 2)//结束阶段
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
        return false;//是否到达端点
    }

    void EndShake()
    {
        mPawn.SuResetPosToStart();
        mNeedShake = false;
        mInShake = false;
        if (mWhenEnd != null) mWhenEnd();
    }

}
