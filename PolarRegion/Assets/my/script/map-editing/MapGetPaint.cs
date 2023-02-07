using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface ICanPaint
{
    public void IPaintTo(Vector2Int gridCoord, IfoTilePick pick);
    public void IEraseFrom(Vector2Int gridCoord);
    public IfoTilePick IGetPick(Vector2Int gridCoord);
}

public class MapGetPaint : MonoBehaviour
{
    //负责取色与准备往指定格子上涂色，但只有涂色行为数据，具体造成的影响，外界自己控制

    //==============================

    public bool meInUse => mInUse;

    public FieldSelectBox neSelect { get; set; }//外界可以读取选取状态数据，方便扩展功能

    FieldGridMager mLattice;//引用自外界，管不了

    GameObject mSelectBox;//标记当前所选的Tile
    
    ICanPaint mTarget;

    bool mInUse;

    string mKeyFill = "w";
    string mKeyBlank = "e";

    public void MakeDown()
    {
        Destroy(mSelectBox);
        neSelect = null;
        mInUse = false;
    }

    public void MakeReady(ICanPaint target)
    {
        mTarget = target;
    }

    public void MakeUse(FieldGridMager one)
    {
        mLattice = one;

        BuildBox();
        neSelect.SuShowBox(true);

        mInUse = true;
    }

    //选择功能=================================

    IfoTilePick mCurSelect => mTarget.IGetPick(neSelect.me.CurSelect);

    IfoTilePick[][] mCurSelects
    {
        get
        {
            Vector2Int[][] selects = neSelect.me.CurSelects;
            IfoTilePick[][] picks = new IfoTilePick[selects.Length][];
            for (int i = 0; i < selects.Length; i++)
            {
                picks[i] = new IfoTilePick[selects[i].Length];
                for (int j = 0; j < selects[i].Length; j++)
                {
                    IfoTilePick pick = new IfoTilePick();
                    IfoTilePick refer = mTarget.IGetPick(selects[i][j]);
                    pick.family = refer.family;
                    pick.set = refer.set;
                    pick.index = refer.index;
                    picks[i][j] = pick;
                }
            }
            return picks;
        }
    }

    void BuildBox()
    {
        mSelectBox = new GameObject("select");//不作为单元格的子物体，因为单元格可以多选
        neSelect = mSelectBox.AddComponent<FieldSelectBox>();
        IfoSelectBox ifo = new IfoSelectBox();
        ifo.border = MapPlates.It.meSelectBoxPaletteDp;
        ifo.scale = 1;
        neSelect.MakeReady(mLattice, ifo);
        neSelect.SuWhenSingleSelectEnd += () => {
            if (Input.GetKey(KeyCode.LeftAlt))
                MapPalette.It.UpdateSelect(true, mCurSelect);
        };
        neSelect.SuWhenMultiSelectEnd += () => {
            if (Input.GetKey(KeyCode.LeftAlt))
                MapPalette.It.UpdateSelects(true, mCurSelects);
        };
    }

    void PaintTileRange(Dictionary<Vector2Int, IfoTilePick> picks)
    {
        foreach (Vector2Int at in picks.Keys)
        {
            mTarget.IPaintTo(at, picks[at]);
        }
    }

    //涂抹功能====================================

    void Update()
    {
        if (mSelectBox == null) return;

        if (Input.GetKey(mKeyFill))
        {
            if (!MapPalette.It.meInMultiSelect)
            {
                if (mLattice.SuWhetherCursorInField())
                    mTarget.IPaintTo(neSelect.me.CurHover, MapPalette.It.meCurSelect);
            }
            else
            {
                if (neSelect.me.IsHoverSelect)
                {
                    Dictionary<Vector2Int, IfoTilePick> fills = new Dictionary<Vector2Int, IfoTilePick>();
                    IfoTilePick[][] pickSelects = MapPalette.It.meCurSelects;
                    for (int i = 0; i < pickSelects.Length; i++)
                    {
                        for (int j = 0; j < pickSelects[i].Length; j++)
                        {
                            Vector2Int seat = neSelect.me.CurSelect + new Vector2Int(i, -j);
                            //这里保证，总是从左上填充到右下
                            if (mLattice.SuContainCoord(seat))
                            {
                                if (!MapPalette.It.meSelectFromMap)//不同素材来源，素材单元的序数与这里的序数不是同步的
                                    fills.Add(seat, pickSelects[i][j]);//j越大，素材序数越大
                                else
                                    fills.Add(seat, pickSelects[i][pickSelects[i].Length - 1 - j]);//j越大，素材序数越小
                            }
                        }
                    }
                    PaintTileRange(fills);
                }
            }
        }
        else if (Input.GetKeyDown(mKeyBlank))
            mTarget.IEraseFrom(neSelect.me.CurHover);

        if (MapPlates.It.meInTileSelect)
        {
            neSelect.me.CanSelect = false;
        }
        else
        {
            neSelect.me.CanSelect = true;
        }
    }

    //=============================

    public class Service : Srv<MapGetPaint>
    {
        public Service(MapGetPaint belong) : base(belong)
        {
        }
    }

}
