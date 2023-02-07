using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Ŀ�꣺��ʵ���ļ��У���Ӧ���������ļ��У����������ļ��к����õ�ʵ���ļ���
public class VirtualDisk : MonoBehaviour,ISwitchScene
{
    //����Ŀ¼�ĸ߼�ʹ��==========================================

    public bool ExistShelfInVtDisk(IfoShelfFolder ifo)
    {
        OneFolder<string> folderSuper;
        return WhetherExistFolder(out folderSuper, ifo);
    }

    public string[] GetAllShelfInVtDisk(IfoShelfFolder ifo)
    {
        OneFolder<string> folderSuper;
        if (WhetherExistFolder(out folderSuper, ifo))
        {
            string[] shelves = new string[folderSuper.meNumSubFolder];
            int i = 0;
            foreach (var folder in folderSuper.NuEnumSubFolder())
                shelves[i++] = folder.meName;
            return shelves;
        }
        else
            return new string[0];
    }

    //---------------------------------------

    public bool ExistFileInVtDisk(IfoShelfFile ifo)
    {
        OneFolder<string> folderSuper;
        return WhetherExistFile(out folderSuper, ifo);
    }

    public string[] GetAllFileInVtDisk(IfoShelfFolder ifo)
    {
        OneFolder<string> folderSuper;
        if (WhetherExistFolder(out folderSuper, ifo))
        {
            string[] files = new string[folderSuper.meNumSubFile];
            int i = 0;
            foreach (string file in folderSuper.NuEnumSubFile())
                files[i++] = file;
            return files;
        }
        else
            return new string[0];
    }

    public void AddFileToVtDisk(IfoShelfFile ifo, bool needExistFindRoute = false)
    {
        //¼������
        OneFolder<string> fileSuper;
        EFindResult result = FindFileSuper(out fileSuper, ifo);

        if (result != EFindResult.success)
        {
            if (result == EFindResult.errFindTarget)
            {
                if (ifo.withFolder)
                    fileSuper = mCatalogue.AddFolderIn(fileSuper, ifo.name);
            }
            else if (result == EFindResult.errFindRoute)
            {
                if (needExistFindRoute)
                {
                    //Debug.Log("û��������ָ����·��������½���Ӧ�����ļ���");
                    fileSuper = mCatalogue.AddFolderIn(fileSuper, ifo.findRoute);
                }
                else
                {
                    Debug.Log(result.ToString() + " û��������ָ����·�������ʧ��");
                    return;
                }
            }
        }

        mCatalogue.AddFileIn(fileSuper, ifo.name);
    }

    //�鵽ʵ===============================================

    public void DelShelfInRealDisk(IfoShelfFolder ifo, bool alsoDelInVtDisk = true)
    {
        OneFolder<string> find;
        if (WhetherExistFolder(out find, ifo)) 
        {
            string path = ToRealPath(find);
            PathOperator.DelDirectory(path);
            if (alsoDelInVtDisk)
                find.NuGetSuper().SuDelSubFolder(ifo.name);
            return;
        }
        Debug.Log("δ��λ���ļ��д��ڣ�ɾ����Ч");
    }

    //-----------------------------------------------

    public void SaveFileInRealDisk(IfoShelfFile ifo, object content)
    {
        //��ʽ¼�룬��������
        OneFolder<string> fileSuper;
        if (WhetherExistFile(out fileSuper, ifo))
        {
            string json = JsonUse.SuToJson(content);
            string pathIn = ToRealPath(fileSuper);
            PathOperator.BuildDirectory(pathIn);
            FileOperator.WriteToDoc(json, pathIn, ifo.name);
        }
        else
            Debug.Log("��" + fileSuper.meName + "��δ��λ���ļ����ڣ��洢ʧ��");
    }

    public bool LoadFileInRealDisk<T>(IfoShelfFile ifo, out T get) where T : class
    {
        get = null;
        OneFolder<string> fileSuper;
        if (WhetherExistFile(out fileSuper, ifo))
        {
            string pathIn = ToRealPath(fileSuper);
            string content = FileOperator.ReadFromDoc(pathIn, ifo.name);
            if (content == null)
            {
                //Debug.Log(ifo.name + " ��ȡʧ��" + " ������");
                return false;
            }
            else
            {
                if (JsonUse.SuFromJson(content, out get))
                {
                    //Debug.Log(ifo.name + " ��ȡ�ɹ�");
                    return true;
                }
                else
                {
                    //Debug.Log(ifo.name + " ��ȡʧ��");
                    return false;
                }
            }
        }
        else
        {
            //Debug.Log("δ��λ���ļ����ڣ���ȡʧ��");
            return false;
        }
    }

