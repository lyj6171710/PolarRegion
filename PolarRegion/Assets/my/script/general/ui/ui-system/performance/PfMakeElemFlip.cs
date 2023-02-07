using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PfMakeElemFlip : PerformChoose
{//�����ķ�תЧ��

    public bool meFliping { get { return mInFlip; } }

    public bool meInForth { get { return mReverse > 0 ? true : false; } }//�����ţ����Ǳ�����

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

    public void MakeFlipMoment()//������ת���
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
    float mPeriod;//��һ�ߵ��м��ʱ�����ģ�����һ����תʱ�����ĵ�һ��
    bool mIsCommon;//�Ƿ������ձ�����������ǣ���ô���������ֱ�Ӹ������������ĸĶ�(��ת��������)
    Action<bool> mWhenFinish;
    Action<bool> mWhenHalf;
    GameObject[] mChildrens;

    float mCost;//�ɺķѵ�ʱ��
    bool mNeedFlip;
    float mRate;//�߼��ϵģ��ӵ�ǰ����������෴���������Զ�Ǵ�1��-1
    bool mMoreHalf;

    bool mNeedMoment;//�Ƿ���Ҫ˲�����

    int mReverse;//������״̬��������������ʵ�����
    bool mInFlip;

    void Awake()
    {
        mReverse = 1;//���Ǵ�����ת������
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
        mReverse *= -1;//��һ���Ƿ���ת
        if (mWhenFinish != null) mWhenFinish(mReverse > 0 ? true : false);
    }

    void HalfFlip()
    {
        if (mWhenHalf != null) mWhenHalf(mReverse > 0 ? false : true);
        //����ԭ����Եķ���תʱ������false����ԭ���ķ���תʱ������true

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
