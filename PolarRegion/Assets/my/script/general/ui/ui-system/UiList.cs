using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class UiList : MonoBehaviour, AssistSelect.IUse
{
    //选表提供的，有选择、重置、关闭、打开另外选表等支持交互行为的功能
    //被操作后的游戏程序反应，取决于外界怎么反应，和选表本身无关

    //选表不嵌套，选表不能包含选表或面板，
    //因为一个面板下，不会有多少选表，不如直接并列，便于获取
    //而且也不是就没有嵌套功能，面板能嵌套，面板可以包含选表

    //==========================

    [HideInInspector]public UiPanel mSuper;//该选表所属面板

    public bool meInOpen => gameObject.activeSelf;
    public bool meInDown => gameObject.activeSelf && !mInUse;
    public bool meInUse => mInUse;
    
    bool mInUse;
    bool mInPause;
    bool mHaveReady;

    public void MakeReady()
    {
        ReadyBox();
        mHaveReady = true;
    }

    public void MakeUse()
    {
        StartSelect();
        mInUse = true;
    }

    public void MakePause()
    {
        mInPause = true;
        mSelect.MakePause();
    }

    public void MakeContinue()
    {
        mSelect.MakeContinue();
        mInPause = false;
    }

    public void MakeDown()
    {
        mSelect.MakeDown();
        mInUse = false;
    }//可以被外界牵连关闭

    //----------------------

    public void MakeCome()
    {
        if (!mHaveReady) MakeReady();
        if (!meInOpen) gameObject.SetActive(true);
        if (!mInUse) MakeUse();
        
    }//被某个面板或选表需要

    public void MakeLeave()
    {
        if (mInUse) MakeDown();
        if (meInOpen) gameObject.SetActive(false);
    }

    //================================

    public void TakeDown()
    {
        MakeDown();
    }

    public void TakeLeave()
    {
        MakeLeave();
    }

    public void TakeComeIncUp(UiPanel to)
    {
        if (mSuper.SuWhetherHave(to))
            mSuper.TakeFoward(to);
        else
            mSuper.TakeSideComeInUp(to);
    }//打开某个面板，不能是当前未开启面板的子面板

    public void TakeComeIncUp(UiList to)
    {
        if (mSuper.mListMager.SuWhetherHave(to))
            mSuper.mListMager.TakeCome(to);
        else
        {
            TakeComeIncUp(to.mSuper);
            if (to.mSuper.meInOpen)
                to.mSuper.mListMager.TakeCome(to);
        }
    }

    //====================================

    public bool mRememberSelectDp;
    [Required]public UiOption mStartOptionDp;
    [Required]public List<UiOption> mOptionsDp;

    AssistSelect mSelect;

    List<GameObject> mOptions;

    void ReadyBox()
    {
        mSelect = gameObject.AddComponent<AssistSelect>();
        mSelect.MakeReady(this, EToward4.up);
        mSelect.SuNotifyGoAboveList += () => { };//先占个位，待扩充
        mSelect.SuNotifyGoBelowList += () => { };

        mOptions = new List<GameObject>();
        for (int i = 0; i < mOptionsDp.Count; i++)
            mOptions.Add(mOptionsDp[i].gameObject);
    }

    void StartSelect()//激活选择功能
    {
        if (mRememberSelectDp && mOptionsDp[mSelect.meSelectCur])//检查是否需且能有回往
            mSelect.MakeUse(mOptionsDp[mSelect.meSelectCur].mIndex, mOptions, null);
        else if (mStartOptionDp) //检查是否能着陆
            mSelect.MakeUse(mStartOptionDp.mIndex, mOptions, null);
    }

    //=======================================

    public bool mCanBackDp;
    public UnityEvent mCallWhenBackDp;//从该选表回退时会激发的事件

    public bool mLeaveWhenBackDp;
    public UiList mOnlyDvtCtrlToWhenBackDp;
    public UiPanel mMakePanelLeaveWhenBackDp;

    public bool mBackFmPanelDp;//代表所属面板响应返回操作

    private void Update()
    {
        if (mCanBackDp && UnifiedInput.It.meWhenBack())
            TurnBack();
    }

    void TurnBack()
    {
        if (mCanBackDp)//先看能否跳转
        {
            if (mCallWhenBackDp != null)//看有无事务需要处理
                mCallWhenBackDp.Invoke();
            if (mSuper != null)//再看有无跳转目标，没有就不跳转
            {
                //mLastSelect = mOptionsDp[mSelect.meSelectCur];
                //mSuperDp.MakeDivert(this, mDivertToListDp);
            }
        }

        //if (mLeaveWhenBackDp)
        //    MakeLeave();
        //else if (mDownWhenBackDp)
        //    MakeDown();
        //if (mDivertToWhenBackDp != null && mSuper != null)
        //    mSuper.TakeDivert(this, mDivertToWhenBackDp);
    }

    //=========================================

    [ContextMenu("自动搜集可选选项")]
    void CollectOptionsHave()
    {//思想：搜集选项的过程是依次收录特殊子物体的过程
     //由此，用户可以通过控制选项所处的子物体序数而控制被收录到的位置
        mOptionsDp.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            var optionOne = transform.GetChild(i).GetComponent<UiOption>();
            if (optionOne!=null)
            {//如果该物体有特定的选项组件
                optionOne.mSuper = this;//顺便标记其选表
                optionOne.mIndex = i;
                mOptionsDp.Add(optionOne);
            }
        }
    }

    public void iInformConfirmResult(int index)
    {
        mOptionsDp[index].MakeSure();
    }

    public void iInformBackNeed()
    {
        //选表自己不负责识别返回
    }
}
