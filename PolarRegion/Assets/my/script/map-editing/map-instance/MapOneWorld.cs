using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOneWorld : MonoBehaviour
{
    //一个广袤区域，包含多个可探索地域

    //一个unity场景，应只有一个世界


    public string meName => gameObject.name;

    List<MapOneTerritory> mTerritories;
    IfoShelfFile mAttach;

    public void MakeReady()
    {
        BuildCatalogue();

        mTerritories = new List<MapOneTerritory>();
        for (int i = 0; i < transform.childCount; i++)
        {
            mTerritories.Add(transform.GetChild(i).GetComponent<MapOneTerritory>());
            mTerritories[i].GetComponent<MapOneTerritory>().MakeReady(this);
        }
    }

    public void BuildCatalogue()//存储环境
    {
        mAttach = new IfoShelfFile();
        mAttach.name = meName;
        mAttach.super = "worlds";
        mAttach.withFolder = true;
        VirtualDisk.It.AddFileToVtDisk(mAttach);
    }

}

[System.Serializable]
public class IfoWorld
{//基本引用信息
    public string name;
    public int cellSum;
}