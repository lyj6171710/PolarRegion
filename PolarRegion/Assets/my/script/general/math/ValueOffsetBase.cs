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
    //˼·������ֵת������ֵ�����ñ�ֵʹ����ֵ���ȫ��ͨ��

    //ʹ�ó�������ǽӿڣ�����ΪҪ�������س�Ա����

    protected abstract void SetRefer(IfoOffset need, bool ignoreWarn);//�������ݿ���������

    //===================================================

    protected Func<bool,float> GetFitValue;//���������ǰ����ֵ

    protected Action<Vector2> ApplyValue;//���������������ֵӦ����ȥ

    //===================================================

    public abstract bool SuWhetherCloseToEnd();

    public abstract bool SuWhetherCloseToStart();

    public abstract float SuGetProgressToStart();

    public abstract float SuGetProgressToEnd();

    public abstract void SuVaryTo(float progressAt);
}
