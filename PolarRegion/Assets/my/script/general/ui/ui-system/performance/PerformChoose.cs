using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PerformChoose : MonoBehaviour
{
    //负责UI表现效果的增强

    //动态效果组件的使用，应可以临时附加，也可以提前预置。
    //组件内部效果的启用，以参数为接口。

    //临时时，外界或程序输入参数
    //预置时，组件上的预置数据作为参数
    
    protected bool mHaveReady;

    void Start()
    {
        if (!mHaveReady)//start时，还未准备就绪，说明组件是通过预置方式来使用的
        {
            MakeReadyWhenAsPreset();
            //mHaveReady = true;//存在无法预置的数据，
            //那么就只有通过程序传入这部分参数来完成对该动效组件的启动
        }
    }

    protected abstract void MakeReadyWhenAsPreset();
}
