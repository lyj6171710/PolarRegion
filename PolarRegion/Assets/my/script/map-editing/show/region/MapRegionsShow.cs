using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class MapRegionsShow : MonoBehaviour
{
    //越仅负责数据的表达显示

    IfoCell mData;//地图内容

    public Service me;

    public void MakeReady()
    {
        mShows = new Dictionary<Vector2Int, MapRegionShow>();
        me = new Service(this);
    }

    public void MakeUse(IfoCell data)
    {
        mData = data;

        mCurViewIn = ComputeViewRegion();

        RefreshShow();
    }

    public void MakeDown()
    {
        if (mShows != null)
        {
            foreach (MapRegionShow one in mShows.Values) one.MakeRemove();
            mShows.Clear();
        }
    }

    //====================================

    public Action<Vector2Int, Vector2Int> NuWhenNeedRoom;

    public Action<Transform> NuWhenNewSelectFmField;

    Vector2Int mCurMouseIn;//不受区域大小限制

    void Update()
    {
        mCurMouseIn = UnifiedCursor.It.SuComputeCursorIn(mSpanRegion.ToInt());

        if (mData != null)
        {
            UpdateRegion();

            UpdateShow();
        }
    }

    void WhenNeedRoom(MapRegionShow regionBelong, Vector2Int coord)
    {//创建或进入房间
        NuWhenNeedRoom(regionBelong.me.CoordInSuper, coord);
    }

    //显隐控制模块===================================

    Vector2Int mLastViewIn;//视野的上一所在区
    Vector2Int mCurViewIn;
    
    void UpdateShow()
    {
        mCurViewIn = ComputeViewRegion();
        if (mLastViewIn != mCurViewIn)
        {
            RefreshShow();
            mLastViewIn = mCurViewIn;
        }
    }

    void RefreshShow()
    {
        List<Vector2Int> coordBox;
        coordBox = OverTool.GetAround(mLastViewIn);
        foreach (Vector2Int coord in coordBox)
            HideRegion(coord);
        coordBox = OverTool.GetAround(mCurViewIn);
        foreach (Vector2Int coord in coordBox)
            AppearRegion(coord);//周边
        AppearRegion(mCurViewIn);//中心
    }

    void AppearRegion(Vector2Int at)
    {
        if (mShows.ContainsKey(at))
        {
            mShows[at].gameObject.SetActive(true);
        }
        else
        {
            if (mData.SuWhetherHaveHeld(at))
            {
                FormRegion(at);
            }
        }
    }

    void HideRegion(Vector2Int at)
    {
        if (mShows.ContainsKey(at))
        {
            mShows[at].gameObject.SetActive(false);
        }
    }

    //数据操作模块==================================

    Dictionary<Vector2Int, MapRegionShow> mShows;//地图片区，根据地图内容创建出来

    void UpdateRegion()
    {
        if(WhetherCanOperateRegion())
        {
            if (KeyboardInput.SuIsInPress(MapRefer.cKeyArea))
            {
                if (UnifiedInput.It.meTapConfirm())
                    AddRegion(mCurMouseIn);
                else if (UnifiedInput.It.meWhenBack())
                    DelRegion(mCurMouseIn);
            }
        }
    }

    bool WhetherCanOperateRegion()
    {
        //if (!Mager_map.Single.in_game) return;//只游戏状态下有效
        //if (!Mager_region.Single.can_del) return;//确认增加下才有效
        return true;
    }

    void FormRegion(Vector2Int at)//片区从无到有（本来就有内容）
    {
        if (mData.SuWhetherHaveHeld(at))
        {
            InstallRegion(at, mData.GetRegionContent(at));
        }
    }

    void AddRegion(Vector2Int at)//内容和片区从无到有（新加内容）
    {
        if (mData.SuWhetherInRange(at))
        {
            if (mData.SuWhetherHaveHeld(at))
            {
                IfoRegion ifo = mData.GetRegionContent(at);
                if (ifo.wall.meTileSum <= 1)//有内容，但等于没有内容时
                {
                    mShows[at].MakeRemove();
                    mShows.Remove(at);
                    ifo = new IfoRegion(mNumTile.x, mNumTile.y);
                    if (InstallRegion(at, ifo))
                        mData.SetRegionContent(at, ifo);
                }
            }
            else
            {
                IfoRegion ifo = new IfoRegion(mNumTile.x, mNumTile.y);
                //由于单元化了，每个单元的长宽就应该固定，否则一些操作会很麻烦
                if (InstallRegion(at, ifo))
                {
                    mData.SuAddOne(at);
                    mData.SetRegionContent(at, ifo);
                }
            }
        }
        
    }

    void DelRegion(Vector2Int at)//内容和片区都从有到无
    {
        if (mData.SuWhetherHaveHeld(at) && mShows.ContainsKey(at)) 
        {
            Destroy(mShows[at].gameObject);
            mShows.Remove(at);
            mData.SuDelOne(at);
        }
    }

    //---------------------------------------

    bool InstallRegion(Vector2Int at, IfoRegion content)
    {
        if (mShows.ContainsKey(at))
            return false;
        else
        {
            MapRegionShow region = BuildRegionAt(new Vector2(at.x, at.y) * mSpanRegion);
            region.MakeReady(at, MapShowMager.mSpanTile, MapShowMager.mScaleTile);
            region.mWhenNeedRoom += WhenNeedRoom;
            region.NuNotifyNewSelectFmField += NuWhenNewSelectFmField;
            region.MakeUse(content);
            mShows.Add(at, region);
            return true;
        }
    }

    MapRegionShow BuildRegionAt(Vector2 start)
    {
        GameObject obj = new GameObject("region");
        obj.transform.SetParent(transform);
        Vector3 pos = new Vector3(start.x, start.y, 0);
        obj.transform.position = pos;
        MapRegionShow region = obj.AddComponent<MapRegionShow>();
        return region;
    }

    //================================

    Vector2Int ComputeViewRegion()
    {
        Vector2 view_pos = Camera.main.transform.position;
        return MathRect.ComputeCoordIn(view_pos, mSpanRegion.ToInt());
    }

    Vector2Int mNumTile = new Vector2Int(MapRefer.cNumColumn, MapRefer.cNumRow);

    Vector2 mSpanRegion => mNumTile * mSpanScaleTile;

    Vector2 mSpanScaleTile => MapShowMager.mScaleTile * MapShowMager.mSpanTile;

    void GetAt(Vector2Int seek, out Vector2Int regionAt, out Vector2Int inRegionAt)
    {
        regionAt = new Vector2Int(seek.x / mNumTile.x, seek.y / mNumTile.y);
        inRegionAt = new Vector2Int(seek.x % mNumTile.x, seek.y % mNumTile.y);
    }

    Vector2Int GetAt(Vector2Int regionAt, Vector2Int inRegionAt)
    {
        var at = new Vector2Int();
        Vector2Int numSpan = mNumTile;
        at.x = regionAt.x * numSpan.x + inRegionAt.x;
        at.y = regionAt.y * numSpan.y + inRegionAt.y;
        return at;
    }

    //==================================

    public class Service : Srv<MapRegionsShow>,ISymbRefer
    {
        public Service(MapRegionsShow belong) : base(belong)
        {
        }

        public Vector3 GetPosAt(Vector2Int seek)
        {//这个坐标参数，是跨region的

            Vector2Int regionAt, inRegionAt;
            j.GetAt(seek, out regionAt, out inRegionAt);
            return j.mShows[regionAt].mLattice.SuGetPos(inRegionAt);
        }

        public IfoTilePile GetPileAt(Vector2Int seek)
        {
            Vector2Int regionAt, inRegionAt;
            j.GetAt(seek, out regionAt, out inRegionAt);
            if (regionAt.x >= 0 && regionAt.y >= 0)
            {
                if (j.mShows.ContainsKey(regionAt))
                    return j.mShows[regionAt].me.GetPile(inRegionAt);
            }
            return null;
        }

        public Vector2Int GetCoordIn(Vector3 pos)
        {
            if (pos.x >= 0 && pos.y >= 0)
            {
                int xTile = MathNum.FloorToIntAndZero(pos.x / j.mSpanScaleTile.x);
                int yTile = MathNum.FloorToIntAndZero(pos.y / j.mSpanScaleTile.y);
                return new Vector2Int(xTile, yTile);
            }
            else
                return - Vector2Int.one;
        }

        public void OnlyShowLayerAt(int layer)
        {
            foreach (MapRegionShow one in j.mShows.Values) one.me.OnlyShowLayerAt(layer);
        }

        public void ShowAllLayer()
        {
            foreach (MapRegionShow one in j.mShows.Values) one.me.ShowAllLayer();
        }

        //===============================

        public Action<Transform> INotifyNewSelectFmField { get => j.NuWhenNewSelectFmField; set => j.NuWhenNewSelectFmField = value; }

        public Dictionary<Vector2Int, GameObject> iPlaneUnits
        {
            get
            {
                Dictionary<Vector2Int, GameObject> list = new Dictionary<Vector2Int, GameObject>();
                foreach (Vector2Int coord in j.mShows.Keys)
                {
                    var units = j.mShows[coord].me.iPlaneUnits;
                    foreach (Vector2Int at in units.Keys)
                        list.Add(j.GetAt(coord, at), units[at]);
                }
                return list;
            }
        }

        public bool iIsCursorOn => j.mData.SuWhetherHaveHeld(j.mCurMouseIn);

        public Vector2Int iIsOn => j.mShows[j.mCurMouseIn].me.iIsOn;

        public bool iInShow => j.mShows.Count > 0;

    }

}
