using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TouchReact : MonoBehaviour, IClick
{//临时添加给那些需要触屏感应的事物上

    public TouchReact AcceptBase(int index, Action<int> deal = null)
    {
        mIndex = index;
        mClick += deal;
        return this;
    }

    public TouchReact AcceptDown(Action<int> deal)
    {
        mDown += deal;
        return this;
    }

    public TouchReact AcceptUp(Action<int> deal)
    {
        mUp += deal;
        return this;
    }

    public void OnoffReact(bool onoff)
    {
        mEnable = onoff;
    }

    //---------------------------------

    int mIndex;
    Action<int> mClick;
    Action<int> mDown;
    Action<int> mUp;
    bool mEnable;

    public void WhenHit()
    {
        if (mClick != null && mEnable) mClick(mIndex);
    }

    public void WhenDown()
    {
        if (mDown != null && mEnable) mDown(mIndex);
    }

    public void WhenUp()
    {
        if (mUp != null && mEnable) mUp(mIndex);
    }

    void Awake()
    {
        tag = CoordUse.cReactCan;//不要放在start中，初始化而已，外界有调变空间
        mEnable = true;
    }
}
