using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MapRoomEditCtrl : MonoBehaviour
{
    [HideInPlayMode]
    public MapCellEditCtrl mShowCellDp;

    [HideInEditorMode, ShowIf("@this.gameObject.activeSelf")]
    [Button("RemoveRoom")]
    public void DelCurEditRoom()
    {
        mApplyCell.AccessRoom(mCurApplyRoom).MakeRemove();
        BackToSuper();
    }

    [HideInEditorMode, ShowIf("@this.gameObject.activeSelf")]
    [InlineButton("ResizeRoom", "Resize")]
    public Vector2Int mRoomSizeSet;

    [PropertySpace]
    [HideInEditorMode, ShowIf("@this.gameObject.activeSelf")]
    [Button("SaveTiles")]
    void Save()
    {
        mApplyCell.AccessRoom(mCurApplyRoom).MakeSave(EMapIfo.tile);
    }

    [HideInEditorMode, ShowIf("@this.gameObject.activeSelf")]
    [Button("SaveSymbs")]
    void SaveSymbsToPack()
    {
        mApplyCell.AccessRoom(mCurApplyRoom).MakeSave(EMapIfo.symbol);
    }


    //===========================

    MapRoomShow mShowRoom;
    MapSymbolsShow mShowSymbols;

    Vector2Int mCoordCellInTerritory;//����cell������
    List<Vector2Int> mCurApplyRoom;

    public bool meInShow => mInShow;
    bool mInShow;

    public void MakeReady()
    {
        mShowRoom = GetComponent<MapRoomShow>();
        mShowRoom.MakeReady(Vector2Int.zero, MapShowMager.mSpanTile, MapShowMager.mScaleTile);
        mShowRoom.mWhenNeedRoom += ForwardToRoom;

        mShowSymbols = GetComponent<MapSymbolsShow>();
        mShowSymbols.MakeReady(mShowRoom.me, (coord) => mApplyCell.AccessRoom(mCurApplyRoom).neIfoMapSymbs.scatter[coord]);
    }

    public void MakeUse(Vector2Int cellAt, List<Vector2Int> roomAt)
    {
        if (mInShow) return;

        gameObject.SetActive(true);

        mInShow = true;

        mCoordCellInTerritory = cellAt;
        mCurApplyRoom = roomAt; // �������þͿ����ˣ���Ϊͬʱֻ����ʾһ������

        if (gameObject.activeSelf && SysGeneral.meInGame && !mShowCellDp.meInShow)
        {
            MapOneRoom room = mApplyCell.AccessRoom(mCurApplyRoom, true);
            //�ӵ�ͼ�༭���ĵ�ͼ�н���ķ��䣬�Ǿ�һ��Ҫ����ڣ�room��Ӧ�ó���null�����
            if (room.mDataIfo == null) 
                room.mDataIfo = new IfoRoom(mCurApplyRoom, 10, 10);
            mRoomSizeSet = new Vector2Int(room.mDataIfo.wall.meWidth, room.mDataIfo.wall.meHeight);
            mShowRoom.MakeUse(room.mDataIfo);
            mShowSymbols.MakeUse();
            mShowSymbols.MakeNeed(room.neIfoSymbs);
            mShowSymbols.MakeNeed(CollectRoomHave());
        }
        else
            Debug.Log("�����Ϲ�������");
    }

    public void MakeDown()
    {
        mShowRoom.MakeDown();
        mShowSymbols.MakeDown();
        mInShow = false;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!KeyboardInput.mePressInMonitorAny && UnifiedInput.It.meWhenBack()) BackToSuper();
    }

    //==============================

    IfoSymbsInField CollectRoomHave()
    {
        IfoSymbsInField symbsField = new IfoSymbsInField();
        symbsField.kind = EKindSymb.room;
        var rooms = mApplyCell.AccessRoom(mCurApplyRoom).meRooms;
        foreach (var coord in rooms)
            symbsField.scatter.Add(coord, IfoSymbsEachUnit.ForPlaceHolder());
        return symbsField;
    }

    void BackToSuper()
    {
        MakeDown();
        if (mCurApplyRoom.Count > 1)
            MakeUse(mCoordCellInTerritory, mCurApplyRoom.SelfRemoveLast());
        else
            mShowCellDp.MakeUse(mCoordCellInTerritory);
    }

    void ForwardToRoom(Vector2Int at)
    {
        MakeDown();
        MakeUse(mCoordCellInTerritory, mCurApplyRoom.SelfAddLast(at));
    }

    void ResizeRoom()
    {
        MapOneRoom room = mApplyCell.AccessRoom(mCurApplyRoom);
        room.mDataIfo.wall.NuResizeTo(mRoomSizeSet);
        MakeDown();
        MakeUse(mCoordCellInTerritory, mCurApplyRoom);
    }

    MapOneCell mApplyCell => mShowCellDp.mTerritoryIn.AccessCell(mCoordCellInTerritory);

    //===================================

}
