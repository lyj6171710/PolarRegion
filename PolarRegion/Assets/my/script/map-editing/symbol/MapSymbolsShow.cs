using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Sirenix.OdinInspector.Editor;

public interface ISymbRefer
{
    public Action<Transform> INotifyNewSelectFmField { get; set; }

    public Dictionary<Vector2Int, GameObject> iPlaneUnits { get; }

    public bool iIsCursorOn { get; }
    
    public Vector2Int iIsOn { get; }//应在IsCursorOn为真后使用

    public bool iInShow { get; }
}

public class MapSymbolsShow : MonoBehaviour
{
    ISymbRefer mMap;
    Dictionary<Vector2Int, GameObject> mGrids;
    Dictionary<ShowFor, SymbolsShow> mSymbEntities;
    Dictionary<EKindSymb, ShowNeed> mSymbKinds;

    public void MakeReady(ISymbRefer map, Func<Vector2Int, IfoSymbsEachSite> getIfoSiteSymbs) 
    {
        mMap = map;
        mMap.INotifyNewSelectFmField += ReactSelectOneSite;

        mSymbEntities = new Dictionary<ShowFor, SymbolsShow>();
        mSymbKinds = new Dictionary<EKindSymb, ShowNeed>();

        mGetIfoSiteSymbs = getIfoSiteSymbs;//对site类型标志，会有专门对待

        IfoSymbsEachSite.NuWhenNumChanged += ReactSymbsChangedOnSite;
        IfoOneSiteSymb.NuWhenValueChanged += ReactSymbsChangedOnSite;
        //注意这里需要使用+=，因为该组件可以同时存在多个的，而左侧委托是静态变量
        //具体哪一个赋值生效，有更上层逻辑的控制
    }

    public void MakeUse() 
    {
        mGrids = mMap.iPlaneUnits;//格子是可以刷新的，需要在这里获取
    }

    public void MakeNeed(IfoSymbsInField symbs)
    {
        if (symbs == null) return;

        ShowNeed showNeed = new ShowNeed();
        showNeed.field = symbs;//不管是新增还是修改，都需要赋值
        if (!mSymbKinds.ContainsKey(symbs.kind))
        {
            showNeed.divide = SymbRefer.GetDivideNeed(symbs.kind);
            mSymbKinds.Add(symbs.kind, showNeed);
        }
        else //已经存在这种类型标志的集合信息，所以只需要更换更新当前信息
        {
            showNeed.divide = mSymbKinds[symbs.kind].divide;
            mSymbKinds[symbs.kind] = showNeed;//更新数据
        }

        RebuildField(symbs.kind);
    }

    public void MakeDown() 
    {
        foreach (ShowFor showFor in mSymbEntities.Keys)
            mSymbEntities[showFor].MakeRemove();
        mSymbEntities.Clear();
    }

    //=======================================

    void RebuildField(EKindSymb kind)//重新显示一种类型的所有标记
    {
        UnloadSymbsShowOnField(kind);

        IfoSymbsInField ifoField = mSymbKinds[kind].field;
        
        var abandons = new List<Vector2Int>();
        foreach (Vector2Int coord in ifoField.scatter.Keys)
        {
            if (mGrids.ContainsKey(coord))
            {
                ShowFor showFor = new ShowFor(coord, ifoField.kind);
                UpdateShowOnUnit(showFor);
            }
            else
                abandons.Add(coord);
        }
        foreach (Vector2Int coord in abandons)
        {
            ifoField.scatter.Remove(coord);
            if (ifoField.neDataSync != null) ifoField.neDataSync.FollowDel(coord);
        }
    }

    void UnloadSymbsShowOnField(EKindSymb kind)
    {
        mSymbEntities.ForEachCanModify((showFor) =>
        {
            if (showFor.kind == kind)
            {
                mSymbEntities[showFor].MakeRemove();
                mSymbEntities.Remove(showFor);
            }
        });
    }

    void UpdateShowOnUnit(ShowFor showFor)//其它部分不用考虑显示器的存在与否
    {
        if (!mSymbEntities.ContainsKey(showFor))//先确保可以显示
        {
            SymbolsShow show = mGrids[showFor.coord].AddComponent<SymbolsShow>();
            mSymbEntities.Add(showFor, show);//标志的载体
        }

        IfoSymbsEachUnit symbs = mSymbKinds[showFor.kind].field.scatter[showFor.coord];
        if (mSymbEntities[showFor].meHaveReady)//标志的显示
        {
            mSymbEntities[showFor].MakeDown();
            mSymbEntities[showFor].MakeUse(symbs);
        }
        else
        {
            mSymbEntities[showFor].MakeReady(showFor.kind, SymbRefer.GetDivideNeed(showFor.kind));
            mSymbEntities[showFor].MakeUse(symbs);
        }
    }

    //对场地类型标记的特殊对待=======================================

    private void Update()
    {
        if (mMap.iIsCursorOn) 
        {
            if (KeyboardInput.SuIsInPress(MapRefer.cKeySymb))
            {
                if (UnifiedInput.It.meTapConfirm())
                {
                    AddSiteSymbToMap(mMap.iIsOn);
                }
                else if (UnifiedInput.It.meWhenBack())
                {
                    DelSiteSymbFromMap(mMap.iIsOn, UnifiedInput.It.meNumInput - 1);
                }
            }
        }
    }

    void DelSiteSymbFromMap(Vector2Int coord, int index)
    {
        ShowFor showFor = new ShowFor(coord, EKindSymb.site);
        if (mSymbEntities.ContainsKey(showFor))
        {
            IfoSymbsEachUnit unit = mSymbEntities[showFor].mSymbsList;
            if (index >= 0 && index < unit.symbs.Count)
            {
                unit.symbs.RemoveAt(index);
                unit.neDataSync.FollowDel(index);
                mSymbEntities[showFor].UpdateShow(unit);
            }
        }
    }

