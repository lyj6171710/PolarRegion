using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualInput : MonoBehaviour,ISwitchScene
{
    //支持模拟输入的统一输入

    public bool GetWipingS(MonoBehaviour apply)//支持模拟输入
    {
        if (apply == mSimulator)
            if (mSimulateWipe)
                return mWipingS;

        return UnifiedCursor.It.meIsSweeping;//没有模拟输入或不匹配时，和默认输入是一样的
    }

    public Vector2 GetWipeStateS(MonoBehaviour apply)
    {
        if (apply == mSimulator)
            if (mSimulateWipe)
                return mWipeStateS;

        return UnifiedCursor.It.meOverOffset;
    }

    //================================

    [HideInInspector] public bool mSimulateWipe;
    bool mWipingS;
    Vector2 mWipeStateS;
    MonoBehaviour mSimulator;//可以让模拟输出只对某个物件有效

    public void SetWipe(bool inWipe, Vector2 howWipe, MonoBehaviour apply)
    {
        mWipingS = inWipe;
        mWipeStateS = howWipe;
        mSimulator = apply;
    }

    //架构需要=================================

    public static VirtualInput It;

    public void WhenAwake()
    {
        It = this;
    }

    public void WhenSwitchScene()
    {

    }
}
