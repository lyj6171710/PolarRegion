using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PfMakeElemMove : PerformChoose
{
    public struct Ifo
    {
        public Vector2 startAt;
        public Vector2 endAt;

        public Action whenMoveFinish;

        public float speedStart;
        public float speedAcc;
        public float speedAim;
        public EKindAct kind;

        RectTransform apply;

        public RectTransform meApply { get { return apply; } }

        public Ifo(RectTransform target)
        {
            startAt = Vector2.zero;
            endAt = Vector2.one;
            apply = target;
            speedStart = 1;
            speedAcc = 1;
            speedAim = 1;
            kind = EKindAct.constant;
            whenMoveFinish = null;
        }

        public static Ifo CopySpeed(Ifo turn)//ֻ�����ٶ�����
        {
            Ifo copy = new Ifo(turn.apply);
            copy.speedStart = turn.speedStart;
            copy.speedAcc = turn.speedAcc;
            copy.kind = turn.kind;
            return copy;
        }
    }

    public bool mCanMove;

    protected override void MakeReadyWhenAsPreset()
    {
        throw new System.NotImplementedException();
    }

    public PfMakeElemMove MakeReady(Ifo need)
    {
        mInfo = need;

        ReadyAttr();

        mHaveReady = true;

        return this;
    }

    public void ResetPos()
    {
        mProgressNeed = 0;
        mValue.SuVaryTo(mProgressNeed);
    }

    public Vector2 GetPosAt()
    {
        return mValue.SuGetAt();
    }

    //-----------------------------------------

    Ifo mInfo;
    ToolOffsetUiLinear mValue;
    float mProgressNeed = 0;

    float mSpeedNow;//���ƶ�ģʽ��ͬ���в�ͬ����

    void ReadyAttr()
    {
        mValue = new ToolOffsetUiLinear();
        IfoOffset offset = new IfoOffset();
        offset.start = mInfo.startAt;
        offset.toEnd = mInfo.endAt;
        offset.form = EFormOffset.to;
        mValue.MakeReady(new ToolOffsetUiLinear.InfoTarget(mInfo.meApply), offset, EKindUiOffset.ui_pos);

        mCanMove = false;

        mProgressNeed = 0;

        mSpeedNow = mInfo.speedStart;
    }

    void OnDisable()
    {

    }

    void Update()
    {
        if (!mHaveReady) return;

        if (mCanMove)
        {
            if (mInfo.kind == EKindAct.lerp)
            {
                mProgressNeed = Mathf.Lerp(mProgressNeed, 1, Time.deltaTime * mSpeedNow);
            }
            else if (mInfo.kind == EKindAct.constant)
            {
                mProgressNeed += mSpeedNow * Time.deltaTime;//ÿ��ǰ��ָ���ٷֱȵľ���
            }
            else if (mInfo.kind == EKindAct.accelerate)
            {
                if (mSpeedNow < mInfo.speedAim)
                    mSpeedNow += mInfo.speedAcc * Time.deltaTime;//ÿ�����
                mProgressNeed += mSpeedNow * Time.deltaTime;//ÿ��ǰ������
            }

            mValue.SuVaryTo(mProgressNeed);

            if (mValue.SuWhetherCloseToEnd())
            {
                if (mInfo.whenMoveFinish != null) mInfo.whenMoveFinish();
                mCanMove = false;//һ���Ե�
            }

        }
    }

}