    public void DelFileInRealDisk(IfoShelfFile ifo, bool alsoDelInVtDisk = true)
    {
        OneFolder<string> fileSuper;
        if (WhetherExistFile(out fileSuper, ifo))
        {
            if (ifo.withFolder)
            {
                string path = ToRealPath(fileSuper);
                PathOperator.DelDirectory(path);
                if (alsoDelInVtDisk)
                    fileSuper.NuGetSuper().SuDelSubFolder(ifo.name);
            }
            else
            {
                string path = ToRealPath(fileSuper) + "/" + ifo.name;
                FileOperator.DelFile(path, "json");
                if (alsoDelInVtDisk)
                    fileSuper.SuDelSubFile(ifo.name);
            }
        }
        else
            Debug.Log("δ��λ���ļ����ڣ�ɾ����Ч");
    }

    //����Ŀ¼�Ļ����ӿ�==========================================

    bool WhetherExistFolder(out OneFolder<string> find, IfoShelfFolder ifo)
    {
        OneFolder<string> lastIn;
        EFindResult result = FindFolderSuper(out lastIn, ifo);
        if (result == EFindResult.success && lastIn.SuExistSubFolder(ifo.name))
        {
            find = lastIn.SuGetSubFolder(ifo.name);
            return true;
        }
        else
        {
            find = null;
            return false;
        }
    }

    bool WhetherExistFile(out OneFolder<string> lastIn, IfoShelfFile ifo)
    {//ֻ��ѯ��������Ҫ����ļ�������
        EFindResult result = FindFileSuper(out lastIn, ifo);
        if (result == EFindResult.success && lastIn.SuExistSubFile(ifo.name))
            return true;
        else
            return false;
    }

    EFindResult FindFileSuper(out OneFolder<string> lastIn, IfoShelfFile ifo)
    {
        //��ѯ�ɹ�ʱ�����ص��ļ��в����׸�
        //��ȷ����Ŀ���ļ������ᾡ����ȷ���丸�ļ���

        if (ifo.super != null)
        {
            if (!ifo.withFolder)
                return ProFindSuper(out lastIn, ifo, ifo.super);
            else
            {//���������superֵ��Ŀ���ļ��׸����׸�
                IfoShelfFolder ifoShelf = new IfoShelfFolder();
                ifoShelf.super = ifo.super;//to do ����30�������ֺ�11����һ�����ڲ��Žṹ(�����⸳ֵ��Ӧ���Ѿ�����ˣ�ֻ�Ǳ��³��ֵĴ����谭����֤)
                ifo.CopyBaseValueTo(ifoShelf);
                //ֱ����׼�ļ���������ļ��У�����Ӱ������������
                EFindResult result = FindFolderSuper(out lastIn, ifoShelf);
                if (result == EFindResult.success)
                {
                    var fileFolder = lastIn.SuGetSubFolder(ifo.name);
                    if (fileFolder != null)
                    {
                        lastIn = fileFolder;
                        if (fileFolder.SuExistSubFile(ifo.name))
                            return EFindResult.success;
                        else
                            return EFindResult.errFindTarget;
                    }
                    else
                        return EFindResult.errFindTarget;
                }
                else
                {
                    //Debug.Log(result);
                    return result;
                }
            }
        }
        else
        {//��������Ƿ�ָ�������׸������˶����ڵ㣬�κ��ļ���������丸

            EFindResult result = GetLimitIn(out lastIn, ifo);
            if (result == EFindResult.success)
            {
                OneFolder<string> fileSuper = GetFileInLimit(lastIn, ifo);
                if (fileSuper != null)
                {
                    lastIn = fileSuper;
                    return EFindResult.success;
                }
                else
                    return EFindResult.errFindTarget;
            }
            return result;
        }
    }

    EFindResult FindFolderSuper(out OneFolder<string> lastIn, IfoShelfFolder ifo)
    {
        //��ȷ����Ŀ���ļ��У����ᾡ����ȷ���丸�ļ���

        if (ifo.super != null)
            return ProFindSuper(out lastIn, ifo, ifo.super);
        else
        {
            EFindResult result = GetLimitIn(out lastIn, ifo);
            if (result == EFindResult.success)
            {
                OneFolder<string> aimFolder = GetFolderInLimit(lastIn, ifo);
                if (aimFolder != null)
                {
                    lastIn = aimFolder.NuGetSuper();
                    return EFindResult.success;
                }
                else
                    return EFindResult.errFindTarget;
            }
            return result;
        }
    }

