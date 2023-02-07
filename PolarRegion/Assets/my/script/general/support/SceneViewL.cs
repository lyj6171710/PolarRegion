using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SceneViewL : MonoBehaviour, ISwitchScene
{
    //�����ܰ�������Ұview������window����Ļscreen��Ĺ�ϵ�����ƴ��������ṩ��������
    //��Ϸ���ݾ���������Ұ�༭�빹��

    //=========================================

    //window��view������
    //screen��window������

    public int meViewWidth { get; }
    public int meViewHeight { get; }

    public Vector3 SuPosRatioInView(Vector3 worldPos)
    {
        return Camera.main.WorldToViewportPoint(worldPos);
    }

    public bool SuWhetherInView(Vector3 worldPos)//�ж�һ���������Ƿ��������Χ��
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

    public bool SuWhetherByView(Vector3 worldPos)//�Ƿ������������
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
            Screen.orientation = ScreenOrientation.LandscapeLeft;//����
        else if (dir == EToward4.down)
            Screen.orientation = ScreenOrientation.PortraitUpsideDown;//����
    }

    public void SuVaryResoWidth(int pixels)//����ֱ���
    {
        Screen.SetResolution(pixels, Screen.height, false);
        //Screen.height������ָ���÷ֱ���
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

    public void SuAdaptDevice()//����
    {
        Resolution[] resolutions = Screen.resolutions;//��ȡ���õ�ǰ��Ļ�ֱ���
        //����ӿ�Ӧ����ָ����Ϸ�ӿڶ�Ӳ����Ļռ�õ������������ǣ�������ռ��������ǰ�ӿڴ�С������ռ����û��Ӱ��
        Screen.SetResolution(resolutions[resolutions.Length - 1].width, resolutions[resolutions.Length - 1].height, true);//���õ�ǰ�ֱ���
        Screen.fullScreen = true;  //���ó�ȫ��
    }

    //====================================================

    //�κ�����Ұ��С�ı�/�ƶ��Ĳ�������Ӧ��ʹ������Ľӿ�

    public Action meWhenViewableSizeChange;//���ӷ�Χ�ı�ʱ

    //------------------------------

    Vector2Int mSizeViewCur;
    Vector2Int mSizeViewLast;
    float mRatioSizeHW;//��ǰ�߿��
    public Action meWhenViewSizeChange;

    public Vector2Int SuGetSizeViewCur()
    {
        return new Vector2Int(Screen.width, Screen.height);//�����ʵ�����
        //return new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height);//������������
    }

    public void SuSetSizeViewCur(Vector2Int need)//���Ǹı��ӿڴ�С�ģ�����չʾ��������Ϸ�������ᷢ���仯
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
                Camera.main.orthographicSize *= ratio;  //�����ӿ����������󣬵ȱ����������ʾ�ĳ���������
                                                        //�ɴ�����Ϸ���ݴ�Сʼ�ձ��ֲ���
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
    {//�ƺ���lateUpdate�������ã���Ȼ���ܶ���
        if (offset.magnitude != 0)
        {
            mPosLast = Camera.main.transform.position;
            Camera.main.transform.Translate(offset);//��Ա仯��ã������п��ƿռ�
            meWhenViewMove();
        }
    }

    //------------------------------

    float mSizeContentCur;//��Ϸ���ݵĴ�С(ȫ��)
                          //��Ϸ���ݴ�С�ĸı䣬Ҫô��Ϸ���ݱ�������(����)��Ҫô�ı�����Ϸ���������յ�λ�Ĵ�С(ȫ��)

    [Range(3, 10)]
    public float mSizeContentStartDp = 6;

    public void SuOffsetViewContentSize(float offset)//��Ӹı���Ұ��С
    {
        if (Camera.main.orthographicSize > 1)//����Ϊ1
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
