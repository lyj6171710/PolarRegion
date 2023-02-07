using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//内部处理=======================================================================

public class LocalBranch : ILocalBranch
{//包装内部机制，外界不知道这个类的存在或作用
    GameObject mSelfIn;//该下级个体自己所属物体
    MonoBehaviour mSelf;//该分支负责逻辑承接的那部分
    
    ILocalBranch mBranchSelf;//下级需要有的标志
    LocalJoint mJoint;//作为上级一个部门的事物，可能同时也是某一个局部的中心，这里获取到而可以方便控制
    
    bool mHaveSelfReady;//该局部框架对应实际组分的准备工作

    public LocalBranch(MonoBehaviour sub, ILocalBranch iBranch)
    {
        mSelfIn = sub.gameObject;
        mSelf = sub;
        mBranchSelf = iBranch;
    }
    
    public void MakeReadyNer()//该框架自身的准备工作，主要是对接
    {//会由直属上级对自己初始化后调用

        mHaveSelfReady = false;

        ILocalFocus status = mSelfIn.GetComponent<ILocalFocus>();
        if (status != null)
        {
            //子级自身可能同时也携带一个focus，也需要被顺带初始化，系统自己可以做，外界不用管了
            //注意，就算自己是一个小focus，父级自身携带有的focus可能也只是一个更大体系的局部中心
            mJoint = mSelfIn.AddComponent<LocalJoint>();
            mJoint.NotifyReadyNer();//驱动其准备工作的完成
            //分支同时承载局部中心效果时，一定不会是顶峰
            mJoint.mFocusBranch = this;
            //利用当前已知信息
        }
        else
            mJoint = null;
    }

    public void MakeSelfReadyNer()//游戏内容方面的初始化
    {
        SelfReady();//内容初始化
    }

    public LocalFocus GetSelfReadyNer()
    {
        return mJoint;
    }

    //功能扩展=================================================

    public bool isActive { get { return mSelfIn.activeSelf; } }

    public GameObject meSelfIn { get { return mSelfIn; } }
    
    //上级可用（有系统内部逻辑的加持或约束）-----------------------------------

    public void Open()
    {
        if (!mSelfIn.activeSelf)
        {
            mSelfIn.SetActive(true);
            mBranchSelf.Open();
            //中心开启时，子级不一定需要随之开启
        }
    }

    public void Close()
    {
        if (mSelfIn.activeSelf)
        {
            if (mJoint != null)
                //中心关闭时，子级一定随之被关闭
                mJoint.TurnActiveAllBranch(false);
            mBranchSelf.Close();
            mSelfIn.SetActive(false);
        }
    }

    public void SelfReady()//游戏逻辑内容上的初始化
    {
        if (!mHaveSelfReady)
        {
            mBranchSelf.SelfReady();
            mHaveSelfReady = true;
        }
    }
    
    public InfoSeal Refresh(InfoSeal seal)
    {
        return mBranchSelf.Refresh(seal);
    }
    
    //--------------------------------

    //特殊交流

    public MonoBehaviour GetSelf()
    {
        return mSelf;
    }

    public T To<T>() where T : MonoBehaviour
    {
        return mSelf as T;
    }
}

public class LocalSib
{
    LocalBranch mBranchCorr;
    public LocalSib(LocalBranch branch) { mBranchCorr = branch; }

    //同级可用=====================================

    public bool isActive { get { return mBranchCorr.isActive; } }

    //应仅限特殊操作，同级之间还是不能权限给得太多==================

    public T To<T>() where T : MonoBehaviour
    {
        return mBranchCorr.To<T>();
    }
    
}

public class LocalSibs
{
    LocalFocus mSuperior;//对兄弟姐妹的引用
    public LocalSib this[Enum sign] { get { return mSuperior.GetSibNer(sign); } }

    public LocalSibs(LocalFocus superior)
    {
        mSuperior = superior;
    }
}