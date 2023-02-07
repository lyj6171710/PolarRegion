using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EChartAlone { hp, selector }

public class CtrlAloneChart : CtrlChart
{
    //可能同时存在多个的每一种面板

    List<GameObject> mCopyList;

    public GameObject SuUse(EChartAlone sign)
    {
        if (mCites.ContainsKey(sign))
        {
            GameObject copy = Instantiate(mCites[sign]);
            copy.transform.SetParent(transform);
            copy.transform.localPosition = Vector3.zero;
            copy.gameObject.SetActive(true);
            mCopyList.Add(copy);
            return copy;
        }
        else
        {
            return null;
        }
    }

    //=================================

    Dictionary<EChartAlone, GameObject> mCites;

    public override void MakeReady()
    {
        mCites = new Dictionary<EChartAlone, GameObject>();
        mCopyList = new List<GameObject>();
        base.MakeReady();
    }

    protected override void DealEachChild(GameObject child, int index)
    {
        child.gameObject.SetActive(false);
        GameObject prefab = child.GetComponent<PrefabCite>().SelfTemplate;
        try
        {
            EChartAlone corr =  child.name.toEnum<EChartAlone>();
            if (prefab != null) { mCites.Add(corr, prefab); }
        }
        catch (Exception ex)
        {
        }
    }
}
