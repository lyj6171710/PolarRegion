using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPeak : LocalFocus//只有处于顶峰的中心，才能下发总任务
{
    //外界只使用LocalFocus即可，peak和joint只是为了框架内部区分自身内容成分用的

    ILocalPeak mPeak;

    void Awake()
    {
        mPeak = GetComponent<ILocalPeak>();
        if (mPeak != null)
        {
            MakeReadyNer(Kind.peak);
            //能执行到这里，各分支在框架本身方面，全部已经准备好了才行
            mPeak.SelfReady();//顶峰自己的准备工作
            MakeSubReady();//应在这里再做一系列具体的游戏内容上的初始准备
        }
    }
    
    void MakeSubReady()//在运用框架的游戏内容方面，自己及下级的准备工作
    {
        Stack<LocalFocus> branchFocusReady = new Stack<LocalFocus>();

        Queue<LocalFocus> branchFocusCollect = new Queue<LocalFocus>();
        branchFocusCollect.Enqueue(this);//最高级对它第一层子级的操作
        while (branchFocusCollect.Count > 0)//准备工作的执行次序上，按广度优先方式，确保父级对子级的可用
        {
            LocalFocus focusPick = branchFocusCollect.Dequeue();
            if (focusPick != null)
            {
                branchFocusReady.Push(focusPick);//先进后出，最上一级的最后做准备
                List<LocalFocus> focusNeed = GetSubs(focusPick);
                //从上到下，按层级依次收纳
                foreach (LocalFocus need in focusNeed)
                {
                    if (need != null)
                    {
                        branchFocusCollect.Enqueue(need);
                    }
                }
            }
        }

        while (branchFocusReady.Count > 0)//从下到上准备，越下层越待定未知，因此应优先做好准备，上级才好做后续安排
        {
            LocalFocus focusPick = branchFocusReady.Pop();
            foreach (LocalBranch branch in focusPick.mDicBranchNer.Values)
            {
                branch.meSelfIn.SetActive(true);//暂时强制显化，得以触发关闭流程的执行
                //Debug.Log(branch.meSelfIn.name);
                branch.MakeSelfReadyNer();
            }
        }

        TurnActiveAllBranch(false);//使得等待发配安排，保留唯一存活的顶层，从它开始运行
    }

    List<LocalFocus> GetSubs(LocalFocus focus)
    {
        List<LocalFocus> branchFocus = new List<LocalFocus>();

        foreach (LocalBranch branch in focus.mDicBranchNer.Values)
        {
            LocalFocus need = branch.GetSelfReadyNer();
            if (need != null)
            {
                branchFocus.Add(need);
            }
        }

        return branchFocus;
    }

    public override void TurnSelfActive(bool show)
    {
        //自己需要保持开启，因此自己不会受该函数影响
        TurnActiveAllBranch(show);//仅影响子级状态
    }

}
