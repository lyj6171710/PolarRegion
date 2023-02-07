using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class MapSymbolsPack : MonoBehaviour
{
    //���ű������ͼ����¼�֮��

    //�������������ã�
    //һ��ֱ�۵���ʾ��༭��ͼ��Ϣ
    //�����ṩ��ͼͼ���ڷ�ͼ�������Ϣ������
    //�����¼������

    //����һ��ͳһ�ķ����б�
    //�����б�������ָ�������Ϣ��һ�ַ��϶�Ӧһ����Ϣ����
    //��ͼ��Ԫ��ʹ�÷�����������Լ�ĳ������

    //==================================

    public IfoSymbsInField meSymbs
    {
        get
        {
            if (mSymbs == null) return null;
            IfoSymbsInField fieldSymbs = mSymbs.GetIfo();
            fieldSymbs.kind = EKindSymb.site;
            return fieldSymbs;
        }
    }

    [HideInInspector]
    public IfoSymbsInLattice mSymbs;

    IfoShelfFile mAttach;

    public void MakeReady(IfoShelfFile attachRefer)
    {
        BuildCatalogue(attachRefer);

        mSymbs = new IfoSymbsInLattice();

        LoadSymbsFromPack();
    }

    public void BuildCatalogue(IfoShelfFile attachRefer)//�洢����
    {
        mAttach = attachRefer.GetCopy();//����Ҫ���ŵ�����洢��ͬ���ط�
        if (mAttach.withFolder)
        {
            mAttach.withFolder = false;
            if (mAttach.routeUntilSuper)
                mAttach.findRoute.SelfAddLast(mAttach.super);
            mAttach.super = mAttach.name;
        }
        mAttach.name = "sybms-pack";
        VirtualDisk.It.AddFileToVtDisk(mAttach);
    }

    void LoadSymbsFromPack()
    {
        StoSymbsInLattice stoSymbsInLattice;
        VirtualDisk.It.LoadFileInRealDisk(mAttach, out stoSymbsInLattice);
        if (stoSymbsInLattice == null)
            mSymbs = null;//��Щ��ͼ��û�д洢����ǵĻ�����û������
        else
            mSymbs = stoSymbsInLattice.ToIfo();
    }

    public void MakeSave()
    {
        VirtualDisk.It.SaveFileInRealDisk(mAttach, mSymbs.ToSto());
    }
}

//================================================

public class IfoSymbsInLattice:Ifo, ISyncMatchCollect<IfoSymbsEachUnit,Vector2Int>,
    ICanSto<StoSymbsInLattice>, IGetIfo<IfoSymbsInField>
{
    public Dictionary<Vector2Int, IfoSymbsEachSite> scatter;
    public IfoSymbsInLattice() { scatter = new Dictionary<Vector2Int, IfoSymbsEachSite>(); }

    public IfoSymbsInField GetIfo()
    {
        IfoSymbsInField symbsInField = new IfoSymbsInField(this);
        foreach (Vector2Int coord in scatter.Keys)
            symbsInField.scatter.Add(coord, scatter[coord].GetIfo());
        return symbsInField;
    }

    public StoSymbsInLattice ToSto()
    {
        StoSymbsInLattice stoSymbsInLattice = new StoSymbsInLattice();
        foreach (Vector2Int coord in scatter.Keys)
            stoSymbsInLattice.scatter.Add(coord, scatter[coord].ToSto());
        return stoSymbsInLattice;
    }

    public IfoSymbsEachUnit FollowAdd(Vector2Int coord)
    {
        var set = new IfoSymbsEachSite();
        scatter.Add(coord, set);
        return scatter[coord].GetIfo();
    }

    public void FollowDel(Vector2Int coord)
    {
        scatter.Remove(coord);
    }
}

