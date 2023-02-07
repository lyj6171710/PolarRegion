using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class UiListsCtrl : MonoBehaviour
{
    //本质也是UiPanel的功能，拆离出来单独弄而已

    //一个面板下可以同时存在多个选表，并且可以分别操作各个选表

    //一个面板的子面板处于开启状态时，子面板的直属父级所含选表将不能被操作
    //想要被操作也不是不行，因为只是直属父级所含选表不能被操作，
    //该父级包含的子面板的所持选表默认仍然可以被操作，
    //因此可以通过设计为非回溯路线上的选表，来间接实现需求

    public UiPanel mCtrlDp;
    public UiList mStartListDp;

    UiList mListFocus;
    UiList mListLastFocus;
    Dictionary<UiList, EStateSubIn> mSubLists;

    bool mInPause;

    internal void FollowAcceptOneSubList(UiList one)
    {
        if (mSubLists == null)
            mSubLists = new Dictionary<UiList, EStateSubIn>();
        one.gameObject.SetActive(false);
        mSubLists.Add(one, EStateSubIn.Close);
    }

    internal void FollowUse()
    {
        if (mStartListDp != null)
        {
            mListFocus = mStartListDp;
            mListFocus.MakeCome();
        }
    }

    internal void FollowDown()
    {
        mListFocus.MakeDown();
        mListLastFocus = mListFocus;
    }

    internal void FollowPause()
    {
        if (!mInPause) ;
    }

    internal void FollowContinue()
    {
        
    }

    //=================================

    internal void TakeDown()
    { }

    internal void TakeLeave()
    { }

    internal void TakeCome(UiPanel to)
    {

    }//打开某个面板，不能是当前未开启面板的子面板

    internal void TakeCome(UiList to)
    {

    }

    //=================================

    internal bool SuWhetherHave(UiList to)
    {
        return mSubLists.ContainsKey(to);
    }

}
