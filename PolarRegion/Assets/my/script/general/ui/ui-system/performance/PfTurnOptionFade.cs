using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PfTurnOptionFade : PerformChoose
{
    public delegate void WhenAppear();

    public struct Ifo
    {
        public Vector2 centerAt;
        public Vector2 dir;
        public float speed;
        public WhenAppear whenTurnTo;

        List<RectTransform> applys;

        public List<RectTransform> meApplys { get { return applys; } }

        public Ifo(List<RectTransform> target)//默认值提示
        {
            centerAt = Vector2.zero;
            applys = target;
            dir = Vector2.zero;
            speed = 1;
            whenTurnTo = () => { };
        }
    }

    protected override void MakeReadyWhenAsPreset()
    {
        throw new System.NotImplementedException();
    }

    public void AcceptBase(Ifo need)
    {
        mInfo = need;
        if (mInfo.dir == Vector2.zero)
        {
            Debug.Log("方向不能为0");
            return;
        }
        else
            mInfo.dir = mInfo.dir.normalized;
        mInfo.speed = Mathf.Abs(mInfo.speed);

        Vector3 centerAtScene = new Vector3(mInfo.centerAt.x, mInfo.centerAt.y, mInfo.meApplys[0].localPosition.z);
        Vector3 firstOffset = centerAtScene - mInfo.meApplys[0].localPosition;
        mStartAt = new Vector3[mInfo.meApplys.Count];
        mOptionImg = new Image[mInfo.meApplys.Count];
        mOptionText = new Text[mInfo.meApplys.Count];
        for (int i = 0; i < mInfo.meApplys.Count; i++)
        {
            mStartAt[i] = mInfo.meApplys[i].localPosition + firstOffset;//坐标系相对某个点的偏移
            mOptionImg[i] = mInfo.meApplys[i].transform.GetComponentInChildren<Image>(true);
            mOptionText[i] = mInfo.meApplys[i].transform.GetComponentInChildren<Text>(true);

            mInfo.meApplys[i].tag = CoordUse.cReactMask;
        }

        mHaveReady = true;
    }

    public bool mCanTurn = false;

    public void ResetPos()
    {
        mSelectCur = 0;
        mOffsetNeed = 0;
        for (int i = 0; i < mInfo.meApplys.Count; i++)
        {
            mInfo.meApplys[i].localPosition = mStartAt[i];
        }
    }

    //-----------------------------------------

    Ifo mInfo;
    Vector3[] mStartAt;
    Image[] mOptionImg;
    Text[] mOptionText;

    //-----------------------------------------

    bool mHaveReady;

    float mLimit = 0;//相对偏移限度
    float mOffsetNeed = 0;//固定值描述的偏移，相对共用的中心点

    int mSelectCur = 0;
    int mSelectLast = 0;

    float mHaveInWipe;//处在滑动状态的时间
    float mTimeCanShift;//每一次切换，都要等一段时间才能再做切换了
    //缓解刚切换过去，又再次检测，因为距离不足，又切换回来的情况

    void Awake()
    {
        mHaveReady = false;
    }

    void Start()
    {
        Vector2 box = new Vector2(mInfo.meApplys[0].rect.width, mInfo.meApplys[0].rect.height);//最长的那个方向
        float dot = Vector2.Dot(box.normalized, mInfo.dir);//滑动方向越贴近最长方向，门槛自然越高
        mLimit = box.magnitude * Mathf.Abs(dot) / 100;//门槛再高也不能高，一般用户需要改变选择时，就只是稍微滑动，表达他自己的意图
        mLimit = 0;

        mHaveInWipe = 0;
        mTimeCanShift = 0;

        mInfo.meApplys[mSelectCur].tag = CoordUse.cReactCan;
    }

    void OnDisable()
    {

    }
    void Update()
    {
        if (!mHaveReady) return;

        if (mCanTurn)
        {
            if (TouchInput.It.meWiping)
            {
                Vector2 wipeDir = TouchInput.It.meWipeState.normalized;
                float wipeRatio = Vector2.Dot(wipeDir, mInfo.dir);
                mOffsetNeed = Mathf.Lerp(mOffsetNeed, mOffsetNeed + 300 * wipeRatio, Time.deltaTime * mInfo.speed);//转移速度不取决于滑动速度，但滑动角度会影响转移速度

                mHaveInWipe += Time.deltaTime;
                TryShiftSelect(wipeRatio);
            }
            else
            {
                if (TouchInput.It.meHaveTouch)
                {
                    //静止
                }
                else
                {
                    Vector3 tmpPos = mStartAt[mSelectCur];
                    float offset = (new Vector2(tmpPos.x, tmpPos.y) - mInfo.centerAt).magnitude;
                    mOffsetNeed = Mathf.Lerp(mOffsetNeed, -offset, Time.deltaTime * mInfo.speed);
                }
                mHaveInWipe = 0;
            }
            UpdateOffset();
        }

        if (mSelectLast != mSelectCur)
        {
            mInfo.meApplys[mSelectCur].tag = CoordUse.cReactCan;
            mInfo.meApplys[mSelectLast].tag = CoordUse.cReactMask;
            mSelectLast = mSelectCur;
        }

        if (mHaveInWipe > 0.5f) mTimeCanShift = 0;
        else if(mTimeCanShift > 0) mTimeCanShift -= Time.deltaTime;
    }

    void UpdateOffset()
    {
        for (int i = 0; i < mInfo.meApplys.Count; i++)
        {
            mInfo.meApplys[i].localPosition = new Vector2(mStartAt[i].x, mStartAt[i].y) + mOffsetNeed * mInfo.dir;

            Vector3 tmpPos = mInfo.meApplys[i].localPosition;
            float distance = (new Vector2(tmpPos.x, tmpPos.y) - mInfo.centerAt).magnitude;
            if (mOptionImg[i] != null)
            {
                Color tmpSet = mOptionImg[i].color;
                tmpSet.a = 1 - distance / 500;
                mOptionImg[i].color = tmpSet;
            }
            if (mOptionText[i] != null)
            {
                Color tmpSet = mOptionText[i].color;
                tmpSet.a = 1 - distance / 1000;
                mOptionText[i].color = tmpSet;
            }
        }
    }


    void TryShiftSelect(float wipeTo)
    {
        if (mTimeCanShift > 0) return;

        Vector3 curPos = mInfo.meApplys[mSelectCur].localPosition;
        float gap = (new Vector2(curPos.x, curPos.y) - mInfo.centerAt).magnitude;
        if (gap >= mLimit)//滑动超过敏感值时，导致选择的切换
        {
            int optionNum = mInfo.meApplys.Count;

            if (mSelectCur == 0)
            {
                if (wipeTo < 0)//确定方向是往下一个选项翻动的
                    mSelectCur = 1 < optionNum ? 1 : 0;
            }
            else if (mSelectCur == optionNum - 1)
            {
                if (wipeTo > 0) mSelectCur = (optionNum - 2) < 0 ? 0 : optionNum - 2;
            }
            else
            {
                if (wipeTo < 0)
                    mSelectCur++;
                else if (wipeTo > 0)
                    mSelectCur--;
            }

            mTimeCanShift = 0.2f;//下一次能再进行切换的时机
        }
    }
}
