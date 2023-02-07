using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class FieldGridMager : IGridOperate
{
    //�����ϣ��а������ɸ����ε�Ԫ���������ڵİ��ţ���һ�����ǰ����ɰ��ŵ�
    //��λ�ϣ��䲻�ɻ�ȱ��������ֱ�۱��֣�����ڳ������ǳ����е�ʵ��

    //���߼�����ϵ��ʵ������ϵ���ڲ��߼�
    //�߼�����ϵ�ϣ�x���ҷ�������y���Ϸ��������������(0,0)
    //ʵ������ϵ��ԭ���������������ϵ�ϵ��κ�λ�ã�������x������ʵ��x����ķ���Ҳ��һ�������ҵ�

    //ʵ������״̬�����߼�����״̬�������߼����������Ͳ���ʵ����������
    //����ȷ�����ǣ������������ݵ����꣬һ�����߼����֮꣬��һ����ת����ʵ���������������

    public Action<Vector2Int> meWhenHoverOneGridNew;//ʹ������ģ���ֻĳһ������
    
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
    protected Vector2 mPosBegin;//���λ�õĳ�ʼֵ
    //�������λ�ã���Ϊ��ʼ��Ԫ������ĵ�λ������
    //����˵����ʼ��Ԫ�񣬲��ǵ�һ����Ԫ�񣬶�������Ϊ(0��0)ʱ����Ӧ�ĵ�Ԫ��

    protected Dictionary<Vector2Int, FieldGrid> mGrids;
    //���񲻽���ʹ���б���Ϊɾ���ֲ��ǿ��Ժ�Ƶ����
    //���ҵ�Ԫ������꣬���������Ԫ�����������
    protected Vector2Int mSmallest;
    protected Vector2Int mBiggest;
    
    IGridOperate mCallback;//ʹ����Ҳ������Ӧ

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
        {//�������Ԫ
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

    public abstract bool SuWhetherCursorInField();//�����޷��ж��Ƿ��������ڣ�������ڵ�Ԫ��İ��Ų���

    public Vector2Int SuNearestCoord(Vector2Int expect)//ȡ����ָ���������������
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

    //�ڲ�����=================================

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

    protected abstract ETowardX4 GetCoordDirInWorld();//������������Ӧ������λ���ƶ�����

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
