using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorShiftOnMatrixCanSlant : MonoBehaviour
{
    //支持在方阵地图上，上下左右行走
    //可以提前指定目标，那么角色将径直往目标位置走，此时允许斜向

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
        else//快速移动功能(支持连续往右走，如果连续促发往右走，那么角色不会走到第一个右侧位置就停下来，其它同理)
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
