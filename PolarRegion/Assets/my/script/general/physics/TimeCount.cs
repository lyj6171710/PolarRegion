using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeCount : MonoBehaviour,ISwitchScene
{//������ṩ��������

    //===================================

    Dictionary<string,TimeRest> mWaitTwoClick;

    public bool SuMakeClickIfTwo(string id, float gapLimit)//������������Σ���᷵��true
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

    bool MakeCount(TimeRest wait)//��׽˫����Ϊ
    {
        wait.count++;
        if (wait.count == 1)//�״ν���ȷ�ϣ�������ʱ��
        {
            wait.startTime = Time.time;
            return false;
        }
        else if (wait.count == 2)//���ڶ��ν���ȷ��
        {
            if (Time.time - wait.startTime < wait.gapLimit)//ʱ��������Ҫ��ʱ
            {
                wait.count = 0;
                return true;
            }
            else
            {//�ڶ���ȷ�ϣ��������һ��ȷ�Ϻܾ��ˣ����迼�Ǵ�ʱͻȻ����ȷ������
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
        public int count;//˫������
        public float gapLimit;
        public float startTime;//����˫��
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
