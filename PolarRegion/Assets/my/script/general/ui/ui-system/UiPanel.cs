using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EStateSubIn { Use, Down, Open, Close }

public class UiPanel : MonoBehaviour
{
    //特性是，面板可以嵌套面板，每个面板唯一拥有若干个特定的选表

    //面板，支持向上回溯，支持同级切换，支持嵌套内进
    //面板对其直接管辖的选表，支持开闭
    //面板主动支持的交互类行为，都由直辖选表驱动进行
    //面板的交互内容由其管辖的选表填充，面板可以没有选表，但也就没有可交互的元素
    //可以认为，一个面板由若干个选表组成，然后可以向上、向下、向左、向右开闭其它面板

    //面板刚被打开时，有面板自己的启动工作
    //面板被打开时，可能是因为另外选表，也可能是因为外界要求
    //如果是因为选表，那么按选表要求做启动工作，如果因为外界，那么按默认值

    string division;

    //==============================

    public bool meInUse => mInUse;
    public bool meInDown => gameObject.activeSelf && !mInUse;
    public bool meInOpen => gameObject.activeSelf;
    
    [HideInInspector]public bool mAsMainPanel;//是最高层级的面板，不附属于任何面板

    bool meHaveList => mListMager != null;//有无直辖选表
    bool meHaveSubInOpen => mCountSubInOpen != 0;

    [HideInInspector]public UiPanel mSuper;
    internal UiListsCtrl mListMager;//单独管理直辖选表，选表没有嵌套性
    bool mInUse;
    bool mHaveReady;
    int mCountSubInOpen;

    void MakeReady()
    {
        ReadySub();
        mHaveReady = true;
    }//没有被用到时，还不会进行准备(提高启动软件的速度)

    void MakeUse()
    {
        if (meHaveList) mListMager.FollowUse();
        mInUse = true;
    }//确保面板处于open状态后，才能调用执行该方法
    //子面板开启时，其父级面板一定也需处于开启状态，子面板依赖父级面板

    void MakeDown()
    {
        //一定伴随子级的停用
        mSubPanels.ForEachCanModify((sub)=> SureGetDown(sub));
        if (meHaveList) mListMager.FollowDown();
        mInUse = false;
    }

    //---------------------------------

    internal void MakeCome(UiPanel which)
    {
        if (mAsMainPanel || (which == mSuper && mSuper.meInOpen))
        { //除了主面板，只支持由父级来使用，而父级调用也需要父级处于开启状态
            if (!mHaveReady) MakeReady();
            if (!meInOpen) gameObject.SetActive(true);
            if (!mInUse) MakeUse();
        }
    }
    //是否调用取决于用户需要或选表需要
    //选表可以启动，任何当前开启状态面板下的任一面板
    //选表启动的前提是选表是开启状态，
    //选表开启状态是其所属面板是开启状态，
    //面板是开启状态的前提是其父级面板是开启状态
    //用户可以启动面板的前提也是同理的，开启状态的选区才能被用户看见与操作

    internal void MakeLeave()
    {
        if (mInUse) MakeDown();
        if (meInOpen) gameObject.SetActive(false);
    }

    //===================================

    Dictionary<UiPanel, EStateSubIn> mSubPanels;

    internal void TakeLeaveToSuper()
    {
        MakeLeave();
        if (!mSuper.meHaveSubInOpen)
            mListMager.FollowContinue();//选表行为继续
    }

    internal void TakeFoward(UiPanel sub)
    {
        if (SureGetUse(sub))
            mListMager.FollowPause();//选表行为暂停
    }

    internal void TakeSideComeInUp(UiPanel partner)
    {
        UiPanel partnerSuper = Method.ReverseFindInRecur(this,
            (one) => one.mSuper,//这里认为父级一定处于开启状态
            (one) => one.NuEnumSubPanelInOpen(),//遍历所有当前打开状态的面板
            (one) =>
            {
                if (one.mSubPanels.ContainsKey(partner))
                {
                    if (one.mSubPanels[partner] != EStateSubIn.Use)
                        return true;//找到了
                }
                return false;
            });
            partnerSuper.TakeFoward(partner);
    }

    //================================

    IEnumerable<UiPanel> NuEnumSubPanelInOpen()
    {
        var iter = mSubPanels.Keys.GetEnumerator();
        while (iter.MoveNext())
        {
            if (!iter.Current.meInOpen)
                yield return iter.Current;
        }
        yield break;
    }

    bool SureGetUse(UiPanel sub)
    {
        if (mSubPanels[sub] != EStateSubIn.Use)
        {
            if (!sub.meInOpen) mCountSubInOpen++;
            sub.MakeCome(this);
            mSubPanels[sub] = EStateSubIn.Use;
            return true;
        }
        else return false;
    }

    bool SureGetDown(UiPanel sub)
    {
        if (mSubPanels[sub] == EStateSubIn.Use)
        {
            sub.MakeDown();
            mSubPanels[sub] = EStateSubIn.Down;
            return true;
        }
        else return false;
    }

    bool SureGetClose(UiPanel sub)
    {
        if (mSubPanels[sub] != EStateSubIn.Close)
        {
            sub.MakeLeave();
            mSubPanels[sub] = EStateSubIn.Close;
            mCountSubInOpen--;
            return true;
        }
        else return false;
    }

    void ReadySub()
    {
        mListMager = GetComponent<UiListsCtrl>();
        mSubPanels = new Dictionary<UiPanel, EStateSubIn>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            UiPanel panel = child.GetComponent<UiPanel>();
            if (panel != null)
            {
                panel.mSuper = this;
                panel.gameObject.SetActive(false);//这个是必要的，而且不使用leave
                mSubPanels.Add(panel, EStateSubIn.Close);
            }
            else
            {//顺便记录存在有的list
                UiList list = child.GetComponent<UiList>();
                if (list != null)
                {
                    list.mSuper = this;
                    mListMager.FollowAcceptOneSubList(list);
                }
            }
        }
    }

    //======================================

    internal bool SuWhetherHave(UiPanel sub)
    {
        return mSubPanels.ContainsKey(sub);
    }

}

