using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class PathOperator
{
    internal static string UniformPath(string path)
    {
        string pathFit = path.Replace('\\', '/');
        pathFit = pathFit.TrimEnd('/');
        return pathFit + "/";
    }

    //===========================

    internal static void BuildDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    internal static void DelDirectory(string path)
    {
        if (Directory.Exists(path))
            Directory.Delete(path, true);
    }

    internal static string[] GetSubDirectories(string pathIn)
    {
        return Directory.GetDirectories(pathIn);
    }

    internal static string[] GetSubRelaDirectories(string pathIn)
    {
        pathIn = UniformPath(pathIn);
        string[] result = Directory.GetDirectories(pathIn);
        for (int i = 0; i < result.Length; i++)
            result[i] = StrAnalyse.TrimPrefix(result[i], pathIn);
        return result;
    }

    //============================

    internal static string[] GetSubFilesOnlyName(string pathIn)
    {
        pathIn = UniformPath(pathIn);
        return GetSubFiles(pathIn, (s) => {
            string rest = StrAnalyse.GetPrefixFromRight(s, '.');
            return StrAnalyse.TrimPrefix(rest, pathIn);
        });
    }

    internal static string[] GetSubFiles(string pathIn,Func<string,string> deal)
    {
        string[] files = Directory.GetFiles(pathIn);
        for (int i = 0; i < files.Length; i++)
        {
            files[i]=deal(files[i]);
        }
        return files;
    }


    //===============================

    void Sample()
    {
        //Directory.GetFiles();//��ȡĿ¼�е��ļ�����������Ŀ¼
        //Directory.GetDirectories();//��ȡĿ¼�е���Ŀ¼����������Ŀ¼����Ŀ¼
        //Directory.GetFileSystemEntries();//�������ļ�����Ŀ¼��������Ŀ¼�µ��ļ�����Ŀ¼

        //FileInfo.Exists����ȡָ���ļ��Ƿ���ڣ�
        //FileInfo.Name��FileInfo.Extensioin����ȡ�ļ������ƺ���չ����
        //FileInfo.FullName����ȡ�ļ���ȫ�޶����ƣ�����·������
        //FileInfo.Directory����ȡ�ļ�����Ŀ¼����������ΪDirectoryInfo��
        //FileInfo.DirectoryName����ȡ�ļ�����Ŀ¼��·��������·������
        //FileInfo.Length����ȡ�ļ��Ĵ�С���ֽ�������
        //FileInfo.IsReadOnly����ȡ�ļ��Ƿ�ֻ����
        //FileInfo.Attributes����ȡ������ָ���ļ������ԣ���������ΪFileAttributesö�٣������Ƕ��ֵ�����
        //FileInfo.CreationTime��FileInfo.LastAccessTime��FileInfo.LastWriteTime���ֱ����ڻ�ȡ�ļ��Ĵ���ʱ�䡢����ʱ�䡢�޸�ʱ�䣻
    }

    public static void GetAllFiles(string dirIn, ref List<string> list)
    {//�ݹ����
        string[] subFiles = Directory.GetFiles(dirIn);
        foreach (string subFile in subFiles)
        {
            list.Add(subFile);
        }
        string[] subDirs = Directory.GetDirectories(dirIn);
        foreach (string subDir in subDirs)
        {
            GetAllFiles(subDir, ref list);
        }
    }

    public static void GetNeedFiles(string dirIn, Func<FileInfo, bool> filter, ref List<string> list)
    {//�ݹ����
        string[] subFiles = Directory.GetFiles(dirIn);
        foreach (string subFile in subFiles)
        {
            FileInfo ifo = new FileInfo(subFile);
            if (filter(ifo)) list.Add(subFile);
        }
        string[] subDirs = Directory.GetDirectories(dirIn);
        foreach (string subDir in subDirs)
        {
            GetNeedFiles(subDir, filter, ref list);
        }
    }

    public static List<string> GetNeedFormatFiles(string dirIn, string[] formats)
    {
        List<string> list = new List<string>();
        Func<FileInfo, bool> filter = (ifo) => {
            if (StrAnalyse.HaveSame(formats, ifo.Extension)) return true; else return false;
        };
        GetNeedFiles(dirIn, filter, ref list);
        return list;
    }

    //------------------------------------------

    public static void GetFilesQueue(string path, ref List<string> list)
    {//���б���
        Queue<string> queue = new Queue<string>();
        queue.Enqueue(path);
        while (queue.Count > 0)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(queue.Dequeue());
            foreach (DirectoryInfo dirchildInfo in dirInfo.GetDirectories())
            {
                queue.Enqueue(dirchildInfo.FullName);
            }
            foreach (FileInfo filechildInfo in dirInfo.GetFiles())
            {
                list.Add(filechildInfo.FullName);
            }

        }
    }

    public static void GetFilesStack(string path, List<string> list)
    {//��ջ����
        Stack<string> stack = new Stack<string>();
        stack.Push(path);
        while (stack.Count > 0)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(stack.Pop());
            foreach (DirectoryInfo dirchildinfo in dirInfo.GetDirectories())
            {
                stack.Push(dirchildinfo.FullName);
            }
            foreach (FileInfo filechidlinfo in dirInfo.GetFiles())
            {
                list.Add(filechidlinfo.FullName);
            }
        }
    }

    /// <summary>
    /// ��ȡ·���������ļ��Լ����ļ������ļ�
    /// </summary>
    /// <param name="path">ȫ·����Ŀ¼</param>
    /// <param name="FileList">��������ļ���ȫ·��</param>
    /// <param name="RelativePath"></param>
    /// <returns></returns>
    public static void GetFile(string path, string fileName, ref string smallDir)
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        FileInfo[] fil = dir.GetFiles();
        DirectoryInfo[] dii = dir.GetDirectories();
        foreach (FileInfo f in fil)
        {
            string name = Path.GetFileName(f.FullName.ToString());
            //Console.WriteLine(name);
            if (name.Contains(fileName))
            {
                Console.WriteLine(Path.GetDirectoryName(f.FullName));
                string[] temp = Path.GetDirectoryName(f.FullName).Split('\\');
                smallDir = temp[temp.Length - 1];
                return;
            }
        }
        //��ȡ���ļ����ڵ��ļ��б��ݹ����
        foreach (DirectoryInfo d in dii)
        {
            if (smallDir == "")
                GetFile(d.FullName, fileName, ref smallDir);
        }
    }

}
