using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOneWorld : MonoBehaviour
{
    //һ���������򣬰��������̽������

    //һ��unity������Ӧֻ��һ������


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

    public void BuildCatalogue()//�洢����
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
{//����������Ϣ
    public string name;
    public int cellSum;
}