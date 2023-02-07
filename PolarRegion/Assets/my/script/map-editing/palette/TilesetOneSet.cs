using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TilesetOneSet : MonoBehaviour
{   //�����زı��������
    //ÿһ��tilesetʵ����Ӧ�óа�һ�����׵���Ƭ�飬��ר�����ڻ���ĳ�ֽ������������������༭�������ɿ���

    //===================================

    [HideInPlayMode]
    public int mRowsDp;//�����Ӱ��ͬʱռ�ݶ����Ԫ�����Ƭ�ı�ʾ
    [HideInPlayMode]
    public int mColsDp;

    [PropertySpace(20)]
    [HideInPlayMode]
    public List<Sprite> mTilesReferDp;//�������и�ֵ��

    //-------------------------------------

    [Button]
    [PropertySpace(20)]
    [HideInEditorMode]
    void ClearTileset()
    {
        if (mTiles.Count > 0)
            mTiles.Clear();
    }

    [Button]
    [HideInEditorMode]
    [EnableIf("CanNewTileset")]
    void NewTileset()
    {
        if (meTiles.Count == 0)
            BuildIfos();
        else
            Debug.Log("û�����ʱ�������½�");
    }

    bool CanNewTileset() 
    {
        if (!SysGeneral.meInGame) return false;
        if (meTiles.Count == 0) 
            return true; 
        else 
            return false; 
    }

    [Button]
    [PropertySpace(20)]
    [HideInEditorMode]
    void SaveTileset()
    {
        if (SysGeneral.meInGame)
            SaveSet();
    }

    //===================================

    public string meName => gameObject.name;
    public string meFamily => transform.parent.gameObject.name;

    public int meSum {
        get{
            if (!mHaveLoad)
            {
                LoadSet();
                mHaveLoad = true;
            }
            return mTiles.Count;
        }
    }

    public List<IfoTile> meTiles {
        get{
            if (!mHaveLoad)//�б�ʹ�õ�ʱ���ż������ݣ�������������ʱ�Ŀ���
            {
                LoadSet();
                mHaveLoad = true;
            }
            return mTiles;
        }
    }

    [HideInEditorMode]
    [ShowInInspector]
    [ReadOnly]
    List<IfoTile> mTiles;

    //-------------------------------------

    IfoShelfFile mAttach;
    StoTileset mData;
    bool mHaveLoad;

    void Awake()
    {
        mAttach = new IfoShelfFile();
        mAttach.super = "Tilesets";
        mAttach.name = meName;
        VirtualDisk.It.AddFileToVtDisk(mAttach);

        mHaveLoad = false;
    }

    void SaveSet()
    {
        mData.tiles.Clear();
        for (int i = 0; i < mTiles.Count; i++)
        {
            IfoTile ifo = mTiles[i];
            StoTile sto = new StoTile();
            sto.viaAbove = ifo.viaAbove;
            sto.viaBelow = ifo.viaBelow;
            sto.isWall = ifo.isWall;
            sto.attrs = new StrAttrs.StoSave();
            if (ifo.attrs != null)
            {
                sto.attrs.ReadIn(ifo.attrs);
            }
            mData.tiles.Add(sto);
        }
        VirtualDisk.It.SaveFileInRealDisk(mAttach, mData);
    }

    void LoadSet()
    {
        VirtualDisk.It.LoadFileInRealDisk(mAttach, out mData);
        if (mData == null)mData = new StoTileset();
        mTiles = new List<IfoTile>();
        for (int i = 0; i < mData.tiles.Count; i++)
        {
            IfoTile ifo = new IfoTile();
            StoTile sto = mData.tiles[i];
            ifo.sprite = mTilesReferDp[i];
            ifo.viaAbove = sto.viaAbove;
            ifo.viaBelow = sto.viaBelow;
            ifo.isWall = sto.isWall;
            if (sto.attrs != null)
                ifo.attrs = sto.attrs.WriteOut();
            else
                ifo.attrs = new StrAttrs();
            mTiles.Add(ifo);
        }

        mHaveLoad = true;
    }

    void BuildIfos()//�½�һ��tileset��ǰ��
    {
        mTiles = new List<IfoTile>();
        for (int i = 0; i < mTilesReferDp.Count; i++)
        {
            IfoTile tile = new IfoTile();
            tile.sprite = mTilesReferDp[i];
            mTiles.Add(tile);
        }
    }


}

[System.Serializable]
public class IfoTile
{
    public Sprite sprite;

    public bool viaAbove = true;

    public bool viaBelow;

    public bool isWall;

    [HideInInspector]
    public StrAttrs attrs;
}


[System.Serializable]
public struct IfoTileset
{
    public int set;//ʵ�ʴ�����洢��ʹ������
    public string name;//����ʹ���ַ���
}

//======================================

[System.Serializable]
public class StoTileset
{
    public List<StoTile> tiles;

    public StoTileset() { tiles = new List<StoTile>(); }
}

[System.Serializable]
public class StoTile
{
    public bool viaAbove = true;//��ǰ���������Ƭͬһ�㼶ʱ���Ƿ�������(����ͬһ�㼶ʱ���϶����ܴ�����)

    public bool viaBelow;

    public bool isWall;

    public StrAttrs.StoSave attrs;//������ر����Լ�
}