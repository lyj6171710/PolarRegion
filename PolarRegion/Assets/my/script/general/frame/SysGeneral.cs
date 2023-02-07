using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SysGeneral : MonoBehaviour,ISwitchScene 
{//游戏中全局通用的数据，而且在不同游戏中也会涉及到的属性

    //外界可操作=====================================

    public AssetBundle stuffPack { get { return stuff_pack; } }//外界可提取利用
                                                               //用法举例：GameObject s = MySystem.Root.stuffPack.LoadAsset<GameObject>("sign");

    public static bool meInGame => mInGame;

    //一次性赋值=====================================

    public int hope_frame_rate = 60;
    
    //私用变量=====================================

    AssetBundle stuff_pack;

    static bool mInGame;

    //架构需要=====================================

    public static SysGeneral It;

    public void WhenAwake()
    {
        It = this;

        //stuff_pack = AssetBundle.LoadFromFile(AddrAbs.path_base_stuff);
        Application.targetFrameRate = hope_frame_rate;

        mInGame = true;
    }

    public void WhenSwitchScene()
    {
        
    }

    private void OnDisable()
    {
        mInGame = false;
    }

    //[RuntimeInitializeOnLoadMethod]
    //static void Initialize()
    //{
    //    if (SceneManager.GetActiveScene().name == "start_scene")//运行始终从某一场景开始
    //        return;
    //    else
    //        SceneManager.LoadScene("start_scene");
    //}

}
