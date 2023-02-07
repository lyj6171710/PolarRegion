using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class AssistSelect : MonoBehaviour
{//用以选择列表选项的工具
 //每一个菜单生成该类的实例，并作为成员来使用
    public interface IUse
    {
        public void iInformConfirmResult(int index);

        public void iInformBackNeed();
    }

    public bool meCustomInput;//自定义输入的层级低于能否输入，能输入的前提下才能生效
    public bool meBanSelect;

    //委托是为了可选化
    public Action SuNotifySelectByAction;
    public Action<int> SuNotifySelectByCasual;
    public Action SuNotifySelectAny;
    public Action SuNotifyConfirmByAction;
    public Action<int> SuNotifyConfirmByCasual;
    public Action SuNotifyConfirmAny;
    public Action SuNotifyGoAboveList;
    public Action SuNotifyGoBelowList;

    public int meSelectCur => mSelectCur;
    public int meSelectLast => mSelectLast;
    public bool meInPause => mInPause;

    //=================================

    List<AssistOption> mOptionsCanList;//选项可选性
    List<AssistOption> mOptionsHaveList;//选项序数的来源，由需要选择时，所供给的选项列表的状态决定
    List<bool> mOptionsInvalid;//与CanList同步

    int mNumOption;
    IUse mUser;
    EToward4 mDirNext;
    RectTransform selector;//选框

    int mSelectLast;
    int mSelectCur;
    bool mInPause;//和禁止选择不一样，这里是一种可选状态
    int mTmpPartIn;

    MethodRangeAccess mFocusLockCtrl;
    static bool mFocusLock = false;
    static AssistSelect mInFocus;
    //如果是由光标选择，焦点不在当前选择列表上，就不会发生确定行为
    //一个情境中，可以同时出现多个不同选区各自需要选择的情况

    public void MakeReady(IUse user, EToward4 selectDir)
    {
        mUser = user;
        mDirNext = selectDir;

        selector = UiMager.It.meAlone.SuUse(EChartAlone.selector).GetComponent<RectTransform>();
        selector.gameObject.SetActive(false);

        mOptionsCanList = new List<AssistOption>();
        mOptionsInvalid = new List<bool>();
        mOptionsHaveList = new List<AssistOption>();

        SuNotifySelectByAction += () => { };
        SuNotifySelectByCasual += (i) => { };
        SuNotifySelectAny += () => { };
        SuNotifyConfirmByAction += () => { };
        SuNotifyConfirmByCasual += (i) => { };
        SuNotifyConfirmAny += () => { };
        SuNotifyGoAboveList += () => { };
        SuNotifyGoBelowList += () => { };

        mFocusLockCtrl = new MethodRangeAccess();
    }

    public void MakeUse(int startIndex, List<GameObject> selectAble, int[] tmpBans)
    {
        if (!mInPause) MakePause();
        ResetMemberVar();
        AcceptNewSelect(selectAble, tmpBans);
        MakeContinue();

        ShiftSelect(startIndex);
        UpdateHover();
        SuNotifySelectByAction();

        AlterFocusOnSelf();
    }

    public void MakeDown()
    {
        for (int i = 0; i < mOptionsHaveList.Count; i++)
        {
            mOptionsHaveList[i].MakeDown();
            Destroy(mOptionsHaveList[i]);
        }
        mOptionsHaveList = null;
        mOptionsCanList = null;
        mUser = null;

        Destroy(selector.gameObject);
    }//结束使用

    public void MakePause()
    {
        mInPause = true;
        selector.gameObject.SetActive(false);
        for (int i = 0; i < mOptionsCanList.Count; i++)
        {
            mOptionsCanList[i].MakeDown();
        }
        UnifiedInput.It.NormalizeArea(this);
    }//暂停使用

    public void MakeContinue()
    {
        mInPause = false;
        selector.gameObject.SetActive(true);
        for (int i = 0; i < mOptionsCanList.Count; i++)
        {
            mOptionsCanList[i].MakeUse(this, i);
        }
    }//中途可能暂停选择

    //=================================

    public void SuSelectTo(int to, bool byCasual)
    {
        if (CanSelect())
        {
            int change = ShiftSelect(to).change;
            if (change > 0)
            {
                UpdateHover();
                LimitCursorCanConfirm();
                SuNotifySelectAny();
                if (byCasual)
                    SuNotifySelectByCasual(mTmpPartIn);
                else
                    SuNotifySelectByAction();
            }
            else
            {
                if (change < 0)
                    SelectToEdge(to);
            }
        }
    }
    //已经处于选择状态时，不管是外界还是内部，都可且应使用该接口，调整所选选项

    public void NuLockFocus(MonoBehaviour asker)
    {
        if (mFocusLockCtrl.RequestUse(asker))
            mFocusLock = true;
    }

    public void NuUnlockFocus(MonoBehaviour asker)
    {
        if (mFocusLockCtrl.LeaveUse(asker))
            mFocusLock = false;
    }

    //=======================================

    public void ExciteSelectByOption(int index, int partId) 
    {
        AlterFocusOnSelf();
        if (CanSelect())
        {
            mTmpPartIn = partId;
            SuSelectTo(index, true);
        }
    }
    //选项主动告知其相关的事件
    //选项来告知能更方便与自由

    public void ExciteConfirmByOption(int index, int partId)
    {
        if (CanSelect())
        {
            if (mSelectCur == index)
            {
                mTmpPartIn = partId;
                MenuConfirm(true);
            }
        }
    }

    //内部机制=================================

    void Update()
    {
        if (!meCustomInput) 
        {
            if (UnifiedInput.It.meGoOneStep(this) == mDirNext)
            {
                MenuForward();
            }
            else if(UnifiedInput.It.meGoOneStep(this) == mDirNext.Reverse())
            {
                MenuBackward();
            }
            else if (UnifiedInput.It.meTapConfirm(this))
            {
                MenuConfirm(false);
            }
            else if (UnifiedInput.It.meWhenBack(this))
            {
                MenuCancel();
            }
        }
    }

    void AcceptNewSelect(List<GameObject> selectAble, int[] tmpBans)//纯选项基础数据的重置
    {
        mOptionsCanList.Clear();
        mOptionsInvalid.Clear();

        for (int i = 0; i < selectAble.Count; i++)
        {
            AssistOption opt = selectAble[i].GetComponent<AssistOption>();
            if (opt == null)
            {
                opt = selectAble[i].AddComponent<AssistOption>();
                opt.MakeReady();
                mOptionsHaveList.Add(opt);
            }
            mOptionsCanList.Add(opt);
            mOptionsInvalid.Add(false);
        }

        mNumOption = mOptionsCanList.Count;

        //-------------------------------

        if (tmpBans != null)
        {
            for (int i = 0; i < tmpBans.Length; i++)
            {
                mOptionsInvalid[tmpBans[i]] = true;
                mOptionsCanList[tmpBans[i]].FollowNotValid(false);
            }
        }
    }

    void ResetMemberVar()
    {
        mSelectLast = 0;
        mSelectCur = 0;
        mTmpPartIn = 0;
        meBanSelect = false;
    }

    void MenuBackward()
    {
        if (CanSelect()) 
        {
            int plan = FindBackOptionCan();
            if (plan >= 0)
                SuSelectTo(plan, false);
        }
    }
    void MenuForward() 
    {
        if (CanSelect()) 
        {
            int plan = FindNextOptionCan();
            if (plan >= 0)
                SuSelectTo(plan, false);
        }
    }
    void MenuConfirm(bool byCasual)
    {
        if (CanSelect()) 
        {
            SuNotifyConfirmAny();
            if (byCasual) SuNotifyConfirmByCasual(mTmpPartIn);
            else SuNotifyConfirmByAction();
            mOptionsCanList[mSelectCur].FollowEffectClick();
            mUser.iInformConfirmResult(mSelectCur);
        }
    }
    void MenuCancel()
    {
        if (CanSelect()) { mUser.iInformBackNeed(); }
    }

    bool CanSelect()
    {
        if (meBanSelect || mInPause || (mInFocus != this)) 
            return false;
        else
            return true;
    }

    void SelectToEdge(int want)
    {
        int can = FindTheFirstOptionCan();
        if (want < can)
        {
            want = FindTheLastOptionCan();
            if (want >= 0) SuSelectTo(want, false);
        }
        else
        {
            can = FindTheLastOptionCan();
            if (want > can)
            {
                want = FindTheFirstOptionCan();
                if (want >= 0) SuSelectTo(want, false);
            }
        }
    }

    IfoShift ShiftSelect(int want)
    {
        IfoShift ifo = ProSetNewSelect(want);
        if (ifo.change < 0)
        {
            if (ifo.overflow < 0)
                SuNotifyGoAboveList();
            else if (ifo.overflow > 0)
                SuNotifyGoBelowList();
        }
        return ifo;
    }

    void AlterFocusOnSelf()
    {
        if (!mFocusLock)
        {
            UnifiedInput.It.NormalizeArea(mInFocus);
            mInFocus = this;
            LimitCursorCanConfirm();
            //必需在放开区域限制后再做区域限制，不然没效果
        }
    }

    void LimitCursorCanConfirm()
    {
        RectMeter rect = mOptionsCanList[mSelectCur].meSitu.SuGetCornerAtInScreen(true);
        UnifiedInput.It.ConfineArea(rect, this);
    }

    public void UpdateHover() 
    {
        mOptionsCanList[mSelectLast].FollowEffectHover(false);
        mOptionsCanList[mSelectCur].FollowEffectHover(true);

        RectTransform curRectOption = mOptionsCanList[mSelectCur].GetComponent<RectTransform>();
        selector.sizeDelta = new Vector2(curRectOption.rect.width, curRectOption.rect.height);
        selector.position = curRectOption.position;
    }

    //内部工具===============================

    IfoShift ProSetNewSelect(int want) 
    {
        if (want < 0)//先判断越界
            return new IfoShift(want, -1);
        else
        {
            if (want > mNumOption - 1)
                return new IfoShift(want - (mNumOption - 1), -1);
            else
            {
                if (want == mSelectCur)//判断是否没发生移动
                    return new IfoShift(0, 0);
                else
                {
                    if (mOptionsInvalid[want])
                        return new IfoShift(0, -1);
                    else
                    {
                        mSelectLast = mSelectCur;
                        mSelectCur = want;
                        return new IfoShift(0, 1);
                    }
                }
            }
        }
    }

    int FindTheFirstOptionCan() 
    {
        for (int i = 0; i < mNumOption; i++)
        {
            if (!mOptionsInvalid[i]) return i;
        }
        return -1;
    }

    int FindTheLastOptionCan() 
    {
        for (int i = mNumOption - 1; i >= 0; i--) 
        {
            if(!mOptionsInvalid[i]) return i;
        }
        return -1;
    }

    int FindNextOptionCan() 
    {
        for (int i = mSelectCur + 1; i < mNumOption ; i++)
        {
            if (!mOptionsInvalid[i]) return i;
        }
        return -1;
    }

    int FindBackOptionCan()
    {
        int curIndex = mSelectCur;
        while (curIndex > 0)
        {
            curIndex -= 1;
            if (!mOptionsInvalid[curIndex])
            {
                return curIndex;
            }
        }
        return -1;
    }

    struct IfoShift
    {
        public int overflow;//代表目标选项相对当前列表的溢出量
        public int change;//-1代表无效的选项，0代表没移动，1代表移动了

        public IfoShift(int overflow, int change)
        {
            this.overflow = overflow;
            this.change = change;
        }
    }

}
