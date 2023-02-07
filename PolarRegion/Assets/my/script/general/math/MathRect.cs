using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathRect : MonoBehaviour
{

    public static float CalcHighestPosY(float inclineHalf, float radianToUR, float rotateNow)
    {//����һ��ƽ�����������ϵ�е���ߵ�λ��

        float rotateNow180 = MathNum.Abs(MathAngle.GetSameAngle180(rotateNow));
        //һ�ǻع鵽һ�������ڣ����ǿ������ӷ��ţ��ᷢ�������Ľ����һ����

        float radianNow;
        if (rotateNow180 <= 90)
        {//���������϶Խ������������
            radianNow = MathAngle.GetRadian(rotateNow180) + radianToUR;
            //��ǰ��ת״̬=��תǰ+��ת��
        }
        else
        {//���������¶Խ������������
            radianNow = MathAngle.GetRadian(rotateNow180) - radianToUR;
        }

        float HighY = MathNum.Abs(inclineHalf * Mathf.Sin(radianNow));

        return HighY;
    }

    public static float CalcHighestPosY(Vector2 size, float rotateNow)
    {//����һ��ƽ�����������ϵ�е���ߵ�λ��
        float inclineHalf = CalcRectInclineHalf(size);
        float radianToUR = CalcRadianToUR(size);
        //ˮƽ����ʱ�����϶Խ�����x��ļнǻ���
        return CalcHighestPosY(inclineHalf, radianToUR, rotateNow);
    }

    public static float CalcRectInclineHalf(Vector2 size)
    {
        float incline = Mathf.Sqrt(size.x * size.x + size.y * size.y);
        return incline / 2;
    }

    public static float CalcRadianToUR(Vector2 size)
    {
        return Mathf.Atan(size.y / size.x);
    }

    //================================

    public static bool SuWhetherInside(Vector2 at, RectMeter area, float pad = 0)
    {//at��area�����ͬһ����ϵ�����������Լ�
     //�������½�Ϊԭ��
        if (at.x < area.leftBottom.x - pad) return false;
        else if (at.y <  area.leftBottom.y - pad) return false;

        if (at.x > area.rightTop.x + pad) return false;
        else if (at.y >  area.rightTop.y + pad) return false;

        return true;
    }

    public static Vector2Int ComputeCoordIn(Vector2 pos, Vector2Int unit)//Ĭ�ϴ��㿪ʼ����
    {//�����Ա�׼���Ϊ��λ�����Ͻ��������
        int coordX = Mathf.FloorToInt(pos.x / unit.x);//unit��x����x�����ϴ���������ĳ���
        int coordY = Mathf.FloorToInt(pos.y / unit.y);
        return new Vector2Int(coordX, coordY);
    }

}
