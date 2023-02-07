using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LinearOffset : UnitOffset
{//�ɸ����κζ�ά���Ե����Ա仯��֧�ֶ�ά�ϵ�һά��ֵ(��һά��ֵ����)

    protected override void SetRefer(IfoOffset need, bool ignoreWarn)
    {
        base.SetRefer(need, ignoreWarn);

        //-----------------------------------------

        if (mStartPos.x == mEndPos.x)//���������ֵ�����
        {
            if (mStartPos.y == mEndPos.y)
            {
                mDrift = EDrift.none;
                if (!ignoreWarn) Debug.Log("error");//0���������������Ը�����ĸ��������޷�ʹ��
            }
            else
                mDrift = EDrift.vertical;
        }
        else if (mStartPos.y == mEndPos.y)
        {
            if (mStartPos.x == mEndPos.x)
            {
                mDrift = EDrift.none;
                if (!ignoreWarn) Debug.Log("error");
            }
            else
                mDrift = EDrift.horizontal;
        }
        else
            mDrift = EDrift.oblique;//���ܱ��ΪEDrift.obliqueʱ�������ܻ��еȣ���˲����ж�

        //-----------------------------------------

        mVaryOffset = mEndPos - mStartPos;//�����ɱ�׼ֵ��ʽ
        mXGapMore = Mathf.Abs(mVaryOffset.x) > Mathf.Abs(mVaryOffset.y);
        mVaryDir = mVaryOffset.normalized;
        mVarySpan = mVaryOffset.magnitude;
    }

    //===================================================

    public float SuRatioToSpan(float proportion)
    {
        proportion = Mathf.Clamp(proportion, 0.0f, 1.0f);
        return mVarySpan * proportion;
    }

    public float SuSpanToRatio(float span)
    {
        if (meDrift == EDrift.none) return 0;

        span = span >= 0 ? span : -span;
        return span / mVarySpan;
    }

    //===================================================

    public override float SuGetProgressToStart()
    {
        if (meDrift == EDrift.none) return 1;//��Զ�����˶˵�

        if (mXGapMore)
            //������Ҫ׼ȷһЩ��Ȼ�󻹵��ų�����һ��ά��Ϊ������(ע��������Ϊ��ʱ��������⣬����û��)
            //�ȵ��ǲ�࣬�����þ���ֵ
            return Mathf.Abs((GetFitValue(true) - mStartPos.x) / mVaryOffset.x);
            //����г̣�Ҳ�������Ǹ���
        else
            return Mathf.Abs((GetFitValue(false) - mStartPos.y) / mVaryOffset.y);
    }

    public override float SuGetProgressToEnd()
    {
        if (meDrift == EDrift.none) return 1;//��Զ�����˶˵�

        if (mXGapMore)
            return Mathf.Abs((mEndPos.x - GetFitValue(true)) / mVaryOffset.x);
        else
            return Mathf.Abs((mEndPos.y - GetFitValue(false)) / mVaryOffset.y);

    }

    public override void SuVaryTo(float progressAt)//˲�䵽��
    {
        progressAt = Mathf.Clamp(progressAt, 0.0f, 1.0f);

        Vector2 curNeed = mStartPos + progressAt * mVaryOffset;
        //���������Ҫ����ָ���ٷֱ�λ��ʱ����Ӧ�ı�׼ֵ�Ƕ���

        ApplyValue(curNeed);
    }

    //=====================================================

    EDrift mDrift;

    bool mXGapMore;//x�����ϣ��ӿ�ʼ���������������
    Vector2 mVaryOffset;
    Vector2 mVaryDir;
    float mVarySpan;

    protected EDrift meDrift { get { return mDrift; } }
}
