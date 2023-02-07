using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//内部处理=======================================================================

public class TreeBranch
{//主要用于与上级联系

    GameObject mSelfIn;//该下级个体自己所属物体
    MonoBehaviour mSelf;//该分支负责逻辑承接的那部分
    
    ITreeBranch mSelfBranch;//下级需要有的标志
    
    public int mId;//分支相对所在树的唯一标识
    public TreeJoint mSelfJoint;//作为上级一个部门的事物，可能同时也是某一个局部的中心，这里获取到而可以方便控制
    TreeFocus mLeader;//最近的父级
    public TreePeak meSuper;//最高级

    bool mHaveSelfReady;//该局部框架对应实际组分的准备工作

    public TreeBranch(int id, MonoBehaviour sub, ITreeBranch iBranch)
    {
        mId = id;
        mSelfIn = sub.gameObject;
        mSelf = sub;
        mSelfBranch = iBranch;
    }

    public void MakeReady(TreeFocus leader, TreePeak super)//该框架自身的准备工作，主要是对接
    {//会由直属上级对自己初始化后调用
        
        mLeader = leader;
        meSuper = super;

        mHaveSelfReady = false;

        ITreeFocus status = mSelfIn.GetComponent<ITreeFocus>();
        if (status != null)
        {
            //子级自身可能同时也携带一个focus，也需要被顺带初始化，系统自己可以做，外界不用管了
            //注意，就算自己是一个小focus，父级自身携带有的focus可能也只是一个更大体系的局部中心
            mSelfJoint = mSelfIn.AddComponent<TreeJoint>();
            mSelfJoint.MakeReady(TreeFocus.Kind.joint, meSuper, this);//驱动其准备工作的完成
            //分支同时承载局部中心效果时，一定不会是顶峰
            mSelfJoint.mFocusBranch = this;
            //利用当前已知信息
        }
        else
            mSelfJoint = null;
    }

    public bool GetJoint(out TreeFocus corr)
    {
        corr = mSelfJoint;
        if (mSelfJoint == null) return false;
        else return true;
    }

    public void SelfReady()//游戏逻辑内容上的初始化
    {
        if (!mHaveSelfReady)
        {
            mSelfBranch.SelfReady(this);
            mHaveSelfReady = true;
        }
    }

    //-------------------------------------

    public void RespondNotify(InfoSeal seal)
    {
        mSelfBranch.ReceiveNotifyFromUp(seal);
    }

    public object RespondRead(InfoSeal seal,out int source)
    {
        return mSelfBranch.RespondReadFromUp(seal,out source);
    }

    //上下级使用===============================

    public T Su<T>() where T : MonoBehaviour//很特殊局部的逻辑，直接用这个相互交流，不再需要通过框架交流
    {
        return mSelfBranch as T;
    }

    //本级可用=================================

    public object SuRequestToUp(InfoSeal seal)//直到有机构回应自己
    {
        object feedback = mLeader.RespondRequest(seal);
        if (feedback == null)
        {
            if (mLeader.mKind == TreeFocus.Kind.joint)
                return mLeader.mSelfJoint.mFocusBranch.SuRequestToUp(seal);
            else
                return mLeader.RespondRequest(seal);
        }
        else
        {
            return feedback;
        }
    }

    public void SuNotifyToUp(InfoSeal seal)//向上传达消息
    {
        if (!mLeader.RespondNotify(seal))
        {
            if (mLeader.mKind == TreeFocus.Kind.joint)
                mLeader.mSelfJoint.mFocusBranch.SuNotifyToUp(seal);
            else
                mLeader.RespondNotify(seal);
        }
    }

    public T SuFind<T>(Enum sign) where T : MonoBehaviour//建议外界不频繁使用时，用这个来得到引用
    {
        return meSuper.mListAllSub[sign.GetHashCode()].mSelf as T;
    }

    public T SuLeader<T>() where T : MonoBehaviour
    {
        return mLeader.Su<T>();
    }
}