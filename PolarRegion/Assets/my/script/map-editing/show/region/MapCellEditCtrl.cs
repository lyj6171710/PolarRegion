using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MapCellEditCtrl : MonoBehaviour
{
    [HideInEditorMode]
    [ShowIf("@this.gameObject.activeSelf")]
    [PropertyRange(-1,MapRefer.cLayerMax)]
    [InlineButton("ApplyLayer")]
    public int LayerShow;
    void ApplyLayer()
    {
        if (LayerShow < 0)
            mShowCell.me.ShowAllLayer();
        else
        {
            mShowCell.me.OnlyShowLayerAt(LayerShow);
            MapPlates.It.SetLayerHandle(LayerShow);
        }
    }

    [HideInPlayMode] 
    public MapTerritoryEditCtrl mShowTerritoryDp;

    [PropertySpace]
    [HideInEditorMode, ShowIf("@this.gameObject.activeSelf")]
    [Button("SaveTiles")]
    void Save()
    {
        mTerritoryIn.AccessCell(mCoordInTerritory).MakeSave(EMapIfo.tile);
    }


    [HideInEditorMode, ShowIf("@this.gameObject.activeSelf")]
    [Button("SaveSymbs")]
    void SaveSymbsToPack()
    {
        mTerritoryIn.AccessCell(mCoordInTerritory).MakeSave(EMapIfo.symbol);
    }

    [HideInPlayMode]
    public MapRoomEditCtrl mShowRoomDp;

    //===============================

    Vector2Int mCoordInTerritory;//与cell直接对应
    [HideInInspector] public MapOneTerritory mTerritoryIn;//对所属大地图的引用

    MapRegionsShow mShowCell;
    MapSymbolsShow mShowSymbols;

    public bool meInShow => mInShow;
    bool mInShow;

    public void MakeReady()
    {
        mShowCell = GetComponent<MapRegionsShow>();
        mShowCell.MakeReady();

        mShowCell.NuWhenNeedRoom += ForwardToRoom;

        mShowSymbols = GetComponent<MapSymbolsShow>();
        mShowSymbols.MakeReady(mShowCell.me, (coord) => mTerritoryIn.AccessCell(mCoordInTerritory).neIfoMapSymbs.scatter[coord]);
    }
    
    void Update()
    {
        if (!KeyboardInput.mePressInMonitorAny && UnifiedInput.It.meWhenBack()) BackToTerritory();
    }

    public void MakeUse(Vector2Int coord, MapOneTerritory territory = null)
    {
        if (mInShow) return;

        gameObject.SetActive(true);

        mInShow = true;

        mCoordInTerritory = coord;
        if (territory != null) mTerritoryIn = territory;//如果没有值，就保持原来有的

        if (SysGeneral.meInGame)
        {
            MapOneCell cellIn = mTerritoryIn.AccessCell(mCoordInTerritory, true);
            IfoCell apply = cellIn.mDataIfo;
            mShowCell.MakeUse(apply);
            mShowSymbols.MakeUse();
            mShowSymbols.MakeNeed(mTerritoryIn.AccessCell(mCoordInTerritory).neIfoSymbs);
            mShowSymbols.MakeNeed(CollectRoomHave());
        }
        else
            Debug.Log("不符合构造条件");
    }

    public void MakeDown()
    {
        mShowCell.MakeDown();
        mShowSymbols.MakeDown();
        mInShow = false;
        gameObject.SetActive(false);
    }

    //===================================

    IfoSymbsInField CollectRoomHave()
    {
        IfoSymbsInField symbsField = new IfoSymbsInField();
        symbsField.kind = EKindSymb.room;
        var rooms = mTerritoryIn.AccessCell(mCoordInTerritory).meRooms;
        foreach (var coord in rooms)
            symbsField.scatter.Add(coord, IfoSymbsEachUnit.ForPlaceHolder());
        return symbsField;
    }

    void BackToTerritory()
    {
        MakeDown();
        mShowTerritoryDp.MakeUse();
    }

    void ForwardToRoom(Vector2Int regionAt, Vector2Int inRegionAt)
    {
        MakeDown();
        mShowRoomDp.MakeUse(mCoordInTerritory, new List<Vector2Int>() { inRegionAt });
    }
}
