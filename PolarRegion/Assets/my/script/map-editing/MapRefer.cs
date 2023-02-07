using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EMapUse { world, territory, cell, room }//接口层面

public enum EMapUnit { world, territory, cell, wall, tile, grid }//语义层面

public enum EMapIfo { tile, symbol, affair, attach }//信息维度

public static class MapRefer//管理静止不变的数据
{
    public const int cBaseNum = 5;//一个当前视野可见区域的长宽基数
    public const int cNumRow = cBaseNum * 3;//长宽比一般保持4：3
    public const int cNumColumn = cBaseNum * 4;
    public const int cNumSum = cNumRow * cNumColumn;

    public const int cSpan = 1;//默认一个单元格，等长于transform.position值的长度单位
    public const float cSpanStart = 0.5f;

    public const int cRegionNumMax = 10;
    public const int cCellNumMax = 40;

    public const int cLayerMax = 4;//图层上限，共5层
    //图层不仅控制了显示优先级，也代表着逻辑上的高低，会被参照
    //图层状态被地图实例拥有，不被地图元素类型拥有
    public const int cLayerGround = 0;
    public const int cLayerForeGround = 4;
    //第1层是地面层，第2层到4层是物件层，第5层是前景层

    public const KeyCode cKeyArea = KeyCode.C;
    public const KeyCode cKeySymb = KeyCode.S;
    public const KeyCode cKeyLayer = KeyCode.L;
}
//===================================================

public abstract class MapDataHandle
{
    //用来加速对离散型地图所含某个子区域的查询(当本来是以列表结构来存储数据时)
    //离散型地图，不预置一个矩阵容量的数据，而是有内容才存储，没内容就不存储
    //离散型地图，地图内容不穷遍所有地方，也因此单元格的对应序数，无法通过简单计算确定

    Dictionary<Vector2Int, int> mapIndex;//加速查询实际地址，而不是每次遍历
    bool[][] mapHold;//加速确认相关位置是否有单元格
    int maxSpan;

    public int meSum { get { return mapIndex.Count; } }

    public MapDataHandle(int max)
    {
        InitialSelf(max);
    }

    void InitialSelf(int max)
    {
        maxSpan = max;

        mapIndex = new Dictionary<Vector2Int, int>();

        mapHold = new bool[maxSpan][];
        for (int i = 0; i < maxSpan; i++)
            mapHold[i] = new bool[maxSpan];
    }

    protected abstract int ReConstruct();//照顾子类它自己的情况与需求

    public bool SuInitialMap()//使用一个数据集前，外界应手动调用一次该函数
    {
        if (mapIndex == null)//可能出现已经生成对象，但并未发生初始化的情况(特别针对序列化还原时)
        {
            InitialSelf(ReConstruct());
            SuAddOne(Vector2Int.zero);//应至少使得有一个元素，不然容易出问题(这类问题本来就可以避免考虑，没必要考虑)
            return false;//返回值代表是否存在元素，不存在时，已经自动增加了一个元素
        }

        RefreshIndexMap();
        if (mapIndex.Count == 0)
        {
            SuAddOne(Vector2Int.zero);
            return false;
        }
        else
        {
            RefreshHoldMap();
            return true;
        }
    }

    void RefreshIndexMap()
    {
        mapIndex.Clear();
        GetCurHoldToMap((pos, index) => {
            mapIndex.Add(pos, index);});
    }

    protected abstract void GetCurHoldToMap(Action<Vector2Int, int> deal);//子类多次传递数据给deal

    void RefreshHoldMap()
    {
        foreach (Vector2Int use in mapIndex.Keys)
        {
            mapHold[use.x][use.y] = true;
        }
    }

    //---------------------------------------

    public Vector2Int[] SuGetAllHoldPos()
    {
        int i = 0;
        Vector2Int[] poses = new Vector2Int[mapIndex.Count];
        foreach (Vector2Int one in mapIndex.Keys) poses[i++] = one;
        return poses;
    }

    public void SuAddOne(Vector2Int at)
    {
        if (!SuWhetherInRange(at)) return;

        if (!SuWhetherHaveHeld(at))
        {
            AddOne(at);
            mapIndex.Add(at, meSum);//add后的容量是，add前的meSum+1
            mapHold[at.x][at.y] = true;
        }
    }

    protected abstract void AddOne(Vector2Int at);

    public void SuDelOne(Vector2Int at)
    {
        if (!SuWhetherInRange(at)) return;

        if (SuWhetherHaveHeld(at))
        {
            DelOne(at);
            RefreshIndexMap();
            //mapIndex.Remove(at);错误的，因为随着子类列表的remove，
            //被remove的元素之后的各元素，同一序数对应的坐标变了
            mapHold[at.x][at.y] = false;
        }
    }

    protected abstract void DelOne(Vector2Int at);

    //---------------------------------

    public bool SuWhetherHaveHeld(Vector2Int at)
    {
        if (SuWhetherInRange(at))
            return mapHold[at.x][at.y];
        else
            return false;
    }

    public bool SuWhetherInRange(Vector2Int at)
    {
        if (at.x >= 0 && at.y >= 0 && at.x < maxSpan && at.y < maxSpan)
            return true;
        else
            return false;
    }

    public int SuGetIndex(Vector2Int coord)
    {
        if (mapIndex.ContainsKey(coord))
            return mapIndex[coord];
        else
            return -1;
    }

    protected abstract Vector2Int GetCoordStraight(int index);//子类便于知道，这里不便于知道

    public Vector2Int SuGetCoord(int index)
    {
        if (index < 0 || index >= meSum)
            return -1 * Vector2Int.one;
        //对于离散型地图，坐标没有可能为负数的必要
        //坐标一般从零往正数计并利用，所以-1可以表示越界，外界自行处理
        else
            return GetCoordStraight(index);
    }
}