using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathAngle
{
    //算法思路是：
    //为了利于思考与分辨，对外反馈的值，一定得在正负360之间
    //面向往右为零度，逆时针为正，顺时针为负的背景

    //角度相关===================================================================
    public static float AngleNip(Vector2 high, Vector2 low)
    {//求两个向量的夹角,取后者对应方向的线的夹角为0，逆时针为正向,上限为180度,下限-180度
        float radian_high = GetRadian(high);
        float radian_low = GetRadian(low);
        float radian_nip;
        //分别求出对应弧度后，作差即得弧度差
        //注意是以low开始计算夹角，需要分类讨论

        float temp_nip = radian_high - radian_low;
        //弧度差上下限分别为2*Mathf.PI与-2*Mathf.PI

        if (temp_nip > Mathf.PI && radian_low < radian_high)
            radian_nip = temp_nip - 2 * Mathf.PI;
        //此时high在下方，low在high的右上方

        else if (temp_nip < -Mathf.PI && radian_low > radian_high)
            radian_nip = 2 * Mathf.PI + temp_nip;
        //此时high在上方，low在high的右下方

        else//-Mathf.PI <= temp_nip <= Mathf.PI
            radian_nip = temp_nip;
        //此时high在下方，low在high的左上方或右下方或左下方
        //此时high在上方，low在high的左下方或右上方或左上方

        return GetAngle(radian_nip);
        //弧度差对应值即两向量间夹角对应的弧度值,由此可逆得出角度数
    }
    public static float GetAngle(float radian)
    {//由所给弧度，反馈出相应度数
        return GetSameAngle360(radian * 180 / Mathf.PI);
    }
    public static float GetAngle(Vector2 ask)
    {//默认返回可对应的正数角度（没有正负标准时，统一为0到360）
        float angle = Vector2.Angle(ask, Vector2.right);
        Vector3 side = Vector3.Cross(ask, Vector3.right);
        if (side.z > 0) angle = 360 - angle;
        return angle;
    }
    public static float GetSameAngle360(float angle)
    {//返回表面上，指定角度所等效的，绝对值会低于360度的度数
        return MathNum.CountRemainder(angle, 360);//去掉周期性
    }

    public static float GetSameAngle180(float angle)
    {//将角度保持在-179至180度之间
        float angle360 = Mathf.Abs(GetSameAngle360(angle));
        if (angle360 > 180) return angle360 - 360;
        else if (angle360 <= -180) return angle360 + 360;
        else return angle360;

    }

    //弧度相关===================================================================
    //以下get_radian函数只会反馈表面上所能等效的
    //低于360度的正度数所对应的弧度，也足够处理所有方向
    public static float GetRadian(Vector2 ask)//求原点至指定坐标的连线，与x轴正向线的夹角弧度
    {//Mathf中arctan函数会把对应坐标，中心对称到cos大于零的坐标情况(其实是因为中心对称的向量，对应tan值相同所致)
     //arctan回馈的值，是斜边相对正右方的弧度，且范围为-Mathf.PI/2到Mathf.PI/2
        float radian = Mathf.Atan(ask.y / ask.x);
        //tan值不受向量是否是单位向量影响，仅受它方向影响
        //这里注意先放第二个形式参数y，再紧接写第一个参数x
        if (ask.x >= 0 && ask.y >= 0)//第一象限
            return radian;
        else if (ask.x < 0 && ask.y < 0)//第三象限
            return radian + Mathf.PI;
        else if (ask.x >= 0 && ask.y < 0)//第四象限
            return 2 * Mathf.PI + radian;
        else//第二象限
            return Mathf.PI + radian;
        //此函数把所有角度看作是从正右方逆时针旋转所得，方便区分不同角度
        //也因此返回的弧度，上限为2*Mathf.PI,下限为0
    }
    public static float GetRadian(float angle)
    {//每一个弧度对应特定角度，特定角度又对应特定sin，cos，tan值
        angle = GetSameAngle360(angle);//等效到一个周期内的度数
        return Mathf.PI * angle / 180;
    }

    public static float GetSameRadian(float radian)
    {
        return MathNum.CountRemainder(radian, 2 * Mathf.PI);
    }

    //坐标相关===================================================================

    public static Vector2 AngleToVector(float angle)
    {
        float radian = GetRadian(angle);//先转弧度
        return RadianToVector(radian);
    }

    public static Vector2 RadianToVector(float radian)
    {
        radian = GetSameRadian(radian);
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 CoordTransfer(Vector2 start, Vector2 toward, float distance)
    {//返回从指定坐标按指定方向行进指定距离后的坐标点数据
        float radian = GetRadian(toward);
        float cos_radian = Mathf.Cos(radian);
        float sin_radian = Mathf.Sin(radian);
        Vector2 end;
        float end_x = start.x + distance * cos_radian;
        float end_y = start.y + distance * sin_radian;
        end = new Vector2(end_x, end_y);
        return end;
    }
    public static Vector2 CoordTransfer(Vector2 start, Vector2 offset)
    {
        Vector2 end;
        float end_x = start.x + offset.x;
        float end_y = start.y + offset.y;
        end = new Vector2(end_x, end_y);
        return end;
    }
    public static Vector2 CoordAgainst(Vector2 pos)
    {
        return new Vector2(-pos.x, -pos.y);
    }
    public static Vector2 CoordXAgainst(Vector2 pos)
    {
        return new Vector2(-pos.x, pos.y);
    }
    public static Vector2 CoordYAgainst(Vector2 pos)
    {
        return new Vector2(pos.x, -pos.y);
    }
    public static Vector2 OffsetByDir(float angle, float distance)
    {//在指定角度下(角度以度数描述)从原点前进指定距离后，与现在位置的坐标差(坐标值相对场景坐标系描述)
     //当路程为1时，所计算结果将会为该角度对应的直角边关系，因为此时斜边为1，投影到x，y的值就是相应边与斜边的比值,也会是相应角度的单位向量
        float radian = GetRadian(angle);
        Vector2 direction = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        return new Vector2(distance * direction.x, distance * direction.y);
    }
    public static Vector2 OffsetByDir(Vector2 toward, float distance)
    {//在指定角度下(角度以相对坐标值描述)从原点前进指定距离后，与现在位置的坐标差
        float radian = GetRadian(toward);
        Vector2 direction = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        return new Vector2(distance * direction.x, distance * direction.y);
    }
    public static Vector2 CoordRotate(Vector2 target, float angle)
    {//将指定坐标点相对原点进行旋转，返回与原点保持距离且旋转指定角度后，所成的坐标点
        float tg_length = target.magnitude;//求长度
        float tg_angle = GetAngle(target);//求角度
        float last_angle = tg_angle + angle;//求旋转后角度
        return OffsetByDir(last_angle, tg_length);
    }

    //共通区，不遵守上面代码的共通规则===============================

    public static int IsAcute(Vector2 one, Vector2 other)
    {//假设它们是向量，函数返回其夹角情况
        float dot=Vector2.Dot(one, other);
        if (dot > 0) return 1;
        else if (MathNum.Approximately(dot, 0)) return 0;
        else return -1;
    }

    public static bool WhetherInAngle(Vector2 other, Vector2 self, Vector2 selfFaceTo, float angleLimit)
    {//此伤害是否在某个夹角内
        Vector2 vectorToOther = other - self;//判断攻击夹角
        float diverge = MathAngle.AngleNip(vectorToOther, selfFaceTo);
        if (diverge > angleLimit * 0.5f)
            return false;
        else
            return true;
    }

    public static float LerpAngle(float a, float b, float t)
    {//Same as Lerp but makes sure the values interpolate correctly when they wrap around
     //     360 degrees.
        float num = MathNum.Repeat(b - a, 360f);
        if (num > 180f)
        {
            num -= 360f;
        }

        return a + num * MathNum.Clamp01(t);
    }


    public static float DeltaAngle(float current, float target)
    {//Calculates the shortest difference between two given angles given in degrees.
        float num = MathNum.Repeat(target - current, 360f);
        if (num > 180f)
        {
            num -= 360f;
        }

        return num;
    }

    public static float MoveTowardsAngle(float current, float target, float maxDelta)
    {//Same as MoveTowards but makes sure the values interpolate correctly when they
     //     wrap around 360 degrees.
        float num = DeltaAngle(current, target);
        if (0f - maxDelta < num && num < maxDelta)
        {
            return target;
        }

        target = current + num;
        return MathNum.MoveTowards(current, target, maxDelta);
    }

}
