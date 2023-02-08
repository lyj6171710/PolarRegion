using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Ŀǰ��ϵ������У�
//ѡ��ѡ�����ĳ�Ա��
//�߸����express��performance
//�����ӽǣ�UI����������塢ѡ��
//�����ṩӲ�ӿڣ����UIԤ�����ݳ��ز�����Ϸ����
//UI���໮��֮���⡢���塢����

//UIϵͳ��������Ϊ������Ϸ���е�ǰ�ᡣ
//UIϵͳ�ƶ���Ϸ���̷�չ����Ϸ�����ܵĿ��ء�

public class UiMager : MonoBehaviour,ISwitchScene
{
    //������������Ҫ���������л�UI���Լ��ṩUI�����Ĺ��ܣ���Ψһ����

    //��UIϵͳ����ֻ���ṩ��������Ч����UI�������ʹ�ã�
    //���������Ϸ���ܵĽű�����Ҫ���ڸ�UIϵͳ�¡�

    //��UIϵͳ��UI���棬�趨��������һ����Σ����ǽ������Ʋ㡣
    //�ò㽫�û�����Ϸ���н���ʱ������Ч����ѡ������ȣ�����һ�ֿ��Ե����趨��״̬��
    //�ò�Ĺ����ɸ�UIϵͳ�ṩ���ò�ľ�����֣�����Ϸ�����Լ�������
    //�ҵ����Լ�����ʱ����ͨ��Ԥ�á�Ԥ��ķ�ʽ�������������Ϸ�����໥������û��ֱ�ӹ�ϵ��
    //�����ѡ���Լ�������ĳ����Ϸ����Ľӿڣ����ƶ�����չ��

    //==============================

    public CtrlFullChart meFull => mChartFullDp;
    public CtrlSingleChart meSingle => mChartSingleDp;
    public CtrlAloneChart meAlone => mChartAloneDp;

    //Ч��������������UI�����������Լ���Ҫ�����ã�����ô��UIϵͳ�ܿ�
    public CtrlFullChart mChartFullDp;//����
    public CtrlSingleChart mChartSingleDp;//Ψһ
    public CtrlAloneChart mChartAloneDp;//�ɸ���

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
    }//�����ֻ�������������

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
        CanvasScaler scaler = ThingRefer.It[ERefer.OverlayNormalCanvas].GetComponent<CanvasScaler>();//�ҵ�����Ӧ�³�����Ҫ��ȫ�ֻ���

        if (meScaler == null) meScaler = GetComponent<CanvasScaler>();
        meScaler.referenceResolution = scaler.referenceResolution;
        meScaler.screenMatchMode = scaler.screenMatchMode;
        meScaler.matchWidthOrHeight = scaler.matchWidthOrHeight;
        meScaler.referencePixelsPerUnit = scaler.referencePixelsPerUnit;
    }

}
