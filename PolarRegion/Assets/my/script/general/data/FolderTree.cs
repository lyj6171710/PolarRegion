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
        return path;//��β�޷ָ���
    }
}

public class OneFolder<T> : OneFolder//�����Լ�����ʹ��
{
    Func<T, T, bool> WhetherEqual;

    public void Initial(string name, OneFolder<T> super, int id, Func<T, T, bool> compare)
    {
        //���캯���������Ź��캯���У��Ա����������

        indexFolderMap = new Dictionary<string, int>();
        indexFilesMap = new Dictionary<T, int>();
        WhetherEqual = compare;
        InitialBase(name, (a, b) => WhetherEqual((T)a, (T)b), super);
        FormPath(id);//�Լ�����ϼ�������
    }

    public static OneFolder<T> FormRoot(string name, Func<T, T, bool> WhetherEqual)
    {
        OneFolder<T> root = new OneFolder<T>();
        root.Initial(name, null, -1, WhetherEqual);
        return root;
    }

    //����===================================

    public int meID{
        get{
            if (super != null)
                return mIDPath[mIDPath.Length - 1];
            else
                return -1;
        }
    }//ͬ���ļ����е�ID

    public int mePathLength => mIDPath.Length;

    int[] mIDPath;//����������ݵ�
                             //�����Ӹ��ļ��е��Լ������·��
                             //������ʱֵ����Ŀ¼�������������²���Ψһ��Ӧ��ϵ
    
    void FormPath(int id)
    {
        //ʹ��ǰ�����Ѿ����úø��ļ���
        //�����Ӹ��ļ��е���һ�ļ�����ʱʹ��

        if (super == null)//��߼��ı�־
        {
            mIDPath = null;//�������ϼ�·��
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

    //����==============================

    Dictionary<string, int> indexFolderMap;//�ʺ�ͬ���ļ���֮��û���Ⱥ��ϵʱ�������������
    Dictionary<T, int> indexFilesMap;//ɾ���б�Ԫ��ʱ�����ֵ䲻��Ҫ���ã�int��ID�����Ǵ���

    //�ļ���ʹ��=================================

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
            Debug.Log("�����ڵ��ļ���");
    }

    public override bool SuExistSubFolder(string name)
    {
        if (indexFolderMap.ContainsKey(name))
            return true;
        else
            return false;
    }

    //�ļ�ʹ��=================================

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
            Debug.Log("�����ڵ��ļ�");
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

public abstract class OneFolder//���ݿ��ظ��ĳ����ļ���
{
    string name;//�Լ�����
    protected OneFolder super;//�����ļ��е�����

    int indexAcc;//IDֻ�ᰴ���򲻶�����
    protected Dictionary<int, OneFolder> subFolders;//���ļ����µĸ��ļ���
    protected Dictionary<int, object> subFiles;//���ļ����µĸ��ļ��ı�־
    //ʹ���ֵ�ṹ����ʾ�ļ��䡢�ļ��м䲻�����໥˳������ƣ���ͬʱ���ǿ����ظ���
    //int�����������ֲ�ͬ�ļ����ļ��ж��ѣ�û�д���ֻ��ΨһID

    public string meName { get { return name; } }
    public int meNumSubFolder { get { return subFolders.Count; } }
    public int meNumSubFile { get { return subFiles.Count; } }

    Func<object, object, bool> WhetherEqual;//��ͬһ������(�ļ�)���÷���true�����򷵻�false

    //��ʼ��==============================

    protected void InitialBase(string name, Func<object, object, bool> compare, OneFolder super = null)
    {
        this.name = name;
        this.super = super;
        indexAcc = 0;
        subFiles = new Dictionary<int, object>();
        subFolders = new Dictionary<int, OneFolder>();
        WhetherEqual = compare;
    }

    //�������==================================

    protected int AddFolder(OneFolder folder)
    {//�����ظ�����������ظ�������Լ����߼�����
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

    //�Ƴ�����===============================

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

    //��ȡ==================================

    protected OneFolder GetOneFolder(int id)
    {
        if (subFolders.ContainsKey(id)) return subFolders[id]; else return null;
    }

    protected object GetOneFile(int id)
    {
        if (subFiles.ContainsKey(id)) return subFiles[id]; else return null;
    }

    //����====================================

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