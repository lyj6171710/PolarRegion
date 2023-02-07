using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class FolderTree
{
    public static string GetDiskPathFromRoot<T>(this OneFolder<T> self, OneFolder<T> root)
    {
        string path = "";
        OneFolder[] chain = self.NuSearchByPath(root);
        for (int i = 0; i < self.mePathLength; i++) 
        {
            string nameSec = chain[i].meName;
            if (i < self.mePathLength - 1)
                path += nameSec + "/";
            else
                path += nameSec;
        }
        return path;//首尾无分隔号
    }
}

public class OneFolder<T> : OneFolder//加速以及方便使用
{
    Func<T, T, bool> WhetherEqual;

    public void Initial(string name, OneFolder<T> super, int id, Func<T, T, bool> compare)
    {
        //构造函数，但不放构造函数中，以便更灵活的需求

        indexFolderMap = new Dictionary<string, int>();
        indexFilesMap = new Dictionary<T, int>();
        WhetherEqual = compare;
        InitialBase(name, (a, b) => WhetherEqual((T)a, (T)b), super);
        FormPath(id);//自己相对上级的序数
    }

    public static OneFolder<T> FormRoot(string name, Func<T, T, bool> WhetherEqual)
    {
        OneFolder<T> root = new OneFolder<T>();
        root.Initial(name, null, -1, WhetherEqual);
        return root;
    }

    //回溯===================================

    public int meID{
        get{
            if (super != null)
                return mIDPath[mIDPath.Length - 1];
            else
                return -1;
        }
    }//同级文件夹中的ID

    public int mePathLength => mIDPath.Length;

    int[] mIDPath;//用来方便回溯的
                             //描述从根文件夹到自己这里的路径
                             //属于临时值，与目录名在运行周期下才是唯一对应关系
    
    void FormPath(int id)
    {
        //使用前提是已经设置好父文件夹
        //外界添加该文件夹到另一文件夹中时使用

        if (super == null)//最高级的标志
        {
            mIDPath = null;//不存在上级路径
            return;
        }
        else
        {
            OneFolder<T> orderSuper = super as OneFolder<T>;
            if (orderSuper.mIDPath == null)
            {
                mIDPath = new int[1];
                mIDPath[0] = id;
            }
            else
            {
                mIDPath = new int[orderSuper.mIDPath.Length + 1];
                for (int i = 0; i < orderSuper.mIDPath.Length; i++)
                {
                    mIDPath[i] = orderSuper.mIDPath[i];
                }
                mIDPath[mIDPath.Length - 1] = id;
            }
        }
    }

    internal OneFolder[] NuSearchByPath(OneFolder<T> root)
    {
        OneFolder[] path = new OneFolder[mePathLength];
        OneFolder<T> cur = root;
        for (int i = 0; i < mePathLength; i++)
        {
            cur = cur.GetSubFolder(mIDPath[i]);
            path[i] = cur;
        }
        return path;
    }

    //加速==============================

    Dictionary<string, int> indexFolderMap;//适合同级文件夹之间没有先后关系时，用这个来加速
    Dictionary<T, int> indexFilesMap;//删除列表元素时，该字典不需要重置，int是ID而不是次序

    //文件夹使用=================================

    public OneFolder<T> SuAddSubFolder(string name)
    {
        OneFolder<T> folder = new OneFolder<T>();
        int id = AddFolder(folder);
        folder.Initial(name, this, id, WhetherEqual);
        indexFolderMap.Add(name, id);
        return folder;
    }

    public OneFolder<T> SuGetSubFolder(string name)
    {
        if (indexFolderMap.ContainsKey(name))
            return GetSubFolder(indexFolderMap[name]);
        else
            return null;
    }

    public void SuDelSubFolder(string name)
    {
        if (indexFolderMap.ContainsKey(name))
            DelOneFolder(indexFolderMap[name]);
        else
            Debug.Log("不存在的文件夹");
    }

    public override bool SuExistSubFolder(string name)
    {
        if (indexFolderMap.ContainsKey(name))
            return true;
        else
            return false;
    }

    //文件使用=================================

    public void SuAddSubFile(T mark)
    {
        int id = AddFile(mark);
        indexFilesMap.Add(mark, id);
    }

