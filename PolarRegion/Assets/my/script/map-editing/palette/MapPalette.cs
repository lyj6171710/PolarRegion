using System.Collections.Generic;
using UnityEngine;

public interface IPalette
{
    public IfoTilePick meCurSelect { get; }

    public IfoTilePick[][] meCurSelects { get; }
}

public class MapPalette : MonoBehaviour,
    IPalette, ITileOperate, IGridOperate
{
    //负责展示tileset

    //应变数据============================================

    TilesetOneSet mSetUse;

    public void MakeReady()//不要在awake时调用
    {
        mTiles = new List<FieldTile>();
        mLattice = new FieldMatrixMager();
        ReadyLattice();
        SuWhenNewSelect += () => { };
    }

    public void MakeUse(TilesetOneSet use)
    {
        mSetUse = use;

        CreateLattice();
        DrawLattice();

        FormSelectBox();
        mSelect.SuShowBox(true);
    }

    public void MakeDown()
    {
        mLattice.RemoveAllGrids();
        mTiles.Clear();

        Destroy(mSelectBox);
        mSelect = null;
    }

    public void MakeRemove() { }//持久存在，不会出现需要remove的情况

    //网格==============================================

    List<FieldTile> mTiles;//承载显示tile的格子
    FieldMatrixMager mLattice;

    float mScale = 0.5f;//单元格的伸缩

    void ReadyLattice()
    {
        mLattice.MakeReady(Camera.main.transform, this);
        Vector2 viewSpan = SceneViewL.It.SuGetCornerOffsetFromViewCentre();
        Vector2 posBeginOffset = new Vector2(-viewSpan.x + MapRefer.cSpanStart, viewSpan.y - MapRefer.cSpanStart);
        mLattice.ReadyAttr(posBeginOffset, 1, mScale * Vector2.one);

        SceneViewL.It.meWhenViewableSizeChange += () => {
            Vector2 viewSpan = SceneViewL.It.SuGetCornerOffsetFromViewCentre();
            Vector2 posBeginOffset = new Vector2(-viewSpan.x + MapRefer.cSpanStart, viewSpan.y - MapRefer.cSpanStart);
            mLattice.OffsetLattice(posBeginOffset);

            mSelect.SuUpdateByMager();
        };

        SceneViewL.It.meWhenViewMove += () => {
            mSelect.SuUpdateByMager();
        };
    }

    void CreateLattice()
    {
        mLattice.CreateLattice(mSetUse.mRowsDp, mSetUse.mColsDp, false);
        for (int i = 0; i < mLattice.meGridsNum; i++)
        {
            Vector2Int at = mLattice.SuGetCoordByOrderedCount(i);
            mLattice[at].transform.localPosition += new Vector3(0, 0, 1);//相对位置要在摄像机前面一些，保持显示
            FieldTile tile = mLattice[at].gameObject.AddComponent<FieldTile>();
            tile.MakeReady(this, mLattice[at]);
            mTiles.Add(tile);
        }
    }

    void DrawLattice()
    {
        for (int i = 0; i < mSetUse.meSum; i++)
        {
            IfoTilePick pick = new IfoTilePick();
            MapPlates.It.GetSetIndex(mSetUse.meFamily, mSetUse.meName, out pick.family, out pick.set);
            pick.index = i;
            mTiles[i].NuDrawTileAtLayer(0, pick);
        }
    }

    //选择===================================

    public IfoTilePick meCurSelect => mCurSelect;
    public IfoTilePick[][] meCurSelects => mCurSelects;
    public bool meSelectFromMap => mSelectFromMap;
    public bool meInMultiSelect => mInMultiSelect;
    public FieldSelectBox meSelect => mSelect;

    public System.Action SuWhenNewSelect;

    GameObject mSelectBox;//标记当前所选的tile
    FieldSelectBox mSelect;

    bool mSelectFromMap = false;
    bool mInMultiSelect = false;
    IfoTilePick mCurSelect;
    IfoTilePick[][] mCurSelects;

    public void UpdateSelect(bool fromMap, IfoTilePick pick = default(IfoTilePick))
    {
        mInMultiSelect = false;
        mSelectFromMap = fromMap;
        mCurSelect = pick;
        SuWhenNewSelect();
    }

    public void UpdateSelects(bool fromMap, IfoTilePick[][] picks = null)
    {
        mInMultiSelect = true;
        mSelectFromMap = fromMap;
        mCurSelects = new IfoTilePick[picks.Length][];
        for (int i = 0; i < picks.Length; i++)
        {
            mCurSelects[i] = new IfoTilePick[picks[i].Length];
            for (int j = 0; j < picks[i].Length; j++)
                mCurSelects[i][j] = picks[i][j];
        }
    }
    
    void FormSelectBox()
    {
        mSelectBox = new GameObject("select");//不作为单元格的子物体，因为单元格可以多选
        mSelect = mSelectBox.AddComponent<FieldSelectBox>();
        IfoSelectBox ifo = new IfoSelectBox();
        ifo.border = MapPlates.It.meSelectBoxPaletteDp;
        ifo.scale = mScale;
        mSelect.MakeReady(mLattice, ifo);
        mSelect.SuWhenSingleSelectEnd += () => { 
            IfoTilePick pick = new IfoTilePick();
            MapPlates.It.GetSetIndex(mSetUse.meFamily, mSetUse.meName, out pick.family, out pick.set);
            pick.index = mLattice.SuGetOrderedIndexFromBegin(mSelect.me.CurSelect);
            UpdateSelect(false, pick);
        };
        mSelect.SuWhenMultiSelectEnd += () => {
            int family, set;
            MapPlates.It.GetSetIndex(mSetUse.meFamily, mSetUse.meName, out family, out set);
            Vector2Int[][] selectCoords = mSelect.me.CurSelects;
            int[][] selects = new int[selectCoords.Length][];
            for (int i = 0; i < selects.Length; i++)
            {
                selects[i] = new int[selectCoords[i].Length];
                for (int j = 0; j < selects[i].Length; j++)
                    selects[i][j] = mLattice.SuGetOrderedIndexFromBegin(selectCoords[i][j]);
            }
            IfoTilePick[][] picks = new IfoTilePick[selects.Length][];
            for (int i = 0; i < selects.Length; i++)
            {
                picks[i] = new IfoTilePick[selects[i].Length];
                for (int j = 0; j < selects[i].Length; j++)
                {
                    IfoTilePick pick = new IfoTilePick();
                    pick.family = family;
                    pick.set = set;
                    pick.index = selects[i][j];
                    picks[i][j] = pick;
                }
            }
            UpdateSelects(false, picks);
        };

        UpdateSelect(false);
    }

    void UpdateSelectBox()
    {
        if (MapPlates.It.meInTileSelect)
        {
            mSelect.me.CanSelect = true;
        }
        else
        {
            mSelect.me.CanSelect = false;
        }
    }

    public void IWhenHoverTileFmOpera(IfoTilePick pick)
    {
        
    }

    public void IWhenHoverGridFmOpera(Vector2Int coord)
    {
        MapPlates.It.mInTileSelect = true;
    }

    //========================================

    public static MapPalette It;

    void Awake()
    {
        It = this;
    }

    void Update()
    {
        UpdateSelectBox();
    }

}
