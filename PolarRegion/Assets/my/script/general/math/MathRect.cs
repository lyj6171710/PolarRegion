using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathRect : MonoBehaviour
{

    public static float CalcHighestPosY(float inclineHalf, float radianToUR, float rotateNow)
    {//计算一个平面矩形在坐标系中的最高点位置

        float rotateNow180 = MathNum.Abs(MathAngle.GetSameAngle180(rotateNow));
        //一是回归到一个周期内，二是可以无视符号，会发现正负的结果是一样的

        float radianNow;
        if (rotateNow180 <= 90)
        {//现在是右上对角线起决定作用
            radianNow = MathAngle.GetRadian(rotateNow180) + radianToUR;
            //当前旋转状态=旋转前+旋转后
        }
        else
        {//现在是右下对角线起决定作用
            radianNow = MathAngle.GetRadian(rotateNow180) - radianToUR;
        }

        float HighY = MathNum.Abs(inclineHalf * Mathf.Sin(radianNow));

        return HighY;
    }

    public static float CalcHighestPosY(Vector2 size, float rotateNow)
    {//计算一个平面矩形在坐标系中的最高点位置
        float inclineHalf = CalcRectInclineHalf(size);
        float radianToUR = CalcRadianToUR(size);
        //水平放置时，右上对角线与x轴的夹角弧度
        return CalcHighestPosY(inclineHalf, radianToUR, rotateNow);
    }

    public static float CalcRectInclineHalf(Vector2 size)
    {
        float incline = Mathf.Sqrt(size.x * size.x + size.y * size.y);
        return incline / 2;
    }

    public static float CalcRadianToUR(Vector2 size)
    {
        return Mathf.Atan(size.y / size.x);
    }

    //================================

    public static bool SuWhetherInside(Vector2 at, RectMeter area, float pad = 0)
    {//at和area都相对同一坐标系在描述他们自己
     //皆以左下角为原点
        if (at.x < area.leftBottom.x - pad) return false;
        else if (at.y <  area.leftBottom.y - pad) return false;

        if (at.x > area.rightTop.x + pad) return false;
        else if (at.y >  area.rightTop.y + pad) return false;

        return true;
    }

    public static Vector2Int ComputeCoordIn(Vector2 pos, Vector2Int unit)//默认从零开始计数
    {//这里以标准跨度为单位，保障结果有意义
        int coordX = Mathf.FloorToInt(pos.x / unit.x);//unit的x描述x方向上从零延申出的长度
        int coordY = Mathf.FloorToInt(pos.y / unit.y);
        return new Vector2Int(coordX, coordY);
    }

}
