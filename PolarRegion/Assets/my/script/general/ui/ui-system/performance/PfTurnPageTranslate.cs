using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PfTurnPageTranslate : PerformChoose
{//非需预置，在需要时用代码将该组件加上去

    public struct Ifo
    {
        public Vector2 startAt ;
        public Vector2 endAt ;

        public Action whenTurnTo;

        public float speed ;
        public float turnSpeed;

        RectTransform apply;

        public RectTransform meApply { get { return apply; } }

        public Ifo(RectTransform target)
        {
            startAt = Vector2.zero;
            endAt = Vector2.one;
            apply = target;
            speed = 1;
            turnSpeed = 1;
            whenTurnTo = () => { };
        }

        public static Ifo CopyBase(Ifo turn)
        {
            Ifo copy = new Ifo(turn.apply);
            copy.speed = turn.speed;
            copy.turnSpeed = turn.turnSpeed;
            return copy;
        }
    }
    protected override void MakeReadyWhenAsPreset()
    {
        throw new System.NotImplementedException();
    }

    public void MakeReady(Ifo need)
    {
        mInfo = need;

        mOffset = mInfo.endAt - mInfo.startAt;
        mOver = mOffset.magnitude;
        mDir = mOffset.normalized;
        mLimit = mOffset / 4;

        mProgressNeed = 0;
    }

    public void ResetPos()//每次重新开始一次滑动时，需被外界调用一次，避免上次滑动后遗留数据造成的影响
    {
        mLoading = false;
        mProgressNeed = 0;
        UpdateProgress();
    }

    public void ForceTurnReverse()
    {
        PfTurnPageTranslate reverse = mInfo.meApply.gameObject.AddComponent<PfTurnPageTranslate>();
        Ifo info = Ifo.CopyBase(mInfo);
        info.startAt = mInfo.endAt;
        info.endAt = mInfo.startAt;
        info.whenTurnTo = () => { 
            Destroy(reverse);
            ResetPos();
        };
        reverse.MakeReady(info);
        reverse.ForceTurn();

        mCanTurn = false;
    }

    public void ForceTurn()
    {
        SimulateInput.It.MakeWipe(mDir, this);

        mCanTurn = true;//可被后续影响
    }

    public bool mCanTurn;

    //-----------------------------------------

    Ifo mInfo;

    float mOver = 0;
    Vector2 mOffset= Vector2.zero;
    Vector2 mDir= Vector2.zero;
    Vector2 mLimit= Vector2.zero;

    bool mLoading = false;
    float mProgressNeed = 0;

    void Update()
    {
        if (mCanTurn)
        {
            if (!mLoading)
            {
                if (mProgressNeed >= 0)
                {
                    if (mProgressNeed > mLimit.magnitude)
                    {
                        mLoading = true;
                    }
                    else
                    {
                        if (VirtualInput.It.GetWipingS(this))
                        {
                            float wipeLength = Vector2.Dot(VirtualInput.It.GetWipeStateS(this), mDir);
                            mProgressNeed += mInfo.speed * wipeLength;
                        }
                        else
                        {
                            mProgressNeed = Mathf.Lerp(mProgressNeed, 0, Time.deltaTime * mInfo.turnSpeed);
                        }
                    }
                }
                else
                {
                    mProgressNeed = 0;
                }
            }
            else
            {
                mProgressNeed = Mathf.Lerp(mProgressNeed, mOver, Time.deltaTime * mInfo.turnSpeed);
                if (Mathf.Abs(mOver - mProgressNeed) < 10)
                {
                    mLoading = false;
                    mCanTurn = false;//一次性的
                    mInfo.whenTurnTo();
                }
            }

            UpdateProgress();
        }
    }

    void UpdateProgress()
    {
        mInfo.meApply.localPosition = mInfo.startAt + mDir * mProgressNeed;
    }
}
