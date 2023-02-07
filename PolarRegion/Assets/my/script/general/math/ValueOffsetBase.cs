using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct IfoOffset
{
    public Vector2 start;
    public Vector2 toEnd;
    public EFormOffset form;
}

public abstract class ValueOffsetBase
{
    //思路：任意值转换成量值，又用比值使用量值，最安全、通用

    //使用抽象类而非接口，是因为要对外隐藏成员函数

    protected abstract void SetRefer(IfoOffset need, bool ignoreWarn);//给的数据可能有问题

    //===================================================

    protected Func<bool,float> GetFitValue;//子类给出当前具体值

    protected Action<Vector2> ApplyValue;//让子类把所给具体值应用上去

    //===================================================

    public abstract bool SuWhetherCloseToEnd();

    public abstract bool SuWhetherCloseToStart();

    public abstract float SuGetProgressToStart();

    public abstract float SuGetProgressToEnd();

    public abstract void SuVaryTo(float progressAt);
}
