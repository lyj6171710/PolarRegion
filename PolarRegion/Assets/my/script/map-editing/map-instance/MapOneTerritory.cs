using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

public class MapOneTerritory : MonoBehaviour
{
    //=======================================

    MapOneWorld mWorldBelong;
    public IfoTerritory mDataMap;//对当前地图内容的记录，将可以还原
    StoTerritory mDataSto;
    IfoShelfFile mAttach;
    
    public Service me;

    public void MakeReady(MapOneWorld belong)
    {
        me = new Service(this);
        mWorldBelong = belong;

        BuildCatalogue();
        MakeLoad();
        mCells = GetComponent<MapTerritoryCells>();
        mCells.MakeReady();
        mSymbolsPack = GetComponent<MapSymbolsPack>();
        mSymbolsPack.MakeReady(mAttach);
    }

    //==============================

    public void BuildCatalogue()//存储环境
    {
        mAttach = new IfoShelfFile();
        mAttach.name = me.StrWorldAndTerri;
        mAttach.super = me.StrWorld;
        mAttach.withFolder = true;
        VirtualDisk.It.AddFileToVtDisk(mAttach);
    }

    void MakeLoad()
    {
        VirtualDisk.It.LoadFileInRealDisk(mAttach, out mDataSto);

        mDataMap = new IfoTerritory();
        mDataMap.SuInitialMap();

        if (mDataSto != null)
        {
            for (int i = 0; i < mDataSto.cells.Count; i++)
            {
                Vector2Int coord = mDataSto.cells[i].coord;
                mDataMap.SuAddOne(coord);
                mDataMap.SetCellTile(coord, mDataSto.cells[i].pick);
            }
        }
    }

    public void MakeSave(EMapIfo kind)
    {
        if (kind == EMapIfo.tile)
        {
            mDataSto = new StoTerritory();
            for (int i = 0; i < mDataMap.meSum; i++)
            {
                StoTerritory.StoCellUnit unit = new StoTerritory.StoCellUnit();
                unit.coord = mDataMap.SuGetCoord(i);
                unit.pick = mDataMap.GetCellTile(unit.coord);
                mDataSto.cells.Add(unit);
            }
            VirtualDisk.It.SaveFileInRealDisk(mAttach, mDataSto);
        }
        else if (kind == EMapIfo.symbol)
        {
            mSymbolsPack.MakeSave();
        }
    }

    //=================================

    MapSymbolsPack mSymbolsPack;
    public IfoSymbsInField neIfoSymbs => mSymbolsPack.meSymbs;
    public IfoSymbsInLattice neIfoMapSymbs => mSymbolsPack.mSymbs;

    //=================================

    MapTerritoryCells mCells;

    public MapOneCell AccessCell(Vector2Int coord, bool force = false) => mCells.AccessCell(coord, force);

    public class Service : Srv<MapOneTerritory>
    {
        public Service(MapOneTerritory belong) : base(belong)
        {
        }

        public GameObject WorldBelong => j.mWorldBelong.gameObject;

        public string Name => j.gameObject.name;

        public string StrWorldAndTerri => StrWorld + "-" + Name;

        public string StrWorld => j.mWorldBelong.meName;
    }
}


public class IfoTerritory : MapDataHandle,IDiscreteUnits
{
    List<IfoCellUnit> cells;//某行某列是否被占据，属于三维数据

    public IfoTerritory() : base(MapRefer.cCellNumMax)
    {
        cells = new List<IfoCellUnit>();
    }

    protected override int ReConstruct()
    {
        cells = new List<IfoCellUnit>();
        return MapRefer.cCellNumMax;
    }

    //---------------------------------------

    public IfoCell GetCellContent(Vector2Int at)
    {
        return cells[SuGetIndex(at)].content;
    }

    public void SetCellContent(Vector2Int at, IfoCell content)
    {
        cells[SuGetIndex(at)].content = content;
    }

    public IfoTilePick GetCellTile(Vector2Int at)
    {
        return cells[SuGetIndex(at)].pick;
    }

    public void SetCellTile(Vector2Int at, IfoTilePick pick)
    {
        cells[SuGetIndex(at)].pick = pick;
    }

    //---------------------------------------

    protected override Vector2Int GetCoordStraight(int index)
    {
        return cells[index].coord;
    }

    //---------------------------------------

    protected override void GetCurHoldToMap(Action<Vector2Int, int> deal)
    {
        for (int i = 0; i < cells.Count; i++)
        {
            deal(cells[i].coord, i);
        }
    }

    protected override void AddOne(Vector2Int at)
    {
        IfoCellUnit cell = new IfoCellUnit(at);
        cells.Add(cell);
    }

    protected override void DelOne(Vector2Int at)
    {
        cells.RemoveAt(SuGetIndex(at));
    }

    class IfoCellUnit//用来支持动态的地图内容大小
    {
        //作为会被存储的数据，要尽量精简

        public Vector2Int coord;
        public IfoTilePick pick;//图块自身的表达
        public IfoCell content;//临时性的，因为不可能存一个地图，而把所有地图数据都再存一遍
                               //这个会在运行后，读取数据时，临时还原引用关系
                               //这个变量不会被存储的

        public IfoCellUnit(Vector2Int coord)
        {
            this.coord = coord;
            pick = new IfoTilePick();
        }
    }
}

//==========================================

[System.Serializable]
public class StoTerritory
{
    public List<StoCellUnit> cells;//某行某列是否被占据，属于三维数据

    public StoTerritory() { cells = new List<StoCellUnit>(); }

    [System.Serializable]
    public class StoCellUnit
    {
        public Vector2Int coord;
        public IfoTilePick pick;//图块自身的表达
    }
}