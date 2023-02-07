using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IMatrixMapPosIfo
{
    public Vector3 SuGetPosAt(Vector2Int at);

    public bool SuCanWalkTo(Vector2Int from, EToward4 to);
    //����ֵ�����ˣ���ָ��λ���ܷ�ǰ�������������ϵ���һ������ֵ��Ԥ��ֵ

    public bool SuIfNearGridCenter(Vector2 pos, out Vector2 toGridCenter, out Vector2Int nearTo);
    //�ڶ����������ǳ������ڽ���Ԫ�������ĵķ���

    public bool SuNowCanWalkTo(EToward4 to);
    //����ֵ�����ˣ��ӵ�ǰλ�ü�״̬���ܷ�ǰ�������������ϵ���һ�񣬷�����ֵ��ʵ��ֵ
}


public class ActorShiftOnMatrixCanPlan : MonoBehaviour
{
    //��֧��б���ߣ��ᰴԭ��ָ������λ���ߣ�ֱ������ָ����λ��

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
        //mCoordPlanAt == mCoordStartAt;���������жϣ���Ϊ���ܳ�����ǰ����������ͻȻ�����Ĳ���������ʱ����δ��ɼƻ�
    Vector2Int mCoordStartAt;
    Vector2Int mCoordPlanAt;

    List<Vector2Int> mWaitsMove;//0Ԫ��Ϊ��ʼλ��
   
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
        else//�����ƶ����ܣ�������
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

    int mIndexMoveTurn;//ת��

    void MoveAtTurn()
    {
        //����չ
    }

    EToward4 mDirCur;
    Vector2Int mCoordCurAt;//�ӽ���(��δ������)��һ����������ʱ���Ż����

    void WhenMoveReachNew(Vector2Int near, EToward4 from)//���ص�ǰ�ж��ƻ������ݵ�ǰʵ��������ж��Ƿ��ܰ�ԭ�ƻ���
    {
        mCoordCurAt = near;
        mDirCur = from;
        SuWhenMoveReachOneWill(mCoordCurAt);

        //��Ӧ��û�и����ƶ��б�
        if (mWaitsMove.Count == 2)
        {//�Ѿ��������ƶ������ڵ�ʱ�������Σ���ǰֻ��仯״̬����һ������
            mWaitsMove.RemoveAt(0);
        }
        else if(mWaitsMove.Count > 2)
        {
            //���������Ͻ�Ҫ�ߵ�·������һ��ʵ�ʿ���
            if (mMap.SuNowCanWalkTo((mWaitsMove[2] - mWaitsMove[1]).ToDir()))
            {
                mWaitsMove.RemoveAt(0);
            }
            else
            {
                mWaitsMove.Clear();
                mWaitsMove.Add(mCoordCurAt);
                mCoordPlanAt = mCoordCurAt;//���ܰ�ԭ�����������ƻ���
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

    //���Ķ���===============================

    Vector3 mPosLastTarget;

    void MoveMake()//���������������յ�ǰ���ƶ��б�����ƶ�
    {
        mIndexMoveTurn = MatrixMap.GetFirstTurnAt(mWaitsMove);
        if (mIndexMoveTurn == 0)//�ִ�Ԥ��Ŀ��
        {
            mCoordStartAt = mCoordPlanAt;
        }
        MoveToAt(mWaitsMove[mIndexMoveTurn]);//����ǰ������ͣ����
    }

    void MoveToAt(Vector2Int at)//����Ĳ�����һ�����������һ���ط�������������
    {
        Vector3 toPos = mMap.SuGetPosAt(at);
        if (mPosLastTarget != toPos)
        {
            mExeMove.SuMoveTo(toPos, false, false);
            mPosLastTarget = toPos;
        }
    }

    //״̬ʶ��====================================

    bool mInReach;
    
    void MoveState(Vector2 dirMove)
    {
        Vector2Int nearTo;
        Vector2 toGrid;
        if (mMap.SuIfNearGridCenter(transform.position, out toGrid, out nearTo))
        {
            if (nearTo != mCoordCurAt)//�Ȼ�ӽ�����
            {
                //����ж��ֽ�Ҫ�뿪��ǰ���ӽ��ĵ�Ԫ��
                //���ǽӽ�����ʱ����ʲô����Զ������ʱ�������ĵķ���һ����һ��?
                //�������Ǵ�ģ�Ӧ�Բ����߹�ȥ���߻��������
                //������ͨ��ʶ���Ƿ��ڵ�Ԫ���м�����ķ�ʽ���ж��Ƿ���Զ��?
                //��������Ҫ������ֱ���жϽӽ���Զ����㹻���ӽ���Զ�뻹�����Ⱥ�˳�����Ϣ
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