    //--------------------------

    EFindResult ProFindSuper(out OneFolder<string> lastIn, IfoShelf ifo, string super)
    {
        string fileName = ifo.name;
        ifo.name = super;//��ʱ��Ŀ��ת��Ϊ���ļ���
        if (ifo.routeUntilSuper)//������Ҫע���������
            ifo.findRoute = ifo.findRoute.SelfRemoveLast();
        //--------------------------
        EFindResult result = ProFindFolder(out lastIn, ifo);
        //--------------------------
        if (ifo.routeUntilSuper) ifo.findRoute.Add(ifo.name);//��ԭ
        ifo.name = fileName;
        return result;
    }

    EFindResult ProFindFolder(out OneFolder<string> lastIn, IfoShelf ifo)
    {
        OneFolder<string> findLimit;
        EFindResult sign = GetLimitIn(out findLimit, ifo);
        if (sign != EFindResult.success)
        {
            lastIn = findLimit;
            return sign;
        }

        OneFolder<string> aimFolder = GetFolderInLimit(findLimit, ifo);
        if (aimFolder != null)
        {
            lastIn = aimFolder;
            return EFindResult.success;
        }
        else
        {
            lastIn = findLimit;
            return EFindResult.errFindTarget;
        }
    }

    enum EFindResult { success, errFindIn, errFindRoute, errFindTarget }

    EFindResult GetLimitIn(out OneFolder<string> lastIn, IfoShelf ifo)
    {
        OneFolder<string> findFrom;//���Ƕ����ļ���(��Ŀ¼���)

        if (ifo.findIn != null)
        {
            findFrom = mCatalogue.FindFolderBreadthFirst(ifo.findIn, mCatalogue.RootFolder);
            if (findFrom == null)
            {
                lastIn = null;
                return EFindResult.errFindIn;
            }
        }
        else
            findFrom = mCatalogue.RootFolder;

        OneFolder<string> findStart;//�����ļ�׷��·��

        if (ifo.findRoute != null)
        {
            findStart = mCatalogue.FindFolderRoute(ifo.findRoute, findFrom);
            if (findStart == null)
            {
                lastIn = findFrom;//�������ƥ�䵽��
                return EFindResult.errFindRoute;
            }
        }
        else
            findStart = findFrom;

        lastIn = findStart;
        return EFindResult.success;
    }

    OneFolder<string> GetFolderInLimit(OneFolder<string> limitIn, IfoShelf ifo)
    {
        if (ifo.routeUntilSuper)//��ȷ��ֱ���׸���
        {
            return limitIn.SuGetSubFolder(ifo.name);
        }
        else
        {
            if (limitIn.meName == ifo.name)//���ܾ��ǲ�ѯ��Χ·�ߵ��յ㣬������Ҫ�ڷ�Χ�ڼ�������
            {
                return limitIn;
            }
            return mCatalogue.FindFolderDepthFirst(ifo.name, limitIn, ifo.findVia);
        }
    }

    OneFolder<string> GetFileInLimit(OneFolder<string> limitIn, IfoShelfFile ifo)
    {
        //���践�ص���ֱ�Ӹ���������͵���null
        OneFolder<string> super;
        if (ifo.routeUntilSuper)//��ȷ��ֱ���׸���(ֱ�Ӹ���)
        {
            if (limitIn.SuExistSubFile(ifo.name))
                super = limitIn;
            else
                super = null;
        }
        else
            super = mCatalogue.FindFileDepthFirst(ifo.name, limitIn, ifo.findVia);
        return super;
    }

    //----------------------------

    string ToRealPath(OneFolder<string> last)
    {
        if (last == null) return null;
        string path = last.GetDiskPathFromRoot(mCatalogue.RootFolder);
        return RootPath + path;
    }

    //��ʼ��=========================================

    string RootPath => PathOperator.UniformPath(PathSet.PathProject + "/MyRecord");

    CatalogueLowRepeat<string> mCatalogue;//���õ��ļ��У�������ά��������ṩ����

    void BuildCatalogueFromRealDisk()
    {
        mCatalogue = new CatalogueLowRepeat<string>("VirtualSpace", (a, b) => { if (a == b) return true; else return false; });
        BuildFromRealDisk(RootPath, mCatalogue.RootFolder);
    }

