using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PfChangeElemSize : PerformChoose
{
    //临时增加到需要相应效果的物体上

    public bool mMakeForth;//需求扩展
    public bool mMakeBack;//需求回缩
    public bool mThenBack;//扩展后立即回缩

    public struct Ifo
    {
        public EKindUiOffset kind;//变化途径
        public EFormOffset form;//变化方式
        public RectTransform target;//变化对象
        public Vector2 extend;//变化量
        public float speed;//变化速度
        public float tolerance;//容忍度，越小越精准到外界所需
    }

    protected override void MakeReadyWhenAsPreset()
    {
        throw new System.NotImplementedException();
    }

    public void MakeReady(Ifo refer)
    {
        if (refer.tolerance < 0.02f)
            refer.tolerance = 0.02f;
        mInfo = refer;

        ReadyAttr();

        mHaveReady = true;
    }

    public void InstantToOrigin()
    {
        VarySizeTo(0, -1f);
        mInForth = true;
    }

    //=================================================

    Ifo mInfo;

    ToolOffsetUiLinear mValue;

    bool mInForth;

    float mProgressCur;

    void ReadyAttr()
    {
        Vector2 OriginSize = Vector2.zero;
        if (mInfo.kind == EKindUiOffset.rect)
        {
            OriginSize = new Vector2(mInfo.target.rect.width, mInfo.target.rect.height);
        }
        else if (mInfo.kind == EKindUiOffset.scale)
        {
            OriginSize = mInfo.target.localScale;
        }
        else
        {
            Debug.Log("不支持");
        }

        mValue = new ToolOffsetUiLinear();
        IfoOffset offset = new IfoOffset();
        offset.start = OriginSize;
        offset.toEnd = mInfo.extend;
        offset.form = mInfo.form;
        mValue.MakeReady(new ToolOffsetUiLinear.InfoTarget(mInfo.target), offset, mInfo.kind);
        mProgressCur = 0;
        mValue.SuVaryTo(mProgressCur);
        
        mInForth = true;
        mMakeForth = false;
        mThenBack = false;
        mMakeBack = false;
    }

    void OnDisable()
    {
        InstantToOrigin();
    }

    void Update()
    {
        if (!mHaveReady) return;

        if (mInForth)
        {
            if (mMakeForth)
            {
                VarySizeTo(1);

                if (mValue.SuGetProgressToEnd() < mInfo.tolerance) 
                {
                    mInForth = false;
                    mMakeForth = false;
                }
                else
                {
                    mMakeBack = false;//直到可以回缩的时候，才允许回缩操作
                }
            }
        }
        else
        {
            if (mMakeBack||mThenBack)
            {
                float speedUp = 1;
                float gap = mValue.SuGetProgressToStart();
                if (gap < 0.75f) speedUp = 0.75f / gap;//加速精准化回到原来状态
                VarySizeTo(0, speedUp);

                if (mValue.SuWhetherCloseToStart())
                {
                    mInForth = true;
                    mMakeBack = false;
                    mThenBack = false;
                }
                else
                    mMakeForth = false;
            }
        }
    }

    void VarySizeTo(float toProgress, float speedUp = 1)
    {
        if (speedUp >= 0)
            mProgressCur = Mathf.Lerp(mProgressCur, toProgress, Time.deltaTime * mInfo.speed * speedUp);
        else
            mProgressCur = toProgress;
        mValue.SuVaryTo(mProgressCur);
    }
}
