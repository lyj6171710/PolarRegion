using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOneRoom : MapTreeRoom, ISignUpRead
{
    MapOneCell mBelong;

    IfoShelfFile mAttach;

    List<string> mBelongChain;//从territory开始,而route是从cell开始

    public Service me;

    protected override void ReadReady(MapTreeRoom root, List<Vector2Int> route)
    {
        base.ReadReady(root, route);
        me = new Service(this);

        mBelong = root as MapOneCell;
        mBelongChain = new List<string>();
        mBelongChain.Add(mBelong.me.FileName);
        for (int i = 0; i < mRoute.Count - 1; i++)//注意不要伸延到自己
            mBelongChain.Add(Service.FileNameByCoord(mRoute[i]));

        mAttach = new IfoShelfFile();
        mAttach.name = me.FileName;
        mAttach.super = me.FileIn;
        mAttach.withFolder = true;
        mAttach.findRoute = mBelongChain;
        mAttach.routeUntilSuper = true;
        mAttach.findIn = mBelong.me.meTerri.StrWorldAndTerri;

        VirtualDisk.It.AddFileToVtDisk(mAttach, true);

        MakeLoad();

        mSymbolsPack = gameObject.AddComponent<MapSymbolsPack>();
        mSymbolsPack.MakeReady(mAttach);

        InquirySubHave();
        mHaveReady = true;
    }

    public void MakeRemove()
    {
        foreach (MapOneRoom subRoom in mRooms.Values)
            subRoom.MakeRemove();
        VirtualDisk.It.DelFileInRealDisk(mAttach);
        BackFromRoom(mRoute.GetLast(), ERoomMsg.del);
        GameobjPool.It.Revert(gameObject);
        //这里不用把游戏物体destroy了，房间是用对象池创建的
    }

    //===============================

    void InquirySubHave()
    {
        IfoShelfFolder ifo = new IfoShelfFolder();
        ifo.findIn = mBelong.me.meTerri.StrWorldAndTerri;
        ifo.routeUntilSuper = true;
        ifo.findRoute = mBelongChain;
        ifo.name = mAttach.name;
        string[] datas = VirtualDisk.It.GetAllShelfInVtDisk(ifo);
        List<string> dataCoords = StrAnalyse.FilterStrings(datas, Service.FilePrefix + RegexHelp.StrPtnFmVec2Int);
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
        GameObject roomGbj = GameobjPool.It.GetPure(transform,Service.FileNameByCoord(coord));
        mRooms.Add(coord, roomGbj.AddComponent<MapOneRoom>());
    }

    //============================

    public IfoRoom mDataIfo;

    void MakeLoad()
    {
        StoRoom stoRoom;
        VirtualDisk.It.LoadFileInRealDisk(mAttach, out stoRoom);
        if (stoRoom != null) mDataIfo = stoRoom.ToIfo();
    }

    public void MakeSave(EMapIfo kind)
    {
        if (kind == EMapIfo.tile)
            VirtualDisk.It.SaveFileInRealDisk(mAttach, mDataIfo.ToSto());
        else if (kind == EMapIfo.symbol)
            mSymbolsPack.MakeSave();
    }

    //=========================

    MapSymbolsPack mSymbolsPack;

    public IfoSymbsInField neIfoSymbs => mSymbolsPack.meSymbs;
    public IfoSymbsInLattice neIfoMapSymbs => mSymbolsPack.mSymbs;

    //==========================

    public class Service : Srv<MapOneRoom>
    {
        public Service(MapOneRoom belong) : base(belong)
        { }

        public string FileName => FileNameByCoord(j.mRoute[j.mRoute.Count - 1]);

        public string FileIn
        {
            get
            {
                if (j.mRoute.Count > 1)
                    return FileNameByCoord(j.mRoute[j.mRoute.Count - 2]);
                else
                    return j.mBelong.me.FileName;
            }
        }

        public static string FileNameByCoord(Vector2Int coord) => FilePrefix + coord.ToString();

        public const string FilePrefix = @"room-";

    }
}

public class IfoRoom : Ifo,ICanSto<StoRoom>
{
    public List<Vector2Int> coordChain;//第一个元素是位于cell中的坐标位置，最后一个元素是它自己相对上级所处的坐标位置

    public IfoWall wall;

    public IfoRoom(List<Vector2Int> coordChain, int width, int height)
    {
        this.coordChain = coordChain.CopyValueToNew();
        wall = new IfoWall(width, height);
    }

    public StoRoom ToSto()
    {
        StoRoom sto = new StoRoom();
        sto.coordChain = coordChain.CopyValueToNew();
        sto.wall = wall.ToSto();
        return sto;
    }
}

//-----------------------------------

[System.Serializable]
public class StoRoom : Sto, IForIfo<IfoRoom>//地图内容信息
{
    //房间可以是细胞的子房间，也可以是子房间的子房间
    public List<Vector2Int> coordChain;//从位于细胞的坐标延申到该房间的路径

    public StoWall wall;

    public StoRoom()
    {
        wall = new StoWall();
        coordChain = new List<Vector2Int>();
    }

    public IfoRoom ToIfo()
    {
        var ifo = new IfoRoom(coordChain.CopyValueToNew(), wall.width, wall.meHeight);
        ifo.wall = wall.ToIfo();
        return ifo;
    }
}