using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EKindFieldOffset { world_pos, world_rot, world_scale, local_pos, local_rot, local_scale }

public class ToolOffsetFieldLinear : LinearOffset
{//�����е���������Ա仯

    public struct IfoTarget
    {
        public Transform trans;

        public IfoTarget(Transform trans)
        {
            this.trans = trans;
        }
    }

    public void MakeReady(IfoTarget pawn, IfoOffset refer, EKindFieldOffset varyTo, bool ignoreWarn = true)
    {//�仯�����������仯�����仯����
        SetRefer(refer,ignoreWarn);

        mPawn = pawn;//��������

        mKind = varyTo;//������ʽ

        AssembleRespond();
    }

    public void SuResetBy(IfoOffset refer, bool ignoreWarn = true)
    {
        SetRefer(refer, ignoreWarn);
    }


    //===================================================

    IfoTarget mPawn;
    EKindFieldOffset mKind;

    void AssembleRespond()
    {
        if (mKind == EKindFieldOffset.world_pos)
        {
            GetFitValue = (compare) =>
            {
                if (compare) return mPawn.trans.position.x;
                else return mPawn.trans.position.y;
            };
            ApplyValue = (result) =>
            {
                Vector3 origin = mPawn.trans.position;
                mPawn.trans.position = new Vector3(result.x, result.y, origin.z);
            };
        }
        else if (mKind == EKindFieldOffset.world_rot)
        {
            GetFitValue = (compare) =>
            {
                return mPawn.trans.eulerAngles.z;
            };
            ApplyValue = (result) =>
            {
                Vector3 origin = mPawn.trans.localEulerAngles;
                if (meDrift == EDrift.horizontal)//ȷ����Ч��ά������һ��(Ĭ��ֻ����һ��ά���ڱ䣬ʹ����ת)
                    mPawn.trans.eulerAngles = new Vector3(origin.x, origin.y, result.x);
                else if (meDrift == EDrift.vertical)
                    mPawn.trans.eulerAngles = new Vector3(origin.x, origin.y, result.y);
                else if (meDrift == EDrift.none)
                    mPawn.trans.eulerAngles = new Vector3(origin.x, origin.y, result.x);
            };
        }
        else if (mKind == EKindFieldOffset.world_scale)
        {
            GetFitValue = (compare) =>
            {
                if (compare) return mPawn.trans.lossyScale.x;
                else return mPawn.trans.lossyScale.y;
            };
            ApplyValue = (result) =>
            {
                float originZ = mPawn.trans.localScale.z;

                Vector2 localBasisAcc = GbjAssist.GetSumScaleWhenParent(mPawn.trans);

                mPawn.trans.localScale = new Vector3(result.x / localBasisAcc.x, result.y / localBasisAcc.y, originZ);
            };
        }
        else if (mKind == EKindFieldOffset.local_pos)
        {
            GetFitValue = (compare) =>
            {
                if (compare) return mPawn.trans.localPosition.x;
                else return mPawn.trans.localPosition.y;
            };
            ApplyValue = (result) =>
            {
                Vector3 origin = mPawn.trans.localPosition;
                mPawn.trans.localPosition = new Vector3(result.x, result.y, origin.z);
            };
        }
        else if (mKind == EKindFieldOffset.local_rot)
        {
            GetFitValue = (compare) =>
            {
                return mPawn.trans.localEulerAngles.z;
            };
            ApplyValue = (result) =>
             {
                 Vector3 origin = mPawn.trans.localEulerAngles;
                 if (meDrift == EDrift.horizontal)
                     mPawn.trans.localEulerAngles = new Vector3(origin.x, origin.y, result.x);
                 else if (meDrift == EDrift.vertical)
                     mPawn.trans.localEulerAngles = new Vector3(origin.x, origin.y, result.y);
                 else if (meDrift == EDrift.none)
                     mPawn.trans.localEulerAngles = new Vector3(origin.x, origin.y, result.x);
             };
        }
        else if (mKind == EKindFieldOffset.local_scale)
        {
            GetFitValue = (compare) =>
            {
                if (compare) return mPawn.trans.localScale.x;
                else return mPawn.trans.localScale.y;
            };
            ApplyValue = (result) =>
            {
                Vector3 origin = mPawn.trans.localScale;
                mPawn.trans.localScale = new Vector3(result.x, result.y, origin.z);
            };
        }
        else
        {
            GetFitValue = (compare) =>
            {
                Debug.Log("��֧��ʶ��");
                return 0;
            };
            ApplyValue = (result) =>
            {
                Debug.Log("��֧��ʶ��");
            };
        }
    }
}
