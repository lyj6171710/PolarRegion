using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PRAbilityMove : MonoBehaviour, IMatrixMapPosIfo
{
    //机制目标：可以实现行走在瓦片地图上时，主要的、大多数情况的效果需求
    //人物图层一定在单元格可具有各图层高度的间隔中，不会相等，每一个图层可以拥有一个瓦片
    //人物逻辑层级控制行动能力，图层控制显示关系

    public MapRegionsShow mMapDp;

    [ShowInInspector]
    public int meHighAt => mLayerHeightCurAt;

    [Button]
    public void Land()
    {
        mLayerHeightCurAt = 1;//人物最低层级，至少应能站在地面上
        mViaLast.highLayerShould = mLayerHeightCurAt;
        mNowAt = Vector2Int.zero;
        mShift.StartMove(Vector2Int.zero);
    }

    //=========================================

    Dictionary<IfoRecord, IfoWillToBe> mRecords;//保留临时计算结果
    
    public bool SuCanWalkTo(Vector2Int from, EToward4 to)
    {
        if (to == EToward4.middle)
            return false;
        else
        {
            IfoRecord record = new IfoRecord(from, to, mLayerHeightCurAt);
            //注意这里总是基于人物当前高度层级来计算的前路，
            //面前的路绝对有效，但未来可走的路就可能出问题，外界自己负责
            if (mRecords.ContainsKey(record))
                return mRecords[record].canPass;
            else
            {
                IfoWillToBe will = new IfoWillToBe();//备份计算结果
                mRecords.Add(record, will);
                will.highLayerShould = mLayerHeightCurAt;//一般情况都是保持
                will.pile = mMapDp.me.GetPileAt(from + OverTool.TowardToVector(to).ToInt());
                if (will.pile == null)
                {
                    will.canPass = false;
                    return false;
                }

                //从人物自己的视角出发，看待前方能否走动过去
                //先看能否上楼梯(人物逻辑高度的上升)
                IfoTile tile;//准备用于承载瓦片信息
                IfoTilePile pileCurOn = mMapDp.me.GetPileAt(from);
                if (pileCurOn.SuHaveTile(mLayerHeightCurAt))
                {
                    tile = GetIfoTile(pileCurOn, mLayerHeightCurAt);
                    if (CanStairUp(tile, to))
                    {
                        tile = GetIfoTile(will.pile, mLayerHeightCurAt + 1);
                        will.showLayerShould = IfGoThrough(tile, mLayerHeightCurAt + 1);
                        if (will.showLayerShould > 0)
                        {
                            will.highLayerShould = mLayerHeightCurAt + 1;
                            will.canPass = true;
                            return true;
                        }
                    }
                }
                //没有向上楼梯可走时
                GoPassOn(ref will);
                return will.canPass;
            }
        }

        void GoPassOn(ref IfoWillToBe ahead)//决定是否能够维持人物海拔平直或稍降地走动过去
        {
            if (ahead.pile.SuHaveTile(mLayerHeightCurAt))
            {//查找等于人物当前高度逻辑层级的地图瓦片(瓦片也有逻辑层级，图层是最后结果)
                IfoTile tile = GetIfoTile(ahead.pile, mLayerHeightCurAt);
                ahead.showLayerShould = IfGoThrough(tile, mLayerHeightCurAt);
                if (ahead.showLayerShould > 0) ahead.canPass = true;
                else ahead.canPass = false;
            }
            else if (mLayerHeightCurAt >= 1 && ahead.pile.SuHaveTile(mLayerHeightCurAt - 1))
            {
                IfoTile tile = GetIfoTile(ahead.pile, mLayerHeightCurAt - 1);

                //查看有向下楼梯吗
                if (mLayerHeightCurAt > 1 && CanStairDown(tile, to))
                {
                    ahead.showLayerShould = IfGoThrough(tile, mLayerHeightCurAt - 1);
                    if (ahead.showLayerShould > 0)
                    {
                        ahead.highLayerShould = mLayerHeightCurAt - 1;
                        ahead.canPass = true;
                        return;
                    }
                }
                //此时没有向下楼梯可走

                if (tile.isWall)//如果是墙体，则不能在上面走
                    ahead.canPass = false;
                else
                {//可以在低一级层面的瓦片上，保持自身高度地行走
                    ahead.showLayerShould = (mLayerHeightCurAt - 1) * 2 + 1;
                    ahead.canPass = true;
                }
            }
            else//不能从高处跃到低处(相差一个层级以上)
                ahead.canPass = false;
        }

        bool CanStairDown(IfoTile toTile, EToward4 dirFrom)
        {
            string dir;
            if (toTile.attrs.Get("stairDown", out dir))
            {
                if (dir == dirFrom.ToString())
                {
                    return true;
                }
            }
            return false;
        }

        bool CanStairUp(IfoTile onTile, EToward4 dirTo)
        {
            string dir;
            if (onTile.attrs.Get("stairUp", out dir))
            {
                if (dir == dirTo.ToString())
                {
                    return true;
                }
            }
            return false;
        }

        int IfGoThrough(IfoTile tile, int tileLayer)
        {
            if (tile.viaAbove)
            {//如果这个瓦片本身允许人物从它上面穿过，
                //那么能穿过单元格，人物当前图层高于该瓦片图层一级
                return tileLayer * 2 + 1;
            }
            else if (tile.viaBelow)
            {//如果这个瓦片本身允许人物从它下面穿过，
                //那么能穿过单元格，人物图层状态低于该瓦片图层一级
                return tileLayer * 2 - 1;
            }
            else//这一层级瓦片不允许穿过，那就不能穿行
            {
                return 0;
            }
        }

        IfoTile GetIfoTile(IfoTilePile pile, int highAt)
        {
            IfoTilePick pick = pile.GetPick(highAt);
            return MapPlates.It.SuGetTileAttrSafe(pick);
        }
    }

    public Vector3 SuGetPosAt(Vector2Int at)
    {
        return mMapDp.me.GetPosAt(at);
    }

    public bool SuIfNearGridCenter(Vector2 pos, out Vector2 toGridCenter, out Vector2Int nearTo)
    {
        nearTo = mMapDp.me.GetCoordIn(pos);
        Vector2 posCenter = mMapDp.me.GetPosAt(nearTo);
        toGridCenter = pos - posCenter;
        float gap = toGridCenter.magnitude;
        if (gap < 0.2f)//这里要注意，如果人物移动过快，可能就捕捉不到接近的时候
            return true;
        else
            return false;
    }

    public bool SuNowCanWalkTo(EToward4 to)
    {
        IfoRecord record = new IfoRecord(mNowAt, to, mLayerHeightCurAt);
        if (mRecords.ContainsKey(record))
            return mRecords[record].canPass;
        else
            return SuCanWalkTo(mNowAt, to);
    }

    //========================================

    ActorShiftOnMatrixCanPlan mShift;
    SpriteRenderer mBody;

    int mLayerHeightCurAt;//当前层级，0为起点，最高层取决于地图层次高度，地图最低层也应为0，对应地面
    IfoWillToBe mViaLast;
    Vector2Int mNowAt;

    void FitShowLayerTo(EToward4 to)
    {
        IfoRecord record = new IfoRecord(mNowAt, to, mLayerHeightCurAt);
        //凡是能前行的，一定已经经过了计算并还在字典中，所以不需要判断字典是否包含，有错那就是逻辑有问题
        IfoWillToBe mViaNowTo = mRecords[record];
        if (mViaNowTo.showLayerShould > mViaLast.showLayerShould)
        {//人物图层需要降低时，在到达下一个地点前，暂时保留当前图层高度，因为也需要适应当前地点的层级 
            mBody.sortingOrder = mViaNowTo.showLayerShould;
        }
        mViaLast = mViaNowTo;
    }

    void FitShowLayerArrive(Vector2Int where)
    {
        mBody.sortingOrder = mViaLast.showLayerShould;
        mLayerHeightCurAt = mViaLast.highLayerShould;
        mNowAt = where;
    }

    void Awake()
    {
        mRecords = new Dictionary<IfoRecord, IfoWillToBe>();
        mShift = gameObject.AddComponent<ActorShiftOnMatrixCanPlan>();
        mShift.MakeReady(this);
        mShift.SuWhenMoveToNext += FitShowLayerTo;
        mShift.SuWhenMoveReachOneWill += FitShowLayerArrive;
        mShift.SuWhenMoveReachOne += () =>{
            if (mShift.meIsFinishPlan)
                mRecords.Clear(); //大多数数据，很难再用到，不如重新算
        };
        mBody = gameObject.GetComponent<SpriteRenderer>();
        mViaLast = new IfoWillToBe();
    }

    void Update()
    {
        mShift.SuMoveBy(UnifiedInput.It.meGoOneStep());
    }

    struct IfoRecord//用以节省性能
    {
        public Vector2Int from;
        public EToward4 to;
        public int heightRefer;//计算所参考人物高度也一样时

        public IfoRecord(Vector2Int from, EToward4 to,int height)
        { this.from = from;this.to = to;heightRefer = height; }
    }

    class IfoWillToBe
    {
        public bool canPass;
        public IfoTilePile pile;
        public int showLayerShould;
        public int highLayerShould;
    }

}
