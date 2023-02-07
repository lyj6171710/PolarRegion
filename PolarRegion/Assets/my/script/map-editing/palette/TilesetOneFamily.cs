using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TilesetOneFamily : MonoBehaviour
{
    public string meName => gameObject.name;

    [HideInPlayMode]
    public List<IfoTileset> mMapSetStringDp;//ʵ��������ʹ�����ʹ�ģ������ֶ���ʶ���ͺ��ַ����Ķ�Ӧ��ϵ

    //==============================

    Dictionary<int, TilesetOneSet> mMapIndexSet;//�����͵���Ӧͼ��
    Dictionary<string, int> mMapSetIndex;//���ַ�������Ӧͼ���Ķ�Ӧ����(����֮��Ĵ�С��ϵû�����壬�������໥��ͬ)

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
    public int family;//ʵ�ʴ�����洢��ʹ������
    public string name;//����ʹ���ַ���
}