    public void SuDelSubFile(T mark)
    {
        if (indexFilesMap.ContainsKey(mark))
            DelOneFile(indexFilesMap[mark]);
        else
            Debug.Log("不存在的文件");
    }

    public override bool SuExistSubFile(object mark)
    {
        T aim = (T)mark;
        if (indexFilesMap.ContainsKey(aim))
            return true;
        else
            return false;
    }

    //=======================================

    internal IEnumerable<T> NuEnumSubFile()
    {
        var iter = IterAllFile();
        while(iter.MoveNext())
            yield return (T)GetOneFile(iter.Current);
        yield break;
    }

    internal IEnumerable<OneFolder<T>> NuEnumSubFolder()
    {
        var iter = IterAllFolder();
        while (iter.MoveNext())
            yield return GetOneFolder(iter.Current) as OneFolder<T>;
        yield break;
    }

    OneFolder<T> GetSubFolder(int id) => subFolders[id] as OneFolder<T>;

    T GetSubFile(int id) => (T)subFiles[id];

    internal OneFolder<T> NuGetSuper() => super as OneFolder<T>;

}

public abstract class OneFolder//内容可重复的抽象文件夹
{
    string name;//自己名称
    protected OneFolder super;//父级文件夹的引用

    int indexAcc;//ID只会按次序不断增大
    protected Dictionary<int, OneFolder> subFolders;//该文件夹下的各文件夹
    protected Dictionary<int, object> subFiles;//该文件夹下的各文件的标志
    //使用字典结构，表示文件间、文件夹间不具有相互顺序的限制，但同时还是可以重复的
    //int类型用于区分不同文件或文件夹而已，没有次序，只是唯一ID

    public string meName { get { return name; } }
    public int meNumSubFolder { get { return subFolders.Count; } }
    public int meNumSubFile { get { return subFiles.Count; } }

    Func<object, object, bool> WhetherEqual;//对同一个事物(文件)，得返回true，否则返回false

    //初始化==============================

    protected void InitialBase(string name, Func<object, object, bool> compare, OneFolder super = null)
    {
        this.name = name;
        this.super = super;
        indexAcc = 0;
        subFiles = new Dictionary<int, object>();
        subFolders = new Dictionary<int, OneFolder>();
        WhetherEqual = compare;
    }

    //添加内容==================================

    protected int AddFolder(OneFolder folder)
    {//允许重复，如果不能重复，外界自己加逻辑控制
        indexAcc++;
        subFolders.Add(indexAcc, folder);
        return indexAcc;
    }

    protected int AddFile(object file)
    {
        indexAcc++;
        subFiles.Add(indexAcc, file);
        return indexAcc;
    }

    //移除内容===============================

    protected void DelOneFolder(OneFolder folder)
    {
        subFolders.ForEachUntilModify((id) =>
        {
            if (subFolders[id] == folder)
            {
                subFolders.Remove(id);
                return true;
            }
            else
                return false;
        });
    }

    protected void DelOneFolder(int id)
    {
        if(subFolders.ContainsKey(id))
            subFolders.Remove(id);
    }

    protected void DelOneFile(object file)
    {
        subFiles.ForEachUntilModify((id) =>
        {
            if (subFiles[id] == file)
            {
                subFiles.Remove(id);
                return true;
            }
            else
                return false;
        });
    }

    protected void DelOneFile(int id)
    {
        if (subFiles.ContainsKey(id))
            subFiles.Remove(id);
    }

    //读取==================================

    protected OneFolder GetOneFolder(int id)
    {
        if (subFolders.ContainsKey(id)) return subFolders[id]; else return null;
    }

    protected object GetOneFile(int id)
    {
        if (subFiles.ContainsKey(id)) return subFiles[id]; else return null;
    }

    //工具====================================

    public virtual bool SuExistSubFolder(string name)
    {
        foreach (OneFolder folder in subFolders.Values)
        {
            if (folder.name == name)
                return true;
        }
        return false;
    }

    public virtual bool SuExistSubFile(object mark)
    {
        foreach (object file in subFiles.Values)
        {
            if (WhetherEqual(file, mark))
                return true;
        }
        return false;
    }

    protected IEnumerator<int> IterAllFolder()
    {
        return subFolders.Keys.GetEnumerator();
    }

    protected IEnumerator<int> IterAllFile()
    {
        return subFiles.Keys.GetEnumerator();
    }
}