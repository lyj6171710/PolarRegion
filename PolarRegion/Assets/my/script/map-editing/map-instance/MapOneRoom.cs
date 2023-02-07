using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOneRoom : MapTreeRoom, ISignUpRead
{
    MapOneCell mBelong;

    IfoShelfFile mAttach;

    List<string> mBelongChain;//��territory��ʼ,��route�Ǵ�cell��ʼ

    public Service me;

    protected override void ReadReady(MapTreeRoom root, List<Vector2Int> route)
    {
        base.ReadReady(root, route);
        me = new Service(this);

        mBelong = root as MapOneCell;
        mBelongChain = new List<string>();
        mBelongChain.Add(mBelong.me.FileName);
        for (int i = 0; i < mRoute.Count - 1; i++)//ע�ⲻҪ���ӵ��Լ�
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
        //���ﲻ�ð���Ϸ����destroy�ˣ��������ö���ش�����
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
        //ƥ�����꣨�洢ʱ���������������ģ����õ�����Щ�����ͼ����
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
    public List<Vector2Int> coordChain;//��һ��Ԫ����λ��cell�е�����λ�ã����һ��Ԫ�������Լ�����ϼ�����������λ��

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
public class StoRoom : Sto, IForIfo<IfoRoom>//��ͼ������Ϣ
{
    //���������ϸ�����ӷ��䣬Ҳ�������ӷ�����ӷ���
    public List<Vector2Int> coordChain;//��λ��ϸ�����������굽�÷����·��

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