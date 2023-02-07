using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualInput : MonoBehaviour,ISwitchScene
{
    //֧��ģ�������ͳһ����

    public bool GetWipingS(MonoBehaviour apply)//֧��ģ������
    {
        if (apply == mSimulator)
            if (mSimulateWipe)
                return mWipingS;

        return UnifiedCursor.It.meIsSweeping;//û��ģ�������ƥ��ʱ����Ĭ��������һ����
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
    MonoBehaviour mSimulator;//������ģ�����ֻ��ĳ�������Ч

    public void SetWipe(bool inWipe, Vector2 howWipe, MonoBehaviour apply)
    {
        mWipingS = inWipe;
        mWipeStateS = howWipe;
        mSimulator = apply;
    }

    //�ܹ���Ҫ=================================

    public static VirtualInput It;

    public void WhenAwake()
    {
        It = this;
    }

    public void WhenSwitchScene()
    {

    }
}
