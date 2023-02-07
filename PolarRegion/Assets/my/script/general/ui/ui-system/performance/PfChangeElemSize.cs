using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PfChangeElemSize : PerformChoose
{
    //��ʱ���ӵ���Ҫ��ӦЧ����������

    public bool mMakeForth;//������չ
    public bool mMakeBack;//�������
    public bool mThenBack;//��չ����������

    public struct Ifo
    {
        public EKindUiOffset kind;//�仯;��
        public EFormOffset form;//�仯��ʽ
        public RectTransform target;//�仯����
        public Vector2 extend;//�仯��
        public float speed;//�仯�ٶ�
        public float tolerance;//���̶ȣ�ԽСԽ��׼���������
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
            Debug.Log("��֧��");
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
                    mMakeBack = false;//ֱ�����Ի�����ʱ�򣬲������������
                }
            }
        }
        else
        {
            if (mMakeBack||mThenBack)
            {
                float speedUp = 1;
                float gap = mValue.SuGetProgressToStart();
                if (gap < 0.75f) speedUp = 0.75f / gap;//���پ�׼���ص�ԭ��״̬
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
