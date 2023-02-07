using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldMatrixMager : FieldGridMager
{
    //�����а��ŵĵ�Ԫ��

    public int meNumRow { get { return mNumRow; } }

    public int meNumCol { get { return mNumCol; } }

    public Vector2 SuGetCentrePos()
    {
        return mGrids[new Vector2Int((mBiggest.x + mSmallest.x) / 2, 
            (mBiggest.y + mSmallest.y) / 2)].transform.position;
    }

    public Vector2Int SuGetCoordByOrderedCount(int index)
    {
        //��������˼�ǣ���0��ʼ���������к��У�ֱ������index��Ȼ��ȡ���������ĵ�Ԫ�������
        int x = index % mNumCol;
        int y = index / mNumCol;
        return new Vector2Int(x, y);
    }

    public int SuGetOrderedIndexFromBegin(Vector2Int coord)
    {
        //���Ϸ������꣬�������������������ʱ��һ�����磬���Բ�����ʽ˵��
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

    public Vector2 SuGetTheoreticalPos(Vector2Int coord)//���������Ƿ������ص�Ԫ��
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

    public void CreateLattice(int numRow, int numColum, bool upRight = true)//�Ծ�����ʽ��������
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
            //��[������λ��+ƫ����]���ڳ������������ɵ�Ԫ��
            //�����������ң������ϣ�˳Ӧ��ʶ�����˼ά
            //���ɷ��θ��ӣ��ɳ���һЩ�趨���궨��������Ϣ
        }
        else
        {
            for (int i = 0; i < mNumRow; i++)
                for (int j = 0; j < mNumCol; j++)
                    AddOneGrid(new Vector2Int(j,i),
                        mPosBegin + new Vector2(mSpan * mScale.x * j, -mSpan * mScale.y * i));
            //�����������ң������£����Ӧ�������Ͻǵ�λ��
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
