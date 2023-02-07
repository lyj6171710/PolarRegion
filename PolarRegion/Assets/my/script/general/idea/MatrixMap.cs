using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixMap 
{
    public static List<int> GetAllTurnAt(List<Vector2Int> route)
    {//���ؾ��е�ת�۵�
        List<int> turns = new List<int>();
        if (route.Count > 2) 
        {//ת��ֻ���ܷ����ڵڶ���������
            Vector2Int last = route[0];
            for (int i = 2; i < route.Count; i++)
            {
                Vector2Int vary = route[i] - last;//����
                if (vary.x != 0 && vary.y != 0)//ת�۵Ļ���ˮƽ����ֱ����������Ϊ0
                    turns.Add(i - 1);
                last = route[i - 1];
            }
        }
        return turns;
    }

    public static int GetFirstSideTurnAt(List<Vector2Int> route)//Ĭ��ÿ��Ԫ�����������ڵģ��������������ҵ�һ��������
    {//��������ֱ·�ߵ��յ㣨��ת����ת�ۣ�
        if (route == null || route.Count == 0) return -1;
        else if (route.Count == 1) return 0;
        else if (route.Count == 2) return 1;
        else //route.Count > 2
        {//ת��ֻ���ܷ����ڵ�һ�������󡢵ڶ����ص��
            Vector2Int last = route[0];
            for (int i = 2; i < route.Count; i++)
            {
                Vector2Int vary = route[i] - last;//����
                if (vary.x != 0 && vary.y != 0)//ת�۵Ļ���ˮƽ����ֱ����������Ϊ0
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
    {//��������ֱ·�ߵ��յ�
        if (route == null || route.Count == 0) return -1;
        else if (route.Count == 1) return 0;
        else if (route.Count == 2) return 1;
        else //route.Count > 2
        {
            Vector2Int last = route[0];
            for (int i = 2; i < route.Count; i++)
            {
                Vector2Int varyNew = route[i] - route[i - 1];//�����
                Vector2Int varyOld = route[i - 1] - last;//�Ȳ���
                if (varyNew != varyOld)//�����һ�£���ô�ͷ�����ת��
                {
                    //����ת�۵�����ж��֣���ת����ת����ת(�۷�)
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
        //8����
        List<Vector2Int> nears = null;
        if (GetOneInRange == null)
            nears = new List<Vector2Int>();

        for (int i = -1; i < 2; i++)//���������ܱ�Ԫ�ص�һ���б�
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)//���������Լ�
                    continue;
                else
                {
                    int nearX = coordAt.x + j;
                    int nearY = coordAt.y + i;
                    if (nearY >= 0 && nearX >= 0 &&
                        nearY < numRow && nearX < numColumn)//���ܳ�������Χ
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
        //4������������
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

        for (int i = -1; i < 2; i++)//���������ܱ�Ԫ�ص�һ���б�
        {
            if (i == 0) continue;//���������Լ�
            int nearY = coordAt.y + i;
            if (nearY >= 0 && nearY < numRow)//���ܳ�������Χ
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
