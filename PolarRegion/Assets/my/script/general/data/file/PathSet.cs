using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSet
{
    public static string PathProject => System.Environment.CurrentDirectory;

    public static string PathStreamingAssets => Application.streamingAssetsPath;

    //System.Environment.CurrentDirectory         //��ȡ�����ع��̵ľ���·��
    //Application.dataPath     //Assets��Դ�ļ��еľ���·��
    //Application.persistentDataPath     //�־��Ե����ݴ洢·�����ڲ�ͬƽ̨·����ͬ���������ڣ�����·��
    //Application.streamingAssetsPath     //Assets��Դ�ļ�����StreamingAssets�ļ���Ŀ¼�ľ���·��
    //Application.temporaryCachePath     //��Ϸ����ʱ�Ļ���Ŀ¼��Ҳ�Ǿ���·��
}
