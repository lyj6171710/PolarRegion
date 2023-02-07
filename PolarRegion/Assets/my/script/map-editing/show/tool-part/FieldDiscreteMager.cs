using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDiscreteUnits
{
    //暂时没有需要
}

public class FieldDiscreteMager : FieldGridMager
{
    //离散的单元格

    public override Vector2Int SuGetCoordPosIn(Vector2 pos)
    {
        float unitSizeX = mSpan * mScale.x;
        float unitSizeY = mSpan * mScale.y;
        int x = MathNum.FloorToIntAndZero(pos.x / unitSizeX);
        int y = MathNum.FloorToIntAndZero(pos.y / unitSizeY);
        return new Vector2Int(x, y);
    }

    protected override ETowardX4 GetCoordDirInWorld()
    {
        return ETowardX4.upRight;
    }

    public override bool SuWhetherCursorInField()
    {
        Vector3 cursorPos = UnifiedCursor.It.meCursorPos;
        //待做，这里可以优化，就是先判断鼠标是否在small和big所围的范围内，在的话，才遍历
        foreach (Vector2Int coord in mGrids.Keys)//单元格离散的地图，是否在地图内，就只有遍历了
        {
            if (MathRect.SuWhetherInside(cursorPos, GetRectHold(coord)))
                return true;
        }
        return false;
    }

    //====================================================

    IDiscreteUnits mRefer;//承载由离散单元格所构区域的数据，采用了特殊的数据结构
                          //使用离散地图的时候，一定依赖的一个地图数据集，所以是同步的，这里可以利用起来

    public void AcceptRefer(IDiscreteUnits refer)
    {
        mRefer = refer;
    }

}
