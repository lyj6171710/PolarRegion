using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EChartFull { }

public class CtrlFullChart : CtrlChart
{
    //管控占据全屏的界面显示，各个界面互斥

    bool mInUse;
    EChartFull mUseEFC;

    public ExpressGather SuUse(EChartFull sign)
    {
        if (!mInUse)
        {
            if (mCites.ContainsKey(sign))
            {
                ExpressGather gather = mCites[sign];
                gather.gameObject.SetActive(true);
                mInUse = true;
                mUseEFC = sign;
                return gather;
            }
            else return null;
        }
        else
            return null;
    }

    public void SuClose(EChartFull sign)
    {
        if (mInUse && sign == mUseEFC) 
        {
            mCites[sign].gameObject.SetActive(false);
            mInUse = false;
        }
    }

    //======================================

    Dictionary<EChartFull, ExpressGather> mCites;

    protected override void DealEachChild(GameObject child, int index)
    {
        child.gameObject.SetActive(false);
        ExpressGather gather = child.GetComponent<ExpressGather>();
        try
        {
            EChartFull corr = child.name.toEnum<EChartFull>();
            if (gather != null) mCites.Add(corr, gather);
        }
        catch (Exception ex)
        {
            
        }
        
    }

    public override void MakeReady()
    {
        mCites = new Dictionary<EChartFull, ExpressGather>();
        base.MakeReady();
    }

}
