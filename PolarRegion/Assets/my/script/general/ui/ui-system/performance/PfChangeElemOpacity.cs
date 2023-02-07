using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PfChangeElemOpacity : PerformChoose
{
    //��ʱ���ӵ���Ҫ��ӦЧ����������

    [HideInInspector]public bool meMakeForth;//����ִ��
    [HideInInspector]public bool meMakeBack;//����ع�

    public float speedDp;
    public IfoOffset offsetDp;

    protected override void MakeReadyWhenAsPreset()
    {
        MakeReady(GetComponent<RectTransform>(), speedDp, offsetDp, false);
    }

    public void MakeReady(RectTransform target, float speed, IfoOffset offset, bool ignoreWarn)
    {
        //ignoreWarn��������Ҫ���������Լ����������

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
            //Ĭ���Ǵ�ԭ��ֵ��ָ��ֵ֮��ı仯����������ֵһ��ʼ���غ�ʱ������⣬������ܻ��غϣ������Ҫ���ø���������ӿ�������
            //һ����˵ԭ��ֵ����1�����Բ������offset������Ҳ�Ǵ�1��0��
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
                    meMakeBack = false;//ֱ�����Ի�����ʱ�򣬲������������
                }
            }
        }
        else
        {
            if (meMakeBack)
            {
                float speedUp = 1;
                float gap = mValues[0].SuGetProgressToStart();
                if (gap < 0.75f) speedUp = 0.75f / gap;//���پ�׼���ص�ԭ��״̬
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
