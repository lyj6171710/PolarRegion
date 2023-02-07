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
        //Directory.GetFiles();//获取目录中的文件，不包含子目录
        //Directory.GetDirectories();//获取目录中的子目录，不包含子目录的子目录
        //Directory.GetFileSystemEntries();//好像是文件及子目录，不含子目录下的文件及子目录

        //FileInfo.Exists：获取指定文件是否存在；
        //FileInfo.Name，FileInfo.Extensioin：获取文件的名称和扩展名；
        //FileInfo.FullName：获取文件的全限定名称（完整路径）；
        //FileInfo.Directory：获取文件所在目录，返回类型为DirectoryInfo；
        //FileInfo.DirectoryName：获取文件所在目录的路径（完整路径）；
        //FileInfo.Length：获取文件的大小（字节数）；
        //FileInfo.IsReadOnly：获取文件是否只读；
        //FileInfo.Attributes：获取或设置指定文件的属性，返回类型为FileAttributes枚举，可以是多个值的组合
        //FileInfo.CreationTime、FileInfo.LastAccessTime、FileInfo.LastWriteTime：分别用于获取文件的创建时间、访问时间、修改时间；
    }

    public static void GetAllFiles(string dirIn, ref List<string> list)
    {//递归遍历
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
    {//递归遍历
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
    {//队列遍历
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
    {//堆栈遍历
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
    /// 获取路径下所有文件以及子文件夹中文件
    /// </summary>
    /// <param name="path">全路径根目录</param>
    /// <param name="FileList">存放所有文件的全路径</param>
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
        //获取子文件夹内的文件列表，递归遍历
        foreach (DirectoryInfo d in dii)
        {
            if (smallDir == "")
                GetFile(d.FullName, fileName, ref smallDir);
        }
    }

}
