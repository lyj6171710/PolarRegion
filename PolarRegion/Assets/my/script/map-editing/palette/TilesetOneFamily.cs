using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TilesetOneFamily : MonoBehaviour
{
    public string meName => gameObject.name;

    [HideInPlayMode]
    public List<IfoTileset> mMapSetStringDp;//实际数据是使用整型存的，这里手动标识整型和字符串的对应关系

    //==============================

    Dictionary<int, TilesetOneSet> mMapIndexSet;//从整型到相应图集
    Dictionary<string, int> mMapSetIndex;//从字符串到相应图集的对应整数(整数之间的大小关系没有意义，但必须相互不同)

    public void InitialSets()
    {
        mMapIndexSet = new Dictionary<int, TilesetOneSet>();
        mMapSetIndex = new Dictionary<string, int>();

        for (int i = 0; i < transform.childCount; i++)
        {
            TilesetOneSet thisSet = transform.GetChild(i).GetComponent<TilesetOneSet>();
            if (thisSet != null)
            {
                for (int j = 0; j < mMapSetStringDp.Count; j++)
                {
                    IfoTileset ifo = mMapSetStringDp[j];
                    if (thisSet.meName == ifo.name)
                    {
                        mMapIndexSet.Add(ifo.set, thisSet);
                        mMapSetIndex.Add(ifo.name, ifo.set);
                        mMapSetStringDp.RemoveAt(j);
                        break;
                    }
                }
            }
        }
    }

    public TilesetOneSet GetSet(int set)
    {
        if (mMapIndexSet.ContainsKey(set))
            return mMapIndexSet[set];
        else
            return null;
    }

    public int GetSetIndex(string set)
    {
        return mMapSetIndex[set];
    }

    public List<GameObject> SuGetSets()
    {
        List<GameObject> sets = new List<GameObject>();
        foreach (TilesetOneSet one in mMapIndexSet.Values) sets.Add(one.gameObject);
        return sets;
    }
}


[System.Serializable]
public struct IfoTilesetFamily
{
    public int family;//实际传输与存储，使用整型
    public string name;//交流使用字符串
}