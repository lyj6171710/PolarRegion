using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathVector
{
    public static Vector3 LerpReverse(Vector3 cur, Vector3 to, float alpha)
    {
        float one = (to - cur).magnitude;
        float two = 1 / Mathf.Abs(one - Mathf.Sqrt(one));
        float three = two * alpha;
        Vector3 four = (to - cur) * three;
        return cur + four;
    }

    //==============================

    public static Vector3 ToVector3(this Vector2 v)
    {
        return new Vector3(v.x, v.y, 0);
    }

    public static Vector2 ToVector2(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static Vector3 Product(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static Vector3 Divide(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }

    public static Vector2 Product(Vector2 a, Vector2 b)
    {
        return new Vector2(a.x * b.x, a.y * b.y);
    }

    public static Vector2 Divide(Vector2 a, Vector2 b)
    {
        return new Vector2(a.x / b.x, a.y / b.y);
    }

    public static Vector3Int Sign(this Vector3 v)
    {
        
        return new Vector3Int(MathNum.Sign1(v.x), MathNum.Sign1(v.y), MathNum.Sign1(v.z));
    }

    public static Vector3 Abs(this Vector3 v)
    {
        return new Vector3(MathNum.Abs(v.x), MathNum.Abs(v.y), MathNum.Abs(v.z));
    }

    public static void Lerp(ref Vector2 cur, Vector2 need, float t)//线性插值
    {//注意，如果向量用来反应角度，向量虽然是线性插值，但角度变化就是非线性的了
        if (Vector2.Dot(cur, need) < -0.99f) //相反的方向，无法插值
        {
            need = MathAngle.CoordRotate(need, 90);//短时间内以这种方式变化
            cur = Vector2.Lerp(cur, need, t * 2);
        }
        else
        {
            cur = Vector2.Lerp(cur, need, t);
        }
    }

    //============================

    public static bool IsSmaller(this Vector2Int left, Vector2Int right)
        => left.x <= right.x && left.y <= right.y;

    public static bool IsBigger(this Vector2Int left,Vector2Int right)
        => left.x >= right.x && left.y >= right.y;

    //==========================

    public static int XYOffsetSum(this Vector2Int from, Vector2Int to)
    {
        return MathNum.Abs(from.x - to.x) + MathNum.Abs(from.y - to.y);
    }
}
