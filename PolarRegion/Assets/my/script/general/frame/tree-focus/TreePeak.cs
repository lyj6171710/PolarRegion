using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreePeak : TreeFocus//只有处于顶峰的中心，才能下发总任务
{
    //外界主要使用Focus即可，peak和joint是为了框架内部区分自身内容成分用的
    
    ITreePeak mPeak;

    public Dictionary<int,TreeBranch> mListAllSub;//顺便收集起所有分支点

    void Awake()
    {
        mPeak = GetComponent<ITreePeak>();
        if (mPeak != null)
        {
            MakeReady(Kind.peak, this, null);
            //能执行到这里，各分支在框架本身方面，全部已经准备好了才行
            //开始做一系列的游戏内容上的具体准备
            mPeak.SelfReady();//顶峰自己的准备工作
            mListAllSub = new Dictionary<int, TreeBranch>();
            MakeSubReady();//子级的准备工作
        }
    }
    
    void MakeSubReady()//在运用框架的游戏内容方面，自己及下级的准备工作
    {
        Queue<TreeFocus> branchFocusReady = new Queue<TreeFocus>();

        Queue<TreeFocus> branchFocusCollect = new Queue<TreeFocus>();
        branchFocusCollect.Enqueue(this);
        while (branchFocusCollect.Count > 0)//先搜集齐所有部门，这样每个部门自己的准备工作更好做
        {
            TreeFocus focusPick = branchFocusCollect.Dequeue();
            if (focusPick != null)
            {
                branchFocusReady.Enqueue(focusPick);//先进先出，最上一级的最先做准备
                List<TreeFocus> focusNeed = GetSubs(focusPick);//从上到下，按层级依次收纳
                foreach (TreeFocus need in focusNeed)
                {
                    branchFocusCollect.Enqueue(need);
                }
            }
        }

        while (branchFocusReady.Count > 0)//准备工作的执行次序上，按广度优先方式，确保父级对子级可用
        {
            TreeFocus focusPick = branchFocusReady.Dequeue();
            foreach (TreeBranch branch in focusPick.mDicBranch.Values)
            {
                branch.SelfReady();
            }
        }
    }

    List<TreeFocus> GetSubs(TreeFocus focus)
    {
        List<TreeFocus> branchFocus = new List<TreeFocus>();

        foreach (TreeBranch branch in focus.mDicBranch.Values)
        {
            TreeFocus need;
            if (branch.GetJoint(out need))
            {
                branchFocus.Add(need);
            }
            mListAllSub.Add(branch.mId, branch);//顺便收集
        }

        return branchFocus;
    }
}
