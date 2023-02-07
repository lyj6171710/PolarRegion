using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnifiedCursor : MonoBehaviour, ISwitchScene
{
    //��ȡ����״̬����Գ��ء����������ڵȸ���Ҫ�ص�״̬

    //�������Ԥ�ù����������Զ�ȫ��(�պ�ռ������)�Ļ���������

    //��겻����꣬��һ�ֶ�ά����������������ĳ��󣬹��ֻ����������

    //���ڴ���============================================

    public Vector2 meMeterAt => mOver;//��������Ļ����ֵ����

    public Vector2 mePercentIn => CoordFrame.SuCoordPercentInScreenFromMeter(mOver);

    public Vector2Int SuComputeCursorIn(Vector2Int unit)//�����������λ��
    {
        return MathRect.ComputeCoordIn(mCursorPos, unit);
    }

    //���ڳ���=============================================

    public Vector3 meCursorPos => mCursorPos;

    //---------------------------------------

    public Vector2 meSlideInScene => mOffsetScene;

    //���ڻ���=============================================

    public Vector2 SuGetCursorLocate(RectTransform refer)//�����Ի���������λ��
    {//referָ����õ������λ�ã�������refer���ڻ���������ϵ��������ϵ��ԭ�������½�

        return CoordFrame.SuCoordInCanvasFromScreenByMeterF(refer, mOver);

    }

    //-----------------------------------

    public GameObject meUiOver => mUiHover;

    public void SuListenOver(GameObject canvasFocus, string tagSelect)
    {
        mCanvasFocus = canvasFocus.GetComponent<GraphicRaycaster>();
        mTagSelect = tagSelect;
        mListenJust = true;
    }//�л����״̬

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
    }//��ȡָ�������У�ָ��λ�ô�ͣ����UI

    void UpdateListen()
    {
        if (meIsSweeping || mListenJust)//���û���ƶ���ʱ��֮ǰ�����ڵ�ui�ض������ֶ������ټ��(��������Ԫ��˲�Ƶ����⣬���������Ӧ��������Լ����)
        {//����ձ�Ҫ���л���������ʱ����������Ƿ��ƶ���
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

    Vector2 mOffsetScene;//������λ�ƣ���Խ�˶��ٳ�������
    Vector2 mLastIn;

    Vector2 mOffsetCanvas;//������λ�ƣ���Խ�˶��ٻ�������
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

    //u3d���ӿ�����ϵ����Ļ���½�Ϊԭ��
    //�����ӿ�����ϵ�ļ�����λ������ӿ�����ϵ����
    Vector2 mSweep;//��ǰ����
    Vector2 mOverLastMid;
    Vector2 mOverLast;
    Vector2 mOverOffet;
    Vector2 mOver;//��ͣλ��

    EKindInput mSweepBind;
    float mSweepPeriod;

    public void ExciteOver(Vector2 posInScreen, EKindInput eKind)//��Ҫ�����̼�
    {
        if (posInScreen != mOver)
        {
            if (UnifiedInput.IsAvailable(ref mSweepBind, eKind))
            {
                mSweepPeriod = 0.5f;//��ʱ���ڣ����ٽ���������������
                                    //��ά�ֶԸ������ʶ�𣬲��ᱻ����ȥ
                mOver = posInScreen;
            }
        }
    }

    void UpdateOver()
    {
        mSweep = mOver - mOverLastMid;
        mOverLastMid = (mOver + mOverLastMid) / 2;//���������õ���ǰλ����,������ƽ�Ȼ�Ч��

        mOverOffet = mOver - mOverLast;//��ȷƫ����
        mOverLast = mOver;

        if (mSweepPeriod < 0)
            mSweepBind = EKindInput.none;
        else
            mSweepPeriod -= Time.deltaTime;
    }

    //====================================

    Vector3 mCursorPos;//����ڳ����е���άλ�ã���Ϊ�ᱻ���Ƶ����ȡ�����Դ�Ϊ����ʡ����Щ

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

    public void WhenSwitchScene()//���䵽�³�������
    {
    }

}
