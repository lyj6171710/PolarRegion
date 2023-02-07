using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SceneViewL : MonoBehaviour, ISwitchScene
{
    //尽可能包揽，视野view，窗口window，屏幕screen间的关系的妥善处理，对外提供辅助功能
    //游戏内容尽量面向视野编辑与构造

    //=========================================

    //window是view的上限
    //screen是window的上限

    public int meViewWidth { get; }
    public int meViewHeight { get; }

    public Vector3 SuPosRatioInView(Vector3 worldPos)
    {
        return Camera.main.WorldToViewportPoint(worldPos);
    }

    public bool SuWhetherInView(Vector3 worldPos)//判断一个场景点是否在相机范围内
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(worldPos);
        if (viewPos.x > 0 && viewPos.x < 1)
        {
            if (viewPos.y > 0 && viewPos.y < 1)
            {
                if (viewPos.z >= Camera.main.nearClipPlane && viewPos.z <= Camera.main.farClipPlane)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool SuWhetherByView(Vector3 worldPos)//是否在相机视线内
    {
        Vector3 dir = (Camera.main.transform.position - worldPos).normalized;
        float dot = Vector3.Dot(Camera.main.transform.forward, dir);
        return dot > 0;
    }

    public Vector2 SuGetCornerOffsetFromViewCentre(float depth = 0, bool perspectiveMode = false)
    {
        if (perspectiveMode)
        {
            float halfFOV = MathAngle.GetRadian(Camera.main.fieldOfView / 2);

            float height = depth * Mathf.Tan(halfFOV);
            float width = height * Camera.main.aspect;

            return new Vector2(width, height);
        }
        else
        {
            float height = Camera.main.orthographicSize;
            float width = height * Camera.main.aspect;

            return new Vector2(width, height);
        }
    }

    //==============================================

    public const int cScreenWidth = 1080;
    public const int cScreenHeight = 1920;

    public void SuMakeLay(EToward4 dir)
    {
        if (dir == EToward4.left)
            Screen.orientation = ScreenOrientation.LandscapeLeft;//横屏
        else if (dir == EToward4.down)
            Screen.orientation = ScreenOrientation.PortraitUpsideDown;//竖屏
    }

    public void SuVaryResoWidth(int pixels)//横向分辨率
    {
        Screen.SetResolution(pixels, Screen.height, false);
        //Screen.height好像是指可用分辨率
    }

    public void SuVaryResoHeight(int pixels)
    {
        Screen.SetResolution(Screen.width, pixels, false);
    }

    public void SuTakeNewResolution(int needX, int needY)
    {
        Screen.SetResolution(needX, needY, false);
    }

    public void SuResetResolution()
    {
        Screen.SetResolution(cScreenWidth, cScreenHeight, false);
    }

    public void SuAdaptDevice()//待定
    {
        Resolution[] resolutions = Screen.resolutions;//获取设置当前屏幕分辩率
        //这个接口应该是指，游戏视口对硬件屏幕占用的像素量？不是，是理想占用量，当前视口大小对理想占用量没有影响
        Screen.SetResolution(resolutions[resolutions.Length - 1].width, resolutions[resolutions.Length - 1].height, true);//设置当前分辨率
        Screen.fullScreen = true;  //设置成全屏
    }

    //====================================================

    //任何让视野大小改变/移动的操作，都应该使用这里的接口

    public Action meWhenViewableSizeChange;//可视范围改变时

    //------------------------------

    Vector2Int mSizeViewCur;
    Vector2Int mSizeViewLast;
    float mRatioSizeHW;//当前高宽比
    public Action meWhenViewSizeChange;

    public Vector2Int SuGetSizeViewCur()
    {
        return new Vector2Int(Screen.width, Screen.height);//这个是实际情况
        //return new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height);//这个是理想情况
    }

    public void SuSetSizeViewCur(Vector2Int need)//这是改变视口大小的，可以展示出来的游戏内容量会发生变化
    {
        mSizeViewCur = need;
        mRatioSizeHW = need.y / need.x;
        Screen.SetResolution(need.x, need.y, false);
    }

    public void SuTranslateViewSize(float offset)
    {
        float to = offset + 1;
        SuSetSizeViewCur(new Vector2Int((int)(to * mSizeViewCur.x), (int)(to * mSizeViewCur.y)));
    }

    void UpdateViewSize()
    {
        mSizeViewCur = SuGetSizeViewCur();
        if (mSizeViewLast != mSizeViewCur)
        {
            float ratio = (float)mSizeViewCur.x / mSizeViewLast.x;
            if (Camera.main.orthographic)
            {
                Camera.main.orthographicSize *= ratio;  //随着视口容量的增大，等比增大可以显示的场景网格量
                                                        //由此让游戏内容大小始终保持不变
            }
            meWhenViewSizeChange();
            meWhenViewableSizeChange();
            mSizeViewLast = mSizeViewCur;
        }
    }

    //------------------------------

    public Action meWhenViewMove;
    Vector2 mPosLast;

    public void SuTranslateView(Vector2 offset)
    {//似乎在lateUpdate里调用最好，不然可能抖动
        if (offset.magnitude != 0)
        {
            mPosLast = Camera.main.transform.position;
            Camera.main.transform.Translate(offset);//相对变化最好，外界可有控制空间
            meWhenViewMove();
        }
    }

    //------------------------------

    float mSizeContentCur;//游戏内容的大小(全局)
                          //游戏内容大小的改变，要么游戏内容本身伸缩(个体)，要么改变了游戏内容所参照单位的大小(全局)

    [Range(3, 10)]
    public float mSizeContentStartDp = 6;

    public void SuOffsetViewContentSize(float offset)//间接改变视野大小
    {
        if (Camera.main.orthographicSize > 1)//下限为1
        {
            mSizeContentCur += offset;
            Camera.main.orthographicSize = mSizeContentCur;
            meWhenViewableSizeChange();
        }
    }

    //------------------------------

    void AwakeViewSize()
    {
        meWhenViewSizeChange += () => { };
        mSizeViewLast = SuGetSizeViewCur();
        mSizeViewCur = SuGetSizeViewCur();
        mRatioSizeHW = mSizeViewCur.y / mSizeViewCur.x;

        //-----------------------

        Camera.main.orthographicSize = mSizeContentStartDp;
        mSizeContentCur = Camera.main.orthographicSize;

        //-----------------------

        meWhenViewMove += () => { };
        mPosLast = Camera.main.transform.position;
    }

    //====================================================

    public static SceneViewL It;

    public void WhenAwake()
    {
        It = this;
        AwakeViewSize();
    }

    public void WhenSwitchScene()
    {

    }

    void Update()
    {
        UpdateViewSize();
    }

}
