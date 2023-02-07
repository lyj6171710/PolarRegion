using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EKindVarOffset { Float,Vector }

public class ToolVarOffset : LinearOffset
{//基础类型属性的变化

    public void SuResetBy(IfoOffset refer, bool ignoreWarn = true)
    {
        SetRefer(refer, ignoreWarn);
    }

    //=====================================

    public struct IfoTarget//给予被变化的目标，不需要初始化
    {
        public float degree;
        public Vector2 dir;
    }

    public void MakeReady(IfoTarget handle, IfoOffset refer, EKindVarOffset varyTo, bool ignoreWarn = true)
    {//变化对象所属，变化量，变化对象
        SetRefer(refer, ignoreWarn);

        mHandle = handle;//操作对象

        mKind = varyTo;//操作方式

        AssembleRespond();
    }

    //===================================================

    IfoTarget mHandle;
    EKindVarOffset mKind;

    void AssembleRespond()
    {
        if (mKind == EKindVarOffset.Float)
        {
            GetFitValue = (compare) =>
            {
                return mHandle.degree;
            };
            ApplyValue = (result) =>
            {
                mHandle.degree = result.x;
            };
        }
        else if (mKind == EKindVarOffset.Vector)
        {
            GetFitValue = (compare) =>
            {
                if (compare) return mHandle.dir.x;
                else return mHandle.dir.y;
            };
            ApplyValue = (result) =>
            {
                mHandle.dir = result;
            };
        }
    }

}