    void BuildFromRealDisk(string pathIn, OneFolder<string> container)
    {
        string[] files = PathOperator.GetSubFilesOnlyName(pathIn);
        for (int i = 0; i < files.Length; i++)
        {
            container.SuAddSubFile(files[i]);
        }
        string[] directories = PathOperator.GetSubRelaDirectories(pathIn);
        for (int i = 0; i < directories.Length; i++)
        {
            OneFolder<string> subContainer = container.SuAddSubFolder(directories[i]);
            string subPathIn = pathIn + "/" + directories[i];
            BuildFromRealDisk(subPathIn, subContainer);
        }
    }

    //======================================

    public static VirtualDisk It;

    public void WhenAwake()
    {
        It = this;
        PathOperator.BuildDirectory(RootPath);
        BuildCatalogueFromRealDisk();
    }

    public void WhenSwitchScene()
    {
        throw new NotImplementedException();
    }
}

public class IfoShelf 
{
    //֧��ĵص���Ϣ
    //ֻ�ڲ�ʹ�ã���粻Ҫʹ��

    //��������-------------------------
    //������

    public string name;//�Լ������ƣ���ʹ��ʱ������һ�����ļ��У�

    //��������--------------------------
    //���Ͽ�����ǿ�޶��ԣ������ϾͰ�����ݵķ�ʽ�Ѳ�

    public string findVia;//�����⼶�ĸ��������ƣ����Բ��
                           //�������Ӧ�������ļ���ͬ����������������ͬ����
                           //���ǵ���������һ�����ƻ���Ψһ���ļ�����

    public string findIn;//ָ������һ���ļ�����Ѱ�ң�����ó�Ա�и�ֵ��
                          //��ô��������Ѱ���ļ�ʱ�����ȹ�ȱ�������������
                          //���ڸ��ļ����н�����ȱ��������������ļ��������ӵ�

    public List<string> findRoute;  //���������ȷ��·����
                                    //��ô���������·�����ӵ�ǰ��ʼ�����������
                                    //��󽫸ý����ļ�����Ϊ��ѯ��ʼ�㣬������ȱ���

    public bool routeUntilSuper;//ָʾfindRoute��ֱ�����굽�׸���������ͣ�����ϼ��й̶���ĳһ��
                                //�����а���������Ч������������������Լ��жϴ洢���

    internal void CopyBaseValueTo(IfoShelf other)
    {
        other.name = name;
        other.findVia = findVia;
        other.findIn = findIn;
        if (findRoute != null)
            other.findRoute = findRoute.CopyValueToNew();
        other.routeUntilSuper = routeUntilSuper;
    }
}

public class IfoShelfFile:IfoShelf,ICopySelf<IfoShelfFile>//�ļ��ĸ����ص���Ϣ
{
    //��������----------------------------

    public bool withFolder;//�Ƿ���Ϊһ���ļ���(ͬʱ��Ϊ�ļ����Լ����ļ����ڵ�һ���ļ�)
                           //�ļ����Լ�����ļ�����ͬ�����ļ���
                           //Ӧ�������Ϊͬһ���ļ��Ҵ����ļ��������㼶�����Դ���
                           //�ڲ�����ҲӦ�ȵ�û������ļ��У�����super�µ�һ���ļ�����
    
    public string super;//ֱ�Ӹ��������ƣ�һ�����ļ��У��ļ��������Ӽ���
                        //������ŵ�IfoShelf�У���Ϊ��������

    public IfoShelfFile GetCopy()
    {
        IfoShelfFile copy = new IfoShelfFile();

        CopyBaseValueTo(copy);

        copy.withFolder = withFolder;
        copy.super = super;

        return copy;
    }

}

public class IfoShelfFolder : IfoShelf,ICopySelf<IfoShelfFolder>//�ļ��еĸ����ص���Ϣ
{
    //��������----------------------------

    public string super;

    public IfoShelfFolder GetCopy()
    {
        IfoShelfFolder copy = new IfoShelfFolder();

        CopyBaseValueTo(copy);

        copy.super = super;

        return copy;
    }
}

public class CatalogueLowRepeat<T>//����T�����ļ���ʲô��ʽ����
{//ͬһ�ļ����£����ļ��м����Ʋ����ظ������ļ������Ʋ����ظ������ļ��к��ļ������ƿ����ظ�
 //����������Լ�����߻��ڸ�������һ���м�㣬ʹ�ò�ͬ�㼶�£��ļ���Ҳ������ͬ

    OneFolder<T> mCatalogue;

