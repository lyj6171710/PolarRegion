using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataDynamicLibrary : MonoBehaviour
{
    //��̬���ݿ⣬��������Ϸ���̻��иı�����ݣ���������ṩһЩ��̬����İ���������Ҫ��̬�洢�����ݿ��������̳и���

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
