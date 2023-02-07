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
    //������ݺ�ͼ�������������ͬ���ģ�ͬһ�����������������ã�����Ӧ�õ�ͼ����
    //����ע�⣬�޷�����Ӧ�õ��Ѿ�ѡ����ͼ���ĵ�ͼ�飬��Ҫ���¼���������ͼ����

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
