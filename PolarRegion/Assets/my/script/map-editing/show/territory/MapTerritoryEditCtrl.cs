using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MapTerritoryEditCtrl : MonoBehaviour
{
    [HideInPlayMode]
    public MapsSource mMapDataDp;//只是预置用，运行态下不需要知道
    [HideInPlayMode]
    public MapCellEditCtrl mShowCellDp;
    [Required,HideInPlayMode]
    public MapOneTerritory mStartSelectTerritoryDp;

    [HideInEditorMode, EnableIf("@this.gameObject.activeSelf")]
    [ValueDropdown("GetWorlds")]
    public GameObject mCurSelectWorldPanel;

    [HideInEditorMode, EnableIf("@this.gameObject.activeSelf")]
    [ValueDropdown("GetTerritorys")]
    [OnValueChanged("SwitchTerritory")]
    public GameObject mCurSelectTerritoryPanel;

    [HideInEditorMode, ShowIf("@this.gameObject.activeSelf")]
    [Button("SaveTiles")]
    void Save()
    {
        mCurApplyTerritory.MakeSave(EMapIfo.tile);
    }


    [HideInEditorMode, ShowIf("@this.gameObject.activeSelf")]
    [Button("SaveSymbs")]
    void SaveSymbsToPack()
    {
        mCurApplyTerritory.MakeSave(EMapIfo.symbol);
    }

    GameObject[] GetWorlds()
    {
        return GbjAssist.GetSubGbjs(mMapDataDp.transform);
    }

    GameObject[] GetTerritorys()
    {
        if (mCurSelectWorldPanel != null)
            return GbjAssist.GetSubGbjs(mCurSelectWorldPanel.transform);
        else
            return null;
    }

    void SwitchTerritory()
    {
        MapOneTerritory recent = mCurSelectTerritoryPanel.GetComponent<MapOneTerritory>();
        if (recent != null && recent != mCurApplyTerritory)
        {
            MakeDown();
            mCurApplyTerritory = recent;
            MakeUse();
        }
    }

    //===========================

    MapTerritoryShow mShowTerritory;
    MapSymbolsShow mShowSymbols;
    MapOneTerritory mCurApplyTerritory;

    public bool meInShow => mInShow;
    bool mInShow;

    public void MakeReady()
    {
        mShowTerritory = GetComponent<MapTerritoryShow>();
        mShowTerritory.MakeReady();
        mShowTerritory.NuWhenDoubleCell += WhenExploreCell;

        mShowSymbols = GetComponent<MapSymbolsShow>();
        mShowSymbols.MakeReady(mShowTerritory.me, (coord) => mCurApplyTerritory.neIfoMapSymbs.scatter[coord]);

        mCurApplyTerritory = mStartSelectTerritoryDp;
        mCurSelectTerritoryPanel = mCurApplyTerritory.gameObject;
        mCurSelectWorldPanel = mCurApplyTerritory.me.WorldBelong;
    }

    public void MakeUse()
    {
        if (mInShow) return;

        gameObject.SetActive(true);

        mInShow = true;

        if (gameObject.activeSelf && SysGeneral.meInGame && !mShowTerritory.me.iInShow)
            ShowTerritoryCurSelect();
        else
            Debug.Log("不符合构造条件");
    }

    public void MakeDown()
    {
        mShowTerritory.MakeDown();
        mShowSymbols.MakeDown();
        mInShow = false;
        gameObject.SetActive(false);
    }

    //==============================

    void ShowTerritoryCurSelect()
    {
        mShowTerritory.MakeUse(mCurApplyTerritory.mDataMap);
        mShowSymbols.MakeUse();
        mShowSymbols.MakeNeed(mCurApplyTerritory.neIfoSymbs);
    }

    void WhenExploreCell(Vector2Int where)
    {
        MakeDown();
        mShowCellDp.MakeUse(where, mCurApplyTerritory);
    }

    //===================================


}
