using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OverTool
{
    const float cSqrtHalf = 0.717f;

    public static Vector2 TowardToVector(EToward4 toward)
    {
        switch (toward)
        {
            case EToward4.up: return Vector2.up;
            case EToward4.down: return Vector2.down;
            case EToward4.left: return Vector2.left;
            case EToward4.right: return Vector2.right;
            default: return Vector2.zero;
        }
    }

    public static Vector2 TowardToVector(EToward8 toward)
    {
        if (toward.toInt() < 5)
            return TowardToVector((EToward4)toward.toInt());
        else
        {
            switch (toward)
            {
                case EToward8.upLeft: return new Vector2(-cSqrtHalf, cSqrtHalf);
                case EToward8.upRight: return new Vector2(cSqrtHalf, cSqrtHalf);
                case EToward8.downLeft: return new Vector2(-cSqrtHalf, -cSqrtHalf);
                case EToward8.downRight: return new Vector2(cSqrtHalf, -cSqrtHalf);
                default: return Vector2.zero;
            }
        }
    }

    public static EToward4 IntToToward(int index)
    {
        switch (index)
        {
            case 0: return EToward4.up;
            case 1: return EToward4.down;
            case 2: return EToward4.left;
            case 3: return EToward4.right;
            default: return EToward4.middle;
        }
    }

    public static Vector2Int TowardToVector(ETowardX4 toward)
    {
        switch (toward)
        {
            case ETowardX4.upLeft: return new Vector2Int(-1, 1);
            case ETowardX4.upRight: return new Vector2Int(1, 1);
            case ETowardX4.downLeft: return new Vector2Int(-1, -1);
            case ETowardX4.downRight: return new Vector2Int(1, -1);
            default: return Vector2Int.zero;
        }
    }

    public static EToward4 Reverse(this EToward4 origin)
    {
        switch (origin)
        {
            case EToward4.up:return EToward4.down;
            case EToward4.down:return EToward4.up;
            case EToward4.left:return EToward4.right;
            case EToward4.right:return EToward4.left;
            default:return EToward4.middle;
        }
    }

    public static List<Vector2Int> GetAround(Vector2Int centre)
    {
        List<Vector2Int> around = new List<Vector2Int>();
        //超过范围的坐标，将应能被自动忽略过滤
        around.Add(new Vector2Int(centre.x, centre.y + 1));//上
        around.Add(new Vector2Int(centre.x, centre.y - 1));//下
        around.Add(new Vector2Int(centre.x - 1, centre.y));//左
        around.Add(new Vector2Int(centre.x + 1, centre.y));//右
        around.Add(new Vector2Int(centre.x - 1, centre.y + 1));//左上
        around.Add(new Vector2Int(centre.x + 1, centre.y - 1));//右下
        around.Add(new Vector2Int(centre.x - 1, centre.y - 1));//左下
        around.Add(new Vector2Int(centre.x + 1, centre.y + 1));//右上
        return around;
    }

}