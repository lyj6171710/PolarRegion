using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UiOption : MonoBehaviour
{//选项由于众多，只打算使用同一种脚本，不同选项只是不同实例
 //选项触发游戏程序的局部流程
 //选项同时可以触发选表的切换或重置或关闭，这个靠使用者自己预置好数据
 //也就是说，UI也负责了游戏程序的逻辑，但只是交互行为本身的限定限制上

    [HideInInspector] public int mIndex;//在用选表搜集选项时自动录入
    [HideInInspector] public UiList mSuper;//自动录入

    public UnityEvent mCallWhenPressDp;//将要触发的逻辑代指
    public UiList mDivertToOtherDp;

    public void MakeSure()
    {
        if (mCallWhenPressDp != null)
            mCallWhenPressDp.Invoke();
        if (mDivertToOtherDp != null)
        {
            //mListBelong.CallOffBox();
            //mListBelong.TakeDivert(mDivertToOtherDp);
        }
    }
}