[Serializable]
public class IfoSymbsEachSite:Ifo, ISyncMatchClectMsgAL<IfoOneUnitSymb, int, IfoSymbsEachUnit>,
    ICanSto<StoSymbsEachSite>, IGetIfo<IfoSymbsEachUnit>
{
    [OnCollectionChanged("OnNumChanged")]
    public List<IfoOneSiteSymb> symbs;

    public IfoSymbsEachSite() { symbs = new List<IfoOneSiteSymb>();}

    public IfoSymbsEachUnit GetIfo()
    {
        IfoSymbsEachUnit symbsEachUnit = new IfoSymbsEachUnit(this);
        for (int i = 0; i < symbs.Count; i++)
            symbsEachUnit.symbs.Add(symbs[i].GetIfo());
        return symbsEachUnit;
    }

    public StoSymbsEachSite ToSto()
    {
        StoSymbsEachSite stoSymbsEachUnit = new StoSymbsEachSite();
        for (int i = 0; i < symbs.Count; i++)
            stoSymbsEachUnit.symbs.Add(symbs[i].ToSto());
        return stoSymbsEachUnit;
    }

    public IfoOneUnitSymb FollowAdd()
    {
        var set = new IfoOneSiteSymb();
        symbs.Add(set);
        return set.GetIfo();
    }

    public void FollowDel(int index)
    {
        if (index >= 0)
            symbs.RemoveAt(index);
        else
            symbs.Clear();
    }

    public IfoSymbsEachUnit MsgFmUser(params object[] args)
    {
        if ((string)args[0] == "sort")
        {
            symbs.Sort((a, b) =>
            {//����ʾ���ȼ�����

                if (SymbolPreset.It.GetPrior(a.sign) > SymbolPreset.It.GetPrior(b.sign))
                    return 1;
                else
                    return -1;
            });
        }
        return GetIfo();
    }

    //=====================================

    public static Action NuWhenNumChanged;
    void OnNumChanged()
    {
        NuWhenNumChanged();
    }
}

[Serializable]
public class IfoOneSiteSymb: Ifo, ISyncMatchChange,
    ICanSto<StoOneSiteSymb>, IGetIfo<IfoOneUnitSymb>
{
    [OnValueChanged("OnValueChanged")] 
    public string sign;
    public int layer;//layer���иߵͲ�Ρ�������site

    public void FollowChange(params object[] smch)
    {
        sign = (string)smch[0];
        layer = (int)smch[1];
    }

    public IfoOneUnitSymb GetIfo()
    {
        IfoOneUnitSymb unitSymbCase = new IfoOneUnitSymb(this);
        unitSymbCase.sign = sign;
        return unitSymbCase;
    }

    public StoOneSiteSymb ToSto()
    {
        StoOneSiteSymb stoSymbCase = new StoOneSiteSymb();
        stoSymbCase.layer = layer;
        stoSymbCase.sign = sign;
        return stoSymbCase;
    }

    //==================================

    public static Action NuWhenValueChanged;

    void OnValueChanged()
    {
        NuWhenValueChanged();
    }

}

//================================================

[Serializable]
public class StoSymbsInLattice : Sto, IForIfo<IfoSymbsInLattice>
{
    public JsonDictionary<Vector2Int, StoSymbsEachSite> scatter;
    public StoSymbsInLattice() { scatter = new JsonDictionary<Vector2Int, StoSymbsEachSite>(); }

    public IfoSymbsInLattice ToIfo()
    {
        IfoSymbsInLattice ifoSymbsInLattice = new IfoSymbsInLattice();
        foreach (Vector2Int coord in scatter.Keys)
            ifoSymbsInLattice.scatter.Add(coord, scatter[coord].ToIfo());
        return ifoSymbsInLattice;
    }
}

[Serializable]
public class StoSymbsEachSite : Sto, IForIfo<IfoSymbsEachSite>
{
    public List<StoOneSiteSymb> symbs;
    public StoSymbsEachSite() { symbs = new List<StoOneSiteSymb>(); }

    public IfoSymbsEachSite ToIfo()
    {
        IfoSymbsEachSite ifoSymbsEachUnit = new IfoSymbsEachSite();
        for (int i = 0; i < symbs.Count; i++)
            ifoSymbsEachUnit.symbs.Add(symbs[i].ToIfo());
        return ifoSymbsEachUnit;
    }
}

[Serializable]
public class StoOneSiteSymb : Sto, IForIfo<IfoOneSiteSymb>
{
    public int layer;
    public string sign;

    public IfoOneSiteSymb ToIfo()
    {
        IfoOneSiteSymb ifoSymbCase = new IfoOneSiteSymb();
        ifoSymbCase.layer = layer;
        ifoSymbCase.sign = sign;
        return ifoSymbCase;
    }
}