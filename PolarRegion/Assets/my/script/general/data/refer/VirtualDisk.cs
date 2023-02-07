using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//目标：从实际文件夹，对应建立虚拟文件夹，操作虚拟文件夹后，作用到实际文件夹
public class VirtualDisk : MonoBehaviour,ISwitchScene
{
    //虚拟目录的高级使用==========================================

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
        //录入线索
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
                    //Debug.Log("没有线索中指定的路径，因此新建对应虚拟文件夹");
                    fileSuper = mCatalogue.AddFolderIn(fileSuper, ifo.findRoute);
                }
                else
                {
                    Debug.Log(result.ToString() + " 没有线索中指定的路径，添加失败");
                    return;
                }
            }
        }

        mCatalogue.AddFileIn(fileSuper, ifo.name);
    }

    //虚到实===============================================

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
        Debug.Log("未定位到文件夹存在，删除无效");
    }

    //-----------------------------------------------

    public void SaveFileInRealDisk(IfoShelfFile ifo, object content)
    {
        //正式录入，运用线索
        OneFolder<string> fileSuper;
        if (WhetherExistFile(out fileSuper, ifo))
        {
            string json = JsonUse.SuToJson(content);
            string pathIn = ToRealPath(fileSuper);
            PathOperator.BuildDirectory(pathIn);
            FileOperator.WriteToDoc(json, pathIn, ifo.name);
        }
        else
            Debug.Log("在" + fileSuper.meName + "中未定位到文件存在，存储失败");
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
                //Debug.Log(ifo.name + " 读取失败" + " 无内容");
                return false;
            }
            else
            {
                if (JsonUse.SuFromJson(content, out get))
                {
                    //Debug.Log(ifo.name + " 读取成功");
                    return true;
                }
                else
                {
                    //Debug.Log(ifo.name + " 读取失败");
                    return false;
                }
            }
        }
        else
        {
            //Debug.Log("未定位到文件存在，读取失败");
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
            Debug.Log("未定位到文件存在，删除无效");
    }

    //虚拟目录的基本接口==========================================

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
    {//只查询最符合外界要求的文件的有无
        EFindResult result = FindFileSuper(out lastIn, ifo);
        if (result == EFindResult.success && lastIn.SuExistSubFile(ifo.name))
            return true;
        else
            return false;
    }

    EFindResult FindFileSuper(out OneFolder<string> lastIn, IfoShelfFile ifo)
    {
        //查询成功时，返回的文件夹才是亲父
        //不确保有目标文件，但会尽可能确定其父文件夹

        if (ifo.super != null)
        {
            if (!ifo.withFolder)
                return ProFindSuper(out lastIn, ifo, ifo.super);
            else
            {//特殊情况，super值是目标文件亲父的亲父
                IfoShelfFolder ifoShelf = new IfoShelfFolder();
                ifoShelf.super = ifo.super;//to do 进入30房间会出现和11房间一样的内部门结构(加了这赋值，应该已经解决了，只是被新出现的错误阻碍了验证)
                ifo.CopyBaseValueTo(ifoShelf);
                //直接瞄准文件所伴随的文件夹，不会影响其它参数的
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
        {//不管外界是否指定了其亲父，除了顶级节点，任何文件都会存在其父

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
        //不确保有目标文件夹，但会尽可能确定其父文件夹

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
        ifo.name = super;//暂时将目标转移为父文件夹
        if (ifo.routeUntilSuper)//这里需要注意这种情况
            ifo.findRoute = ifo.findRoute.SelfRemoveLast();
        //--------------------------
        EFindResult result = ProFindFolder(out lastIn, ifo);
        //--------------------------
        if (ifo.routeUntilSuper) ifo.findRoute.Add(ifo.name);//还原
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
        OneFolder<string> findFrom;//考虑顶级文件夹(非目录起点)

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

        OneFolder<string> findStart;//考虑文件追溯路径

        if (ifo.findRoute != null)
        {
            findStart = mCatalogue.FindFolderRoute(ifo.findRoute, findFrom);
            if (findStart == null)
            {
                lastIn = findFrom;//返回最后匹配到的
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
        if (ifo.routeUntilSuper)//明确会直到亲父级
        {
            return limitIn.SuGetSubFolder(ifo.name);
        }
        else
        {
            if (limitIn.meName == ifo.name)//可能就是查询范围路线的终点，而不需要在范围内继续查找
            {
                return limitIn;
            }
            return mCatalogue.FindFolderDepthFirst(ifo.name, limitIn, ifo.findVia);
        }
    }

    OneFolder<string> GetFileInLimit(OneFolder<string> limitIn, IfoShelfFile ifo)
    {
        //必需返回的是直接父级，否则就得是null
        OneFolder<string> super;
        if (ifo.routeUntilSuper)//明确会直到亲父级(直接父级)
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

    //初始化=========================================

    string RootPath => PathOperator.UniformPath(PathSet.PathProject + "/MyRecord");

    CatalogueLowRepeat<string> mCatalogue;//共用的文件夹，被该类维护与对外提供服务

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
    //支点的地点信息
    //只内部使用，外界不要使用

    //基本线索-------------------------
    //必填项

    public string name;//自己的名称（被使用时，意义一定是文件夹）

    //额外线索--------------------------
    //加上可以增强限定性，不加上就按最宽容的方式搜查

    public string findVia;//第任意级的父级的名称，可以不填，
                           //但不填会应付不了文件夹同名的情况，如果可能同名，
                           //还是得填且填入一个名称基本唯一的文件夹名

    public string findIn;//指定从哪一个文件夹中寻找，如果该成员有赋值，
                          //那么根据线索寻找文件时，会先广度遍历搜索到它，
                          //再于该文件夹中进行深度遍历搜索，其它文件夹是无视的

    public List<string> findRoute;  //如果给了明确的路径，
                                    //那么会依照这个路径，从当前起始点出发到结束
                                    //随后将该结束文件夹作为查询起始点，向内深度遍历

    public bool routeUntilSuper;//指示findRoute是直接延申到亲父级，还是停留在上级中固定的某一处
                                //可以有帮助搜索的效果，还可以利于外界自己判断存储情况

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

public class IfoShelfFile:IfoShelf,ICopySelf<IfoShelfFile>//文件的附属地点信息
{
    //额外线索----------------------------

    public bool withFolder;//是否作为一个文件夹(同时存为文件夹以及该文件夹内的一个文件)
                           //文件夹以及与该文件夹内同名的文件，
                           //应被外界视为同一个文件且处于文件夹所处层级中来对待，
                           //内部处理也应先当没有这个文件夹，而是super下的一个文件来看
    
    public string super;//直接父级的名称（一定是文件夹，文件不会有子级）
                        //不建议放到IfoShelf中，因为不够基本

    public IfoShelfFile GetCopy()
    {
        IfoShelfFile copy = new IfoShelfFile();

        CopyBaseValueTo(copy);

        copy.withFolder = withFolder;
        copy.super = super;

        return copy;
    }

}

public class IfoShelfFolder : IfoShelf,ICopySelf<IfoShelfFolder>//文件夹的附属地点信息
{
    //额外线索----------------------------

    public string super;

    public IfoShelfFolder GetCopy()
    {
        IfoShelfFolder copy = new IfoShelfFolder();

        CopyBaseValueTo(copy);

        copy.super = super;

        return copy;
    }
}

public class CatalogueLowRepeat<T>//泛型T描述文件以什么形式存在
{//同一文件夹下，各文件夹间名称不会重复，各文件间名称不会重复，但文件夹和文件间名称可能重复
 //外界可以自行约定或者基于该类新增一个中间层，使得不同层级下，文件名也互不相同

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
                //Debug.Log("添加了新虚拟文件");
                return true;
            }
            else
            {
                //Debug.Log("已存在同名文件");
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
                //Debug.Log("已存在同名文件夹");
                return folderIn.SuGetSubFolder(name);
            }
        }
        else
            return null;
    }

    public OneFolder<T> AddFolderIn(OneFolder<T> folderIn, List<string> subChain)
    {
        //纵向创建多个文件夹
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
            return findIn;//没有前进，就是根文件夹本身
        
        OneFolder<T> find = findIn.SuGetSubFolder(supers[0]);
        if (supers.Count == 1)
            return find;
        else
        {
            if (find != null)
            {
                List<string> surplus = supers.CopyValueToNew();//list是引用，不能随意修改
                surplus.RemoveAt(0);
                return FindFolderRoute(surplus, find);
            }
            else
                return null;
        }
    }

    public OneFolder<T> FindFolderDepthFirst(string folderName, OneFolder<T> findIn, string folderVia = null)
    {
        //第三个参数可以限制查找范围，描述了一定途径的文件夹，这样可以减少命名冲突

        if (folderName == null || folderName == "")
            return null;

        OneFolder<T> dig;
        Method.FindInStackDF(findIn, (one) => one.NuEnumSubFolder(),
            (one) =>
            {
                if (one.meName == folderName)
                {
                    if (folderVia == null)//如果目录名比较唯一，就不需要给这个参数
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
        OneFolder<T> super = null;//如果找到了，那么不会是null
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
        return super;//所找到文件的所在文件夹
    }

    //---------------------------------

    bool IfViaThenValid(OneFolder<T> endAt, string folderVia, bool includeSelf = false)//该函数只能放这里面，因为需要干涉遍历过程
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
        return false;// 不算，需要继续查找
    }

}