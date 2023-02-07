using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//适配一种从上到下，不断发散的架构
//适用于各个中心机构总存在，但可能或开或闭，并且上级依赖下级，推迟到下级实现
//该架构，用来方便控制与运用相互关系

//使用时，需要符合父子关系，也就是说，如果该物件是另一物件的上级，那么该物件一定也是这另一物件的父级(层级相邻)
//外界只需知道LocalPeak、几个接口及对外说明的机制规则，即可运用了
//首先安置LocalPeak组件，最顶层的中心部门才需要安装，且需要它保持开启状态
//各种游戏类继承接口，填写方法；关于分支，外界使用枚举区分，框架内部以整型区分

public class LocalFocus : MonoBehaviour {//核心组件

    public enum Kind { peak, joint }

    //外界可操作==============

    public LocalBranch this[Enum sign] { get { int index = sign.GetHashCode(); return mDicBranchNer[index]; } }

    //私用变量=============================

    protected List<LocalPart> mListPart;//提前预置者，适合那些稳定存在的事物

    public Dictionary<int, LocalBranch> mDicBranchNer;//会将数组转变为字典结构，加快速度

    protected Dictionary<int, LocalSib> mDicSib;//下级为寻找其同级事物，统一使用的字典
    
    protected ILocalFocus mFocusSelf;//具体实现该中心部门相关游戏逻辑内容的组件

    [HideInInspector] public Kind mKindNer;

    //------------------------------------------
    
    protected void MakeReadyNer(Kind kind)//在框架本身方面，自己及下级的准备工作
    {
        mKindNer = kind;//由子类告知自己所在的特殊类型，做一下记录，便于利用

        mDicBranchNer = new Dictionary<int, LocalBranch>();
        mDicSib = new Dictionary<int, LocalSib>();

        mFocusSelf = GetComponent<ILocalFocus>();
        if (mFocusSelf == null)
        {
            Debug.LogError("无中心负责人");
        }
        else {
            mListPart = mFocusSelf.GetBranch(this);
            if (mListPart == null)
            {
                Debug.LogError("无需要负责的东西");
            }
            else if (mListPart.Count > 0)
            {
                ConvertToDic();//先得准备全数据
                MakePartsReady();//要顺带把各子级初始化工作也给做了
            }
        }
    }
    
    void ConvertToDic()
    {
        foreach (LocalPart part in mListPart)//填充下级字典
        {
            if (part.sub == null)
                Debug.LogError("分支物不存在");
            else
            {
                ILocalBranch iBranch = part.sub as ILocalBranch;//提取接口
                if (iBranch == null)
                    Debug.LogError("分支物并没能负责分支工作");
                else
                {
                    LocalBranch branch = new LocalBranch(part.sub, iBranch);//数据整合
                    mDicBranchNer.Add(part.index, branch);//如果有同名，会直接报错
                }
            }
        }

        foreach (int index in mDicBranchNer.Keys)//填充同级字典
        {
            LocalSib sib = new LocalSib(mDicBranchNer[index]);
            mDicSib.Add(index, sib);
        }
    }
    
    void MakePartsReady()
    {
        foreach (LocalPart part in mListPart)
        {
            ILocalBack iBack = part.sub as ILocalBack;//检查一下是否需要回溯
            if (iBack != null) iBack.Leader = this;//有则给予

            ILocalSib iSib = part.sub as ILocalSib;
            if (iSib != null) iSib.Sibs = new LocalSibs(this);
        }

        foreach (LocalBranch branch in mDicBranchNer.Values)
            branch.MakeReadyNer();//子级自身在该系统方面的初始化
    }
    
    public LocalSib GetSibNer(Enum sign)
    {
        int index = sign.GetHashCode();
        return mDicSib[index];
    }
    
    //本级可用===================================================
    
    public void TurnActiveAllBranch(bool onoff) {//不管是顶端还是分支中心，都是一个效果
        foreach (LocalBranch branch in mDicBranchNer.Values)
        {
            if (onoff)
                branch.Open();
            else
                branch.Close();
        }
    }

    public virtual void TurnSelfActive(bool show) { }//随是否是顶端的不同而不同
    
    public bool SelfActive { get { return gameObject.activeSelf; } }

    //下级可用（在下级个体需要了解与它同层级的另外物体，相对上级的状态时）=====================
    
    public T To<T>() where T:class {
        return mFocusSelf as T;
    }

    public void Notify(InfoSeal seal)
    {
        mFocusSelf.HearOf(seal);
    }
}

public struct LocalPart//帮助赋值用的
{
    public int index;//对应标记
    public MonoBehaviour sub;

    public LocalPart(Enum sign, MonoBehaviour sub)
    {
        index = sign.GetHashCode();
        this.sub = sub;
    }
}