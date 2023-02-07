using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//外部使用(和具体游戏逻辑相关了)===============================================

public interface ITreeBranch//当作为focus的分支时，就需要有组件继承该接口
{
    void SelfReady(TreeBranch shelf);

    object RespondReadFromUp(InfoSeal seal,out int source);//接收上级来的请求，如果第一个下级没有，系统会从更下一级寻找

    void ReceiveNotifyFromUp(InfoSeal seal);
}

public interface ITreeFocus//用来确定，中心部门的具体负责人，让该负责人继承
{
    List<TreePart> GetParts(TreeFocus shelf);//外界自己根据外界情况，给出各分支
                                                //这个函数会在最开始被自动调用，因此外界可以用来做一些其它事情
    /*使用范例：
    enum Sub { a, b }
    TreeFocus focus;
    public List<TreePart> GetBranch(TreeFocus shelf)
    {
        focus = shelf;

        List<TreePart> parts = new List<TreePart>();
        parts.Add(new TreePart(Sub.a, transform.GetChild(0).GetComponent<A>()));
        parts.Add(new TreePart(Sub.b, transform.GetChild(1).GetComponent<B>()));
        return parts;
    }*/

    bool HearOfDown(InfoSeal seal);//作为上级的自身接收由下级汇报通知的一些客观情况

    object RespondRequestFromDown(InfoSeal seal);//接收下级来的请求，如果第一个上级没有，系统会从更上一级寻找
}

//不得不给接口，外界需要在内部规章下办事，不能随便运用
public interface ITreePeak {
    //如果是顶峰，需要继承该接口，同时也继承中心接口，都要继承，随后的中心都只需继承中心接口即可
    void SelfReady();
}