using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IMatrixMapPosIfo
{
    public Vector3 SuGetPosAt(Vector2Int at);

    public bool SuCanWalkTo(Vector2Int from, EToward4 to);
    //返回值代表了，从指定位置能否前进到所给方向上的下一格，理论值、预测值

    public bool SuIfNearGridCenter(Vector2 pos, out Vector2 toGridCenter, out Vector2Int nearTo);
    //第二个参数，是朝向所邻近单元格子中心的方向

    public bool SuNowCanWalkTo(EToward4 to);
    //返回值代表了，从当前位置及状态，能否前进到所给方向上的下一格，非理论值、实际值
}


public class ActorShiftOnMatrixCanPlan : MonoBehaviour
{
    //不支持斜向走，会按原来指定过的位置走，直到最新指定的位置

    public Action<EToward4> SuWhenMoveToNext;
    public Action<Vector2Int> SuWhenMoveReachOneWill;
    public Action SuWhenMoveReachOne;

    public void MakeReady(IMatrixMapPosIfo mapOn)
    {
        mMap = mapOn;
        mExeMove = gameObject.AddComponent<LinearMove>();
        mWaitsMove = new List<Vector2Int>();
        mCoordCurAt = -Vector2Int.one;
    }

    public void StartMove(Vector2Int start)
    {
        mExeMove.MakeReady(transform.position, MoveAtTurn);
        mExeMove.SuWhenMoved += MoveState;
        mCoordPlanAt = start;
        mWaitsMove.Add(mCoordPlanAt);
        MoveToAt(mCoordPlanAt);
    }

    public void SuMoveBy(EToward4 to)
    {
        GoOneStepTo(to);
    }

    //==============================

    IMatrixMapPosIfo mMap;
    LinearMove mExeMove;

    public bool meIsFinishPlan => mIndexMoveTurn == 0;
        //mCoordPlanAt == mCoordStartAt;不能这样判断，因为可能出现往前走两步，又突然后退四步，退两步时并还未完成计划
    Vector2Int mCoordStartAt;
    Vector2Int mCoordPlanAt;

    List<Vector2Int> mWaitsMove;//0元素为起始位置
   
    void GoOneStepTo(EToward4 to)
    {
        if (to == EToward4.middle) return;

        if (meIsFinishPlan)
        {
            if (mMap.SuCanWalkTo(mCoordStartAt, to))
            {
                mCoordPlanAt = mCoordStartAt + OverTool.TowardToVector(to).ToInt();
                mWaitsMove.Add(mCoordPlanAt);
                MoveMake();
            }
        }
        else//快速移动功能（批处理）
        {
            if (mMap.SuCanWalkTo(mCoordPlanAt, to))
            {
                mCoordPlanAt += OverTool.TowardToVector(to).ToInt();
                mWaitsMove.Add(mCoordPlanAt);
                MoveMake();
            }
        }

        DebugDisplay.It.SuShowNewestTmpPosAt(mMap.SuGetPosAt(mCoordPlanAt));
    }

    //======================================

    int mIndexMoveTurn;//转向处

    void MoveAtTurn()
    {
        //待扩展
    }

    EToward4 mDirCur;
    Vector2Int mCoordCurAt;//接近到(还未到中心)下一块区域中心时，才会更新

    void WhenMoveReachNew(Vector2Int near, EToward4 from)//拦截当前行动计划，根据当前实际情况，判断是否还能按原计划走
    {
        mCoordCurAt = near;
        mDirCur = from;
        SuWhenMoveReachOneWill(mCoordCurAt);

        //理应还没有更新移动列表
        if (mWaitsMove.Count == 2)
        {//已经发生了移动，属于当时检测的责任，当前只需变化状态到下一个区域
            mWaitsMove.RemoveAt(0);
        }
        else if(mWaitsMove.Count > 2)
        {
            //这里是马上将要走的路，进行一次实际考察
            if (mMap.SuNowCanWalkTo((mWaitsMove[2] - mWaitsMove[1]).ToDir()))
            {
                mWaitsMove.RemoveAt(0);
            }
            else
            {
                mWaitsMove.Clear();
                mWaitsMove.Add(mCoordCurAt);
                mCoordPlanAt = mCoordCurAt;//不能按原来那样继续计划了
            }
        }
        MoveMake();

        SuWhenMoveReachOne();
    }

    void WhenMoveGoNext(EToward4 to)
    {
        mDirCur = to;
        SuWhenMoveToNext(mDirCur);
    }

    //核心动力===============================

    Vector3 mPosLastTarget;

    void MoveMake()//更新驱动力，按照当前待移动列表进行移动
    {
        mIndexMoveTurn = MatrixMap.GetFirstTurnAt(mWaitsMove);
        if (mIndexMoveTurn == 0)//抵达预期目标
        {
            mCoordStartAt = mCoordPlanAt;
        }
        MoveToAt(mWaitsMove[mIndexMoveTurn]);//继续前进或需停下来
    }

    void MoveToAt(Vector2Int at)//这里的参数不一定是相近的下一个地方，且往往不是
    {
        Vector3 toPos = mMap.SuGetPosAt(at);
        if (mPosLastTarget != toPos)
        {
            mExeMove.SuMoveTo(toPos, false, false);
            mPosLastTarget = toPos;
        }
    }

    //状态识别====================================

    bool mInReach;
    
    void MoveState(Vector2 dirMove)
    {
        Vector2Int nearTo;
        Vector2 toGrid;
        if (mMap.SuIfNearGridCenter(transform.position, out toGrid, out nearTo))
        {
            if (nearTo != mCoordCurAt)//先会接近中心
            {
                //如何判断又将要离开当前所接近的单元格？
                //考虑接近中心时，是什么方向，远离中心时，到中心的方向一定不一样?
                //这样想是错的，应对不了走过去又走回来的情况
                //那这里通过识别是否处于单元格中间区域的方式来判断是否发生远离?
                //甚至不需要，可以直接判断接近和远离就足够，接近和远离还附带先后顺序的信息
                mInReach = true;
                WhenMoveReachNew(nearTo, dirMove.ToInt().ToDir());
            }
        }
        else
        {
            if (mInReach)
            {
                WhenMoveGoNext(dirMove.ToInt().ToDir());
                mInReach = false;
            }
        }
            
    }
}
