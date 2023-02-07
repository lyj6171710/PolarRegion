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

        public Ifo(List<RectTransform> target)//Ĭ��ֵ��ʾ
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
            Debug.Log("������Ϊ0");
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
            mStartAt[i] = mInfo.meApplys[i].localPosition + firstOffset;//����ϵ���ĳ�����ƫ��
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

    float mLimit = 0;//���ƫ���޶�
    float mOffsetNeed = 0;//�̶�ֵ������ƫ�ƣ���Թ��õ����ĵ�

    int mSelectCur = 0;
    int mSelectLast = 0;

    float mHaveInWipe;//���ڻ���״̬��ʱ��
    float mTimeCanShift;//ÿһ���л�����Ҫ��һ��ʱ����������л���
    //������л���ȥ�����ٴμ�⣬��Ϊ���벻�㣬���л����������

    void Awake()
    {
        mHaveReady = false;
    }

    void Start()
    {
        Vector2 box = new Vector2(mInfo.meApplys[0].rect.width, mInfo.meApplys[0].rect.height);//����Ǹ�����
        float dot = Vector2.Dot(box.normalized, mInfo.dir);//��������Խ����������ż���ȻԽ��
        mLimit = box.magnitude * Mathf.Abs(dot) / 100;//�ż��ٸ�Ҳ���ܸߣ�һ���û���Ҫ�ı�ѡ��ʱ����ֻ����΢������������Լ�����ͼ
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
                mOffsetNeed = Mathf.Lerp(mOffsetNeed, mOffsetNeed + 300 * wipeRatio, Time.deltaTime * mInfo.speed);//ת���ٶȲ�ȡ���ڻ����ٶȣ��������ǶȻ�Ӱ��ת���ٶ�

                mHaveInWipe += Time.deltaTime;
                TryShiftSelect(wipeRatio);
            }
            else
            {
                if (TouchInput.It.meHaveTouch)
                {
                    //��ֹ
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
        if (gap >= mLimit)//������������ֵʱ������ѡ����л�
        {
            int optionNum = mInfo.meApplys.Count;

            if (mSelectCur == 0)
            {
                if (wipeTo < 0)//ȷ������������һ��ѡ�����
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

            mTimeCanShift = 0.2f;//��һ�����ٽ����л���ʱ��
        }
    }
}