    void AddSiteSymbToMap(Vector2Int coord)
    {
        IfoSymbsInField field = mSymbKinds[EKindSymb.site].field;
        Dictionary<Vector2Int, IfoSymbsEachUnit> scatter = field.scatter;

        if (!scatter.ContainsKey(coord))
        {
            IfoSymbsEachUnit unit = field.neDataSync.FollowAdd(coord);
            //从一般到特殊，通用型数据导致特殊型数据的改变
            scatter.Add(coord, unit);
        }

        IfoOneUnitSymb symbCase = scatter[coord].neDataSync.FollowAdd();
        symbCase.neDataSync.FollowChange(SymbolPreset.It.meSelectNow, MapPlates.It.meCurLayer);
        scatter[coord] = scatter[coord].neDataSync.MsgFmUser("sort");
        //列表任何元素的变动，都会牵连列表整个被置换为一个新的，反正一个瓦片上的标记不会多
        
        ShowFor showFor = new ShowFor(coord, EKindSymb.site);
        if (mSymbEntities.ContainsKey(showFor))
            mSymbEntities[showFor].UpdateShow(scatter[coord]);
        else
            UpdateShowOnUnit(showFor);
    }

    //-----------------------------------------------

    //以列表形式查看与调整当前所选择瓦片所具有的场地标记

    [HideInEditorMode, ShowIf("@this.gameObject.activeSelf")]
    public IfoSymbsEachSite meSymbsOnSite;
    Vector2Int mSiteAt;
    SymbolsShow mSite;//当前选择的，可以具有标记的，瓦片
    Func<Vector2Int,IfoSymbsEachSite> mGetIfoSiteSymbs;

    void ReactSelectOneSite(Transform target)
    {
        var shows = target.GetComponents<SymbolsShow>();
        mSite = null;
        for (int i = 0; i < shows.Length; i++)
        {
            if (shows[i].meKind == EKindSymb.site)
            {
                mSite = shows[i];
                break;
            }
        }

        mSiteAt = target.GetComponent<IGridIfo>().iCoordInField;

        if (mSite != null)
            meSymbsOnSite = mGetIfoSiteSymbs(mSiteAt);
        else
            meSymbsOnSite = null;
    }

    void ReactSymbsChangedOnSite()
    {
        if (mMap.iInShow)
        {
            if (mSite != null)
            {
                IfoSymbsInField field = mSymbKinds[EKindSymb.site].field;
                field.scatter[mSiteAt] = mGetIfoSiteSymbs(mSiteAt).MsgFmUser("sort"); ;
                //从特殊到一般，特殊型数据的改变，牵动一般型数据的改变
                mSite.UpdateShow(field.scatter[mSiteAt]);
            }
        }
    }

    //对系统类型标记的特殊对待======================



    //=========================================

    struct ShowFor
    {
        public Vector2Int coord;
        public EKindSymb kind;

        public ShowFor(Vector2Int coord, EKindSymb kind) { this.coord = coord; this.kind = kind; }
    }

    struct ShowNeed
    {
        public int divide;
        public IfoSymbsInField field;
    }
}

public class SymbRefer
{
    public static int GetDivideNeed(EKindSymb kind)
    {
        switch (kind)
        {
            case EKindSymb.site: return 3;
            case EKindSymb.room: return 1;
            default: return 0;
        }
    }

    public static int GetLayerNeed(EKindSymb kind)
    {
        switch (kind)
        {
            case EKindSymb.site: return 3;
            case EKindSymb.room: return 1;
            default: return 0;
        }
    }
}

//=========================================

public class IfoSymbsInField // 标记集信息的一般格式，外界都可以使用
{
    [HideInInspector]
    public EKindSymb kind;//一批标记，按类型集中存放

    public Dictionary<Vector2Int, IfoSymbsEachUnit> scatter;
    public IfoSymbsInField(ISyncMatchCollect<IfoSymbsEachUnit, Vector2Int> sync = null)
    {
        neDataSync = sync;
        scatter = new Dictionary<Vector2Int, IfoSymbsEachUnit>();
    }

    //================================

    public ISyncMatchCollect<IfoSymbsEachUnit, Vector2Int> neDataSync;
    //用于与外界数据同步，毕竟与外界信息不是继承关系，外界不需要也不应该继承该类
}

[Serializable]
public class IfoSymbsEachUnit
{
    public IfoSymbsEachUnit(ISyncMatchClectMsgAL<IfoOneUnitSymb, int, IfoSymbsEachUnit> sync = null)
    { 
        neDataSync = sync;
        symbs = new List<IfoOneUnitSymb>();
    }

    public static IfoSymbsEachUnit ForPlaceHolder()//特殊的构造方式
    {
        IfoSymbsEachUnit unit = new IfoSymbsEachUnit();//只需占位
        IfoOneUnitSymb oneSymb = new IfoOneUnitSymb();//只需占位
        unit.symbs.Add(oneSymb);
        return unit;
    }

    //=================================

    public List<IfoOneUnitSymb> symbs;

    //=================================

    public ISyncMatchClectMsgAL<IfoOneUnitSymb, int, IfoSymbsEachUnit> neDataSync;

}

[Serializable]
public class IfoOneUnitSymb
{
    public IfoOneUnitSymb(ISyncMatchChange sync = null) { neDataSync = sync; }

    //================================

    public string sign;

    //=============================

    public ISyncMatchChange neDataSync;

}