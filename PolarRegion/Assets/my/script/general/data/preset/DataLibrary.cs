using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLibrary : MonoBehaviour,ISwitchScene
{
    //原型数据库，存储样板，一般存储那些静态的数据，等着被运用

    //私用变量=========================================

    List<DataCategory.ThingOne> mListThingGather;
    Dictionary<string, int> mDicThingMap;

    bool mLoadEnd = false;//异步加载的时候可能会用到的，暂时就放在这

    //内部机制==========================================

    public void BuildLibrary(List<DataCategory.ThingOne> thingList)
    {
        gameObject.name = "data-library";
        mListThingGather = thingList;
    }

    GameObject GetThing(GameObject prototype,Transform parent)
    {
        GameObject copy = Instantiate(prototype);
        copy.transform.parent = parent;
        copy.transform.localPosition = Vector3.zero;//确保默认是坐标0，降低外界考虑
        copy.name = copy.name.TrimEnd("(Clone)".ToCharArray());
        copy.SetActive(true);
        return copy;
    }

    int GetThingId(string title)
    {
        if (mDicThingMap.ContainsKey(title))
            return mDicThingMap[title];
        else
        {
            foreach (DataCategory.ThingOne limit in mListThingGather)
            {
                if (limit.prefab.name == title)
                {
                    mDicThingMap.Add(title, limit.idCorr);
                    return limit.idCorr;
                }
            }
            return -1;
        }
    }


    //外界可用=======================================

    public GameObject SuGetPrototype(string title)//慎用，不安全，外界只需要预制体所含信息时才用
    {
        int where = GetThingId(title);
        if (where < 0)
            return null;
        else
            return mListThingGather[where].prefab;
    }

    public GameObject SuGetThing(string title,Transform parent)//外界取得的是直接可用的复制品
    {
        int where = GetThingId(title);
        if (where < 0)
            return null;
        else
            return GetThing(mListThingGather[where].prefab,parent);
    }


    //-----------------------------------

    //架构============================================

    public static DataLibrary It;

    public void WhenAwake()
    {
        It = this;
        mDicThingMap = new Dictionary<string, int>();
    }

    public void WhenSwitchScene()
    {

    }

}
    
