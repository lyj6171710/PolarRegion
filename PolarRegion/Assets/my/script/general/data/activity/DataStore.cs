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
        base_path_1 = Application.dataPath + path_rela;//ǰһ���������Ǳ༭����asset�ļ�������
        base_path_2 = "/sdcard/toyEnglish/";//������һ��ǰ��λ�ã�����������Ŀ���������е�������Ͳ���д
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
            Debug.Log("·������");
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

    public T Get<T>(string sign) where T : class//ע�⣬�������ʵ�ε�����λ�÷����仯���洢���ļ���ʧЧ�ˣ���Ҫ���´�
    {
        if (sign == null || sign == "")
        {
            Debug.Log("·������");
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
        return get;//���ʧ�ܣ����ص���null
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
