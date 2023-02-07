using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnifiedCursor : MonoBehaviour, ISwitchScene
{
    //获取光标的状态，相对场地、画布、窗口等各种要素的状态

    //该组件需预置挂载于总是自动全屏(刚好占满窗口)的画布物体上

    //光标不是鼠标，是一种二维类型坐标类型输入的抽象，光标只有悬浮功能

    //基于窗口============================================

    public Vector2 meMeterAt => mOver;//鼠标相对屏幕的量值坐标

    public Vector2 mePercentIn => CoordFrame.SuCoordPercentInScreenFromMeter(mOver);

    public Vector2Int SuComputeCursorIn(Vector2Int unit)//鼠标相对区域的位置
    {
        return MathRect.ComputeCoordIn(mCursorPos, unit);
    }

    //基于场景=============================================

    public Vector3 meCursorPos => mCursorPos;

    //---------------------------------------

    public Vector2 meSlideInScene => mOffsetScene;

    //基于画布=============================================

    public Vector2 SuGetCursorLocate(RectTransform refer)//鼠标相对画布的坐标位置
    {//refer指，你得到的鼠标位置，适用于refer所在画布的坐标系，该坐标系的原点在左下角

        return CoordFrame.SuCoordInCanvasFromScreenByMeterF(refer, mOver);

    }

    //-----------------------------------

    public GameObject meUiOver => mUiHover;

    public void SuListenOver(GameObject canvasFocus, string tagSelect)
    {
        mCanvasFocus = canvasFocus.GetComponent<GraphicRaycaster>();
        mTagSelect = tagSelect;
        mListenJust = true;
    }//切换监察状态

    //----------------------

    public Vector2 meSlideInCanvas => mOffsetCanvas;

    //======================================

    GraphicRaycaster mCanvasFocus;
    string mTagSelect;
    bool mListenJust;
    GameObject mUiHover;

    GameObject GetUiOver(Vector3 inputPos, GraphicRaycaster canvasFocus, string tagSelect)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = inputPos;
        List<RaycastResult> results = new List<RaycastResult>();
        canvasFocus.Raycast(pointerEventData, results);
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.tag == tagSelect)
                return result.gameObject;
        }
        return null;
    }//获取指定画布中，指定位置处停留的UI

    void UpdateListen()
    {
        if (meIsSweeping || mListenJust)//鼠标没有移动的时候，之前悬浮在的ui必定被保持而不需再检测(但还存在元素瞬移的问题，但这个问题应可以外界自己解决)
        {//如果刚被要求切换检测对象，暂时会无视鼠标是否移动过
            if (mCanvasFocus && mTagSelect != null && mTagSelect != "")
            {
                GameObject hit = GetUiOver(mOver, mCanvasFocus, mTagSelect);
                if (hit != null)
                    mUiHover = hit;
                else
                    mUiHover = null;
            }
            mListenJust = false;
        }
    }

    //====================================

    Vector2 mOffsetScene;//划动的位移，跨越了多少场景距离
    Vector2 mLastIn;

    Vector2 mOffsetCanvas;//划动的位移，跨越了多少画布距离
    Vector2 mLastAt;

    bool mJustSlide;

    void StartSlide()
    {
        mJustSlide = true;
    }

    void UpdateSlide()
    {
        if (meIsSweeping && UnifiedInput.It.meInConfirm()) 
        {
            if (mJustSlide)
            {
                mOffsetCanvas = Vector2.zero;
                mLastAt = CoordFrame.SuCoordInCanvasFromScreenByMeter(UiMager.meOverRect, mOver);

                mOffsetScene = Vector2.zero;
                mLastIn = CoordFrame.SuCoordInWorldFromScreenByMeter(mOver);

                mJustSlide = false;
                //Mytool.ff(2);
            }
            else
            {
                Vector2 cur = CoordFrame.SuCoordInCanvasFromScreenByMeter(UiMager.meOverRect, mOver);
                mOffsetCanvas = cur - mLastAt;
                mLastAt = cur;

                cur = CoordFrame.SuCoordInWorldFromScreenByMeter(mOver);
                mOffsetScene = cur - mLastIn;
                mLastIn = cur;
            }
        }
        else
        {
            mJustSlide = true;
        }
    }

    //====================================

    public bool meIsSweeping => mSweep.magnitude != 0;

    public Vector2 meOverOffset => mOverOffet;

    //u3d的视口坐标系，屏幕左下角为原点
    //采用视口坐标系的计量单位，相对视口坐标系描述
    Vector2 mSweep;//当前划动
    Vector2 mOverLastMid;
    Vector2 mOverLast;
    Vector2 mOverOffet;
    Vector2 mOver;//悬停位置

    EKindInput mSweepBind;
    float mSweepPeriod;

    public void ExciteOver(Vector2 posInScreen, EKindInput eKind)//需要连续刺激
    {
        if (posInScreen != mOver)
        {
            if (UnifiedInput.IsAvailable(ref mSweepBind, eKind))
            {
                mSweepPeriod = 0.5f;//短时间内，不再接受其它该类输入
                                    //或维持对该输入的识别，不会被挤出去
                mOver = posInScreen;
            }
        }
    }

    void UpdateOver()
    {
        mSweep = mOver - mOverLastMid;
        mOverLastMid = (mOver + mOverLastMid) / 2;//不立即重置到当前位置上,将能有平稳化效果

        mOverOffet = mOver - mOverLast;//精确偏移量
        mOverLast = mOver;

        if (mSweepPeriod < 0)
            mSweepBind = EKindInput.none;
        else
            mSweepPeriod -= Time.deltaTime;
    }

    //====================================

    Vector3 mCursorPos;//鼠标在场景中的三维位置，因为会被外界频繁读取，所以存为变量省性能些

    void Start()
    {
        StartSlide();
    }

    void Update()
    {
        mCursorPos = CoordFrame.SuCoordInWorldFromScreenByMeter(mOver, transform.position);

        UpdateSlide();

        UpdateListen();

        UpdateOver();
    }

    public static UnifiedCursor It;

    public void WhenAwake()
    {
        It = this;
    }

    public void WhenSwitchScene()//适配到新场景中来
    {
    }

}
