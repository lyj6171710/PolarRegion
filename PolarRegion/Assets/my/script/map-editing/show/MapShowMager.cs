using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum EMapMode { edit, use, preview }

public class MapShowMager : MonoBehaviour
{
    [HideInPlayMode]
    public EMapMode mModeDp;//游戏运行状态下不能改变其值

    bool IfInEdit() => mModeDp == EMapMode.edit;

    public MapTerritoryEditCtrl mTerritoryEditDp;
    public MapCellEditCtrl mCellEditDp;
    public MapRoomEditCtrl mRoomEditDp;

    public void MakeReady()
    {
        It = this;
        mTerritoryEditDp.MakeReady();
        mCellEditDp.MakeReady();
        mRoomEditDp.MakeReady();
    }

    public void MakeUse()
    {
        mTerritoryEditDp.MakeUse();
    }

    public static MapShowMager It;

    public static float mSpanTile = MapRefer.cSpan;

    public static Vector2 mScaleTile = Vector2.one;
}
