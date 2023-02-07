using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FieldWall : MonoBehaviour,
    ITileOperate, IGridOperate, ICanPaint
{
    // һ������Ƭƴ�ӳɵ��������壬�ʾ��ε�����Ȳ���

    //====================================

    public Service me;

    IfoWall mData;//�Ե�ǰ��ͼ���ݵļ�¼�������Ի�ԭ

    Vector2Int mCoordInSuper;//����������԰�����������������λ��

    public void MakeReady(Vector2Int coord, float span, Vector2 scale)
    {
        mLattice = new FieldMatrixMager();
        mTiles = new List<FieldTile>();
        mCoordInSuper = coord;
        mScaleTile = scale;
        mSpanTile = span;
        ReadyPaint();
        
        me = new Service(this);
    }

    protected void MakeUse(IfoWall data)
    {
        mData = data;
        
        CreateLattice();
        ShowRegionOnLattice();

        mPaint.MakeUse(mLattice);
        mPaint.neSelect.SuWhenSingleSelectEnd += () => NuNotifyNewSelectFmField(mLattice[mCoordHover].transform);
    }

    public void MakeDown() 
    {
        mTiles.Clear();
        mLattice.RemoveAllGrids();
        mPaint.MakeDown();
    }

    public void MakeRemove()
    {
        MakeDown();
        Destroy(gameObject);
    }

    //���ݷ�ӳ==================================

    List<FieldTile> mTiles;//��ʱ���ɲ�ʹ�õ�����
    [HideInInspector] public FieldMatrixMager mLattice;

    float mSpanTile;
    Vector2 mScaleTile;

    void CreateLattice()
    {
        mLattice.MakeReady(transform, this);
        mLattice.ReadyAttr(MapRefer.cSpanStart * mScaleTile, mSpanTile, mScaleTile);
        mLattice.CreateLattice(mData.meHeight, mData.meWidth);
        for (int i = 0; i < mLattice.meGridsNum; i++)
        {
            Vector2Int at = mLattice.SuGetCoordByOrderedCount(i);
            FieldTile tile = mLattice[at].gameObject.AddComponent<FieldTile>();
            tile.MakeReady(this, mLattice[at]);
            mTiles.Add(tile);
        }
    }

    void ShowRegionOnLattice()
    {
        for (int i = 0; i < mData.meTileSum; i++)
        {
            mTiles[i].NuDrawTile(mData.GetTile(i));
        }
    }

    //ͿĨ����==========================

    protected MapGetPaint mPaint;

    void ReadyPaint()
    {
        mPaint = gameObject.AddComponent<MapGetPaint>();
        mPaint.MakeReady(this);
    }

    public void IPaintTo(Vector2Int gridCoord, IfoTilePick pick)
    {
        mTiles[mLattice.SuGetOrderedIndexFromBegin(gridCoord)].
            NuDrawTileAtLayer(MapPlates.It.meCurLayer, pick);
        mData.SetTile(gridCoord, pick, MapPlates.It.meCurLayer);
    }

    public void IEraseFrom(Vector2Int gridCoord)
    {
        mTiles[mLattice.SuGetOrderedIndexFromBegin(gridCoord)].
            NuEraseTile(MapPlates.It.meCurLayer);
        mData.SetTile(gridCoord, new IfoTilePick(), MapPlates.It.meCurLayer);
    }

    public IfoTilePick IGetPick(Vector2Int gridCoord)
    {
        return mData.GetTile(gridCoord).GetPick(MapPlates.It.meCurLayer);
    }

    //===============================

    public Action<Transform> NuNotifyNewSelectFmField;

    Vector2Int mCoordHover;

    public void IWhenHoverTileFmOpera(IfoTilePick pick)
    {

    }

    public void IWhenHoverGridFmOpera(Vector2Int coord)
    {
        MapPlates.It.mInTileSelect = false;

        mCoordHover = coord;
    }


    //====================================

    public class Service : Srv<FieldWall>,ISymbRefer
    {
        public Service(FieldWall belong) : base(belong)
        {
        }

        public Vector2Int CoordInSuper => j.mCoordInSuper;

        public IfoTilePile GetPile(Vector2Int coord)
        {
            return j.mData.GetTile(coord);
        }

        public void OnlyShowLayerAt(int layer)
        {
            foreach (FieldTile tile in j.mTiles) tile.me.OnlyPerformOneLayerAt(layer);
        }

        public void ShowAllLayer()
        {
            foreach (FieldTile tile in j.mTiles) tile.me.ShowAllLayer();
        }

        //=================================

        public Action<Transform> INotifyNewSelectFmField { get => j.NuNotifyNewSelectFmField; set => j.NuNotifyNewSelectFmField = value; }

        public Dictionary<Vector2Int, GameObject> iPlaneUnits
        {
            get
            {
                var list = new Dictionary<Vector2Int, GameObject>();
                j.mLattice.SuForeach((coord) => list.Add(coord, j.mLattice[coord].gameObject));
                return list;
            }
        }

        public bool iIsCursorOn => j.mLattice.SuWhetherCursorInField();

        public Vector2Int iIsOn => j.mPaint.neSelect.me.CurHover;

        public bool iInShow => j.mTiles.Count > 0;

    }

}


