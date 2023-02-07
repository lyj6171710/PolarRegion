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
    //�ļ�����================================

    internal static void WriteToDoc(string content, string pathIn, string docName)
    {
        StreamWriter file;
        string fullPath = pathIn + "/" + docName + ".json";
        if (!File.Exists(fullPath))//�������û�ж�Ӧ��json �ļ������´���
        {
            file = File.CreateText(fullPath);
        }
        else
            file = new StreamWriter(fullPath);
        file.Write(content);
        file.Close();
        //Debug.Log("����ɹ�");
    }

    internal static string ReadFromDoc(string pathIn, string docName)
    {
        string fullPath = pathIn + "/" + docName + ".json";
        if (!File.Exists(fullPath))
        {
            //Debug.Log("��ȡ���ļ������ڣ�");
            return null;
        }
        string content = File.ReadAllText(fullPath);
        return content;
    }

    //=====================================

    internal static void DelFile(string fileAt, string extension)
    {
        //����㲻�ǰ�����Ա��ʽ����VS��
        //�����ļ���Ҫ����ԱȨ�޲��ܷ��ʣ���ʱFile.Exists�ͻ᷵��false��
        string fileFullPath = fileAt + "." + extension;
        if (File.Exists(fileFullPath))
            File.Delete(fileFullPath);
        else
            Debug.Log("�ļ������� " + fileFullPath);
    }
}