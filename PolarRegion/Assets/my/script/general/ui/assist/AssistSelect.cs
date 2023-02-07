using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class AssistSelect : MonoBehaviour
{//����ѡ���б�ѡ��Ĺ���
 //ÿһ���˵����ɸ����ʵ��������Ϊ��Ա��ʹ��
    public interface IUse
    {
        public void iInformConfirmResult(int index);

        public void iInformBackNeed();
    }

    public bool meCustomInput;//�Զ�������Ĳ㼶�����ܷ����룬�������ǰ���²�����Ч
    public bool meBanSelect;

    //ί����Ϊ�˿�ѡ��
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

    List<AssistOption> mOptionsCanList;//ѡ���ѡ��
    List<AssistOption> mOptionsHaveList;//ѡ����������Դ������Ҫѡ��ʱ����������ѡ���б��״̬����
    List<bool> mOptionsInvalid;//��CanListͬ��

    int mNumOption;
    IUse mUser;
    EToward4 mDirNext;
    RectTransform selector;//ѡ��

    int mSelectLast;
    int mSelectCur;
    bool mInPause;//�ͽ�ֹѡ��һ����������һ�ֿ�ѡ״̬
    int mTmpPartIn;

    MethodRangeAccess mFocusLockCtrl;
    static bool mFocusLock = false;
    static AssistSelect mInFocus;
    //������ɹ��ѡ�񣬽��㲻�ڵ�ǰѡ���б��ϣ��Ͳ��ᷢ��ȷ����Ϊ
    //һ���龳�У�����ͬʱ���ֶ����ͬѡ��������Ҫѡ������

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
    }//����ʹ��

    public void MakePause()
    {
        mInPause = true;
        selector.gameObject.SetActive(false);
        for (int i = 0; i < mOptionsCanList.Count; i++)
        {
            mOptionsCanList[i].MakeDown();
        }
        UnifiedInput.It.NormalizeArea(this);
    }//��ͣʹ��

    public void MakeContinue()
    {
        mInPause = false;
        selector.gameObject.SetActive(true);
        for (int i = 0; i < mOptionsCanList.Count; i++)
        {
            mOptionsCanList[i].MakeUse(this, i);
        }
    }//��;������ͣѡ��

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
    //�Ѿ�����ѡ��״̬ʱ����������绹���ڲ���������Ӧʹ�øýӿڣ�������ѡѡ��

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
    //ѡ��������֪����ص��¼�
    //ѡ������֪�ܸ�����������

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

    //�ڲ�����=================================

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

    void AcceptNewSelect(List<GameObject> selectAble, int[] tmpBans)//��ѡ��������ݵ�����
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
            //�����ڷſ��������ƺ������������ƣ���ȻûЧ��
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

    //�ڲ�����===============================

    IfoShift ProSetNewSelect(int want) 
    {
        if (want < 0)//���ж�Խ��
            return new IfoShift(want, -1);
        else
        {
            if (want > mNumOption - 1)
                return new IfoShift(want - (mNumOption - 1), -1);
            else
            {
                if (want == mSelectCur)//�ж��Ƿ�û�����ƶ�
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
        public int overflow;//����Ŀ��ѡ����Ե�ǰ�б�������
        public int change;//-1������Ч��ѡ�0����û�ƶ���1�����ƶ���

        public IfoShift(int overflow, int change)
        {
            this.overflow = overflow;
            this.change = change;
        }
    }

}