//==================================

[Serializable]
public class IfoTilePile
{
    [SerializeField] List<IfoTilePick> picks;//�����ۻ�
    //����ֵ�ʹ���ڼ���
    //��̬�㼶���߲㼶û�����ݣ���û������
    //�����n��û�����ݣ�����n+1�������ݣ���ʱ��n��Ҳ����Ҫ��������
    //���ʹ��ʱ��Ӧ����ֱ����Ϊ������Ԥ�������в㣬��������Ҳ�ṩ�ڲ��洢�䶯������������״̬ͬ��

    public int meNumLayer { get { return picks.Count; } }

    public int meIndexHighestHave { get { return picks.Count - 1; } }

    public bool SuHaveTile(int layer)
    {
        if (layer < 0 || layer > MapRefer.cLayerMax)
            return false;
        else if (picks.Count <= layer)
            return false;
        else if (picks[layer].NoBody)
            return false;
        else
            return true;
    }

    //-----------------------------------

    public IfoTilePile Initial()//��紴���ýṹʵ��ʱҪ�ֶ�����һ��
    {//Ӧ�ò��ܷŹ��캯������ݻ�ԭ���������
        if (picks == null)
        {
            picks = new List<IfoTilePick>();
            picks.Add(new IfoTilePick());
        }
        return this;
    }

    public IfoTilePick GetPick(int layer)
    {
        if (layer < 0 || layer > MapRefer.cLayerMax)
            return new IfoTilePick();//�������ݣ�Ĭ��ֵ����
        else if (picks.Count > layer)
            return picks[layer];
        else
            return new IfoTilePick();
    }

    public int SetPick(int layer, IfoTilePick pick)//����ֵ��ʾ���洢�㼶��������
    {
        if (layer < 0 || layer > MapRefer.cLayerMax)
            return 0;
        else if (picks.Count > layer)
        {
            picks[layer] = pick;
            if (picks.Count - 1 == layer)
                return ClearIfInvalid(layer);//���ܳ���������������ʱҪ���������ȷ������������С��
            else
                return 0;
        }
        else
        {
            if (pick.family == 0 && pick.index <= 0)
                return 0;
            else
            {
                int vary = ExpandIfNotEnough(layer);
                picks[layer] = pick;
                return vary;
            }
        }
    }

    int ClearIfInvalid(int checkStart)//�㼶��ߣ�������Ϊ�㣬��ֱ�ӱ����
    {
        if (checkStart == 0) return 0;

        int vary = Method.VaryUntilReach(checkStart, false,
            (turn) =>
            {
                if (turn == 0)//����Ҫʣһ��
                    return true;
                else
                {
                    if (picks[turn].family == 0 && picks[turn].index <= 0)//Ҫ�õ�ǰ��߲㣬���������ݵ�
                        return false;//��������
                    else
                        return true;//����Ҫ��������
                }
            },
            (turn) =>
            {
                picks.RemoveAt(turn);
            });
        return vary;
    }

    int ExpandIfNotEnough(int layerNeed)
    {
        int vary = 0;
        for (int i = picks.Count - 1; i < layerNeed; i++)//���ݣ�ֱ������������㼶
        {
            vary += 1;
            picks.Add(new IfoTilePick());
        }
        return vary;//���������˶��ٸ�Ԫ�أ���������
    }

}

[Serializable]
public struct IfoTilePick
{
    public int family;//ͼ���������
    public int set;//��ǰ������ͼ���زĵ�����ͼ��
    public int index;//��ǰ������ͼ���زĵĶ�Ӧ����

    public bool NoBody => family == 0 ? true : false;//0��𣬶�Ӧ����ϵͳͼ����û�е�������
}

//---------------------------------

[Serializable]
public class StoTilePile
{
    public List<StoTilePick> picks;//�����ۻ�

    public StoTilePile() { picks = new List<StoTilePick>(); }
}

[Serializable]
public class StoTilePick
{
    public int family;//ͼ���������
    public int set;//��ǰ������ͼ���زĵ�����ͼ��
    public int index;//��ǰ������ͼ���زĵĶ�Ӧ����
}

//==================================

