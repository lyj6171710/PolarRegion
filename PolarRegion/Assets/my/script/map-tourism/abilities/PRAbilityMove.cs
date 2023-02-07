using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PRAbilityMove : MonoBehaviour, IMatrixMapPosIfo
{
    //����Ŀ�꣺����ʵ����������Ƭ��ͼ��ʱ����Ҫ�ġ�����������Ч������
    //����ͼ��һ���ڵ�Ԫ��ɾ��и�ͼ��߶ȵļ���У�������ȣ�ÿһ��ͼ�����ӵ��һ����Ƭ
    //�����߼��㼶�����ж�������ͼ�������ʾ��ϵ

    public MapRegionsShow mMapDp;

    [ShowInInspector]
    public int meHighAt => mLayerHeightCurAt;

    [Button]
    public void Land()
    {
        mLayerHeightCurAt = 1;//������Ͳ㼶������Ӧ��վ�ڵ�����
        mViaLast.highLayerShould = mLayerHeightCurAt;
        mNowAt = Vector2Int.zero;
        mShift.StartMove(Vector2Int.zero);
    }

    //=========================================

    Dictionary<IfoRecord, IfoWillToBe> mRecords;//������ʱ������
    
    public bool SuCanWalkTo(Vector2Int from, EToward4 to)
    {
        if (to == EToward4.middle)
            return false;
        else
        {
            IfoRecord record = new IfoRecord(from, to, mLayerHeightCurAt);
            //ע���������ǻ������ﵱǰ�߶Ȳ㼶�������ǰ·��
            //��ǰ��·������Ч����δ�����ߵ�·�Ϳ��ܳ����⣬����Լ�����
            if (mRecords.ContainsKey(record))
                return mRecords[record].canPass;
            else
            {
                IfoWillToBe will = new IfoWillToBe();//���ݼ�����
                mRecords.Add(record, will);
                will.highLayerShould = mLayerHeightCurAt;//һ��������Ǳ���
                will.pile = mMapDp.me.GetPileAt(from + OverTool.TowardToVector(to).ToInt());
                if (will.pile == null)
                {
                    will.canPass = false;
                    return false;
                }

                //�������Լ����ӽǳ���������ǰ���ܷ��߶���ȥ
                //�ȿ��ܷ���¥��(�����߼��߶ȵ�����)
                IfoTile tile;//׼�����ڳ�����Ƭ��Ϣ
                IfoTilePile pileCurOn = mMapDp.me.GetPileAt(from);
                if (pileCurOn.SuHaveTile(mLayerHeightCurAt))
                {
                    tile = GetIfoTile(pileCurOn, mLayerHeightCurAt);
                    if (CanStairUp(tile, to))
                    {
                        tile = GetIfoTile(will.pile, mLayerHeightCurAt + 1);
                        will.showLayerShould = IfGoThrough(tile, mLayerHeightCurAt + 1);
                        if (will.showLayerShould > 0)
                        {
                            will.highLayerShould = mLayerHeightCurAt + 1;
                            will.canPass = true;
                            return true;
                        }
                    }
                }
                //û������¥�ݿ���ʱ
                GoPassOn(ref will);
                return will.canPass;
            }
        }

        void GoPassOn(ref IfoWillToBe ahead)//�����Ƿ��ܹ�ά�����ﺣ��ƽֱ���Խ����߶���ȥ
        {
            if (ahead.pile.SuHaveTile(mLayerHeightCurAt))
            {//���ҵ������ﵱǰ�߶��߼��㼶�ĵ�ͼ��Ƭ(��ƬҲ���߼��㼶��ͼ���������)
                IfoTile tile = GetIfoTile(ahead.pile, mLayerHeightCurAt);
                ahead.showLayerShould = IfGoThrough(tile, mLayerHeightCurAt);
                if (ahead.showLayerShould > 0) ahead.canPass = true;
                else ahead.canPass = false;
            }
            else if (mLayerHeightCurAt >= 1 && ahead.pile.SuHaveTile(mLayerHeightCurAt - 1))
            {
                IfoTile tile = GetIfoTile(ahead.pile, mLayerHeightCurAt - 1);

                //�鿴������¥����
                if (mLayerHeightCurAt > 1 && CanStairDown(tile, to))
                {
                    ahead.showLayerShould = IfGoThrough(tile, mLayerHeightCurAt - 1);
                    if (ahead.showLayerShould > 0)
                    {
                        ahead.highLayerShould = mLayerHeightCurAt - 1;
                        ahead.canPass = true;
                        return;
                    }
                }
                //��ʱû������¥�ݿ���

                if (tile.isWall)//�����ǽ�壬������������
                    ahead.canPass = false;
                else
                {//�����ڵ�һ���������Ƭ�ϣ���������߶ȵ�����
                    ahead.showLayerShould = (mLayerHeightCurAt - 1) * 2 + 1;
                    ahead.canPass = true;
                }
            }
            else//���ܴӸߴ�Ծ���ʹ�(���һ���㼶����)
                ahead.canPass = false;
        }

        bool CanStairDown(IfoTile toTile, EToward4 dirFrom)
        {
            string dir;
            if (toTile.attrs.Get("stairDown", out dir))
            {
                if (dir == dirFrom.ToString())
                {
                    return true;
                }
            }
            return false;
        }

        bool CanStairUp(IfoTile onTile, EToward4 dirTo)
        {
            string dir;
            if (onTile.attrs.Get("stairUp", out dir))
            {
                if (dir == dirTo.ToString())
                {
                    return true;
                }
            }
            return false;
        }

        int IfGoThrough(IfoTile tile, int tileLayer)
        {
            if (tile.viaAbove)
            {//��������Ƭ������������������洩����
                //��ô�ܴ�����Ԫ�����ﵱǰͼ����ڸ���Ƭͼ��һ��
                return tileLayer * 2 + 1;
            }
            else if (tile.viaBelow)
            {//��������Ƭ������������������洩����
                //��ô�ܴ�����Ԫ������ͼ��״̬���ڸ���Ƭͼ��һ��
                return tileLayer * 2 - 1;
            }
            else//��һ�㼶��Ƭ�����������ǾͲ��ܴ���
            {
                return 0;
            }
        }

        IfoTile GetIfoTile(IfoTilePile pile, int highAt)
        {
            IfoTilePick pick = pile.GetPick(highAt);
            return MapPlates.It.SuGetTileAttrSafe(pick);
        }
    }

    public Vector3 SuGetPosAt(Vector2Int at)
    {
        return mMapDp.me.GetPosAt(at);
    }

    public bool SuIfNearGridCenter(Vector2 pos, out Vector2 toGridCenter, out Vector2Int nearTo)
    {
        nearTo = mMapDp.me.GetCoordIn(pos);
        Vector2 posCenter = mMapDp.me.GetPosAt(nearTo);
        toGridCenter = pos - posCenter;
        float gap = toGridCenter.magnitude;
        if (gap < 0.2f)//����Ҫע�⣬��������ƶ����죬���ܾͲ�׽�����ӽ���ʱ��
            return true;
        else
            return false;
    }

    public bool SuNowCanWalkTo(EToward4 to)
    {
        IfoRecord record = new IfoRecord(mNowAt, to, mLayerHeightCurAt);
        if (mRecords.ContainsKey(record))
            return mRecords[record].canPass;
        else
            return SuCanWalkTo(mNowAt, to);
    }

    //========================================

    ActorShiftOnMatrixCanPlan mShift;
    SpriteRenderer mBody;

    int mLayerHeightCurAt;//��ǰ�㼶��0Ϊ��㣬��߲�ȡ���ڵ�ͼ��θ߶ȣ���ͼ��Ͳ�ҲӦΪ0����Ӧ����
    IfoWillToBe mViaLast;
    Vector2Int mNowAt;

    void FitShowLayerTo(EToward4 to)
    {
        IfoRecord record = new IfoRecord(mNowAt, to, mLayerHeightCurAt);
        //������ǰ�еģ�һ���Ѿ������˼��㲢�����ֵ��У����Բ���Ҫ�ж��ֵ��Ƿ�������д��Ǿ����߼�������
        IfoWillToBe mViaNowTo = mRecords[record];
        if (mViaNowTo.showLayerShould > mViaLast.showLayerShould)
        {//����ͼ����Ҫ����ʱ���ڵ�����һ���ص�ǰ����ʱ������ǰͼ��߶ȣ���ΪҲ��Ҫ��Ӧ��ǰ�ص�Ĳ㼶 
            mBody.sortingOrder = mViaNowTo.showLayerShould;
        }
        mViaLast = mViaNowTo;
    }

    void FitShowLayerArrive(Vector2Int where)
    {
        mBody.sortingOrder = mViaLast.showLayerShould;
        mLayerHeightCurAt = mViaLast.highLayerShould;
        mNowAt = where;
    }

    void Awake()
    {
        mRecords = new Dictionary<IfoRecord, IfoWillToBe>();
        mShift = gameObject.AddComponent<ActorShiftOnMatrixCanPlan>();
        mShift.MakeReady(this);
        mShift.SuWhenMoveToNext += FitShowLayerTo;
        mShift.SuWhenMoveReachOneWill += FitShowLayerArrive;
        mShift.SuWhenMoveReachOne += () =>{
            if (mShift.meIsFinishPlan)
                mRecords.Clear(); //��������ݣ��������õ�������������
        };
        mBody = gameObject.GetComponent<SpriteRenderer>();
        mViaLast = new IfoWillToBe();
    }

    void Update()
    {
        mShift.SuMoveBy(UnifiedInput.It.meGoOneStep());
    }

    struct IfoRecord//���Խ�ʡ����
    {
        public Vector2Int from;
        public EToward4 to;
        public int heightRefer;//�������ο�����߶�Ҳһ��ʱ

        public IfoRecord(Vector2Int from, EToward4 to,int height)
        { this.from = from;this.to = to;heightRefer = height; }
    }

    class IfoWillToBe
    {
        public bool canPass;
        public IfoTilePile pile;
        public int showLayerShould;
        public int highLayerShould;
    }

}
