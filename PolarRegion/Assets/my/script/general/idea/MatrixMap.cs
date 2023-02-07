using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixMap 
{
    public static List<int> GetAllTurnAt(List<Vector2Int> route)
    {//返回具有的转折点
        List<int> turns = new List<int>();
        if (route.Count > 2) 
        {//转折只可能发生在第二个决定后
            Vector2Int last = route[0];
            for (int i = 2; i < route.Count; i++)
            {
                Vector2Int vary = route[i] - last;//差量
                if (vary.x != 0 && vary.y != 0)//转折的话，水平或竖直，都不可能为0
                    turns.Add(i - 1);
                last = route[i - 1];
            }
        }
        return turns;
    }

    public static int GetFirstSideTurnAt(List<Vector2Int> route)//默认每个元素是两两相邻的，而且在上下左右的一个方向上
    {//返回无向径直路线的终点（回转不算转折）
        if (route == null || route.Count == 0) return -1;
        else if (route.Count == 1) return 0;
        else if (route.Count == 2) return 1;
        else //route.Count > 2
        {//转折只可能发生在第一个决定后、第二个地点后
            Vector2Int last = route[0];
            for (int i = 2; i < route.Count; i++)
            {
                Vector2Int vary = route[i] - last;//差量
                if (vary.x != 0 && vary.y != 0)//转折的话，水平或竖直，都不可能为0
                {
                    return i - 1;
                }
                else
                {
                    last = route[i - 1];
                }
            }
            return route.Count - 1;
        }
    }

    public static int GetFirstTurnAt(List<Vector2Int> route)
    {//返回有向径直路线的终点
        if (route == null || route.Count == 0) return -1;
        else if (route.Count == 1) return 0;
        else if (route.Count == 2) return 1;
        else //route.Count > 2
        {
            Vector2Int last = route[0];
            for (int i = 2; i < route.Count; i++)
            {
                Vector2Int varyNew = route[i] - route[i - 1];//后差量
                Vector2Int varyOld = route[i - 1] - last;//先差量
                if (varyNew != varyOld)//如果不一致，那么就发生了转折
                {
                    //发生转折的情况有多种，左转、右转、回转(折返)
                    return i - 1;
                }
                else
                {
                    last = route[i - 1];
                }
            }
            return route.Count - 1;
        }
    }

    //=================================

    public static List<Vector2Int> GetAllNear8(Vector2Int coordAt, int numRow, int numColumn, System.Action<Vector2Int> GetOneInRange = null)
    {
        //8方向
        List<Vector2Int> nears = null;
        if (GetOneInRange == null)
            nears = new List<Vector2Int>();

        for (int i = -1; i < 2; i++)//增加所有周边元素到一个列表
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)//不增加它自己
                    continue;
                else
                {
                    int nearX = coordAt.x + j;
                    int nearY = coordAt.y + i;
                    if (nearY >= 0 && nearX >= 0 &&
                        nearY < numRow && nearX < numColumn)//不能超出地域范围
                    {
                        Vector2Int nearOne = new Vector2Int(nearX, nearY);
                        if (GetOneInRange == null)
                            nears.Add(nearOne);
                        else
                            GetOneInRange(nearOne);
                    }
                }
            }
        }

        return nears;
    }

    public static List<Vector2Int> GetAllNear4(Vector2Int coordAt, int numRow, int numColumn, System.Action<Vector2Int> GetOneInRange = null)
    {
        //4方向，上下左右
        List<Vector2Int> nears = null;
        if (GetOneInRange == null)
            nears = new List<Vector2Int>();
        
        for (int i = -1; i < 2; i++)
        {
            if (i == 0) continue;
            int nearX = coordAt.x + i;
            if (nearX >= 0 && nearX < numColumn)
            {
                Vector2Int nearOne = new Vector2Int(nearX, coordAt.y);
                if (GetOneInRange == null)
                    nears.Add(nearOne);
                else
                    GetOneInRange(nearOne);
            }
        }

        for (int i = -1; i < 2; i++)//增加所有周边元素到一个列表
        {
            if (i == 0) continue;//不增加它自己
            int nearY = coordAt.y + i;
            if (nearY >= 0 && nearY < numRow)//不能超出地域范围
            {
                Vector2Int nearOne = new Vector2Int(coordAt.x, nearY);
                if (GetOneInRange == null)
                    nears.Add(nearOne);
                else
                    GetOneInRange(nearOne);
            }
        }


        return nears;
    }
}
