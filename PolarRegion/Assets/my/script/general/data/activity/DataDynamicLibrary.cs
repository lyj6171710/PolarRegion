using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataDynamicLibrary : MonoBehaviour
{
    //动态数据库，就是随游戏进程会有改变的数据，该类可以提供一些动态方面的帮助，让需要动态存储的数据库类的事物继承该类

    static DataDynamicLibrary it;
    public static DataDynamicLibrary It
    {
        get
        {
            if (!it)
                it = GameObject.Find("data-library").GetComponent<DataDynamicLibrary>();
            return it;
        }
    }

}
