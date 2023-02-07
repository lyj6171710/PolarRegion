using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public interface IFileServe
{
    void Open();

    void Close();

    void Write();

    void Read();
}

public class FileOperator
{
    //文件操作================================

    internal static void WriteToDoc(string content, string pathIn, string docName)
    {
        StreamWriter file;
        string fullPath = pathIn + "/" + docName + ".json";
        if (!File.Exists(fullPath))//如果本地没有对应的json 文件，重新创建
        {
            file = File.CreateText(fullPath);
        }
        else
            file = new StreamWriter(fullPath);
        file.Write(content);
        file.Close();
        //Debug.Log("保存成功");
    }

    internal static string ReadFromDoc(string pathIn, string docName)
    {
        string fullPath = pathIn + "/" + docName + ".json";
        if (!File.Exists(fullPath))
        {
            //Debug.Log("读取的文件不存在！");
            return null;
        }
        string content = File.ReadAllText(fullPath);
        return content;
    }

    //=====================================

    internal static void DelFile(string fileAt, string extension)
    {
        //如果你不是按管理员方式启动VS，
        //而此文件需要管理员权限才能访问，此时File.Exists就会返回false。
        string fileFullPath = fileAt + "." + extension;
        if (File.Exists(fileFullPath))
            File.Delete(fileFullPath);
        else
            Debug.Log("文件不存在 " + fileFullPath);
    }
}