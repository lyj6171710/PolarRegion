using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PfMakeElemFlip : PerformChoose
{//渐进的翻转效果

    public bool meFliping { get { return mInFlip; } }

    public bool meInForth { get { return mReverse > 0 ? true : false; } }//正对着，还是背对着

    protected override void MakeReadyWhenAsPreset()
    {
        throw new System.NotImplementedException();
    }

    public void MakeFlip()
    {
        if (!mInFlip)
        {
            mNeedFlip = true;
            mMoreHalf = false;
            mRate = 1;
            mCost = mPeriod;
        }
    }

    public void MakeFlipMoment()//立即翻转完毕
    {
        mNeedFlip = true;
        mNeedMoment = true;
    }

    public void MakeReady(RectTransform backdrop, float period, bool common = true, Action<bool> whenFinish = null, Action<bool> whenHalf = null)
    {
        if (!mHaveReady)
        {
            mBackdrop = backdrop;
            mIsCommon = common;
            mPeriod = period / 2 > 0.1f ? period / 2 : 0.1f;
            mWhenFinish = whenFinish;
            mWhenHalf = whenHalf;
            mHaveReady = true;

            if (mIsCommon)
            {
                mChildrens = new GameObject[transform.childCount];
                for (int i = 0; i < transform.childCount; i++)
                    mChildrens[i] = transform.GetChild(i).gameObject;
            }
        }
    }

    //=================================

    RectTransform mBackdrop;
    float mPeriod;//从一边到中间的时间消耗，是整一个翻转时间消耗的一半
    bool mIsCommon;//是否是最普遍的情况，如果是，那么该组件可以直接负责起对子物体的改动(翻转导致隐藏)
    Action<bool> mWhenFinish;
    Action<bool> mWhenHalf;
    GameObject[] mChildrens;

    float mCost;//可耗费的时间
    bool mNeedFlip;
    float mRate;//逻辑上的，从当前情况到与其相反的情况，永远是从1到-1
    bool mMoreHalf;

    bool mNeedMoment;//是否需要瞬间完成

    int mReverse;//相对最初状态的正反情况，标记实际情况
    bool mInFlip;

    void Awake()
    {
        mReverse = 1;//先是从正向转到反向
        mHaveReady = false;
    }

    void OnDisable()
    {

    }

    void Update()
    {
        if (!mHaveReady) return;

        if (mNeedFlip)
        {
            if (mNeedMoment)
            {
                if (!mMoreHalf) HalfFlip();
                mBackdrop.localScale = new Vector3(-mReverse, 1, 1);
                EndFlip();
            }
            else
            {
                if (!mInFlip) mInFlip = true;

                mCost -= Time.deltaTime;
                mRate = mCost / mPeriod;
                mBackdrop.localScale = new Vector3(mRate * mReverse, 1, 1);

                if (!mMoreHalf && mRate <= 0)
                {
                    HalfFlip();
                    mMoreHalf = true;
                }
                else if (mMoreHalf && mRate <= -1)
                {
                    EndFlip();
                }
            }
        }
    }

    void EndFlip()
    {
        mNeedFlip = false;
        mNeedMoment = false;
        mInFlip = false;
        mReverse *= -1;//下一次是反向翻转
        if (mWhenFinish != null) mWhenFinish(mReverse > 0 ? true : false);
    }

    void HalfFlip()
    {
        if (mWhenHalf != null) mWhenHalf(mReverse > 0 ? false : true);
        //往与原本相对的方向翻转时，传入false；往原本的方向翻转时，返回true

        if (mIsCommon)
        {
            if (mReverse > 0)
            {
                foreach (GameObject child in mChildrens) child.SetActive(false);
            }
            else
            {
                foreach (GameObject child in mChildrens) child.SetActive(true);
            }
        }
    }
}
