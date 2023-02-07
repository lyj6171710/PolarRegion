using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MapTerritoryCells : MonoBehaviour, ISignUpRead
{
    public int SignRead => throw new System.NotImplementedException();

    //应该每一个cell就安装一个组件，不要集合所有cell在一个组件中，不过这个组件留着管理这些分散的组件

    MapOneTerritory mCellsBelong;

    Dictionary<Vector2Int, MapOneCell> mCells;

    public void MakeReady()
    {
        mCellsBelong = GetComponent<MapOneTerritory>();

        mCells = new Dictionary<Vector2Int, MapOneCell>();

        InquirySubHave();
    }

    void InquirySubHave()
    {
        IfoShelfFolder attach = new IfoShelfFolder();
        attach.name = mCellsBelong.me.StrWorldAndTerri;
        string[] datas = VirtualDisk.It.GetAllShelfInVtDisk(attach);
        List<string> dataCoords = StrAnalyse.FilterStrings(datas, MapOneCell.Service.FilePrefix + RegexHelp.StrPtnFmVec2Int);
        //匹配坐标（存储时就是用坐标命名的），得到有哪些具体地图数据
        foreach (string coordStr in dataCoords)
        {
            int[] coordParts = StrAnalyse.PickNum(coordStr);
            Vector2Int coord = new Vector2Int(coordParts[0], coordParts[1]);
            FormSubHave(coord);
        }
    }

    MapOneCell FormSubHave(Vector2Int coord)
    {
        GameObject cellGbj = GameobjPool.It.GetPure(transform, "cell-" + coord.ToString());
        MapOneCell cell = cellGbj.AddComponent<MapOneCell>();
        mCells.Add(coord, cell);
        return cell;
    }

    public MapOneCell AccessCell(Vector2Int coord, bool force = false)
    {
        if (mCells.ContainsKey(coord))
        {
            if (!mCells[coord].mHaveReady)
                mCells[coord].ReadReady(mCellsBelong, coord);
            return mCells[coord];
        }
        else
        {
            if (force)
            {
                FormSubHave(coord);
                return AccessCell(coord);
            }
            else
            {
                Debug.LogError("访问到未知区域");
                return null;
            }
        }
    }

}


[System.Serializable]
public class IfoCell : MapDataHandle,ICanSto<StoCell>,Ifo
{
    [SerializeField] List<IfoRegionUnit> regions;//某行某列是否被占据，属于三维数据

    public IfoCell() : base(MapRefer.cRegionNumMax)
    {
        regions = new List<IfoRegionUnit>();
    }

    protected override int ReConstruct()
    {
        regions = new List<IfoRegionUnit>();
        return MapRefer.cRegionNumMax;
    }

    //----------------------------------

    public IfoRegion GetRegionContent(Vector2Int at)
    {
        return regions[SuGetIndex(at)].content;
    }

    public void SetRegionContent(Vector2Int coord, IfoRegion content)
    {
        IfoRegionUnit regionUnit = regions[SuGetIndex(coord)];
        regionUnit.content = content;
    }

    //---------------------------------------

    protected override Vector2Int GetCoordStraight(int index)
    {
        return regions[index].coord;
    }

    //---------------------------------------
    protected override void AddOne(Vector2Int at)
    {
        IfoRegionUnit cell = new IfoRegionUnit(at);
        regions.Add(cell);
    }

    protected override void DelOne(Vector2Int at)
    {
        regions.RemoveAt(SuGetIndex(at));
    }

    protected override void GetCurHoldToMap(System.Action<Vector2Int, int> deal)
    {
        for (int i = 0; i < regions.Count; i++)
        {
            deal(regions[i].coord, i);
        }
    }

    //---------------------------------------

    public static IfoCell PureIfo()
    {
        IfoCell ifo = new IfoCell();
        ifo.SuInitialMap();
        return ifo;
    }

    public StoCell ToSto()
    {
        StoCell sto = new StoCell();

        for (int i = 0; i < meSum; i++)
        {
            StoCell.StoRegionUnit unit = new StoCell.StoRegionUnit();
            unit.coord = SuGetCoord(i);
            unit.content = GetRegionContent(unit.coord).ToSto();
            sto.regions.Add(unit);
        }

        return sto;
    }

    [System.Serializable]
    class IfoRegionUnit
    {
        //用来支持动态的地图内容大小，而且能一样查询快速

        public Vector2Int coord;
        public IfoRegion content;//地图内容的承载者
                                 //内容情况的决定权在外界

        public IfoRegionUnit(Vector2Int coord)
        {
            this.coord = coord;
            content = new IfoRegion(1, 1);//外界需要自己创建数据，这里只是应付数据成员的必要需求
        }

    }
}

[System.Serializable]
public class StoCell:Sto,IForIfo<IfoCell>
{
    public List<StoRegionUnit> regions;//某行某列是否被占据，属于三维数据

    public StoCell() { regions = new List<StoRegionUnit>(); }

    public IfoCell ToIfo()
    {
        IfoCell ifo = IfoCell.PureIfo();

        for (int i = 0; i < regions.Count; i++)
        {
            Vector2Int coord = regions[i].coord;
            ifo.SuAddOne(coord);
            ifo.SetRegionContent(coord, regions[i].content.ToIfo());
        }

        return ifo;
    }

    [System.Serializable]
    public class StoRegionUnit
    {
        public Vector2Int coord;
        public StoRegion content;//地图内容的承载者
    }
}