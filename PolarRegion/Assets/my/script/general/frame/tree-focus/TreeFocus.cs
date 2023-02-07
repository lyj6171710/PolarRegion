using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TreeFocus : MonoBehaviour {//核心组件
    //与LocalFocus的区别是，该框架适用于各个中心都必定开启并维持的状态，并且下级依赖上级

    //主要用于与下级联系

    public enum Kind { peak, joint }

    //外界可操作==============

    public TreeBranch this[Enum sign] { get { int index = sign.GetHashCode(); return mDicBranch[index]; } }

    //私用变量==========================

    List<TreePart> mListPart;//提前预置者，适合那些稳定存在的事物

    [HideInInspector] public Dictionary<int,TreeBranch> mDicBranch;//会将数组转变为字典结构，加快速度与外界自由度

    //---------------------------------------

    public Kind mKind { get; set; }
    [HideInInspector] public TreeJoint mSelfJoint;
    ITreeFocus mSelfFocus;//具体实现该中心部门相关游戏逻辑内容的组件
    TreePeak mSuper;

    //------------------------------------------

    public void MakeReady(Kind kind, TreePeak super, TreeBranch focusBranch)//在框架本身方面，自己及下级的准备工作
    {
        mKind = kind;//由子类告知自己所在的特殊类型，做一下记录，便于利用
        mSuper = super;
        if (kind == Kind.joint)
        {
            mSelfJoint = this as TreeJoint;
            mSelfJoint.mFocusBranch = focusBranch;
        }

        mDicBranch = new Dictionary<int, TreeBranch>();

        mSelfFocus = GetComponent<ITreeFocus>();
        if (mSelfFocus == null)
        {
            Debug.LogError("无中心负责人");
        }
        else {
            mListPart = mSelfFocus.GetParts(this);
            if (mListPart == null)
            {
                Debug.LogError("无需要负责的东西");
            }
            else if (mListPart.Count > 0)
            {
                ConvertToDic();//先得准备全数据

                for (int i = 0; i < mListPart.Count; i++)//要顺带把各子级初始化工作也给做了
                {
                    mListPart[i].Branch.MakeReady(this, mSuper);//子级自身在该系统方面的初始化
                }
            }
        }
    }
    
    void ConvertToDic()
    {
        for (int i = 0; i < mListPart.Count; i++)//填充下级字典
        {
            TreePart part = mListPart[i];
            if (part.sub == null)
                Debug.LogError("分支物不存在");
            else
            {
                ITreeBranch iBranch = part.sub as ITreeBranch;//提取接口
                if (iBranch == null)
                    Debug.LogError("分支物并没能负责分支工作");
                else
                {
                    TreeBranch branch = new TreeBranch(part.id, part.sub, iBranch);//数据整合
                    part.Branch = branch;
                    mDicBranch.Add(part.id, branch);//如果有同名，会直接报错
                    //外界需确保依次添加的元素，所含枚举也是从0到n的顺序依次来的，否则基于枚举从该组件会取得错误结果
                    //其实外界可以不按序，系统也有办法有序化，不过没必要
                }
            }
        }
    }
    
    //-----------------------------

    public object RespondRequest(InfoSeal seal)
    {
        return mSelfFocus.RespondRequestFromDown(seal);
    }

    public bool RespondNotify(InfoSeal seal)
    {
        return mSelfFocus.HearOfDown(seal);
    }

    //本级自己用============================

    public void SuNotifyToDown(InfoSeal seal)
    {
        foreach (TreeBranch branch in mDicBranch.Values)
        {
            branch.RespondNotify(seal);
            TreeJoint joint = branch.mSelfJoint;
            if (joint != null) joint.SuNotifyToDown(seal);
        }
    }

    //----------------------------

    public object SuReadFromDown(InfoSeal seal,Enum sign)
    {//sign指引查询路线
        return SuReadFromDown(seal, sign.GetHashCode());
    }

    object SuReadFromDown(InfoSeal seal, int sign)
    {
        int nextSign;
        object feedback = mDicBranch[sign].RespondRead(seal, out nextSign);
        if (feedback == null)
        {
            return mDicBranch[sign].mSelfJoint.SuReadFromDown(seal, nextSign);
        }
        else
        {
            return feedback;
        }
    }

    //上级或下级使用==========================

    public T Su<T>() where T : MonoBehaviour
    {
        return mSelfFocus as T;
    }
}

public class TreePart//帮助赋值用的
{
    public int id;//对应标记
    public MonoBehaviour sub;
    TreeBranch branch;

    public TreePart(Enum sign, MonoBehaviour sub)
    {
        id = sign.GetHashCode();
        this.sub = sub;
    }

    public TreeBranch Branch { get { return branch; }set { branch = value; } }
}