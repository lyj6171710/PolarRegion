using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//外部使用(和具体游戏逻辑相关了)===============================================

public interface ILocalBranch//当作为focus的分支时，就需要有组件继承该接口
{
    //基本理念是下级受上级分配，且在更高级层面上可受上级管控
    //这些接口由上级或同级的另外部门使用，继承该接口的部门内部不要去调用，可能牵扯外部数据的适应性改变
    //通用流程----------------------------------
    //要保持简单基本，性能与功能才好双全
    //提供最为通用的，不够通用的，这里就不给，他们自己单独交流即可
    void SelfReady();//会被自动调用的函数，驱动只限于自己的那些数据的初始化工作的完成
    //注意里面不要放与外界相关的，用来代替Awake，让游戏内容与框架协同化
    //代替Awake，不同脚本的执行顺序可确定化，且不受物体开闭状态影响
    void     Open();//显化，显化前确保接收到了各项基本数据，由此得以正确显示与处理
    void     Close();//隐藏，若涉及内容不多，建议同时把ClearValue做了
    InfoSeal Refresh(InfoSeal seal);//帮助刷新状态，处于open和close之间
}

public interface ILocalBack//用来回溯，下级可能需要回溯其管理者，得知某些信息
{
    LocalFocus Leader { set; }
}

public interface ILocalSib//有时不需要回溯，自身的存在就必定要求同级中有另外特定的事物
{
    LocalSibs Sibs { set; }
}

//-------------------------------------------------

public interface ILocalFocus//用来确定，中心部门的具体负责人，让该负责人继承
{
    List<LocalPart> GetBranch(LocalFocus shelf);//外界自己根据外界情况，给出各分支
                                //这个函数会在最开始被自动调用，因此外界可以用来做一些其它事情
                                /*使用范例：
                                enum Sub { a, b }
                                LocalFocus focus;
                                public List<LocalPart> GetBranch(LocalFocus shelf)
                                {
                                    focus = shelf;

                                    List<LocalPart> parts = new List<LocalPart>();
                                    parts.Add(new LocalPart(Sub.a, transform.GetChild(0).GetComponent<A>()));
                                    parts.Add(new LocalPart(Sub.b, transform.GetChild(1).GetComponent<B>()));
                                    return parts;
                                }*/

    void HearOf(InfoSeal seal);//下级向上级汇报通知它自己的某些客观情况
}

//不得不给接口，外界需要在内部规章下办事，不能随便运用
public interface ILocalPeak {
    //如果是顶峰，需要继承该接口，同时也继承中心接口，都要继承，随后的中心都只需继承中心接口即可
    void SelfReady();
}

//--------------------------------

public class InfoSeal
{
    public string sort;//大类
    public string spec;//小类
    public int id;//号数
    //---------------------------------
    public int integer;
    public float floating;
    public string words;

    public InfoSeal(string sort)
    {
        this.sort = sort;
    }

    public InfoSeal(int id)
    {
        this.id = id;
    }

    public InfoSeal(Enum msg)
    {
        id = msg.GetHashCode();
    }

    //========================

    public static bool operator ==(InfoSeal seal, Enum msg)
    {
        if (seal.id == msg.GetHashCode()) return true;
        else return false;
    }
    public static bool operator !=(InfoSeal seal, Enum msg)
    {
        return !(seal == msg);
    }

    public override bool Equals(object obj)
    {
        InfoSeal faceTo = obj as InfoSeal;
        if (faceTo == null) return false;
        else return faceTo == this;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

}