using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class DataStore : MonoBehaviour,ISwitchScene
{
    public const string path_rela = "/my/store/";

    string base_path_1;

    string base_path_2;

    string base_path;

    void Awake()
    {
        base_path_1 = Application.dataPath + path_rela;//前一个变量就是编辑器中asset文件夹所在
        base_path_2 = "/sdcard/toyEnglish/";//还得有一个前提位置，但可以在项目构建设置中调，这里就不用写
#if UNITY_EDITOR
        base_path = base_path_1;
#elif UNITY_ANDROID
        base_path = base_path_2;
#endif
    }

    string GetPath(string sign)
    {
        if (!Directory.Exists(base_path))
            Directory.CreateDirectory(base_path);
        return base_path + sign + ".bin";
    }

    public void Save<T>(T dataClass, string sign)
    {
        if (sign == null || sign == "")
        {
            Debug.Log("路径有误");
        }

        try
        {
            Stream stream = new FileStream(GetPath(sign), FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, dataClass);
            stream.Close();
            stream.Dispose();

        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        } // end try
    }

    public T Get<T>(string sign) where T : class//注意，如果泛型实参的声明位置发生变化，存储的文件就失效了，需要重新存
    {
        if (sign == null || sign == "")
        {
            Debug.Log("路径有误");
            return null;
        }

        T get = null;
        try
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(GetPath(sign), FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);

            if (stream.Length == 0)
            {
                return get;
            }
            else
            {
                get = (T)formatter.Deserialize(stream);
            } // end if
            stream.Close();
            stream.Dispose();

        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        } 
        return get;//如果失败，返回的是null
    }

    //==============================================

    public static DataStore It;

    public void WhenAwake()
    {
        It = this;
    }

    public void WhenSwitchScene()
    {

    }
}
