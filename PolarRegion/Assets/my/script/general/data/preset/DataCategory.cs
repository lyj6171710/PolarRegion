using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataCategory : MonoBehaviour,ISwitchScene
{
    OneFolder<GameObject> mCatalogue;
    List<ThingOne> mPrefabsCollect;

    int mIdAcc;

    void Collect()
    {
        mIdAcc = 0;
        mPrefabsCollect = new List<ThingOne>();

        mCatalogue = OneFolder<GameObject>.FormRoot("capital", (a, b) => { if (a == b) return true; else return false; });
        AddToCatalogue(mCatalogue, transform);

        GbjAssist.ClearChilds(transform);

        GetComponent<DataLibrary>().BuildLibrary(mPrefabsCollect);
    }

    void AddToCatalogue(OneFolder<GameObject> to, Transform from)//深度遍历的形式，搜集所有预制体及所属分类
    {
        for (int i = 0; i < from.childCount; i++)
        {
            Transform child = from.GetChild(i);
            if (child.gameObject.activeSelf) child.gameObject.SetActive(false);

            PrefabCite file = child.GetComponent<PrefabCite>();
            if (file != null)
            {
                to.SuAddSubFile(file.SelfTemplate);

                ThingOne one = new ThingOne();
                one.prefab = file.SelfTemplate;
                one.idCorr = mIdAcc++;
                mPrefabsCollect.Add(one);
            }
            else
            {
                OneFolder<GameObject> folder = to.SuAddSubFolder(child.name);
                AddToCatalogue(folder, child.transform);
            }
        }
    }

    //=============================

    public static DataCategory It;

    public void WhenAwake()
    {
        It = this;
        Collect();
    }

    public void WhenSwitchScene()
    {
        
    }

    public class ThingOne//根据所对应物体自身而赋值
    {
        public int idCorr;//唯一标识码
        public GameObject prefab;//相应的原型(外界运用的是复制原型后的结果)
    }

}
