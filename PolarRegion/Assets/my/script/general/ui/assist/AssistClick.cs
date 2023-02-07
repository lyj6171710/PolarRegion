using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AssistClick : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{   //辅助对动态生成的UI元素被操作时的感应，通过代码，将该组件临时加入进去
    //在UGUI中对image控件检测鼠标按下和抬起使用OnPointerDown和OnPointerUp方法

    //注意父物体似乎得处于视野中心，且是一个小正方形，才有效，原因未知

    bool mNeedReactDown = false;//按下事件
    IClick mMethodDown;//响应按下事件时，会调用的流程
    int mMouseKey = 0;//鼠标按键或键盘按键都可以触发事件，默认为左键

    bool mNeedReactTouch = false;//触碰事件
    ITouch mMethodHover;//响应进入、离开、悬浮事件
    bool mMouseOver;//鼠标是否覆盖在当前元素上

    bool mNeedReactBoard = false;
    string mBoardKey;//鼠标位于指定范围下时，按键也可以触发点击操作

    object mPara;

    //内外机制====================================

    public void SuReactClick(IClick methodDown, object para = null)//回调参数需要
    {
        mPara = para;
        if (methodDown != null)
        {
            mNeedReactDown = true;
            mMethodDown = methodDown;
        }
    }

    public void SuReactTouch(ITouch methodHover)
    {
        if (methodHover != null)
        {
            mNeedReactTouch = true;
            mMethodHover = methodHover;
        }
    }

    public void SuReactByMouse(int mouseKey)
    {
        mMouseKey = mouseKey;
    }

    public void SuReactByKey(string boardKey)
    {
        mNeedReactBoard = true;
        mBoardKey = boardKey;
    }

    //内部机制=========================================

    public void OnPointerEnter(PointerEventData eventData)
    {
        mMouseOver = true;
        if (mNeedReactTouch)
            mMethodHover.iHoverInsideFmMouse(mPara);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mMouseOver = false;
        if (mNeedReactTouch)
            mMethodHover.iHoverOutsideFmMouse(mPara);//需要被调用函数有接受para的形参
    }

    void InspectClick()
    {
        if (mMouseOver)//鼠标在范围内时
        {
            if (mNeedReactDown)
            {
                if (Input.GetMouseButtonUp(mMouseKey))
                    if (TimeCount.It.SuMakeClickIfTwo("AsCl", 0.2f))
                        mMethodDown.iClickFmConfirmPress(mPara);

                if (mNeedReactBoard)
                    if (Input.GetKeyDown(mBoardKey))
                        mMethodDown.iClickFmConfirmPress(mPara);
            }
        }
    }

    void Update()
    {
        InspectClick();
    }

    //架构需要===========================================

    public interface IClick
    {
        void iClickFmConfirmPress(object para);
    }

    public interface ITouch
    {
        void iHoverInsideFmMouse(object para);
        void iHoverOutsideFmMouse(object para);
    }

}
