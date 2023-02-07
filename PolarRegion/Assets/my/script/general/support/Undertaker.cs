using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Undertaker : MonoBehaviour {//获取承包某种功能的具体事物，对应事物可以不需要以单例形式公开

    //先是名称，然后是能实现该名称所要求目标的负责人
    //负责人是可以随便更换的(手动)，不过通过名称使用相应负责人的人，需要自觉用通用标准的语言来传递信息
    
    //一次性赋值====================================

    public StructDic[] list;

    void Awake()
    {
        turn_to_dic();//添加到字典中
    }
    
    //内部工具==========================

    [System.Serializable]
    public struct StructDic
    {
        public string key;
        public MonoBehaviour value;
    }

    void turn_to_dic()
    {
        branches = new Dictionary<string, MonoBehaviour>();
        for (int i = 0; i < list.Length; i++)
            branches.Add(list[i].key, list[i].value);
    }

    //架构需要=============================
    
    static Undertaker single;
    public static Undertaker Single
    { get { if (!single) single = GameObject.Find("UndertakerList").GetComponent<Undertaker>(); return single; } }

    public Dictionary<string, MonoBehaviour> branches;//inspect不支持直接显示字典,这里通过结构体列表间接填充字典


}
