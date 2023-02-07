using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveByTrackPreset : MonoBehaviour
{
    //传入朝正右方移动时的轨迹
    //预置的物体，都是当朝正右边开始移动时，图像内容的正向，相对场景右方的状态
    //图像会先被旋转，旋转到其内容的正向，朝正右边，
    //然后再做一次旋转，适应向右开始移动时的旋转，然后再做旋转，适应向任意方向开始移动时的旋转

    public struct Ifo
    {
        public Transform posApply;
        public Transform rotApply;
        public Transform scaleApply;
    }

    [Space(10)]
    public EMotion mMotionDp;//与特定轨迹对应的移动形式
    public int mTurnAtDp;

    public Action SuWhenFurthest;
    public Action SuWhenFinish;

    bool haveReady = false;

    List<IfoTransform> mPointsReferTrack;//朝右时的标准
    List<IfoTransform> mPointsCurTrack;

    ToolOffsetFieldLinear mOffsetPosExe;
    IfoOffset mIfoOffsetPos;
    Func<Vector3> mPosCurParent;

    ToolVarOffset mOffsetRotExe;
    IfoOffset mIfoOffsetRot;

    ToolOffsetFieldLinear mOffsetScaleExe;
    IfoOffset mIfoOffsetScale;

    ImgFitToDir mFitDir;

    MoveReady.Ifo mIfoMove;
    Ifo mIfoTarget;

    bool mInMove;
    bool mByWorldOrLocalPos;
    float mProgress;
    float mSpeedRatioCur;
    int mCurIn;//在mCur-1到mCur的路上

    public void MakeReady(float angleFromRight, PresetTrack trackPreset, Ifo target)
    {
        if (haveReady) return;

        mPointsReferTrack = trackPreset.meTrackPoints;

        mIfoTarget = target;

        mFitDir = mIfoTarget.rotApply.gameObject.AddComponent<ImgFitToDir>();
        mFitDir.MakeReady(angleFromRight, EAngle.degree);//后续不用考虑图像角度了

        mPointsCurTrack = new List<IfoTransform>();

        mOffsetPosExe = new ToolOffsetFieldLinear();
        mIfoOffsetPos = new IfoOffset();
        mIfoOffsetPos.form = EFormOffset.to;

        mOffsetRotExe = new ToolVarOffset();
        mIfoOffsetRot = new IfoOffset();
        mIfoOffsetRot.form = EFormOffset.to;

        mOffsetScaleExe = new ToolOffsetFieldLinear();
        mIfoOffsetScale = new IfoOffset();
        mIfoOffsetScale.form = EFormOffset.to;

        SuWhenFurthest += () => { };
        SuWhenFinish += () => { };

        haveReady = true;
    }

    public void SuMakeLaunchByWorld(MoveReady.Ifo ifo, Vector3 posWorldStartRefer)
    {
        if (MakeLaunch1(ifo))
        {
            mByWorldOrLocalPos = true;
            //需要传入开始时的世界坐标，因为出发时的世界坐标状态，本组件是不好负责的，可能与其它功能相互关系
            CalcNewPoint(posWorldStartRefer);//基于初始状态，确立后边的状态

            mIfoOffsetPos.start = mPointsCurTrack[0].pos;
            mIfoOffsetPos.toEnd = mPointsCurTrack[1].pos;

            MakeLaunch2();
        }

    }

    public void SuMakeLaunchByLocal(MoveReady.Ifo ifo, Func<Vector3> posCurParent = null)
    {
        if (MakeLaunch1(ifo))
        {
            mByWorldOrLocalPos = false;
            if (posCurParent == null)
                mPosCurParent = () => { return transform.parent.position; };
            else
                mPosCurParent = posCurParent;
            CalcNewPoint();

            //该组件独立实现相对运动（减少与外界产生牵连）
            //特别是要能相对位置不受到父物体翻转的影响
            mIfoOffsetPos.start = mPosCurParent() + mPointsCurTrack[0].pos;
            mIfoOffsetPos.toEnd = mPosCurParent() + mPointsCurTrack[1].pos;

            MakeLaunch2();
        }
    }

    bool MakeLaunch1(MoveReady.Ifo ifo)
    {
        if (!haveReady) return false;

        mIfoMove = ifo;

        mCurIn = 1;

        return true;
    }

    void MakeLaunch2()
    {
        mOffsetPosExe.MakeReady(new ToolOffsetFieldLinear.IfoTarget(mIfoTarget.posApply),
            mIfoOffsetPos, EKindFieldOffset.world_pos);
        mSpeedRatioCur = mOffsetPosExe.SuSpanToRatio(mIfoMove.speed);
        
        mIfoOffsetScale.start = mPointsCurTrack[0].scale;
        mIfoOffsetScale.toEnd = mPointsCurTrack[1].scale;
        mOffsetScaleExe.MakeReady(new ToolOffsetFieldLinear.IfoTarget(mIfoTarget.scaleApply), 
            mIfoOffsetScale, EKindFieldOffset.local_scale);
        
        mIfoOffsetRot.start = mPointsCurTrack[0].rot;
        mIfoOffsetRot.toEnd = mPointsCurTrack[1].rot;
        mOffsetRotExe.MakeReady(new ToolVarOffset.IfoTarget(), mIfoOffsetRot, EKindVarOffset.Float);

        PushAllTo(0);

        //--------------------------------

        mProgress = 0;

        mInMove = true;
    }

    void Update()
    {
        if (mInMove)
        {
            if (mOffsetPosExe.SuWhetherCloseToEnd())
            {
                if (mCurIn != mPointsCurTrack.Count - 1)
                {
                    mCurIn++;
                    mProgress = 0;

                    mIfoOffsetPos.start = transform.position;
                    mIfoOffsetPos.toEnd = mByWorldOrLocalPos ?
                        mPointsCurTrack[mCurIn].pos:
                        mPosCurParent() + mPointsCurTrack[mCurIn].pos;
                    mOffsetPosExe.SuResetBy(mIfoOffsetPos);
                    mSpeedRatioCur = mOffsetPosExe.SuSpanToRatio(mIfoMove.speed);

                    mIfoOffsetRot.start = new Vector2(transform.localEulerAngles.z, 0);
                    mIfoOffsetRot.toEnd = mPointsCurTrack[mCurIn].rot;
                    mOffsetRotExe.SuResetBy(mIfoOffsetRot);

                    mIfoOffsetScale.start = transform.localScale;
                    mIfoOffsetScale.toEnd = mPointsCurTrack[mCurIn].scale;
                    mOffsetScaleExe.SuResetBy(mIfoOffsetScale);
                }
                else
                {
                    mInMove = false;
                    SuWhenFurthest();
                    SuWhenFinish();
                }
            }
            else
            {
                mProgress += mSpeedRatioCur * Time.deltaTime;

                if (!mByWorldOrLocalPos)//相对时，本地空间的原点位置每一刻都在变化
                {
                    //mIfoOffsetPos.start = mPosCurParent() + mPointsCurTrack[mCurIn - 1].pos;
                    mIfoOffsetPos.toEnd = mPosCurParent() + mPointsCurTrack[mCurIn].pos;
                    mOffsetPosExe.SuResetBy(mIfoOffsetPos);
                }

                PushAllTo(mProgress);
            }
        }
    }

    void CalcNewPoint(Vector3 posWorldStartRefer = default(Vector3))
    {//包含对发射角度的考虑
     //计算在移动一开始就能确定的东西

        float angle = MathAngle.AngleNip(mIfoMove.dir, Vector2.right);//发射角度

        mPointsCurTrack.Clear();

        SVector3 beginPosInWorld = null; //加速用

        for (int i = 0; i < mPointsReferTrack.Count; i++)
        {
            IfoTransform ifo = new IfoTransform();

            if (mByWorldOrLocalPos)
            {
                if (beginPosInWorld == null)
                {
                    //摆放初始位置时，肯定还是先按相对关系摆
                    //就算是相对世界，创建的时候还是相对本地的，利用这个计算之后相对世界的位置情况
                    beginPosInWorld = new SVector3(posWorldStartRefer + 
                        MathAngle.CoordRotate(mPointsReferTrack[0].pos, angle).ToVector3());
                }
                ifo.pos = beginPosInWorld.v + MathAngle.CoordRotate(
                    mPointsReferTrack[i].pos - mPointsReferTrack[0].pos, angle).ToVector3();
                //前一个加数包含了对第一个位置的考虑，后面就都相对于第一个位置来转了
            }
            else
            {
                ifo.pos = MathAngle.CoordRotate(mPointsReferTrack[i].pos, angle);
            }
            
            ifo.rot = new Vector3(mPointsReferTrack[i].rot.z + angle, 0, 0);//用第一维记录角度即可

            ifo.scale = mPointsReferTrack[i].scale;

            mPointsCurTrack.Add(ifo);
        }
    }

    void PushRot(float progress)
    {
        mOffsetRotExe.SuVaryTo(progress);
        mFitDir.meAngleAim = mOffsetRotExe.SuGetAt().x;
        mFitDir.SuFitInstantly();
    }

    void PushAllTo(float progress)
    {
        mOffsetPosExe.SuVaryTo(progress);
        mOffsetScaleExe.SuVaryTo(progress);//伸缩先改，伸缩的符号会影响旋转结果
        PushRot(progress);
    }

}
