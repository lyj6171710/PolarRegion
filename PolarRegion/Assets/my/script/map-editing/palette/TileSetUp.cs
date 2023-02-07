using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TileSetUp : MonoBehaviour
{
    [ReadOnly]
    [HideInEditorMode]
    public string mSetFamily;
    [ReadOnly]
    [HideInEditorMode]
    public string mSetName;

    [HideInEditorMode]
    public IfoTile mSetToNow;
    //这个数据和图集里面的数据是同步的，同一个，所以在这里设置，就能应用到图集中
    //不过注意，无法立即应用到已经选用了图集的地图块，需要重新加载所属地图才行

    [HideInEditorMode]
    public StrAttrs.StoSave mSetAttrToNow;

    //=========================

    MapPalette mPalette;

    public void MakeReady()
    {
        mPalette = GetComponent<MapPalette>();
        mPalette.SuWhenNewSelect += NewSelect;
        mSetAttrToNow = new StrAttrs.StoSave();
        mSetAttrToNow.WhenNumChanged += () => { mSetToNow.attrs = mSetAttrToNow.WriteOut(); };
        mSetAttrToNow.WhenElemChanged += () => { mSetToNow.attrs = mSetAttrToNow.WriteOut(); };
    }

    public void MakeUse()
    {
        NewSelect();
    }

    void NewSelect()
    {
        IfoTilePick pick = MapPalette.It.meCurSelect;
        mSetToNow = MapPlates.It.GetTileAttrRef(pick);
        mSetFamily = MapPlates.It.SuGetFamilyName(pick.family);
        mSetName = MapPlates.It.SuGetSetName(pick.family, pick.set);
        mSetAttrToNow.ResetNum();
        mSetAttrToNow.ReadIn(mSetToNow.attrs);
    }

}
