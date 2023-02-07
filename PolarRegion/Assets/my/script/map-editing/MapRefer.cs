using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EMapUse { world, territory, cell, room }//�ӿڲ���

public enum EMapUnit { world, territory, cell, wall, tile, grid }//�������

public enum EMapIfo { tile, symbol, affair, attach }//��Ϣά��

public static class MapRefer//����ֹ���������
{
    public const int cBaseNum = 5;//һ����ǰ��Ұ�ɼ�����ĳ������
    public const int cNumRow = cBaseNum * 3;//�����һ�㱣��4��3
    public const int cNumColumn = cBaseNum * 4;
    public const int cNumSum = cNumRow * cNumColumn;

    public const int cSpan = 1;//Ĭ��һ����Ԫ�񣬵ȳ���transform.positionֵ�ĳ��ȵ�λ
    public const float cSpanStart = 0.5f;

    public const int cRegionNumMax = 10;
    public const int cCellNumMax = 40;

    public const int cLayerMax = 4;//ͼ�����ޣ���5��
    //ͼ�㲻����������ʾ���ȼ���Ҳ�������߼��ϵĸߵͣ��ᱻ����
    //ͼ��״̬����ͼʵ��ӵ�У�������ͼԪ������ӵ��
    public const int cLayerGround = 0;
    public const int cLayerForeGround = 4;
    //��1���ǵ���㣬��2�㵽4��������㣬��5����ǰ����

    public const KeyCode cKeyArea = KeyCode.C;
    public const KeyCode cKeySymb = KeyCode.S;
    public const KeyCode cKeyLayer = KeyCode.L;
}
//===================================================

public abstract class MapDataHandle
{
    //�������ٶ���ɢ�͵�ͼ����ĳ��������Ĳ�ѯ(�����������б�ṹ���洢����ʱ)
    //��ɢ�͵�ͼ����Ԥ��һ���������������ݣ����������ݲŴ洢��û���ݾͲ��洢
    //��ɢ�͵�ͼ����ͼ���ݲ�������еط���Ҳ��˵�Ԫ��Ķ�Ӧ�������޷�ͨ���򵥼���ȷ��

    Dictionary<Vector2Int, int> mapIndex;//���ٲ�ѯʵ�ʵ�ַ��������ÿ�α���
    bool[][] mapHold;//����ȷ�����λ���Ƿ��е�Ԫ��
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

    protected abstract int ReConstruct();//�չ��������Լ������������

    public bool SuInitialMap()//ʹ��һ�����ݼ�ǰ�����Ӧ�ֶ�����һ�θú���
    {
        if (mapIndex == null)//���ܳ����Ѿ����ɶ��󣬵���δ������ʼ�������(�ر�������л���ԭʱ)
        {
            InitialSelf(ReConstruct());
            SuAddOne(Vector2Int.zero);//Ӧ����ʹ����һ��Ԫ�أ���Ȼ���׳�����(�������Ȿ���Ϳ��Ա��⿼�ǣ�û��Ҫ����)
            return false;//����ֵ�����Ƿ����Ԫ�أ�������ʱ���Ѿ��Զ�������һ��Ԫ��
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

    protected abstract void GetCurHoldToMap(Action<Vector2Int, int> deal);//�����δ������ݸ�deal

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
            mapIndex.Add(at, meSum);//add��������ǣ�addǰ��meSum+1
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
            //mapIndex.Remove(at);����ģ���Ϊ���������б��remove��
            //��remove��Ԫ��֮��ĸ�Ԫ�أ�ͬһ������Ӧ���������
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

    protected abstract Vector2Int GetCoordStraight(int index);//�������֪�������ﲻ����֪��

    public Vector2Int SuGetCoord(int index)
    {
        if (index < 0 || index >= meSum)
            return -1 * Vector2Int.one;
        //������ɢ�͵�ͼ������û�п���Ϊ�����ı�Ҫ
        //����һ������������Ʋ����ã�����-1���Ա�ʾԽ�磬������д���
        else
            return GetCoordStraight(index);
    }
}