using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoordFrame //FieldScreenCanvas
{
    //游戏场景中各通常数据间的关系的利用
    //主要针对 场地、窗口、画布 这三个维度之间的坐标关系
    //场地是程序中模拟现实的三维数据
    //窗口是反映程序的操作系统窗口
    //画布是只用于提示信息而只需二维平铺的面板
    //理想情况，应该是以窗口为标准，窗口最为实际，窗口尺寸下的百分比来描述位置
    
    //相对画布内的划分区域(片区)==============================

    public static Vector2 SuCoordInRectFromCanvas(Vector2 location, RenderMode locMode, RectTransform rectLay, RenderMode rectMode)
    {
        //rectLay是面向的坐标系，rectMode描述目标坐标系的模式，不同模式，坐标值会不同，这里给出适应所需模式的坐标

        Vector3 aimPoint = SuCoordMeterInScreenFromCanvas(location, locMode);

        return SuCoordInRectFromScreen(aimPoint, rectLay, rectMode);
    }//得到position坐标在某个rect下的相对坐标

    public static Vector2 SuCoordInRectFromWorld(Vector3 posNeed, RectTransform rectLay, RenderMode rectMode)
    {
        //rectLay是面向的坐标系，rectMode描述目标坐标系的模式，不同模式，坐标值会不同，这里给出适应所需模式的坐标

        Vector3 aimPoint = SuCoordMeterInScreenFromWorld(posNeed);

        return SuCoordInRectFromScreen(aimPoint, rectLay, rectMode);
    }//得到position坐标在某个rect下的相对坐标

    public static Vector2 SuCoordInRectFromScreen(Vector2 at, RectTransform rectLay, RenderMode rectMode)
    {
        Vector2 outVec;//这个坐标是localposition应该处在的坐标状态
        if (rectMode == RenderMode.ScreenSpaceOverlay)//位置值会与不同模式相关
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectLay, at, null, out outVec);
        }
        else if (rectMode == RenderMode.ScreenSpaceCamera)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectLay, at, Camera.main, out outVec);
        }
        else
        {
            Debug.Log("不支持的模式"); outVec = Vector2.zero;
        }
        return outVec;
    }

    //相对屏幕===============================

    public static Vector2 SuCoordMeterInScreenFromWorld(Vector3 posNeed)
    {
        //需要传入相对场景坐标系的值（要注意transform的position值，不一定代表是场景坐标系下的坐标值）

        return Camera.main.WorldToScreenPoint(posNeed); ;
    }

    public static Vector2 SuCoordMeterInScreenFromCanvas(Vector2 location, RenderMode locMode)
    {
        //location一般来源RectTransform.position
        //locMode指示所给坐标的来源

        Vector3 aimPoint;//先转换到屏幕坐标
        if (locMode == RenderMode.ScreenSpaceOverlay)
        {//Overlay模式下，UGUI的世界坐标就是屏幕坐标
            aimPoint = location;
        }
        else if (locMode == RenderMode.ScreenSpaceCamera)
        {//Camera模式下，UGUI的世界坐标就是场景坐标，场景坐标不等值于屏幕坐标
            aimPoint = Camera.main.WorldToScreenPoint(location);
        }
        else
        {
            Debug.Log("不支持的模式"); aimPoint = Vector3.zero;
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

    //相对场地================================

    public static Vector3 SuCoordInWorldFromScreenByMeter(Vector2 posAtScreen, Vector3 applyTo = default(Vector3))//在场景中的三维位置
    {
        Vector3 referPos = Camera.main.WorldToScreenPoint(applyTo);
        //首先，确定（获取）世界坐标系，将其转化为屏幕坐标系；（需要z轴数据）
        //这样做，说是考虑一个屏幕空间坐标的深度z的影响
        Vector3 posOnScreen = posAtScreen;
        posOnScreen.z = referPos.z;
        //将拿到的z轴数据赋给坐标的z轴，使屏幕坐标与目标对象坐标处于同一层面上；
        Vector3 posInWorld = Camera.main.ScreenToWorldPoint(posOnScreen);
        //这个系统函数，在摄像机正交模式下，前两个轴数值不会变，就不用考虑参数的z轴值。
        //但当摄像机处于透视模式，你还需要指定一个面，因为视野面之间是伸缩关系，不过比例上一致，你给个深度，系统将能知道你指定的面在哪
        return posInWorld;
    }

    //相对画布=======================================

    public static Vector2 SuGetCornerBL(RectTransform area)
    {
        float percent = area.pivot.x;
        float cut = area.sizeDelta.x * percent;
        float bl_x = area.position.x - cut;//position，而非localposition，所以得到的坐标，是相对所处画布的

        percent = area.pivot.y;
        cut = area.sizeDelta.y * percent;
        float bl_y = area.position.y - cut;

        return new Vector2(bl_x, bl_y);
    }//获取元素rect左下角的画布坐标

    public static Vector2 SuGetCornerTR(RectTransform area)
    {
        float percent = area.pivot.x;
        float cut = area.sizeDelta.x * percent;
        float tr_x = area.position.x + cut;

        percent = area.pivot.y;
        cut = area.sizeDelta.y * percent;
        float tr_y = area.position.y + cut;

        return new Vector2(tr_x, tr_y);
    }//获取元素rect右上角的画布坐标

    public static Vector2 SuCoordInCanvasFromScreenByPercent(RectTransform canvasIn, Vector2 posWant)
    {
        //这里参数要求是百分比位置需求，然后转换到具体绝对的坐标状态
        //这里的参数，以屏幕左下角为原点，以屏幕右上角为(1，1)来描述
        Vector2 posAtScreen = new Vector2(Screen.width * posWant.x, Screen.height * posWant.y);
        return SuCoordInCanvasFromScreenByMeter(canvasIn, posAtScreen);
    }

    public static Vector2 SuCoordInCanvasFromScreenByMeter(RectTransform canvasIn, Vector2 posAtScreen)
    {
        Vector2 screen_pos;
        screen_pos.x = posAtScreen.x - (Screen.width / 2);//转换为以屏幕中心为原点的屏幕坐标(只是中间过程)
        screen_pos.y = posAtScreen.y - (Screen.height / 2);
        Vector2 ui_pos;//UI坐标，应该是localPosition类型的值
        ui_pos.x = screen_pos.x * (canvasIn.sizeDelta.x / Screen.width);//转换后的屏幕坐标*画布与屏幕宽高比
        ui_pos.y = screen_pos.y * (canvasIn.sizeDelta.y / Screen.height);
        return ui_pos;//相对画布坐标系的坐标
    }

    public static Vector2 SuCoordInCanvasFromScreenByMeterF(RectTransform refer, Vector2 posAtScreen)
    {//refer指，你得到的位置，适用于refer所在画布的坐标系，该坐标系的原点在左下角，应用在该画布上，会有位置重合的效果

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
            return posAtScreen;//屏幕坐标就是画布坐标
        }
        else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(posAtScreen);//屏幕坐标转换世界坐标
            Vector2 uiPos = canvasLay.InverseTransformPoint(worldPos);//世界坐标转换为本地坐标
            return uiPos;
        }
        return Vector2.zero;
    }

}
