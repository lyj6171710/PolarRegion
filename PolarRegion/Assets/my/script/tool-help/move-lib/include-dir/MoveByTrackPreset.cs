using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveByTrackPreset : MonoBehaviour
{
    //���볯���ҷ��ƶ�ʱ�Ĺ켣
    //Ԥ�õ����壬���ǵ������ұ߿�ʼ�ƶ�ʱ��ͼ�����ݵ�������Գ����ҷ���״̬
    //ͼ����ȱ���ת����ת�������ݵ����򣬳����ұߣ�
    //Ȼ������һ����ת����Ӧ���ҿ�ʼ�ƶ�ʱ����ת��Ȼ��������ת����Ӧ�����ⷽ��ʼ�ƶ�ʱ����ת

    public struct Ifo
    {
        public Transform posApply;
        public Transform rotApply;
        public Transform scaleApply;
    }

    [Space(10)]
    public EMotion mMotionDp;//���ض��켣��Ӧ���ƶ���ʽ
    public int mTurnAtDp;

    public Action SuWhenFurthest;
    public Action SuWhenFinish;

    bool haveReady = false;

    List<IfoTransform> mPointsReferTrack;//����ʱ�ı�׼
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
    int mCurIn;//��mCur-1��mCur��·��

    public void MakeReady(float angleFromRight, PresetTrack trackPreset, Ifo target)
    {
        if (haveReady) return;

        mPointsReferTrack = trackPreset.meTrackPoints;

        mIfoTarget = target;

        mFitDir = mIfoTarget.rotApply.gameObject.AddComponent<ImgFitToDir>();
        mFitDir.MakeReady(angleFromRight, EAngle.degree);//�������ÿ���ͼ��Ƕ���

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
            //��Ҫ���뿪ʼʱ���������꣬��Ϊ����ʱ����������״̬��������ǲ��ø���ģ����������������໥��ϵ
            CalcNewPoint(posWorldStartRefer);//���ڳ�ʼ״̬��ȷ����ߵ�״̬

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

            //���������ʵ������˶���������������ǣ����
            //�ر���Ҫ�����λ�ò��ܵ������巭ת��Ӱ��
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

                if (!mByWorldOrLocalPos)//���ʱ�����ؿռ��ԭ��λ��ÿһ�̶��ڱ仯
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
    {//�����Է���ǶȵĿ���
     //�������ƶ�һ��ʼ����ȷ���Ķ���

        float angle = MathAngle.AngleNip(mIfoMove.dir, Vector2.right);//����Ƕ�

        mPointsCurTrack.Clear();

        SVector3 beginPosInWorld = null; //������

        for (int i = 0; i < mPointsReferTrack.Count; i++)
        {
            IfoTransform ifo = new IfoTransform();

            if (mByWorldOrLocalPos)
            {
                if (beginPosInWorld == null)
                {
                    //�ڷų�ʼλ��ʱ���϶������Ȱ���Թ�ϵ��
                    //������������磬������ʱ������Ա��صģ������������֮����������λ�����
                    beginPosInWorld = new SVector3(posWorldStartRefer + 
                        MathAngle.CoordRotate(mPointsReferTrack[0].pos, angle).ToVector3());
                }
                ifo.pos = beginPosInWorld.v + MathAngle.CoordRotate(
                    mPointsReferTrack[i].pos - mPointsReferTrack[0].pos, angle).ToVector3();
                //ǰһ�����������˶Ե�һ��λ�õĿ��ǣ�����Ͷ�����ڵ�һ��λ����ת��
            }
            else
            {
                ifo.pos = MathAngle.CoordRotate(mPointsReferTrack[i].pos, angle);
            }
            
            ifo.rot = new Vector3(mPointsReferTrack[i].rot.z + angle, 0, 0);//�õ�һά��¼�Ƕȼ���

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
        mOffsetScaleExe.SuVaryTo(progress);//�����ȸģ������ķ��Ż�Ӱ����ת���
        PushRot(progress);
    }

}
