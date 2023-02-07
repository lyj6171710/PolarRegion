using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FieldWall : MonoBehaviour,
    ITileOperate, IGridOperate, ICanPaint
{
    // 一块由瓦片拼接成的区域整体，呈矩形但长宽比不限

    //====================================

    public Service me;

    IfoWall mData;//对当前地图内容的记录，将可以还原

    Vector2Int mCoordInSuper;//区域整体相对包含该整体的总区域的位置

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

    //数据反映==================================

    List<FieldTile> mTiles;//临时生成并使用的网格
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

    //涂抹功能==========================

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
    [SerializeField] List<IfoTilePick> picks;//纵向累积
    //序数值就代表第几层
    //动态层级，高层级没有内容，就没有数据
    //如果第n层没有内容，但第n+1层有内容，此时第n层也会需要有数据了
    //外界使用时，应该能直接认为这里面预置了所有层，不过对外也提供内部存储变动的线索，方便状态同步

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

    public IfoTilePile Initial()//外界创建该结构实例时要手动调用一次
    {//应该不能放构造函数里，数据还原会出现问题
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
            return new IfoTilePick();//需有数据，默认值而已
        else if (picks.Count > layer)
            return picks[layer];
        else
            return new IfoTilePick();
    }

    public int SetPick(int layer, IfoTilePick pick)//返回值表示被存储层级的增减量
    {
        if (layer < 0 || layer > MapRefer.cLayerMax)
            return 0;
        else if (picks.Count > layer)
        {
            picks[layer] = pick;
            if (picks.Count - 1 == layer)
                return ClearIfInvalid(layer);//可能出现置零的情况，此时要做清除处理，确保数据容量最小化
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

    int ClearIfInvalid(int checkStart)//层级最高，但内容为零，会直接被清除
    {
        if (checkStart == 0) return 0;

        int vary = Method.VaryUntilReach(checkStart, false,
            (turn) =>
            {
                if (turn == 0)//至少要剩一层
                    return true;
                else
                {
                    if (picks[turn].family == 0 && picks[turn].index <= 0)//要让当前最高层，总是有内容的
                        return false;//还有隐患
                    else
                        return true;//不需要继续减了
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
        for (int i = picks.Count - 1; i < layerNeed; i++)//扩容，直至能容纳所需层级
        {
            vary += 1;
            picks.Add(new IfoTilePick());
        }
        return vary;//返回新增了多少个元素，保留符号
    }

}

[Serializable]
public struct IfoTilePick
{
    public int family;//图集所在类别
    public int set;//当前所承载图块素材的所属图集
    public int index;//当前所承载图块素材的对应序数

    public bool NoBody => family == 0 ? true : false;//0类别，都应该是系统图集，没有地形意义
}

//---------------------------------

[Serializable]
public class StoTilePile
{
    public List<StoTilePick> picks;//纵向累积

    public StoTilePile() { picks = new List<StoTilePick>(); }
}

[Serializable]
public class StoTilePick
{
    public int family;//图集所在类别
    public int set;//当前所承载图块素材的所属图集
    public int index;//当前所承载图块素材的对应序数
}

//==================================

[Serializable]
public class IfoWall:Ifo,ICanSto<StoWall>
{
    [SerializeField] protected int width = 1;
    [SerializeField] protected int height = 1;
    [SerializeField] protected IfoTilePile[] piles;//横向累积
                                                   //第几列第几行引用有哪些图块

    public int meTileSum { get { return width * height; } }
    public int meWidth { get { return width; } }
    public int meHeight { get { return height; } }

    public IfoWall() { }//由此初始化的，需要使用前调用初始化函数，外界不用管，这是内部使用的

    public IfoWall(int width, int height)//宽度和高度是必要数据
    {
        NuInitial(width, height);
    }

    public void NuInitial(int width, int height)//用于实例化后还能初始化
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

    public void NuResizeTo(Vector2Int size)//需要保持原来地图数据所在坐标
    {
        int lastWidth = width;
        int lastHeight = height;
        IfoTilePile[] lastPiles = piles;

        NuInitial(size.x, size.y);//新建数据，能自动剔除超域的数据

        for (int i = 0; i < width; i++)
        {
            if (i < lastWidth)
            {
                for (int j = 0; j < height; j++)
                {
                    Vector2Int coord = new Vector2Int(i, j);
                    if (j < lastHeight)//原数据复制
                    {
                        IfoTilePile lastPile = lastPiles[coord.y * lastWidth + coord.x];
                        for (int k = 0; k < lastPile.meNumLayer; k++)
                            SetTile(coord, lastPile.GetPick(k), k);
                    }
                    else//超出部分新建
                        SetTile(coord, new IfoTilePick(), 0);
                }
            }
            else
            {
                for (int j = 0; j < height; j++)//超出部分新建
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

    public IfoTilePile GetTile(int posIndex)//按一维数据来读取
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
    public StoTilePile[] piles;//静态存储类型，一旦有这个地图，含1个图块，和含10个图块，所需存储容量一样
                               //第几列第几行引用有哪些图块
                               //横向累积

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