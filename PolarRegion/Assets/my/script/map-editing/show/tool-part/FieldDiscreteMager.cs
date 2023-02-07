using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDiscreteUnits
{
    //��ʱû����Ҫ
}

public class FieldDiscreteMager : FieldGridMager
{
    //��ɢ�ĵ�Ԫ��

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
        //��������������Ż����������ж�����Ƿ���small��big��Χ�ķ�Χ�ڣ��ڵĻ����ű���
        foreach (Vector2Int coord in mGrids.Keys)//��Ԫ����ɢ�ĵ�ͼ���Ƿ��ڵ�ͼ�ڣ���ֻ�б�����
        {
            if (MathRect.SuWhetherInside(cursorPos, GetRectHold(coord)))
                return true;
        }
        return false;
    }

    //====================================================

    IDiscreteUnits mRefer;//��������ɢ��Ԫ��������������ݣ���������������ݽṹ
                          //ʹ����ɢ��ͼ��ʱ��һ��������һ����ͼ���ݼ���������ͬ���ģ����������������

    public void AcceptRefer(IDiscreteUnits refer)
    {
        mRefer = refer;
    }

}
