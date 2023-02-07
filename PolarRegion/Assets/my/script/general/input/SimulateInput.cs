using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulateInput : MonoBehaviour,ISwitchScene
{
    //ģ�����룬��ʵ���û�в����������ڲ�ģ������������һ������
    //ֱ��ȥ����ͳһ����������

    public void MakeWipe(Vector2 dir,MonoBehaviour apply)//���ֻӦ�õ���һ��
    {
        if (dir.magnitude < 0.1f || apply == null) {
            Debug.Log("��Ч�Ĳ���");
            return;
        }
        else
        {
            mWipeOffset = dir.normalized * 25;
            mNeedWipe = true;
            mWipeSpend = 1.5f;
            mApply = apply;
        }
    }

    //=================================

    bool mNeedWipe;
    Vector2 mWipeOffset;
    float mWipeSpend;
    MonoBehaviour mApply;

    void Update()
    {
        if (mNeedWipe)
        {
            VirtualInput.It.mSimulateWipe = true;
            VirtualInput.It.SetWipe(true, mWipeOffset, mApply);
            mWipeSpend -= Time.deltaTime;
            if (mWipeSpend < 0)
            {
                mNeedWipe = false;
            }
        }
        else
            VirtualInput.It.mSimulateWipe = false;
    }

    //�ܹ���Ҫ=========================

    public static SimulateInput It;

    public void WhenAwake()
    {
        It = this;
    }

    public void WhenSwitchScene()
    {
        
    }
}
