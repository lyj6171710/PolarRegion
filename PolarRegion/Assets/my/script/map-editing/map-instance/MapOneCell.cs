using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOneCell : MapTreeRoom, ISignUpRead
{
    //每个cell就有对应的一个这个组件，这一个组件又挂载于单独一个物体上
    //这是因为一个cell可以有它的若干个子地图，需要为每个cell腾出更多空间

    //该组件被动态生成

    public Service me;

    MapOneTerritory mBelong;

    IfoShelfFile mAttach;

    Vector2Int mCoord;

    public void ReadReady(MapOneTerritory belong, Vector2Int coord)
    {
        me = new Service(this);

        mBelong = belong;
        mCoord = coord;

        mAttach = new IfoShelfFile();
        mAttach.name = me.FileName;
        mAttach.super = mBelong.me.StrWorldAndTerri;
        mAttach.withFolder = true;
        mAttach.findIn = mBelong.me.StrWorldAndTerri;
        
        VirtualDisk.It.AddFileToVtDisk(mAttach);
        
        MakeLoad();

        mSymbolsPack = gameObject.AddComponent<MapSymbolsPack>();
        mSymbolsPack.MakeReady(mAttach);

        ReadReady(this, new List<Vector2Int>());
        InquirySubHave();
        mHaveReady = true;
    }

    //======================================

    void InquirySubHave()
    {
        IfoShelfFolder ifo = new IfoShelfFolder();
        ifo.findIn = mBelong.me.StrWorldAndTerri;
        ifo.name = me.FileName;
        string[] datas = VirtualDisk.It.GetAllShelfInVtDisk(ifo);
        List<string> dataCoords = StrAnalyse.FilterStrings(datas, MapOneRoom.Service.FilePrefix + RegexHelp.StrPtnFmVec2Int);
        //匹配坐标（存储时就是用坐标命名的），得到有哪些具体地图数据
        foreach (string coordStr in dataCoords)
        {
            int[] coordParts = StrAnalyse.PickNum(coordStr);
            Vector2Int coord = new Vector2Int(coordParts[0], coordParts[1]);
            FormSubHave(coord);
        }
    }

    protected override void FormSubHave(Vector2Int coord)
    {
        GameObject roomGbj = GameobjPool.It.GetPure(transform, MapOneRoom.Service.FileNameByCoord(coord));
        mRooms.Add(coord, roomGbj.AddComponent<MapOneRoom>());
    }

    public new MapOneRoom AccessRoom(List<Vector2Int> route, bool force = false)
    {
        return base.AccessRoom(route, force) as MapOneRoom;
    }

    //===============================

    StoCell mDataSto;//地图内容
    [HideInInspector] public IfoCell mDataIfo;

    void MakeLoad()
    {
        VirtualDisk.It.LoadFileInRealDisk(mAttach, out mDataSto);
        if (mDataSto != null) mDataIfo = mDataSto.ToIfo();
        else mDataIfo = IfoCell.PureIfo();
    }

    public void MakeSave(EMapIfo kind)
    {
        if (kind == EMapIfo.tile)
            //要确保已经建立起相应环境，这里的环境依赖大环境的存在
            VirtualDisk.It.SaveFileInRealDisk(mAttach, mDataIfo.ToSto());
        else if (kind == EMapIfo.symbol)
            mSymbolsPack.MakeSave();
    }

    //================================

    MapSymbolsPack mSymbolsPack;

    public IfoSymbsInField neIfoSymbs => mSymbolsPack.meSymbs;
    public IfoSymbsInLattice neIfoMapSymbs => mSymbolsPack.mSymbs;

    //==============================

    public class Service : Srv<MapOneCell>
    {
        public Service(MapOneCell belong) : base(belong)
        {
        }

        public string FileName => FilePrefix + j.mCoord.ToString();

        public const string FilePrefix = @"cell-";

        public MapOneTerritory.Service meTerri => j.mBelong.me;

    }
}

//======================================

[System.Serializable]
public class IfoRegion:Ifo,ICanSto<StoRegion>  //地图内容信息
{
    public IfoWall wall;

    public IfoRegion(int width, int height)
    {
        wall = new IfoWall(width, height);
    }

    public StoRegion ToSto()
    {
        StoRegion sto = new StoRegion();
        sto.wall = wall.ToSto();
        return sto;
    }
}

//======================================

[System.Serializable]
public class StoRegion:Sto,IForIfo<IfoRegion>//地图内容信息
{
    public StoWall wall;

    public StoRegion()
    {
        wall = new StoWall();
    }

    public IfoRegion ToIfo()
    {
        var ifo = new IfoRegion(wall.width, wall.meHeight);
        ifo.wall = wall.ToIfo();
        return ifo;
    }
}

//===================================
