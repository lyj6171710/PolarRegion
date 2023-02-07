using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : MonoBehaviour, ISwitchScene
{
    static List<string> sResponds = new List<string>() { "space" };
    static Dictionary<KeyCode,int> sNums = new Dictionary<KeyCode, int>() {
        {KeyCode.Alpha0,0 },{KeyCode.Alpha1,1 },{KeyCode.Alpha2,2 },
        {KeyCode.Alpha3,3 },{KeyCode.Alpha4,4 },{KeyCode.Alpha5,5 },
        {KeyCode.Alpha6,6 },{KeyCode.Alpha7,7 },{KeyCode.Alpha8,8 },{KeyCode.Alpha9,9 }
    };
    static Dictionary<KeyCode, bool> sPressMonitor = new Dictionary<KeyCode, bool>{
        { KeyCode.S,false},{ KeyCode.C,false},{ KeyCode.L,false },{ KeyCode.RightCommand,false }
    };

    List<KeyCode> mPressList;//辅助对字典的遍历，用来解决遍历时不能对字典赋值的问题

    public static bool SuIsInPress(KeyCode key) {
        if (sPressMonitor.ContainsKey(key))
            return sPressMonitor[key];
        else
            return false;
    }

    public static bool mePressInMonitorAny { get { foreach (var state in sPressMonitor.Values) if (state) return true;return false; } }

    void Update()
    {
        //confirm========================

        if (Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.Return)) 
        {
            UnifiedInput.It.ExciteStartSure(EKindInput.keyboard, false);
        }
        else if (Input.GetKeyUp(KeyCode.K) || Input.GetKeyUp(KeyCode.Return))
        {
            UnifiedInput.It.ExciteReleaseSure(EKindInput.keyboard);
        }
        
        if(Input.GetKeyDown(KeyCode.J)||Input.GetKeyDown(KeyCode.Escape))
            UnifiedInput.It.ExciteBack();

        //dir==========================

        if (Input.GetKey(KeyCode.W))
        {
            UnifiedInput.It.ExciteGo(EToward4.up);

            if (Input.GetKey(KeyCode.A))
            {
                UnifiedInput.It.ExciteGo(EToward4.left);
                UnifiedInput.It.ExciteMove(OverTool.TowardToVector(EToward8.upLeft));
            }
            else if (Input.GetKey(KeyCode.D))
            {
                UnifiedInput.It.ExciteGo(EToward4.right);
                UnifiedInput.It.ExciteMove(OverTool.TowardToVector(EToward8.upRight));
            }
            else
                UnifiedInput.It.ExciteMove(Vector2.up);
        }
        else//同理
        {
            if (Input.GetKey(KeyCode.S))
            {
                UnifiedInput.It.ExciteGo(EToward4.down);

                if (Input.GetKey(KeyCode.A))
                {
                    UnifiedInput.It.ExciteGo(EToward4.left);
                    UnifiedInput.It.ExciteMove(OverTool.TowardToVector(EToward8.downLeft));
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    UnifiedInput.It.ExciteGo(EToward4.right);
                    UnifiedInput.It.ExciteMove(OverTool.TowardToVector(EToward8.downRight));
                }
                else
                    UnifiedInput.It.ExciteMove(Vector2.down);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                UnifiedInput.It.ExciteGo(EToward4.left);
                UnifiedInput.It.ExciteMove(Vector2.left);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                UnifiedInput.It.ExciteGo(EToward4.right);
                UnifiedInput.It.ExciteMove(Vector2.right);
            }
        }

        //unified=====================

        if (Input.GetKey(KeyCode.Space))
            UnifiedInput.It.ExciteKey("space");

        foreach (string react in sResponds)
        {
            if (Input.GetKey(react))
                UnifiedInput.It.ExciteKey(react);
        }

        //num======================

        if (SuIsInPress(KeyCode.RightCommand))//降低性能耗费
        {
            foreach (KeyCode num in sNums.Keys)
            {
                if (Input.GetKeyDown(num))
                    UnifiedInput.It.ExciteNum(sNums[num]);
            }
        }

        //condition=====================

        foreach (KeyCode listen in mPressList)
        {
            if (Input.GetKey(listen)) sPressMonitor[listen] = true;
            else sPressMonitor[listen] = false;
        }
    }

    //======================================

    public static KeyboardInput It;

    public void WhenAwake()
    {
        It = this;

        mPressList = new List<KeyCode>(sPressMonitor.Keys);
    }

    public void WhenSwitchScene()
    {
        
    }
}
