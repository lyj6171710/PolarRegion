using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfoTime
{
    public int days = 0;
    public int hours = 0;
    public int minutes = 0;
    public float seconds = 0;//不够精确，只近似
}

public class DateSystem : MonoBehaviour, ISwitchScene
{
    public float mTimeVelDp = 30;

    IfoTime mCount;
    float mTimeInLast;

    private void FixedUpdate()
    {
        UpdatTimeState();
    }

    private void UpdatTimeState()
    {
        mCount.seconds += mTimeVelDp * (Time.time - mTimeInLast);
        mTimeInLast = Time.time;
        while (mCount.seconds >= 60)
        {//每次循环体的执行，做一次进位
            mCount.seconds = mCount.seconds - 60;
            mCount.minutes += 1;
            if (mCount.minutes >= 60)
            {
                mCount.minutes = 0;
                mCount.hours += 1;
                if (mCount.hours >= 24)
                {
                    mCount.hours = 0;
                    mCount.days += 1;
                }
            }
        }
    }

    public void SuGetTimePast()
    {
        Debug.Log("天数：" + mCount.days.ToString() +
            " 时数：" + mCount.hours.ToString() +
            " 分数：" + mCount.minutes.ToString() + 
            " 秒数：" + mCount.seconds.ToString());
    }

    //==================================

    public static DateSystem It;

    public void WhenAwake()
    {
        It = this;

        mCount = new IfoTime();
    }

    public void WhenSwitchScene()
    {
        
    }
}
