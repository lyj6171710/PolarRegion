using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class MapPlates : MonoBehaviour
{//主要管理多个tileset，以及共通影响的数据

    [HideInPlayMode]
    public Sprite meTileDefaultDp;
    [HideInPlayMode]
    public Sprite meTileTestDp;
    [HideInPlayMode]
    public Sprite meSelectBoxPaletteDp;
    [HideInPlayMode]
    public Sprite meSelectBoxMapDp;

    //=====================================

    [HideInPlayMode]
    public List<IfoTilesetFamily> mMapFamilyStringDp;//实际数据是使用整型存的，这里手动标识整型和字符串的对应关系
                                                     //不需要按序数大小顺序存放

    //===============================

    Dictionary<int, TilesetOneFamily> mMapIndexFamily;//从整型到相应种类
    Dictionary<string, int> mMapFamilyIndex;//从字符串到相应种类的对应整数(整数之间的大小关系没有意义，但必须相互不同)
                                            //需要有字符串线索，外界使用时，肯定使用的名称，而不是数字

    void InitialFamilies()
    {
        mMapIndexFamily = new Dictionary<int, TilesetOneFamily>();
        mMapFamilyIndex = new Dictionary<string, int>();

        for (int i = 0; i < transform.childCount; i++)
        {
            TilesetOneFamily thisFamily = transform.GetChild(i).GetComponent<TilesetOneFamily>();
            if (thisFamily != null)
            {
                for (int j = 0; j < mMapFamilyStringDp.Count; j++)
                {
                    IfoTilesetFamily ifo = mMapFamilyStringDp[j];
                    if (thisFamily.meName == ifo.name)
                    {
                        mMapIndexFamily.Add(ifo.family, thisFamily);
                        mMapFamilyIndex.Add(ifo.name, ifo.family);
                        thisFamily.InitialSets();
                        mMapFamilyStringDp.RemoveAt(j);
                        break;
                    }
                }
            }
        }
    }

    TilesetOneSet GetSet(int family, int set)
    {
        if (mMapIndexFamily.ContainsKey(family))
            return mMapIndexFamily[family].GetSet(set);
        else
            return null;
    }

    public void GetSetIndex(string family, string set, out int familyIndex, out int setIndex)
    {
        familyIndex= mMapFamilyIndex[family];
        setIndex = mMapIndexFamily[familyIndex].GetSetIndex(set);
    }

    public IfoTile GetTileAttrRef(IfoTilePick pick)
    {
        TilesetOneSet tileset = GetValidSet(pick);
        if (tileset != null)
            return tileset.meTiles[pick.index];
        else
            return null;
    }

    TilesetOneSet GetValidSet(IfoTilePick pick)
    {
        TilesetOneSet tileset = GetSet(pick.family, pick.set);
        if (tileset != null)
        {
            if (pick.index < 0 || pick.index >= tileset.meSum)
                return null;
            else
                return tileset;
        }
        else
            return null;
    }

    Sprite GetSprite(IfoTilePick pick)
    {
        TilesetOneSet tileset = GetValidSet(pick);
        if (tileset != null)
            return tileset.meTiles[pick.index].sprite;
        else
            return null;
    }

    //---------------------------------

    public string SuGetFamilyName(int index)
    {
        return mMapIndexFamily[index].meName;
    }

    public string SuGetSetName(int familyIndex, int setIndex)
    {
        return mMapIndexFamily[familyIndex].GetSet(setIndex).meName;
    }

    public IfoTile SuGetTileAttrSafe(IfoTilePick pick)
    {
        TilesetOneSet tileset = GetValidSet(pick); 
        if (tileset != null)
        {
            IfoTile copy = new IfoTile();//保障数据安全
            IfoTile refer = tileset.meTiles[pick.index];
            copy.sprite = refer.sprite;
            copy.viaBelow = refer.viaBelow;
            copy.viaAbove = refer.viaAbove;
            copy.isWall = refer.isWall;
            copy.attrs = refer.attrs.GetCopy();
            return copy;
        }
        else
            return null;
    }

    public TilesetOneSet SuGetSet(string family, string set)
    {
        int familyIndex, setIndex;
        GetSetIndex(family, set, out familyIndex, out setIndex);
        if (mMapIndexFamily.ContainsKey(familyIndex))
            return mMapIndexFamily[familyIndex].GetSet(setIndex);
        else
            return null;
    }

    public Sprite SuGetSprite(IfoTilePick pick)//系统内部应该始终传递的数据，表现是单向的、局部的
    {
        if (pick.family == 0)//0系瓦片全部用来标记
        {
            if (pick.index <= 0)
                return meTileDefaultDp;
            else
                return GetSprite(pick);
        }
        else
            return GetSprite(pick);
    }

    public List<GameObject> SuGetFamilies()
    {
        List<GameObject> families = new List<GameObject>();
        foreach (TilesetOneFamily one in mMapIndexFamily.Values)families.Add(one.gameObject);
        return families;
    }

    //================================

    public bool meInTileSelect => mInTileSelect;

    public int meCurLayer => mCurLayer;

    [HideInInspector] public bool mInTileSelect;//如果鼠标进入地图区则标记为false，进入瓦片区则标记为true

    int mCurLayer;//当前所操作层级
    Text mShowLayer;

    public void SetLayerHandle(int to)
    {
        mCurLayer = to;
        mShowLayer.text = mCurLayer.ToString();
    }

    void InitialLayer()
    {
        UiAlone.Ifo refer = new UiAlone.Ifo();
        refer.byWorldOrCanvas = false;
        refer.byAbsoluteOrPercent = false;
        refer.posStart = new Vector2(0.8f, 0.8f);
        mShowLayer = GText.It.SuForm(mCurLayer.ToString(), 0, refer).GetComponent<Text>();
    }

    void UpdateTileLayer()
    {
        if (KeyboardInput.SuIsInPress(MapRefer.cKeyLayer))
        {
            for (int i = 0; i < MapRefer.cLayerMax; i++)
            {
                if (Input.GetKeyDown(i.ToString()))
                {
                    mCurLayer = i;
                    mShowLayer.text = mCurLayer.ToString();
                    break;
                }
            }
        }
    }

    //================================

    public static MapPlates It;

    void Awake()
    {
        It = this;
        InitialFamilies();
        InitialLayer();
    }

    void Update()
    {
        UpdateTileLayer();
    }
}
