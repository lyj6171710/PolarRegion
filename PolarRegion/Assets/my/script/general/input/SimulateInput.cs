using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulateInput : MonoBehaviour,ISwitchScene
{
    //模拟输入，其实外界没有操作，程序内部模拟做出了这样一个输入
    //直接去操纵统一层的输出即可

    public void MakeWipe(Vector2 dir,MonoBehaviour apply)//外界只应该调用一次
    {
        if (dir.magnitude < 0.1f || apply == null) {
            Debug.Log("无效的参数");
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

    //架构需要=========================

    public static SimulateInput It;

    public void WhenAwake()
    {
        It = this;
    }

    public void WhenSwitchScene()
    {
        
    }
}
