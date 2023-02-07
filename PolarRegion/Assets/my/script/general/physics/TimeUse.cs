using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeUse : MonoBehaviour, ISwitchScene
{//向外界提供时间数据

    public bool SuIfJustPastSpecGap(int gap)
    {
        //参数单位是秒
        return mNumState[gap - 1];
    }

    float mTimeCountDown;
    int[] mNum;//记录经历了多少个1秒、2秒、......
    bool[] mNumState;//标记当前是否刚经过
    bool[] mNumWait;

    const int cMax = 2;

    void Start()
    {
        mTimeCountDown = 1;
        mNum = new int[cMax];
        mNumState = new bool[cMax];
        mNumWait = new bool[cMax];
    }

    void Update()
    {
        mTimeCountDown -= Time.deltaTime;

        if (mTimeCountDown < 0)
        {
            mNum[0] += 1;
            mNumWait[0] = true;
            float overflow = mTimeCountDown - 0;
            mTimeCountDown = 1 + overflow;

            for (int i = 1; i < cMax; i++)
            {
                if (mNum[0] % (i+1) == 0)
                {
                    mNum[i] += 1;
                    mNumWait[i] = true;
                }
            }
        }
    }

    void LateUpdate()
    {
        for (int i = 0; i < cMax; i++)
        {
            OverAssist.OnThenOff(ref mNumState[i], ref mNumWait[i]);
        }
    }

    //=========================

    public static TimeUse It;

    public void WhenAwake()
    {
        It = this;
    }

    public void WhenSwitchScene()
    {
        
    }
}
