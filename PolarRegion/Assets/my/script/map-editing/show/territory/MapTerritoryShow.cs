using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class MapTerritoryShow : MonoBehaviour,
    ITileOperate,IGridOperate,ICanPaint
{
    //越仅负责数据的表达显示

    IfoTerritory mData;//对当前地图内容的记录，将可以还原

    //==========================

    public Service me;

    public void MakeReady()
    {
        me = new Service(this);
        NuNotifyNewSelectFmField += (pos) => { };

        mShows = new Dictionary<Vector2Int, FieldTile>();
        mLattice = new FieldDiscreteMager();
        mLattice.MakeReady(transform, this);
        ReadyPaint();
    }

    public void MakeDown()
    {
        if (mShows != null)
        {
            mShows.Clear();
            mLattice.RemoveAllGrids();
            mPaint.MakeDown();
        }
    }

    public void MakeUse(IfoTerritory one)
    {
        mData = one;

        ShowTerritory();

        mPaint.MakeUse(mLattice);

        mPaint.neSelect.SuWhenSingleSelectEnd += () => NuNotifyNewSelectFmField(mLattice[mCoordHover].transform);
    }

    //================================

    Vector2Int mCurMouseIn;
    public Action<Vector2Int> NuWhenDoubleCell;

    private void Update()
    {
        mCurMouseIn = UnifiedCursor.It.SuComputeCursorIn(Vector2Int.one);

        UpdateCell();

        if (UnifiedInput.It.meDoubleConfirm())
        {
            if (mPaint != null && mPaint.meInUse)
            {
                if (mPaint.neSelect.me.IsHoverSelect)
                {
                    Vector2Int cellCoord = mPaint.neSelect.me.CurSelect;
                    NuWhenDoubleCell(cellCoord);
                }
            }
        }
    }

    //================================

    Dictionary<Vector2Int, FieldTile> mShows;//用来显示图块的网格
    FieldDiscreteMager mLattice;

    void UpdateCell()
    {
        if (KeyboardInput.SuIsInPress(MapRefer.cKeyArea))
        {
            if (UnifiedInput.It.meTapConfirm())
                AddCell(mCurMouseIn);
            else if (UnifiedInput.It.meWhenBack())
                DelCell(mCurMouseIn);
        }
    }

    void ShowTerritory()
    {
        if (!me.iInShow)
        {
            mLattice.AcceptRefer(mData);
            mLattice.ReadyAttr(MapRefer.cSpanStart * Vector2.one, 1, Vector2.one);
            Vector2Int[] poses = mData.SuGetAllHoldPos();
            for (int i = 0; i < poses.Length; i++)
                FormCell(poses[i]);
        }
    }

    bool WhetherCanOperateCell()
    {
        return true;
    }

    //--------------------------------

    void FormCell(Vector2Int at)
    {
        if (mData.SuWhetherHaveHeld(at))
        {
            InstallCell(at, mData.GetCellTile(at));
        }
    }

    void AddCell(Vector2Int at)
    {
        if (!WhetherCanOperateCell()) return;

        if (mData.SuWhetherInRange(at) && !mData.SuWhetherHaveHeld(at))
        {
            if (InstallCell(at))
            {
                mData.SuAddOne(at);
                mData.SetCellTile(at, default(IfoTilePick));
            }
        }
    }

    void DelCell(Vector2Int at)
    {
        if (!WhetherCanOperateCell()) return;

        if (mData.SuWhetherHaveHeld(at) && mShows.ContainsKey(at))
        {
            mLattice.SuRemoveOneGrid(at);
            mShows.Remove(at);
            mData.SuDelOne(at);
        }
    }

    //-------------------------------------------

    FieldTile BuildCellAt(Vector2Int at)
    {
        FieldTile tile = mLattice.SuAddOneGrid(at).AddComponent<FieldTile>();
        tile.MakeReady(this, mLattice[at]);
        return tile;
    }

    bool InstallCell(Vector2Int at, IfoTilePick pick = default(IfoTilePick))
    {
        if (mShows.ContainsKey(at))
            return false;
        else
        {
            FieldTile cell = BuildCellAt(at);
            cell.NuDrawTileAtLayer(0, pick);
            mShows.Add(at, cell);
            return true;
        }
    }

    //涂抹功能==========================

    MapGetPaint mPaint;

    void ReadyPaint()
    {
        mPaint = gameObject.AddComponent<MapGetPaint>();
        mPaint.MakeReady(this);
    }

    public void IPaintTo(Vector2Int gridCoord, IfoTilePick pick)
    {
        mData.SetCellTile(gridCoord, pick);
        mShows[gridCoord].NuDrawTileAtLayer(0, pick);
    }

    public void IEraseFrom(Vector2Int gridCoord)
    {
        mData.SetCellTile(gridCoord, new IfoTilePick());
        mShows[gridCoord].NuEraseTile(0);
    }

    public IfoTilePick IGetPick(Vector2Int gridCoord)
    {
        return mData.GetCellTile(gridCoord);
    }

    //=======================================

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

    //===================================

    public class Service : Srv<MapTerritoryShow>,ISymbRefer
    {
        public Service(MapTerritoryShow belong) : base(belong)
        {
        }

        public Action<Transform> INotifyNewSelectFmField { get => j.NuNotifyNewSelectFmField; set => j.NuNotifyNewSelectFmField = value; }

        public Dictionary<Vector2Int, GameObject> iPlaneUnits
        {
            get
            {
                Dictionary<Vector2Int, GameObject> list = new Dictionary<Vector2Int, GameObject>();
                foreach (Vector2Int coord in j.mShows.Keys) list.Add(coord, j.mShows[coord].gameObject);
                return list;
            }
        }

        public bool iIsCursorOn => j.mLattice.SuWhetherCursorInField();

        public Vector2Int iIsOn => j.mPaint.neSelect.me.CurHover;

        public bool iInShow => j.mShows.Count != 0;
    }
}
