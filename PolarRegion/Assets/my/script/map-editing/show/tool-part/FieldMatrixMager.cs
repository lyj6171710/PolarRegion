using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldMatrixMager : FieldGridMager
{
    //以阵列安放的单元格

    public int meNumRow { get { return mNumRow; } }

    public int meNumCol { get { return mNumCol; } }

    public Vector2 SuGetCentrePos()
    {
        return mGrids[new Vector2Int((mBiggest.x + mSmallest.x) / 2, 
            (mBiggest.y + mSmallest.y) / 2)].transform.position;
    }

    public Vector2Int SuGetCoordByOrderedCount(int index)
    {
        //参数的意思是，从0开始计数，先行后列，直到数到index，然后取得所数到的单元格的坐标
        int x = index % mNumCol;
        int y = index / mNumCol;
        return new Vector2Int(x, y);
    }

    public int SuGetOrderedIndexFromBegin(Vector2Int coord)
    {
        //不合法的坐标，利用这个函数所得序数时，一定超界，所以不用显式说明
        return coord.y * mNumCol + coord.x;
    }

    public override Vector2Int SuGetCoordPosIn(Vector2 worldPos)
    {
        float unitSizeX = mSpan * mScale.x;
        float unitSizeY = mSpan * mScale.y;
        int x = MathNum.FloorToIntAndZero(worldPos.x / unitSizeX);
        int y;
        if (mIsUpRight)
        {
            y = MathNum.FloorToIntAndZero(worldPos.y / unitSizeY);
        }
        else
        {
            float yOffset = worldPos.y - mPosBegin.y;
            y = -MathNum.FloorToIntAndZero(yOffset / unitSizeY);
        }
        return new Vector2Int(x, y);
    }

    public override bool SuWhetherCursorInField()
    {
        Vector3 cursorPos = UnifiedCursor.It.meCursorPos;
        Vector2 startPos = SuGetPos(mSmallest);
        Vector2 endPos = SuGetPos(mBiggest);
        RectMeter rectField = new RectMeter();
        float halfSideSpanX = mSpanGridSideX / 2;
        float halfSideSpanY = mSpanGridSideY / 2;
        if (mIsUpRight)
        {
            rectField.leftBottom = new Vector2(startPos.x - halfSideSpanX, startPos.y - halfSideSpanY);
            rectField.rightTop = new Vector2(endPos.x + halfSideSpanX, endPos.y + halfSideSpanY);
        }
        else
        {
            rectField.leftBottom = new Vector2(startPos.x - halfSideSpanX, startPos.y + halfSideSpanY);
            rectField.rightTop = new Vector2(endPos.x + halfSideSpanX, endPos.y - halfSideSpanY);
        }
        return MathRect.SuWhetherInside(cursorPos, rectField);
    }

    public Vector2 SuGetTheoreticalPos(Vector2Int coord)//可以无视是否具有相关单元格
    {
        return GetPosBeginInWorld() + SuGetCoordDirInWorld() *
            new Vector2(coord.x * mSpanGridSideX, coord.y * mSpanGridSideY);
    }

    //==================================

    int mNumRow;
    int mNumCol;
    bool mIsUpRight;

    protected override ETowardX4 GetCoordDirInWorld()
    {
        if (mIsUpRight) return ETowardX4.upRight;
        else return ETowardX4.downRight;
    }

    public void CreateLattice(int numRow, int numColum, bool upRight = true)//以矩阵形式生成网格
    {
        mNumRow = numRow;
        mNumCol = numColum;
        mIsUpRight = upRight;

        if (mIsUpRight)
        {
            for (int i = 0; i < mNumRow; i++)
                for (int j = 0; j < mNumCol; j++)
                    AddOneGrid(new Vector2Int(j,i),
                        mPosBegin + new Vector2(mSpan * mScale.x * j, mSpan * mScale.y * i));
            //从[父物体位置+偏移量]所在出发，依次生成单元格
            //这里是先往右，再往上，顺应常识，简便思维
            //生成方形格子，可承载一些设定，标定出各种信息
        }
        else
        {
            for (int i = 0; i < mNumRow; i++)
                for (int j = 0; j < mNumCol; j++)
                    AddOneGrid(new Vector2Int(j,i),
                        mPosBegin + new Vector2(mSpan * mScale.x * j, -mSpan * mScale.y * i));
            //这里是先往右，再往下，起点应给最左上角的位置
        }
    }

    public void OffsetLattice(Vector2 newPosBegin)
    {
        Vector2 offset = newPosBegin - mPosBegin;
        Vector3 offsetPos = new Vector3(offset.x, offset.y, 0);
        mPosBegin = newPosBegin;
        for (int i = 0; i < mNumRow; i++)
            for (int j = 0; j < mNumCol; j++)
                mGrids[new Vector2Int(j,i)].transform.localPosition += offsetPos;
    }

}
