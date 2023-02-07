using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EChartSingle { dialogue }

public class CtrlSingleChart : CtrlChart
{
    //����ͬʱ���ڶ����ÿһ�����

    Dictionary<EChartSingle, bool> mIsUse;

    public ExpressGather SuOpen(EChartSingle sign)
    {
        if (!mIsUse[sign])
        {
            if (mCites.ContainsKey(sign))
            {
                ExpressGather gather = mCites[sign];
                gather.gameObject.SetActive(true);
                mIsUse[sign] = true;
                return gather;
            }
            else
                return null;
        }
        else
            return null;
    }

    public ExpressGather SuGetUse(EChartSingle sign)
    {
        if (mIsUse[sign])
            return mCites[sign];
        else
            return null;
    }

    public void SuClose(EChartSingle sign)
    {
        if (mIsUse[sign])
        {
            mCites[sign].gameObject.SetActive(false);
            mIsUse[sign] = false;
        }
    }

    //======================================

    Dictionary<EChartSingle, ExpressGather> mCites;

    protected override void DealEachChild(GameObject child, int index)
    {
        child.gameObject.SetActive(false);
        ExpressGather gather = child.GetComponent<ExpressGather>();
        try
        {
            EChartSingle corr = child.name.toEnum<EChartSingle>();
            //����������ƺ�ö�ٵ�����Ӧ��һ�µģ�������������Ӧ��ĳ��ö��ֵ
            if (gather != null)
            {
                mCites.Add(corr, gather);
                mIsUse.Add(corr, false);
            }
        }
        catch (Exception ex)
        {

        }
    }

    public override void MakeReady()
    {
        mCites = new Dictionary<EChartSingle, ExpressGather>();
        mIsUse = new Dictionary<EChartSingle, bool>();
        base.MakeReady();
    }

}
