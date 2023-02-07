using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeCount : MonoBehaviour,ISwitchScene
{//向外界提供次数数据

    //===================================

    Dictionary<string,TimeRest> mWaitTwoClick;

    public bool SuMakeClickIfTwo(string id, float gapLimit)//如果连击了两次，便会返回true
    {
        if (!mWaitTwoClick.ContainsKey(id))
        {
            TimeRest wait = new TimeRest();
            wait.count = 0;
            wait.gapLimit = gapLimit;
            mWaitTwoClick.Add(id, wait);
            return MakeCount(wait);
        }
        else
            return MakeCount(mWaitTwoClick[id]);
    }

    bool MakeCount(TimeRest wait)//捕捉双击行为
    {
        wait.count++;
        if (wait.count == 1)//首次进行确认，启动计时器
        {
            wait.startTime = Time.time;
            return false;
        }
        else if (wait.count == 2)//当第二次进行确认
        {
            if (Time.time - wait.startTime < wait.gapLimit)//时间间隔满足要求时
            {
                wait.count = 0;
                return true;
            }
            else
            {//第二次确认，可能离第一次确认很久了，但需考虑此时突然连续确认两次
                wait.count = 1;
                wait.startTime = Time.time;
                return false;
            }
        }
        else
            return false;
    }

    //==============================

    class TimeRest 
    {
        public int count;//双击计数
        public float gapLimit;
        public float startTime;//测量双击
    }

    public static TimeCount It;

    public void WhenAwake()
    {
        It = this;
        mWaitTwoClick = new Dictionary<string, TimeRest>();
    }

    public void WhenSwitchScene()
    {
        mWaitTwoClick.Clear();
    }
}
