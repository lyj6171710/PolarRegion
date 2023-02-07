using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class FieldGridMager : IGridOperate
{
    //功能上，承包对若干个方形单元格，两两相邻的安放，且一定都是按规律安放的
    //地位上，其不可或缺性在于有直观表现，相关于场景，是场景中的实例

    //有逻辑坐标系和实际坐标系的内部逻辑
    //逻辑坐标系上，x往右方向增大，y往上方向增大，起点在于(0,0)
    //实际坐标系，原点可以在世界坐标系上的任何位置，且随着x的增大，实际x增大的方向也不一定是往右的

    //实际坐标状态后于逻辑坐标状态，能用逻辑坐标解决，就不用实际坐标解决。
    //可以确定的是，管理器所根据的坐标，一定是逻辑坐标，之后一般再转换到实际坐标来解决问题

    public Action<Vector2Int> meWhenHoverOneGridNew;//使用网格的，不只某一个对象
    
    public FieldGrid this[Vector2Int coord] => mGrids[coord];

    public void SuForeach(Action<Vector2Int> eachDo)
    {
        if (mGrids.Count > 0)
            foreach (Vector2Int coord in mGrids.Keys) eachDo(coord);
    }

    //------------------------------------

    protected Transform mParent;
    protected Vector2 mScale;
    protected float mSpan;
    protected Vector2 mPosBegin;//相对位置的初始值
    //描述起点位置，且为起始单元格的中心的位置所在
    //这里说的起始单元格，不是第一个单元格，而是坐标为(0，0)时所对应的单元格

    protected Dictionary<Vector2Int, FieldGrid> mGrids;
    //网格不建议使用列表，因为删除局部是可以很频繁的
    //而且单元格的坐标，才是最代表单元格意义的属性
    protected Vector2Int mSmallest;
    protected Vector2Int mBiggest;
    
    IGridOperate mCallback;//使用者也可以响应

    public void MakeReady(Transform parent, IGridOperate operater)
    {
        mGrids = new Dictionary<Vector2Int, FieldGrid>();
        mCallback = operater;
        meWhenHoverOneGridNew += (i) => { };
        mParent = parent;
    }

    public void ReadyAttr(Vector2 begin, float span, Vector2 scale)
    {
        mScale = scale;
        mSpan = span;
        mPosBegin = begin;
    }

    public void IWhenHoverGridFmOpera(Vector2Int coord)
    {
        mCallback.IWhenHoverGridFmOpera(coord);
        meWhenHoverOneGridNew(coord);
    }

    //=====================================

    public GameObject SuAddOneGrid(Vector2Int coordAt)
    {
        AddOneGrid(coordAt, mPosBegin + mSpan * mScale * coordAt);
        return mGrids[coordAt].gameObject;
    }

    public void SuRemoveOneGrid(Vector2Int coordAt)
    {
        GameObject.Destroy(mGrids[coordAt].gameObject);
        mGrids.Remove(coordAt);
    }

    public void RemoveAllGrids()
    {
        if (mGrids != null && mGrids.Count > 0)
        {//清空网格单元
            List<Vector2Int> coords = new List<Vector2Int>();
            foreach (Vector2Int coord in mGrids.Keys) coords.Add(coord);
            
            for (int i = 0; i < coords.Count; i++)
            {
                SpriteCollidPool.It.Revert(mGrids[coords[i]].gameObject);
                mGrids.Remove(coords[i]);
            }
        }
    }

    //=======================================

    public int meGridsNum => mGrids.Count;

    public GameObject GetAnyOne(out Vector2Int at)
    {
        foreach (var coord in mGrids.Keys)
        {
            at = coord;
            return mGrids[coord].gameObject;
        }
        at = -Vector2Int.one;
        return null;
    }

    public abstract bool SuWhetherCursorInField();//基类无法判断是否在区域内，这相关于单元格的安放策略

    public Vector2Int SuNearestCoord(Vector2Int expect)//取得离指定坐标最近的坐标
    {
        if (mGrids.ContainsKey(expect))
            return expect;
        else
        {
            int shortest = int.MaxValue;
            Vector2Int nearest = -Vector2Int.one;
            foreach (Vector2Int coord in mGrids.Keys)
            {
                int offsetSum = expect.XYOffsetSum(coord);
                if (offsetSum < shortest) 
                {
                    shortest = offsetSum;
                    nearest = coord;
                }
            }
            return nearest;
        }
    }

    public bool SuContainCoord(Vector2Int coord) => mGrids.ContainsKey(coord);

    public bool SuWhetherCursorIn(Vector2Int coord)
    {
        if (!mGrids.ContainsKey(coord)) return false;
        Vector3 cursorPos = UnifiedCursor.It.meCursorPos;
        RectMeter gridRect = GetRectHold(coord);
        return MathRect.SuWhetherInside(cursorPos, gridRect);
    }

    public Vector3 SuGetPos(Vector2Int coord)
    {
        return mGrids[coord].transform.position;
    }

    public abstract Vector2Int SuGetCoordPosIn(Vector2 pos);

    public Vector2Int SuGetCoordDirInWorld()
    {
        return OverTool.TowardToVector(GetCoordDirInWorld());
    }

    //内部工具=================================

    protected void AddOneGrid(Vector2Int coord, Vector2 posAsk)
    {
        GameObject unit = SpriteCollidPool.It.GetPure(mParent, "grid");
        Vector3 pos = new Vector3(posAsk.x, posAsk.y, 0);
        unit.transform.localPosition = pos;
        unit.transform.localScale *= mScale;
        FieldGrid grid = unit.AddComponent<FieldGrid>();
        grid.MakeReady(coord, this);
        mGrids.Add(coord, grid);
        if(coord.IsBigger(mBiggest))mBiggest = coord;
        else if (coord.IsSmaller(mSmallest)) mSmallest = coord;
    }

    protected Vector2 GetPosBeginInWorld()
    {
        return (Vector2)mParent.position + mPosBegin * mParent.localScale;
    }

    protected abstract ETowardX4 GetCoordDirInWorld();//坐标增长所对应的世界位置移动方向

    protected RectMeter GetRectHold(Vector2Int coord)
    {
        Vector2 gridPos = mGrids[coord].transform.position;
        RectMeter gridRect = new RectMeter();
        float halfSideSpanX = mSpanGridSideX / 2;
        float halfSideSpanY = mSpanGridSideY / 2;
        gridRect.leftBottom = new Vector2(gridPos.x - halfSideSpanX, gridPos.y - halfSideSpanY);
        gridRect.rightTop = new Vector2(gridPos.x + halfSideSpanX, gridPos.y + halfSideSpanY);
        return gridRect;
    }

    protected float mSpanGridSideX => MapRefer.cSpan * mScale.x;
    protected float mSpanGridSideY => MapRefer.cSpan * mScale.y;

}
