using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExprStrArray : ExprReferArray
{
    public GameObject[] mShowInDp;

    //============================

    Text[] mShows;
    string[] mStrsNow;

    protected override void WhenSeqElemChange(int index, object state)
    {
        mStrsNow[index] = (string)state;
        mShows[index].text = mStrsNow[index];
    }

    protected override void MakeReady()
    {
        base.MakeReady();

        if (mShowInDp.Length > mCapacityDp) return;

        mStrsNow = new string[mShowInDp.Length];
        mShows = new Text[mShowInDp.Length];
        for (int i = 0; i < mShowInDp.Length; i++)
        {
            mShows[i] = mShowInDp[i].GetComponentInChildren<Text>(true);
        }

        NuWhenEnable += () =>
        {
            for (int i = 0; i < mShowInDp.Length; i++)
            {
                WhenSeqElemChange(i, (string)SuGetElemOne(i));
            }
        };
    }
}
