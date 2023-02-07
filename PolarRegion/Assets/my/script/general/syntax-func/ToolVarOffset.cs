using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EKindVarOffset { Float,Vector }

public class ToolVarOffset : LinearOffset
{//�����������Եı仯

    public void SuResetBy(IfoOffset refer, bool ignoreWarn = true)
    {
        SetRefer(refer, ignoreWarn);
    }

    //=====================================

    public struct IfoTarget//���豻�仯��Ŀ�꣬����Ҫ��ʼ��
    {
        public float degree;
        public Vector2 dir;
    }

    public void MakeReady(IfoTarget handle, IfoOffset refer, EKindVarOffset varyTo, bool ignoreWarn = true)
    {//�仯�����������仯�����仯����
        SetRefer(refer, ignoreWarn);

        mHandle = handle;//��������

        mKind = varyTo;//������ʽ

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
