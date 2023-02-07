using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MouseInput : MonoBehaviour,ISwitchScene//管理全局下的可用输入
{
    //一次性赋值=========================================

    public bool mAxisMode = false;//是否采用的轴映射方式，来识别鼠标输入

    //私用变量=========================================

    Vector2 mMove;
    Vector2 mMoveAcc;

    void Update()
    {
        if (mAxisMode)
        {
            mMove.Set(
                Input.GetAxis("Horizontal"),
                Input.GetAxis("Vertical")
            );
            mMoveAcc += mMove;
        }
        else
        {
            mMoveAcc = Input.mousePosition;
        }

        if (mMoveAcc.magnitude != 0)
            UnifiedCursor.It.ExciteOver(mMoveAcc, EKindInput.mouse);

        //-------------------------------------------

        if (Input.GetMouseButtonDown(0))
            UnifiedInput.It.ExciteStartSure(EKindInput.mouse, true);
        else if (Input.GetMouseButtonUp(0))
            UnifiedInput.It.ExciteReleaseSure(EKindInput.mouse);

        if (Input.GetMouseButtonDown(1))
        {
            if (TimeCount.It.SuMakeClickIfTwo("MoInMR", 0.3f))//双击右键，促发返回操作
                UnifiedInput.It.ExciteBack();
        }

    }

    //==================================

    public static MouseInput It;

    public void WhenAwake()
    {
        It = this;
    }

    public void WhenSwitchScene()
    {
    }
}
