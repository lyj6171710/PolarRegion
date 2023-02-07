using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ExprReferArray : ExpressAble
{
    public enum EMsg { GetElem }

    public int mCapacityDp = 5;

    public object SuGetElemOne(int index)
    {
        return mSeq[index];
    }

    public void SuSetElemOne(int index, object value)
    {
        if (index < mCapacityDp)
        {
            if (mSeq[index] != value)
            {
                mSeq[index] = value;
                WhenSeqElemChange(index, value);
            }
        }
        else
            Debug.Log("³¬³öÈÝÁ¿");
    }

    //===============================

    object[] mSeq;

    protected abstract void WhenSeqElemChange(int index, object value);

    protected override void MakeReady()
    {
        base.MakeReady();
        mSeq = new object[mCapacityDp];
    }
}
