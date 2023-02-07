using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitOffset : ValueOffsetBase
{//�ɸ����κζ�ά���Դ�һ�˵���һ�˵ı仯����֧����ά����

    protected Vector2 mStartPos;//pos����һ�ֱ���
    protected Vector2 mEndPos;

    protected override void SetRefer(IfoOffset need, bool ignoreWarn)
    {
        mStartPos = need.start;//��ʼλ�ò�һ������ԭ�أ���Ҫ����

        Vector2 end;
        switch (need.form)
        {
            case EFormOffset.percent: end = mStartPos * need.toEnd; break;
            case EFormOffset.fix: end = mStartPos + need.toEnd; break;
            case EFormOffset.to: end = need.toEnd; break;
            default: end = Vector2.zero; break;//��ʵ�����ܻ�ִ�е���
        }
        mEndPos = end;//����λ���������ʽ�Ĳ�ͬ����ͬ��Ҳ��Ҫ����

    }

    //===================================================

    public Vector2 SuGetAt()
    {
        float x = GetFitValue(true);
        float y = GetFitValue(false);
        return new Vector2(x, y);
    }

    //===================================================

    public override bool SuWhetherCloseToEnd()//ת���ɱ�׼ֵ���ֶ��ðٷֱȴ������ͨ��
    {
        if (SuGetProgressToEnd() < 0.02f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool SuWhetherCloseToStart()
    {
        if (SuGetProgressToStart() < 0.01f)//��ԭʱ����Ҫ��׼һЩ
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
