using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//目前结合的理念有：
//选项选择器的成员化
//高隔离的express、performance
//划分视角：UI管理器、面板、选表
//程序提供硬接口，配合UI预置数据承载部分游戏功能
//UI种类划分之互斥、单体、个体

//UI系统被建议作为驱动游戏进行的前提。
//UI系统推动游戏进程发展或游戏本身功能的开关。

public class UiMager : MonoBehaviour,ISwitchScene
{
    //面板管理器，主要用来方便切换UI，以及提供UI交互的功能，是唯一单例

    //该UI系统倾向只是提供附带交互效果的UI面板给外界使用，
    //建议带有游戏功能的脚本，不要置于该UI系统下。

    //该UI系统在UI方面，设定并服务于一个层次，就是交互限制层。
    //该层将用户与游戏进行交互时，交互效果、选项、操作等，抽象到一种可以单独设定的状态。
    //该层的功能由该UI系统提供，该层的具体表现，由游戏程序自己决定，
    //且当它自己决定时，是通过预置、预设的方式，相关流程与游戏程序相互独立，没有直接关系。
    //最后，由选项自己主动绑定某个游戏程序的接口，来推动程序发展。

    //==============================

    public CtrlFullChart meFull => mChartFullDp;
    public CtrlSingleChart meSingle => mChartSingleDp;
    public CtrlAloneChart meAlone => mChartAloneDp;

    //效果需求独特特殊的UI，外界根据它自己需要来运用，不那么受UI系统管控
    public CtrlFullChart mChartFullDp;//互斥
    public CtrlSingleChart mChartSingleDp;//唯一
    public CtrlAloneChart mChartAloneDp;//可复制

    //==============================

    public UiPanel mStartPanelDp;
    public List<UiPanel> mMainPanelsDp;
    Dictionary<string, UiPanel> mMainPanels;
    
    void MakeReady()
    {
        mMainPanels = new Dictionary<string, UiPanel>();
        for (int i = 0; i < mMainPanelsDp.Count; i++)
        {
            UiPanel panel = mMainPanelsDp[i];
            panel.mAsMainPanel = true;
            mMainPanels.Add(panel.gameObject.name, panel);
        }
    }

    public void MakeCome(string name)
    {
        UiPanel panel = mMainPanels[name];
        if (!panel.meInUse)
            mMainPanels[name].MakeCome(null);
    }//主面板只能由外界来开启

    public void MakeLeave(string name)
    {
        UiPanel panel = mMainPanels[name];
        if (panel.meInOpen)
            panel.MakeLeave();
    }

    //=============================

    public static UiMager It;
    public static RectTransform meOverRect;
    public static CanvasScaler meScaler;

    public void WhenAwake()
    {
        It = this;
        meOverRect = GetComponent<RectTransform>();
        if (meScaler == null) meScaler = GetComponent<CanvasScaler>();
        mChartFullDp.MakeReady();
        mChartSingleDp.MakeReady();
        mChartAloneDp.MakeReady();
        //MakeReady();
    }

    public void WhenSwitchScene()
    {
        CanvasScaler scaler = ThingRefer.It[ERefer.OverlayNormalCanvas].GetComponent<CanvasScaler>();//找到能适应新场景需要的全局画布

        if (meScaler == null) meScaler = GetComponent<CanvasScaler>();
        meScaler.referenceResolution = scaler.referenceResolution;
        meScaler.screenMatchMode = scaler.screenMatchMode;
        meScaler.matchWidthOrHeight = scaler.matchWidthOrHeight;
        meScaler.referencePixelsPerUnit = scaler.referencePixelsPerUnit;
    }

}
