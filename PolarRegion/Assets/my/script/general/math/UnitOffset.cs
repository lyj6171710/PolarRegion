using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitOffset : ValueOffsetBase
{//可辅助任何二维属性从一端到另一端的变化，不支持三维属性

    protected Vector2 mStartPos;//pos仅是一种比喻
    protected Vector2 mEndPos;

    protected override void SetRefer(IfoOffset need, bool ignoreWarn)
    {
        mStartPos = need.start;//开始位置不一定就在原地，需要外界给

        Vector2 end;
        switch (need.form)
        {
            case EFormOffset.percent: end = mStartPos * need.toEnd; break;
            case EFormOffset.fix: end = mStartPos + need.toEnd; break;
            case EFormOffset.to: end = need.toEnd; break;
            default: end = Vector2.zero; break;//其实不可能会执行到这
        }
        mEndPos = end;//结束位置随需求或方式的不同而不同，也需要外界给

    }

    //===================================================

    public Vector2 SuGetAt()
    {
        float x = GetFitValue(true);
        float y = GetFitValue(false);
        return new Vector2(x, y);
    }

    //===================================================

    public override bool SuWhetherCloseToEnd()//转换成标准值后，又都用百分比处理，会很通用
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
        if (SuGetProgressToStart() < 0.01f)//还原时，需要精准一些
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
