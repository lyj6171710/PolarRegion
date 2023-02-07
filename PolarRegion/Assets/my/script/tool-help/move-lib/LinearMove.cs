using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LinearMove : MonoBehaviour
{
    public bool meInReach => mInReach;

    public Action<Vector2> SuWhenMoved;

    //=================================

    Vector2 mStartPos;
    Vector2 mTargetPos;
    Vector2 mDirMove;
    float mProgress;
    bool mInReach;

    Action mWhenReach;
    IfoOffset mIfoOffset;
    ToolOffsetFieldLinear mExeOffset;

    bool mHaveReady;

    void Update()
    {
        if (!mHaveReady) return;

        if (!mInReach)
        {
            mProgress += Time.deltaTime;
            mExeOffset.SuVaryTo(mProgress);
            SuWhenMoved(mDirMove);
            if (mProgress >= 1)
            {
                mExeOffset.SuVaryTo(1);
                mInReach = true;
                mWhenReach();
            }
        }
    }

    public void MakeReady(Vector3 start, Action whenReach)
    {
        mStartPos = start;
        mTargetPos = mStartPos;
        mProgress = 1;
        mWhenReach += () => { };
        mWhenReach += whenReach;
        SuWhenMoved += (dir) => { };

        mIfoOffset = new IfoOffset();
        mIfoOffset.start = mStartPos;
        mIfoOffset.form = EFormOffset.to;
        mExeOffset = new ToolOffsetFieldLinear();
        mExeOffset.MakeReady(new ToolOffsetFieldLinear.IfoTarget(transform), mIfoOffset, EKindFieldOffset.world_pos);

        mInReach = true;
        mHaveReady = true;
    }

    public void SuMoveTo(Vector3 to, bool keepLastStart = false, bool keepLastProgress = false)
    {
        mTargetPos = to;
        if (!keepLastStart)
        {
            mStartPos = transform.position;
        }
        if (!keepLastProgress)
        {
            mProgress = 0;
        }

        mIfoOffset.start = mStartPos;
        mIfoOffset.toEnd = mTargetPos;
        mExeOffset.SuResetBy(mIfoOffset);

        mDirMove = (mTargetPos - mStartPos).normalized;
        mInReach = false;
    }

}
