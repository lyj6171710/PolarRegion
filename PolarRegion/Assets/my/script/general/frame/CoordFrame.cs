using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoordFrame //FieldScreenCanvas
{
    //��Ϸ�����и�ͨ�����ݼ�Ĺ�ϵ������
    //��Ҫ��� ���ء����ڡ����� ������ά��֮��������ϵ
    //�����ǳ�����ģ����ʵ����ά����
    //�����Ƿ�ӳ����Ĳ���ϵͳ����
    //������ֻ������ʾ��Ϣ��ֻ���άƽ�̵����
    //���������Ӧ�����Դ���Ϊ��׼��������Ϊʵ�ʣ����ڳߴ��µİٷֱ�������λ��
    
    //��Ի����ڵĻ�������(Ƭ��)==============================

    public static Vector2 SuCoordInRectFromCanvas(Vector2 location, RenderMode locMode, RectTransform rectLay, RenderMode rectMode)
    {
        //rectLay�����������ϵ��rectMode����Ŀ������ϵ��ģʽ����ͬģʽ������ֵ�᲻ͬ�����������Ӧ����ģʽ������

        Vector3 aimPoint = SuCoordMeterInScreenFromCanvas(location, locMode);

        return SuCoordInRectFromScreen(aimPoint, rectLay, rectMode);
    }//�õ�position������ĳ��rect�µ��������

    public static Vector2 SuCoordInRectFromWorld(Vector3 posNeed, RectTransform rectLay, RenderMode rectMode)
    {
        //rectLay�����������ϵ��rectMode����Ŀ������ϵ��ģʽ����ͬģʽ������ֵ�᲻ͬ�����������Ӧ����ģʽ������

        Vector3 aimPoint = SuCoordMeterInScreenFromWorld(posNeed);

        return SuCoordInRectFromScreen(aimPoint, rectLay, rectMode);
    }//�õ�position������ĳ��rect�µ��������

    public static Vector2 SuCoordInRectFromScreen(Vector2 at, RectTransform rectLay, RenderMode rectMode)
    {
        Vector2 outVec;//���������localpositionӦ�ô��ڵ�����״̬
        if (rectMode == RenderMode.ScreenSpaceOverlay)//λ��ֵ���벻ͬģʽ���
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectLay, at, null, out outVec);
        }
        else if (rectMode == RenderMode.ScreenSpaceCamera)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectLay, at, Camera.main, out outVec);
        }
        else
        {
            Debug.Log("��֧�ֵ�ģʽ"); outVec = Vector2.zero;
        }
        return outVec;
    }

    //�����Ļ===============================

    public static Vector2 SuCoordMeterInScreenFromWorld(Vector3 posNeed)
    {
        //��Ҫ������Գ�������ϵ��ֵ��Ҫע��transform��positionֵ����һ�������ǳ�������ϵ�µ�����ֵ��

        return Camera.main.WorldToScreenPoint(posNeed); ;
    }

    public static Vector2 SuCoordMeterInScreenFromCanvas(Vector2 location, RenderMode locMode)
    {
        //locationһ����ԴRectTransform.position
        //locModeָʾ�����������Դ

        Vector3 aimPoint;//��ת������Ļ����
        if (locMode == RenderMode.ScreenSpaceOverlay)
        {//Overlayģʽ�£�UGUI���������������Ļ����
            aimPoint = location;
        }
        else if (locMode == RenderMode.ScreenSpaceCamera)
        {//Cameraģʽ�£�UGUI������������ǳ������꣬�������겻��ֵ����Ļ����
            aimPoint = Camera.main.WorldToScreenPoint(location);
        }
        else
        {
            Debug.Log("��֧�ֵ�ģʽ"); aimPoint = Vector3.zero;
        }
        return aimPoint;
    }

    public static Vector2 SuCoordPercentInScreenFromWorld(Vector3 posNeed)
    {
        Vector3 aimPoint = SuCoordMeterInScreenFromWorld(posNeed);
        return new Vector2(aimPoint.x / Screen.width, aimPoint.y / Screen.height);
    }

    public static Vector2 SuCoordPercentInScreenFromCanvas(Vector2 location,RenderMode locMode)
    {
        Vector2 aimPoint = SuCoordMeterInScreenFromCanvas(location, locMode);
        return new Vector2(aimPoint.x / Screen.width, aimPoint.y / Screen.height);
    }

    public static Vector2 SuCoordPercentInScreenFromMeter(Vector2 pos)
    {
        return new Vector2(pos.x / Screen.width, pos.y / Screen.height);
    }

    //��Գ���================================

    public static Vector3 SuCoordInWorldFromScreenByMeter(Vector2 posAtScreen, Vector3 applyTo = default(Vector3))//�ڳ����е���άλ��
    {
        Vector3 referPos = Camera.main.WorldToScreenPoint(applyTo);
        //���ȣ�ȷ������ȡ����������ϵ������ת��Ϊ��Ļ����ϵ������Ҫz�����ݣ�
        //��������˵�ǿ���һ����Ļ�ռ���������z��Ӱ��
        Vector3 posOnScreen = posAtScreen;
        posOnScreen.z = referPos.z;
        //���õ���z�����ݸ��������z�ᣬʹ��Ļ������Ŀ��������괦��ͬһ�����ϣ�
        Vector3 posInWorld = Camera.main.ScreenToWorldPoint(posOnScreen);
        //���ϵͳ�����������������ģʽ�£�ǰ��������ֵ����䣬�Ͳ��ÿ��ǲ�����z��ֵ��
        //�������������͸��ģʽ���㻹��Ҫָ��һ���棬��Ϊ��Ұ��֮����������ϵ������������һ�£��������ȣ�ϵͳ����֪����ָ����������
        return posInWorld;
    }

    //��Ի���=======================================

    public static Vector2 SuGetCornerBL(RectTransform area)
    {
        float percent = area.pivot.x;
        float cut = area.sizeDelta.x * percent;
        float bl_x = area.position.x - cut;//position������localposition�����Եõ������꣬���������������

        percent = area.pivot.y;
        cut = area.sizeDelta.y * percent;
        float bl_y = area.position.y - cut;

        return new Vector2(bl_x, bl_y);
    }//��ȡԪ��rect���½ǵĻ�������

    public static Vector2 SuGetCornerTR(RectTransform area)
    {
        float percent = area.pivot.x;
        float cut = area.sizeDelta.x * percent;
        float tr_x = area.position.x + cut;

        percent = area.pivot.y;
        cut = area.sizeDelta.y * percent;
        float tr_y = area.position.y + cut;

        return new Vector2(tr_x, tr_y);
    }//��ȡԪ��rect���ϽǵĻ�������

    public static Vector2 SuCoordInCanvasFromScreenByPercent(RectTransform canvasIn, Vector2 posWant)
    {
        //�������Ҫ���ǰٷֱ�λ������Ȼ��ת����������Ե�����״̬
        //����Ĳ���������Ļ���½�Ϊԭ�㣬����Ļ���Ͻ�Ϊ(1��1)������
        Vector2 posAtScreen = new Vector2(Screen.width * posWant.x, Screen.height * posWant.y);
        return SuCoordInCanvasFromScreenByMeter(canvasIn, posAtScreen);
    }

    public static Vector2 SuCoordInCanvasFromScreenByMeter(RectTransform canvasIn, Vector2 posAtScreen)
    {
        Vector2 screen_pos;
        screen_pos.x = posAtScreen.x - (Screen.width / 2);//ת��Ϊ����Ļ����Ϊԭ�����Ļ����(ֻ���м����)
        screen_pos.y = posAtScreen.y - (Screen.height / 2);
        Vector2 ui_pos;//UI���꣬Ӧ����localPosition���͵�ֵ
        ui_pos.x = screen_pos.x * (canvasIn.sizeDelta.x / Screen.width);//ת�������Ļ����*��������Ļ��߱�
        ui_pos.y = screen_pos.y * (canvasIn.sizeDelta.y / Screen.height);
        return ui_pos;//��Ի�������ϵ������
    }

    public static Vector2 SuCoordInCanvasFromScreenByMeterF(RectTransform refer, Vector2 posAtScreen)
    {//referָ����õ���λ�ã�������refer���ڻ���������ϵ��������ϵ��ԭ�������½ǣ�Ӧ���ڸû����ϣ�����λ���غϵ�Ч��

        RectTransform canvasLay;
        Canvas canvas = refer.GetComponent<Canvas>();
        if (canvas != null)
            canvasLay = refer;
        else
        {
            canvas = refer.GetComponentInParent<Canvas>();
            canvasLay = canvas.GetComponent<RectTransform>();
        }

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            return posAtScreen;//��Ļ������ǻ�������
        }
        else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(posAtScreen);//��Ļ����ת����������
            Vector2 uiPos = canvasLay.InverseTransformPoint(worldPos);//��������ת��Ϊ��������
            return uiPos;
        }
        return Vector2.zero;
    }

}
