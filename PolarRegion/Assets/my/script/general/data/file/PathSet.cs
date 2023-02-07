using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSet
{
    public static string PathProject => System.Environment.CurrentDirectory;

    public static string PathStreamingAssets => Application.streamingAssetsPath;

    //System.Environment.CurrentDirectory         //获取到本地工程的绝对路径
    //Application.dataPath     //Assets资源文件夹的绝对路径
    //Application.persistentDataPath     //持久性的数据存储路径，在不同平台路径不同，但都存在，绝对路径
    //Application.streamingAssetsPath     //Assets资源文件夹下StreamingAssets文件夹目录的绝对路径
    //Application.temporaryCachePath     //游戏运行时的缓存目录，也是绝对路径
}
