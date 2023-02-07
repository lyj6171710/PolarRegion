using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalJoint : LocalFocus//承上启下，那些既作为分支，又作为中心的部门
{
    [HideInInspector] public LocalBranch mFocusBranch;//这个由分支赋值给自己
    //作为局部核心的自己，可能只是更上一级分布的一个部门，这里获取这个部门，方便控制

    public void NotifyReadyNer()
    {
        MakeReadyNer(Kind.joint);//不放awake中，因为物体可能失活导致不执行，但必须立即执行
    }

    public override void TurnSelfActive(bool show)
    {
        if (show)
            mFocusBranch.Open();//分支负责部门可以负责时，优先它来负责,上级管理的事务已经比较多了
        else
            mFocusBranch.Close();
    }

}
