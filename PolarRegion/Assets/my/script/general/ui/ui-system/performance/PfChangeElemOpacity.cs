using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PfChangeElemOpacity : PerformChoose
{
    //临时增加到需要相应效果的物体上

    [HideInInspector]public bool meMakeForth;//需求执行
    [HideInInspector]public bool meMakeBack;//需求回滚

    public float speedDp;
    public IfoOffset offsetDp;

    protected override void MakeReadyWhenAsPreset()
    {
        MakeReady(GetComponent<RectTransform>(), speedDp, offsetDp, false);
    }

    public void MakeReady(RectTransform target, float speed, IfoOffset offset, bool ignoreWarn)
    {
        //ignoreWarn参数，需要外界根据它自己的情况决定

        mTarget = target;

        mSpeed = speed;

        mOffset = offset;

        ReadyAttr(ignoreWarn);

        mHaveReady = true;
    }

    public void SuForceStart()
    {
        if (mHaveReady)
        {
            mProgressCur = 0;
            foreach (ToolOffsetUiLinear offset in mValues)
            {
                offset.SuVaryTo(mProgressCur);
            }
        }
    }

    public void SuResetRefer(IfoOffset refer)
    {
        foreach (ToolOffsetUiLinear offset in mValues)
        {
            offset.SuResetBy(refer);
        }
    }

    public void SuNewCover(bool ignoreWarn)
    {
        CollectCover(ignoreWarn);
    }

    //=================================================

    RectTransform mTarget;
    Text[] mTexts;
    Image[] mImages;
    ToolOffsetUiLinear[] mValues;

    float mSpeed;
    IfoOffset mOffset;

    bool mInForth;
    float mProgressCur;

    void ReadyAttr(bool ignoreWarn)
    {
        mProgressCur = 0;

        CollectCover(ignoreWarn);

        mInForth = true;
        meMakeForth = false;
        meMakeBack = false;
    }

    void CollectCover(bool ignoreWarn)
    {
        mTexts = mTarget.GetComponentsInChildren<Text>();
        mImages = mTarget.GetComponentsInChildren<Image>();
        int TextNum = mTexts != null ? mTexts.Length : 0;
        int ImageNum = mImages != null ? mImages.Length : 0;

        if (mValues != null)
        {
            for (int i = 0; i < mValues.Length; i++) 
            {
                mValues[i] = null;
            }
        }
        mValues = new ToolOffsetUiLinear[TextNum + ImageNum];

        IfoOffset tmpOffset = mOffset;
        for (int i = 0; i < TextNum; i++)
        {
            float OriginAlpha = mTexts[i].color.a;
            mValues[i] = new ToolOffsetUiLinear();
            tmpOffset.start = new Vector2(OriginAlpha, 0);
            //默认是从原有值到指定值之间的变化，但这两个值一开始就重合时会出问题，如果可能会重合，外界需要调用该组件其它接口来修正
            //一般来说原有值就是1，所以不给这个offset参数，也是从1到0变
            mValues[i].MakeReady(new ToolOffsetUiLinear.InfoTarget(null, mTexts[i]), tmpOffset, EKindUiOffset.opacity, ignoreWarn);
            mValues[i].SuVaryTo(mProgressCur);
        }

        for (int i = 0; i < ImageNum; i++)
        {
            float OriginAlpha = mImages[i].color.a;
            mValues[TextNum + i] = new ToolOffsetUiLinear();
            tmpOffset.start = new Vector2(OriginAlpha, 0);
            mValues[TextNum + i].MakeReady(new ToolOffsetUiLinear.InfoTarget(null, null, mImages[i]), tmpOffset, EKindUiOffset.opacity, ignoreWarn);
            mValues[TextNum + i].SuVaryTo(mProgressCur);
        }
    }

    void OnDisable()
    {
        VarySizeTo(0);
        mInForth = true;
    }

    void Update()
    {
        if (!mHaveReady) return;

        if (mInForth)
        {
            if (meMakeForth)
            {
                float speedUp = 1;
                float gap = mValues[0].SuGetProgressToEnd();
                if (gap < 0.5f) speedUp = 0.5f / gap;
                VarySizeTo(1, speedUp);
                if (mValues[0].SuWhetherCloseToEnd())
                {
                    mInForth = false;
                    meMakeForth = false;
                }
                else
                {
                    meMakeBack = false;//直到可以回缩的时候，才允许回缩操作
                }
            }
        }
        else
        {
            if (meMakeBack)
            {
                float speedUp = 1;
                float gap = mValues[0].SuGetProgressToStart();
                if (gap < 0.75f) speedUp = 0.75f / gap;//加速精准化回到原来状态
                VarySizeTo(0, speedUp);

                if (mValues[0].SuWhetherCloseToStart())
                {
                    mInForth = true;
                    meMakeBack = false;
                }
                else
                    meMakeForth = false;
            }
        }
    }

    void VarySizeTo(float toProgress, float speedUp = 1)
    {
        mProgressCur = Mathf.Lerp(mProgressCur, toProgress, Time.deltaTime * mSpeed * speedUp);
        foreach (ToolOffsetUiLinear offsets in mValues)
        {
            offsets.SuVaryTo(mProgressCur);
        }
    }
}