    public CatalogueLowRepeat(string name, Func<T, T, bool> IfEqual)
    { 
        mCatalogue = OneFolder<T>.FormRoot(name, IfEqual);
    }

    public OneFolder<T> RootFolder => mCatalogue;

    //================================

    public bool AddFileIn(OneFolder<T> folderIn, T mark)
    {
        if (folderIn != null)
        {
            if (!folderIn.SuExistSubFile(mark))
            {
                folderIn.SuAddSubFile(mark);
                //Debug.Log("������������ļ�");
                return true;
            }
            else
            {
                //Debug.Log("�Ѵ���ͬ���ļ�");
                return false;
            }
        }
        else
            return false;
    }

    public OneFolder<T> AddFolderIn(OneFolder<T> folderIn, string name)
    {
        if (folderIn != null)
        {
            if (!folderIn.SuExistSubFolder(name))
            {
                return folderIn.SuAddSubFolder(name);
            }
            else
            {
                //Debug.Log("�Ѵ���ͬ���ļ���");
                return folderIn.SuGetSubFolder(name);
            }
        }
        else
            return null;
    }

    public OneFolder<T> AddFolderIn(OneFolder<T> folderIn, List<string> subChain)
    {
        //���򴴽�����ļ���
        for (int i = 0; i < subChain.Count; i++)
        {
            if (folderIn.SuExistSubFolder(subChain[i]))
                folderIn = folderIn.SuGetSubFolder(subChain[i]);
            else
                folderIn = AddFolderIn(folderIn, subChain[i]);
        }
        return folderIn;
    }

    //------------------------------------

    public OneFolder<T> FindFolderRoute(List<string> supers, OneFolder<T> findIn)
    {
        if (supers == null || supers.Count == 0)
            return findIn;//û��ǰ�������Ǹ��ļ��б���
        
        OneFolder<T> find = findIn.SuGetSubFolder(supers[0]);
        if (supers.Count == 1)
            return find;
        else
        {
            if (find != null)
            {
                List<string> surplus = supers.CopyValueToNew();//list�����ã����������޸�
                surplus.RemoveAt(0);
                return FindFolderRoute(surplus, find);
            }
            else
                return null;
        }
    }

    public OneFolder<T> FindFolderDepthFirst(string folderName, OneFolder<T> findIn, string folderVia = null)
    {
        //�����������������Ʋ��ҷ�Χ��������һ��;�����ļ��У��������Լ���������ͻ

        if (folderName == null || folderName == "")
            return null;

        OneFolder<T> dig;
        Method.FindInStackDF(findIn, (one) => one.NuEnumSubFolder(),
            (one) =>
            {
                if (one.meName == folderName)
                {
                    if (folderVia == null)//���Ŀ¼���Ƚ�Ψһ���Ͳ���Ҫ���������
                        return true;
                    else
                        return IfViaThenValid(one, folderVia);
                }
                else return false;
            }, out dig);
        return dig;
    }

    public OneFolder<T> FindFolderBreadthFirst(string folderName, OneFolder<T> findIn)
    {
        if (folderName == null || folderName == "")
            return null;

        OneFolder<T> dig;
        Method.FindInQueueBF(findIn, (one) => one.NuEnumSubFolder(),
            (one) =>
            {
                if (one.meName == folderName) return true;
                else return false;
            }, out dig);
        return dig;
    }

    //------------------------------------

    public OneFolder<T> FindFileDepthFirst(T file, OneFolder<T> fileIn, string folderVia)
    {
        T dig;
        OneFolder<T> super = null;//����ҵ��ˣ���ô������null
        Method.FindInStackDF(fileIn, (dir) => dir.NuEnumSubFolder(), (dir) => dir.NuEnumSubFile(),
            (dir,one) =>
            {
                if (one.Equals(file))
                {
                    if (folderVia == null)
                    {
                        super = dir;
                        return true;
                    }
                    else
                        return IfViaThenValid(dir, folderVia);
                }
                else return false;
            }, out dig);
        return super;//���ҵ��ļ��������ļ���
    }

    //---------------------------------

    bool IfViaThenValid(OneFolder<T> endAt, string folderVia, bool includeSelf = false)//�ú���ֻ�ܷ������棬��Ϊ��Ҫ�����������
    {
        if (includeSelf && endAt.meName == folderVia) return true;
        OneFolder<T> super = endAt.NuGetSuper();
        while (super != RootFolder)
        {
            if (super.meName == folderVia)
                return true;
            else
                super = super.NuGetSuper();
        }
        return false;// ���㣬��Ҫ��������
    }

}