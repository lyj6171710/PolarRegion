using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LinearOffset : UnitOffset
{//可辅助任何二维属性的线性变化，支持二维上的一维数值(另一维数值不变)

    protected override void SetRefer(IfoOffset need, bool ignoreWarn)
    {
        base.SetRefer(need, ignoreWarn);

        //-----------------------------------------

        if (mStartPos.x == mEndPos.x)//检测所给数值的情况
        {
            if (mStartPos.y == mEndPos.y)
            {
                mDrift = EDrift.none;
                if (!ignoreWarn) Debug.Log("error");//0不能作除数，所以该组件的辅助功能无法使用
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
            mDrift = EDrift.oblique;//当能标记为EDrift.oblique时，不可能会有等，因此不用判断

        //-----------------------------------------

        mVaryOffset = mEndPos - mStartPos;//都看成标准值形式
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
        if (meDrift == EDrift.none) return 1;//永远到不了端点

        if (mXGapMore)
            //差异大的要准确一些，然后还得排除其中一个维度为零的情况(注意两个都为零时，会出问题，这里没管)
            //比的是差距，所以用绝对值
            return Mathf.Abs((GetFitValue(true) - mStartPos.x) / mVaryOffset.x);
            //相对行程，也不可能是负数
        else
            return Mathf.Abs((GetFitValue(false) - mStartPos.y) / mVaryOffset.y);
    }

    public override float SuGetProgressToEnd()
    {
        if (meDrift == EDrift.none) return 1;//永远到不了端点

        if (mXGapMore)
            return Mathf.Abs((mEndPos.x - GetFitValue(true)) / mVaryOffset.x);
        else
            return Mathf.Abs((mEndPos.y - GetFitValue(false)) / mVaryOffset.y);

    }

    public override void SuVaryTo(float progressAt)//瞬间到达
    {
        progressAt = Mathf.Clamp(progressAt, 0.0f, 1.0f);

        Vector2 curNeed = mStartPos + progressAt * mVaryOffset;
        //这里给出想要到达指定百分比位置时，相应的标准值是多少

        ApplyValue(curNeed);
    }

    //=====================================================

    EDrift mDrift;

    bool mXGapMore;//x方向上，从开始到结束，距离更长
    Vector2 mVaryOffset;
    Vector2 mVaryDir;
    float mVarySpan;

    protected EDrift meDrift { get { return mDrift; } }
}
