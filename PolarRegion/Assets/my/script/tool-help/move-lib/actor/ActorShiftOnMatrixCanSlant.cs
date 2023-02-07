using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorShiftOnMatrixCanSlant : MonoBehaviour
{
    //֧���ڷ����ͼ�ϣ�������������
    //������ǰָ��Ŀ�꣬��ô��ɫ����ֱ��Ŀ��λ���ߣ���ʱ����б��

    IMatrixMapPosIfo mMap;

    //==============================

    bool mInReach => mMove.meInReach;
    Vector2Int mCurAt;
    Vector2Int mTargetAt;
    LinearMove mMove;

    public void MakeReady(IMatrixMapPosIfo mapOn)
    {
        mMap = mapOn;
        mMove = gameObject.AddComponent<LinearMove>();
    }

    public void SuMoveBy(EToward4 to)
    {
        GoOneStepTo(to);
    }

    public void SuStartMove()
    {
        mCurAt = Vector2Int.zero;
        mMove.MakeReady(mMap.SuGetPosAt(mCurAt), () => { mCurAt = mTargetAt; });
        mMove.SuMoveTo(mMap.SuGetPosAt(mCurAt));
    }

    void GoOneStepTo(EToward4 to)
    {
        if (mInReach)
        {
            if (mMap.SuCanWalkTo(mCurAt, to))
            {
                Vector2Int aimAt = mCurAt + OverTool.TowardToVector(to).ToInt();
                MoveTo(aimAt);
            }
        }
        else//�����ƶ�����(֧�����������ߣ���������ٷ������ߣ���ô��ɫ�����ߵ���һ���Ҳ�λ�þ�ͣ����������ͬ��)
        {
            if (mMap.SuCanWalkTo(mTargetAt, to))
            {
                Vector2Int aimAt = mTargetAt + OverTool.TowardToVector(to).ToInt();
                MoveTo(aimAt);
            }
        }
    }

    void MoveTo(Vector2Int where)
    {
        mTargetAt = where;
        Vector3 targetPos = mMap.SuGetPosAt(mTargetAt);
        mMove.SuMoveTo(targetPos, false, false);
    }

    //===============================

}
