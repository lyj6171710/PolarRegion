using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TilesetOneSet : MonoBehaviour
{   //负责素材本身的属性
    //每一个tileset实例，应该承包一个成套的瓦片组，能专门用于绘制某种建筑或地理情况，这样编辑起来轻松快速

    //===================================

    [HideInPlayMode]
    public int mRowsDp;//这个会影响同时占据多个单元格的瓦片的表示
    [HideInPlayMode]
    public int mColsDp;

    [PropertySpace(20)]
    [HideInPlayMode]
    public List<Sprite> mTilesReferDp;//辅助进行赋值的

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
            Debug.Log("没有清空时，不能新建");
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
            if (!mHaveLoad)//有被使用到时，才加载数据，减少启动测试时的卡顿
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

    void BuildIfos()//新建一个tileset的前提
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
    public int set;//实际传输与存储，使用整型
    public string name;//交流使用字符串
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
    public bool viaAbove = true;//当前人物与该瓦片同一层级时，是否允许穿过(不在同一层级时，肯定都能穿过的)

    public bool viaBelow;

    public bool isWall;

    public StrAttrs.StoSave attrs;//自身的特别属性集
}