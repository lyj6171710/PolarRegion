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
    
    public Vector2Int iIsOn { get; }//Ӧ��IsCursorOnΪ���ʹ��

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

        mGetIfoSiteSymbs = getIfoSiteSymbs;//��site���ͱ�־������ר�ŶԴ�

        IfoSymbsEachSite.NuWhenNumChanged += ReactSymbsChangedOnSite;
        IfoOneSiteSymb.NuWhenValueChanged += ReactSymbsChangedOnSite;
        //ע��������Ҫʹ��+=����Ϊ���������ͬʱ���ڶ���ģ������ί���Ǿ�̬����
        //������һ����ֵ��Ч���и��ϲ��߼��Ŀ���
    }

    public void MakeUse() 
    {
        mGrids = mMap.iPlaneUnits;//�����ǿ���ˢ�µģ���Ҫ�������ȡ
    }

    public void MakeNeed(IfoSymbsInField symbs)
    {
        if (symbs == null) return;

        ShowNeed showNeed = new ShowNeed();
        showNeed.field = symbs;//���������������޸ģ�����Ҫ��ֵ
        if (!mSymbKinds.ContainsKey(symbs.kind))
        {
            showNeed.divide = SymbRefer.GetDivideNeed(symbs.kind);
            mSymbKinds.Add(symbs.kind, showNeed);
        }
        else //�Ѿ������������ͱ�־�ļ�����Ϣ������ֻ��Ҫ�������µ�ǰ��Ϣ
        {
            showNeed.divide = mSymbKinds[symbs.kind].divide;
            mSymbKinds[symbs.kind] = showNeed;//��������
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

    void RebuildField(EKindSymb kind)//������ʾһ�����͵����б��
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

    void UpdateShowOnUnit(ShowFor showFor)//�������ֲ��ÿ�����ʾ���Ĵ������
    {
        if (!mSymbEntities.ContainsKey(showFor))//��ȷ��������ʾ
        {
            SymbolsShow show = mGrids[showFor.coord].AddComponent<SymbolsShow>();
            mSymbEntities.Add(showFor, show);//��־������
        }

        IfoSymbsEachUnit symbs = mSymbKinds[showFor.kind].field.scatter[showFor.coord];
        if (mSymbEntities[showFor].meHaveReady)//��־����ʾ
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

    //�Գ������ͱ�ǵ�����Դ�=======================================

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
            //��һ�㵽���⣬ͨ�������ݵ������������ݵĸı�
            scatter.Add(coord, unit);
        }

        IfoOneUnitSymb symbCase = scatter[coord].neDataSync.FollowAdd();
        symbCase.neDataSync.FollowChange(SymbolPreset.It.meSelectNow, MapPlates.It.meCurLayer);
        scatter[coord] = scatter[coord].neDataSync.MsgFmUser("sort");
        //�б��κ�Ԫ�صı䶯������ǣ���б��������û�Ϊһ���µģ�����һ����Ƭ�ϵı�ǲ����
        
        ShowFor showFor = new ShowFor(coord, EKindSymb.site);
        if (mSymbEntities.ContainsKey(showFor))
            mSymbEntities[showFor].UpdateShow(scatter[coord]);
        else
            UpdateShowOnUnit(showFor);
    }

    //-----------------------------------------------

    //���б���ʽ�鿴�������ǰ��ѡ����Ƭ�����еĳ��ر��

    [HideInEditorMode, ShowIf("@this.gameObject.activeSelf")]
    public IfoSymbsEachSite meSymbsOnSite;
    Vector2Int mSiteAt;
    SymbolsShow mSite;//��ǰѡ��ģ����Ծ��б�ǵģ���Ƭ
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
                //�����⵽һ�㣬���������ݵĸı䣬ǣ��һ�������ݵĸı�
                mSite.UpdateShow(field.scatter[mSiteAt]);
            }
        }
    }

    //��ϵͳ���ͱ�ǵ�����Դ�======================



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

public class IfoSymbsInField // ��Ǽ���Ϣ��һ���ʽ����綼����ʹ��
{
    [HideInInspector]
    public EKindSymb kind;//һ����ǣ������ͼ��д��

    public Dictionary<Vector2Int, IfoSymbsEachUnit> scatter;
    public IfoSymbsInField(ISyncMatchCollect<IfoSymbsEachUnit, Vector2Int> sync = null)
    {
        neDataSync = sync;
        scatter = new Dictionary<Vector2Int, IfoSymbsEachUnit>();
    }

    //================================

    public ISyncMatchCollect<IfoSymbsEachUnit, Vector2Int> neDataSync;
    //�������������ͬ�����Ͼ��������Ϣ���Ǽ̳й�ϵ����粻��ҪҲ��Ӧ�ü̳и���
}

[Serializable]
public class IfoSymbsEachUnit
{
    public IfoSymbsEachUnit(ISyncMatchClectMsgAL<IfoOneUnitSymb, int, IfoSymbsEachUnit> sync = null)
    { 
        neDataSync = sync;
        symbs = new List<IfoOneUnitSymb>();
    }

    public static IfoSymbsEachUnit ForPlaceHolder()//����Ĺ��췽ʽ
    {
        IfoSymbsEachUnit unit = new IfoSymbsEachUnit();//ֻ��ռλ
        IfoOneUnitSymb oneSymb = new IfoOneUnitSymb();//ֻ��ռλ
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