[Serializable]
public class IfoWall:Ifo,ICanSto<StoWall>
{
    [SerializeField] protected int width = 1;
    [SerializeField] protected int height = 1;
    [SerializeField] protected IfoTilePile[] piles;//�����ۻ�
                                                   //�ڼ��еڼ�����������Щͼ��

    public int meTileSum { get { return width * height; } }
    public int meWidth { get { return width; } }
    public int meHeight { get { return height; } }

    public IfoWall() { }//�ɴ˳�ʼ���ģ���Ҫʹ��ǰ���ó�ʼ����������粻�ùܣ������ڲ�ʹ�õ�

    public IfoWall(int width, int height)//��Ⱥ͸߶��Ǳ�Ҫ����
    {
        NuInitial(width, height);
    }

    public void NuInitial(int width, int height)//����ʵ�������ܳ�ʼ��
    {
        this.width = width;
        this.height = height;
        piles = new IfoTilePile[meTileSum];
        for (int i = 0; i < piles.Length; i++)
        {
            piles[i] = new IfoTilePile();
            piles[i].Initial();
        }
    }

    int GetIndex(Vector2Int coord) { return coord.y * width + coord.x; }

    public void NuResizeTo(Vector2Int size)//��Ҫ����ԭ����ͼ������������
    {
        int lastWidth = width;
        int lastHeight = height;
        IfoTilePile[] lastPiles = piles;

        NuInitial(size.x, size.y);//�½����ݣ����Զ��޳����������

        for (int i = 0; i < width; i++)
        {
            if (i < lastWidth)
            {
                for (int j = 0; j < height; j++)
                {
                    Vector2Int coord = new Vector2Int(i, j);
                    if (j < lastHeight)//ԭ���ݸ���
                    {
                        IfoTilePile lastPile = lastPiles[coord.y * lastWidth + coord.x];
                        for (int k = 0; k < lastPile.meNumLayer; k++)
                            SetTile(coord, lastPile.GetPick(k), k);
                    }
                    else//���������½�
                        SetTile(coord, new IfoTilePick(), 0);
                }
            }
            else
            {
                for (int j = 0; j < height; j++)//���������½�
                    SetTile(new Vector2Int(i, j), new IfoTilePick(), 0);
            }
        }
    }

    //=========================================

    public IfoTilePile GetTile(Vector2Int coord)
    {
        int indexMatch = GetIndex(coord);
        if (indexMatch >= 0 && indexMatch < meTileSum)
            return piles[indexMatch];
        else
            return null;
    }

    public void SetTile(Vector2Int coord, IfoTilePick pick, int layer)
    {
        piles[GetIndex(coord)].SetPick(layer, pick);
    }

    public IfoTilePile GetTile(int posIndex)//��һά��������ȡ
    {
        return piles[posIndex];
    }

    public void SetTile(int posIndex, IfoTilePick pick, int layer)
    {
        piles[posIndex].SetPick(layer, pick);
    }

    //========================================

    public StoWall ToSto()
    {
        StoWall stoWall = new StoWall();

        stoWall.width = meWidth;
        stoWall.piles = new StoTilePile[meTileSum];

        for (int i = 0; i < meTileSum; i++)
        {
            StoTilePile pile = new StoTilePile();
            IfoTilePile ifoPile = GetTile(i);
            for (int k = 0; k < ifoPile.meNumLayer; k++)
            {
                StoTilePick stoPick = new StoTilePick();
                IfoTilePick ifoPick = ifoPile.GetPick(k);
                stoPick.family = ifoPick.family;
                stoPick.set = ifoPick.set;
                stoPick.index = ifoPick.index;
                pile.picks.Add(stoPick);
            }
            stoWall.piles[i] = pile;
        }

        return stoWall;
    }
}

[Serializable]
public class StoWall:Sto,IForIfo<IfoWall>
{
    public int width;
    public StoTilePile[] piles;//��̬�洢���ͣ�һ���������ͼ����1��ͼ�飬�ͺ�10��ͼ�飬����洢����һ��
                               //�ڼ��еڼ�����������Щͼ��
                               //�����ۻ�

    public int meHeight => piles.Length / width;

    public IfoWall ToIfo()
    {
        IfoWall ifoWall = new IfoWall();
        ifoWall.NuInitial(width, piles.Length / width);

        for (int i = 0; i < piles.Length; i++)
        {
            List<StoTilePick> stoPicks = piles[i].picks;
            for (int j = 0; j < stoPicks.Count; j++)
            {
                IfoTilePick ifoPick = new IfoTilePick();
                ifoPick.family = stoPicks[j].family;
                ifoPick.set = stoPicks[j].set;
                ifoPick.index = stoPicks[j].index;
                ifoWall.SetTile(i, ifoPick, j);
            }
        }

        return ifoWall;
    }
}